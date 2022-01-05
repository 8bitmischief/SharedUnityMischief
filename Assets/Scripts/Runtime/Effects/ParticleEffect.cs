using UnityEngine;
using UnityEngine.VFX;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects
{
	[ExecuteInEditMode]
	public class ParticleEffect : PoolableMonoBehaviour
	{
		[SerializeField] private float _duration = 1.0f;
		[SerializeField] private bool _playOnEnable = false;
		private VisualEffect[] _visualEffects;
		private float _playTime = 0f;
		private EndBehavior _endBehavior = EndBehavior.Destroy;
		private bool _isPlaying = false;

		public bool isPlaying => _isPlaying;
		public bool isPlayingEndlessly => _isPlaying && _endBehavior == EndBehavior.Loop;
		public float playTime => _playTime;

		private void Awake()
		{
			_visualEffects = GetComponentsInChildren<VisualEffect>();
		}

		private void OnEnable()
		{
			if (_playOnEnable)
				Play();
		}

		private void Update()
		{
			if (_isPlaying)
			{
				_playTime += Time.deltaTime;
				if (_playTime >= _duration)
				{
					if (_endBehavior == EndBehavior.Loop)
					{
						PlayEndlessly();
					}
					else
					{
						Stop();
						if (_endBehavior == EndBehavior.Destroy)
							DepositToPoolOrDestroy();
					}
				}
			}
		}

		public override void OnWithdrawFromPool()
		{
			if (name.EndsWith(" (In Pool)"))
				name = name.Substring(0, name.Length - " (In Pool)".Length);
		}

		public override void OnDepositToPool()
		{
			if (!name.EndsWith(" (In Pool)"))
				name = $"{name} (In Pool)";
		}

		public void PlayEndlessly() => Play(EndBehavior.Loop);

		public void Play(bool destroyWhenDonePlaying = false)
			=> Play(destroyWhenDonePlaying ? EndBehavior.Destroy : EndBehavior.None);

		private void Play(EndBehavior endBehavior)
		{
			if (!Application.isPlaying)
				_visualEffects = GetComponentsInChildren<VisualEffect>();
			_isPlaying = true;
			_playTime = 0f;
			this._endBehavior = endBehavior;
			foreach (VisualEffect visualEffect in _visualEffects)
				visualEffect.Play();
		}

		public void Stop()
		{
			if (!Application.isPlaying)
				_visualEffects = GetComponentsInChildren<VisualEffect>();
			_isPlaying = false;
			foreach (VisualEffect visualEffect in _visualEffects)
				visualEffect.Stop();
		}

		private enum EndBehavior
		{
			None = 0,
			Destroy = 1,
			Loop = 2
		}
	}
}