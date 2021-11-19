using UnityEngine;
using UnityEngine.Playables;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects {
	[RequireComponent(typeof(PlayableDirector))]
	public class ParticleEffect : PoolableMonoBehavior {
		private PlayableDirector director;

		private void Awake () {
			director = GetComponent<PlayableDirector>();
		}

		private void OnEnable () {
			Play();
		}

		private void Update () {
			if (director.state != PlayState.Playing)
				DepositOrDestroy();
		}

		private void OnDisable () {
			Stop();
		}

		public void Play () {
			director.Stop();
			director.Play();
		}

		public void Stop () {
			director.Stop();
		}
	}
}