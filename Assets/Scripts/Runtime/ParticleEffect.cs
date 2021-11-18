using UnityEngine;

namespace SharedUnityMischief {
	[RequireComponent(typeof(Animator))]
	public class ParticleEffect : MonoBehaviour {
		private static readonly int playHash = Animator.StringToHash("Play");

		private Animator animator;

		private void Awake () {
			animator = GetComponent<Animator>();
		}

		public void Play () {
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
			animator.SetTrigger(playHash);
		}
	}
}