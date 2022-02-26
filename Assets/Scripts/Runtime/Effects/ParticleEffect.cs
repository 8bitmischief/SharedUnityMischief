using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects
{
	[ExecuteInEditMode]
	public class ParticleEffect : PoolableMonoBehaviour
	{
		[SerializeField] private float _duration = 1f;
		[SerializeField] private float _delay = 0f;
		[SerializeField] private bool _loops = false;
		private VisualEffect _visualEffect;
		private List<ParticleEffect> _childParticleEffects;
		private List<VisualEffect> _childVisualEffects;
		private bool _isPlaying = false;
		private float _playTime;
		private bool _hasPlayedEffects;
		private EndBehavior _endBehavior;

		public bool isPlaying => _isPlaying;
		public bool isLooping => _isPlaying && (_endBehavior == EndBehavior.Loop || (_endBehavior == EndBehavior.Auto && _loops));

		private void Awake()
		{
			FindEffects();
		}

		private void Update()
		{
			if (_isPlaying)
			{
				_playTime += Time.deltaTime;
				// Keep particle effect flipped correctly
				if (Application.isPlaying && transform.parent != null)
				{
					transform.localScale = new Vector3(
						Mathf.Sign(transform.parent.lossyScale.x) * Mathf.Abs(transform.localScale.x),
						Mathf.Sign(transform.parent.lossyScale.y) * Mathf.Abs(transform.localScale.y),
						Mathf.Sign(transform.parent.lossyScale.z) * Mathf.Abs(transform.localScale.z));
				}
				// Play all effects after the initial delay
				if (!_hasPlayedEffects && _playTime >= _delay)
				{
					PlayAllEffects();
					_hasPlayedEffects = true;
				}
				// Stop playing or loop
				if (_playTime >= _duration + _delay)
				{
					switch (_endBehavior)
					{
						case EndBehavior.Destroy:
							_isPlaying = false;
							StopAllEffects();
							DepositToPoolOrDestroy();
							break;
						case EndBehavior.Loop:
						case EndBehavior.Auto when _loops:
							_playTime = _delay;
							if (!_loops)
								PlayAllEffects();
							break;
						default:
							_isPlaying = false;
							if (_loops)
								StopAllEffects();
							break;
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

		public void Play() => Play(EndBehavior.Auto);
		public void PlayOnce() => Play(EndBehavior.Stop);
		public void PlayOnceThenDestroy() => Play(EndBehavior.Destroy);
		public void PlayLooping() => Play(EndBehavior.Loop);
		private void Play(EndBehavior endBehavior)
		{
			_isPlaying = true;
			_playTime = 0f;
			_hasPlayedEffects = false;
			_endBehavior = endBehavior;
			if (_delay <= 0f)
			{
				PlayAllEffects();
				_hasPlayedEffects = true;
			}
		}

		public void Stop()
		{
			_isPlaying = false;
			StopAllEffects();
		}

		public void StopLooping()
		{
			_endBehavior = EndBehavior.Stop;
		}

		private void FindEffects()
		{
			_visualEffect = GetComponent<VisualEffect>();
			_childParticleEffects = new List<ParticleEffect>();
			_childVisualEffects = new List<VisualEffect>();
			foreach (Transform child in transform)
			{
				ParticleEffect particleEffect = child.GetComponent<ParticleEffect>();
				if (particleEffect != null)
				{
					_childParticleEffects.Add(particleEffect);
				}
				else
				{
					VisualEffect visualEffect = child.GetComponent<VisualEffect>();
					if (visualEffect != null)
					{
						_childVisualEffects.Add(visualEffect);
					}
				}
			}
		}

		private void PlayAllEffects()
		{
			if (!Application.isPlaying)
				FindEffects();
			if (_visualEffect != null)
				_visualEffect.Play();
			foreach (ParticleEffect particleEffect in _childParticleEffects)
				particleEffect.Play();
			foreach (VisualEffect visualEffect in _childVisualEffects)
				visualEffect.Play();
		}

		private void StopAllEffects()
		{
			if (!Application.isPlaying)
				FindEffects();
			if (_visualEffect != null)
				_visualEffect.Stop();
			foreach (ParticleEffect particleEffect in _childParticleEffects)
				particleEffect.Stop();
			foreach (VisualEffect visualEffect in _childVisualEffects)
				visualEffect.Stop();
		}

		private enum EndBehavior
		{
			None = 0,
			Auto = 1,
			Stop = 2,
			Loop = 3,
			Destroy = 4,
		}
	}
}