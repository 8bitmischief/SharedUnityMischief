using UnityEngine;
using SharedUnityMischief.Entities;
using SharedUnityMischief.Lifecycle;

namespace SharedUnityMischief.Effects
{
	[ExecuteInEditMode]
	public class ParticleEffectTriggerer : EntityComponent
	{
		[SerializeField] private ParticleEffect _particleEffect;

		[Header("Triggerer Config")]
		[SerializeField] private bool _trigger = false;
		[SerializeField] private bool _triggerOnEnable = false;
		[SerializeField] private bool _triggerInEditMode = true;
		private bool _wasTriggered = false;

		private void OnEnable()
		{
			if (_triggerOnEnable && (Application.isPlaying || _triggerInEditMode))
				Trigger();
		}

		private void Update()
		{
			if (!Application.isPlaying && _triggerInEditMode)
			{
				if (_trigger && !_wasTriggered)
					Trigger();
				_wasTriggered = _trigger;
			}
		}

		public override void UpdateState()
		{
			if (UpdateLoop.I.isInterpolating)
			{
				if (_trigger && !_wasTriggered)
					Trigger();
				_wasTriggered = _trigger;
			}
		}

		public void Trigger()
		{
			if (_particleEffect != null)
				_particleEffect.Play();
		}
	}
}