using UnityEngine;
using UnityEngine.VFX;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects
{
	public class ParticleEffect : PoolableMonoBehavior
	{
		[SerializeField] private float _duration = 1.0f;
		private VisualEffect[] _visualEffects;
		private float _playTime = 0f;
		private EndBehavior _endBehavior = EndBehavior.Destroy;
		public bool _isPlaying { get; private set; } = false;

		private void Awake()
		{
			_visualEffects = GetComponentsInChildren<VisualEffect>();
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
						{
							DepositToPoolOrDestroy();
						}
					}
				}
			}
		}

		public override void OnWithdrawFromPool() {}

		public override void OnDepositToPool() {}

		public void PlayEndlessly() => Play(EndBehavior.Loop);

		public void Play(bool destroyWhenDonePlaying = true)
			=> Play(destroyWhenDonePlaying ? EndBehavior.Destroy : EndBehavior.None);

		private void Play(EndBehavior endBehavior)
		{
			_isPlaying = true;
			_playTime = 0f;
			this._endBehavior = endBehavior;
			foreach (VisualEffect visualEffect in _visualEffects)
			{
				visualEffect.Play();
			}
		}

		public void Stop()
		{
			_isPlaying = false;
			foreach (VisualEffect visualEffect in _visualEffects)
			{
				visualEffect.Stop();
			}
		}

		private enum EndBehavior
		{
			None = 0,
			Destroy = 1,
			Loop = 2
		}
	}
}