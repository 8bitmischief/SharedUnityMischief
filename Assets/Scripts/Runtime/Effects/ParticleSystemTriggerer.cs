using UnityEngine;

namespace SharedUnityMischief.Effects
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemTriggerer : MonoBehaviour
	{
		[SerializeField] private bool stopEmitting = false;

#pragma warning disable CS0109 // Ignore "does not hide an accessible" warning during builds
		private new ParticleSystem particleSystem;
#pragma warning restore CS0109
		private bool stopEmittingLastFrame = false;
		
		private void Awake()
		{
			particleSystem = GetComponent<ParticleSystem>();
		}

		private void OnEnable()
		{
			stopEmittingLastFrame = stopEmitting;
			if (particleSystem != null)
			{
				particleSystem.Play(true);
			}
		}

		private void Update()
		{
			if (stopEmitting && !stopEmittingLastFrame && particleSystem != null)
			{
				particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
			stopEmittingLastFrame = stopEmitting;
		}

		private void OnDisable() {}
	}
}