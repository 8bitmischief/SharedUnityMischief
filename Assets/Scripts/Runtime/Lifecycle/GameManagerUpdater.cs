using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	[RequireComponent(typeof(GameManager))]
	public class GameManagerUpdater : MonoBehaviour {
		[SerializeField] [Range(0, 2)] private float timeScale = 1.00f;

		public bool isUpdating { get; private set; } = true;

		private GameManager gameManager;

		private void Awake () {
			gameManager = GetComponent<GameManager>();
		}

		private void Update () {
			if (isUpdating)
				gameManager.Advance(Time.deltaTime * timeScale);
		}

		public void Resume () => isUpdating = true;

		public void Pause () => isUpdating = false;

		public void Advance (float deltaTime) => gameManager.Advance(deltaTime);

		public void AdvanceOneFrame () => gameManager.AdvanceOneFrame();
	}
}