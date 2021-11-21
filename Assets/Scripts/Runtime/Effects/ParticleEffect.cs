using UnityEngine;
using UnityEngine.VFX;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects {
	public class ParticleEffect : PoolableMonoBehavior {
		[SerializeField] private float duration = 1.0f;

		private VisualEffect[] visualEffects;
		private bool isPlaying = false;
		private float playTime = 0f;

		private void Awake () {
			visualEffects = GetComponentsInChildren<VisualEffect>();
		}

		public override void OnWithdrawFromPool () {}

		private void Update () {
			if (isPlaying) {
				playTime += Time.deltaTime;
				if (playTime >= duration)
					Stop();
			}
		}

		public override void OnDepositToPool () {}

		public void Play () {
			isPlaying = true;
			playTime = 0f;
			foreach (VisualEffect visualEffect in visualEffects)
				visualEffect.Play();
		}

		private void Stop () {
			isPlaying = false;
			foreach (VisualEffect visualEffect in visualEffects)
				visualEffect.Stop();
			DepositToPoolOrDestroy();
		}
	}
}