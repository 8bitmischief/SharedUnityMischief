using UnityEngine;

namespace SharedUnityMischief.Effects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemTriggerer : MonoBehaviour
	{
		[SerializeField] private bool _stopEmitting = false;
		private ParticleSystem _particleSystem;
		private bool _stopEmittingLastFrame = false;
		
		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
		}

		private void OnEnable()
		{
			_stopEmittingLastFrame = _stopEmitting;
			if (_particleSystem != null)
			{
				_particleSystem.Play(true);
			}
		}

		private void Update()
		{
			if (_stopEmitting && !_stopEmittingLastFrame && _particleSystem != null)
			{
				_particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
			_stopEmittingLastFrame = _stopEmitting;
		}

		private void OnDisable() {}
	}
}