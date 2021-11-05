using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public class UpdateLoopUpdater : MonoBehaviour {
		[SerializeField] private UpdateLoop updateLoop;

		[Header("Update Loop Config")]
		public bool updateAutomatically = true;
		public bool isPaused = false;
		[Range(0, 2)] public float timeScale = 1.00f;

		private void Update () {
			if (updateAutomatically)
				UpdateState();
		}

		public void UpdateState (bool ignorePause = false) => UpdateState(Time.deltaTime, ignorePause);

		public void UpdateState (float deltaTime, bool ignorePause = false) {
			if (ignorePause || !isPaused)
				updateLoop.Advance(deltaTime * timeScale);
		}

		public void UpdateStateOneFrame (bool ignorePause = false) {
			if (ignorePause || !isPaused)
				updateLoop.AdvanceOneFrame();
		}
	}
}