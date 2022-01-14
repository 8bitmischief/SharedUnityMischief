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
		[SerializeField] private float _cooldown = 0f;
		[SerializeField] private bool _playOnEnable = false;
		private VisualEffect _visualEffect;
		private List<ParticleEffect> _childParticleEffects;
		private List<VisualEffect> _childVisualEffects;
		private bool _isPlaying = false;
		private float _playTime = 0f;
		private bool _hasPlayedEffects = false;
		private bool _hasStoppedEffects = false;
		private EndBehavior _endBehavior = EndBehavior.Destroy;

		public bool isPlaying => _isPlaying;
		public bool isPlayingEndlessly => _isPlaying && _endBehavior == EndBehavior.Loop;

		private void Awake()
		{
			FindEffects();
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
				// Keep particle effect flipped correctly
				if (Application.isPlaying)
				{
					transform.localScale = new Vector3(
						Mathf.Sign(transform.parent.lossyScale.x) * Mathf.Abs(transform.localScale.x),
						Mathf.Sign(transform.parent.lossyScale.y) * Mathf.Abs(transform.localScale.y),
						Mathf.Sign(transform.parent.lossyScale.z) * Mathf.Abs(transform.localScale.z));
				}
				// Play all effects after the initial delay
				if (_playTime >= _delay && !_hasPlayedEffects)
				{
					PlayAllEffects();
					_hasPlayedEffects = true;
				}
				// Stop all effects
				if (_playTime >= _duration + _delay && !_hasStoppedEffects)
				{
					StopAllEffects();
					_hasStoppedEffects = true;
				}
				// Stop playing or loop
				if (_playTime >= _duration + _delay + _cooldown)
				{
					if (_endBehavior == EndBehavior.Loop)
					{
						_playTime = _delay;
						_hasStoppedEffects = false;
						PlayAllEffects();
					}
					else if (_endBehavior == EndBehavior.Destroy)
					{
						_isPlaying = false;
						DepositToPoolOrDestroy();
					}
					else
					{
						_isPlaying = false;
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

		public void Play() => Play(EndBehavior.None);
		public void PlayOnceThenDestroy() => Play(EndBehavior.Destroy);
		public void PlayEndlessly() => Play(EndBehavior.Loop);
		private void Play(EndBehavior endBehavior)
		{
			_hasPlayedEffects = false;
			_hasStoppedEffects = false;
			_playTime = 0f;
			_endBehavior = endBehavior;
			_isPlaying = true;
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
			Destroy = 1,
			Loop = 2
		}
	}
}