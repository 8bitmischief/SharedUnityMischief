using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class UpdateLoop : SingletonMonoBehaviour<UpdateLoop> {
		public static readonly int updatesPerSecond = 60;
		public static readonly float timePerUpdate = 1f / 60f;

		[Header("Update Loop Config")]
		public bool updateAutomatically = true;
		[Range(0, 2)] public float timeScale = 1.00f;

		public float time { get; private set; } = 0f;
		public int frame { get; private set; } = 0;
		public float deltaTime { get; private set; } = 0f;
		public bool isInterpolating { get; private set; } = false;
		public bool isPaused { get; private set; } = false;
		public float percentNextUpdateInterpolated => interpolatedTime / timePerUpdate;

		private float interpolatedTime = 0f;
		private float leftoverInterpolationTime = 0f;

		private void Update () {
			if (updateAutomatically)
				UpdateState();
		}

		public void Pause () => isPaused = true;

		public void Resume () => isPaused = false;

		public void Advance (bool ignorePause = false) => Advance(Time.deltaTime, ignorePause);

		public void Advance (float deltaTime, bool ignorePause = false) {
			deltaTime *= timeScale;
			if (ignorePause || !isPaused) {
				// We haven't advanced enough to count for a full frame, so just interpolate a bit more
				if (leftoverInterpolationTime + deltaTime < timePerUpdate) {
					UpdateState(deltaTime, true);
					leftoverInterpolationTime += deltaTime;
				}
				// We've adanced to the next frame (and possibly beyond)
				else {
					// Get to the next frame
					UpdateState(timePerUpdate - leftoverInterpolationTime, false);
					float unusedDeltaTime = deltaTime - (timePerUpdate - leftoverInterpolationTime);
					// Keep advancing frames for as long as we have unused delta time left
					while (unusedDeltaTime >= timePerUpdate) {
						unusedDeltaTime -= timePerUpdate;
						UpdateState(timePerUpdate, false);
					}
					// If we have a bit of leftover time, interpolate forward a bit
					if (unusedDeltaTime > 0f) {
						leftoverInterpolationTime = unusedDeltaTime;
						UpdateState(unusedDeltaTime, true);
					}
					else
						leftoverInterpolationTime = 0f;
				}
			}
		}

		public void AdvanceOneFrame (bool ignorePause = false) {
			if (ignorePause || !isPaused) {
				// Advance just far enough to get to the next frame
				UpdateState(timePerUpdate - leftoverInterpolationTime, false);
				leftoverInterpolationTime = 0f;
			}
		}

		protected virtual void UpdateState () {}

		private void UpdateState (float deltaTime, bool isInterpolating) {
			// Set all public variables
			time += deltaTime;
			this.deltaTime = deltaTime;
			this.isInterpolating = isInterpolating;
			if (isInterpolating)
				interpolatedTime += deltaTime;
			else {
				frame++;
				interpolatedTime = 0f;
			}
			// Actually update the game state
			UpdateState();
		}
	}
}