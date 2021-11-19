using UnityEngine;
using UnityEngine.Playables;

namespace SharedUnityMischief {
	[RequireComponent(typeof(PlayableDirector))]
	public class ParticleEffect : MonoBehaviour {
		[SerializeField] private bool playOnAwake = false;
		[SerializeField] private bool playOnEnable = false;

		private PlayableDirector director;

		private void Awake () {
			director = GetComponent<PlayableDirector>();
			if (playOnAwake)
				Play();
		}

		private void OnEnable () {
			if (playOnEnable)
				Play();
		}

		public void Play () {
			director.Stop();
			director.Play();
		}
	}
}