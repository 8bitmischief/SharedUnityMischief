using UnityEngine;

namespace SharedUnityMischief {
	[ExecuteInEditMode]
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemTriggerer : MonoBehaviour {
		private new ParticleSystem particleSystem;
		
		private void Awake () {
			particleSystem = GetComponent<ParticleSystem>();
		}

		private void OnEnable () {
			if (particleSystem != null)
				particleSystem.Play();
		}

		private void OnDisable () {}
	}
}