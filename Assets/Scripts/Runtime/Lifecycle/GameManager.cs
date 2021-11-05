using System;
using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class GameManager : SingletonMonoBehaviour<GameManager> {
		public static readonly int updatesPerSecond = 60;
		public static readonly float timePerUpdate = 1f / 60f;

		public float time { get; private set; } = 0f;
		public int frame { get; private set; } = 0;
		public float deltaTime { get; private set; } = 0f;
		public bool isInterpolating { get; private set; } = false;
		public float percentNextUpdateInterpolated => interpolatedTime / timePerUpdate;

		private float interpolatedTime = 0f;
		private float leftoverInterpolationTime = 0f;

		public void Advance (float deltaTime) {
			// We haven't advanced enough to count for a full frame, so just interpolate a bit more
			if (leftoverInterpolationTime + deltaTime < timePerUpdate) {
				Advance(deltaTime, true);
				leftoverInterpolationTime += deltaTime;
			}
			// We've adanced to the next frame (and possibly beyond)
			else {
				// Get to the next frame
				Advance(timePerUpdate - leftoverInterpolationTime, false);
				float unusedDeltaTime = deltaTime - (timePerUpdate - leftoverInterpolationTime);
				// Keep advancing frames for as long as we have unused delta time left
				while (unusedDeltaTime >= timePerUpdate) {
					unusedDeltaTime -= timePerUpdate;
					Advance(timePerUpdate, false);
				}
				// If we have a bit of leftover time, interpolate forward a bit
				if (unusedDeltaTime > 0f) {
					leftoverInterpolationTime = unusedDeltaTime;
					Advance(unusedDeltaTime, true);
				}
				else
					leftoverInterpolationTime = 0f;
			}
		}

		public void AdvanceOneFrame () {
			// Advance just far enough to get to the next frame
			Advance(timePerUpdate - leftoverInterpolationTime, false);
			leftoverInterpolationTime = 0f;
		}

		private void Advance (float deltaTime, bool isInterpolating) {
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

		protected virtual void UpdateState () {}
	}

	public abstract class GameManager<T> : GameManager where T : GameManager {
		private static T instance;
		public static new T I {
			get {
				if (instance == null)
					instance = UnityEngine.Object.FindObjectOfType<T>();
				if (instance == null)
					throw new Exception($"Could not find {typeof(T)} singleton in the scene hierarchy");
				return instance;
			}
		}

		protected override void Awake () {
			if (!ClaimSingletonInstanceOrDestroySelf(indestructible))
				return;
		}

		protected override void OnDestroy () {
			ReleaseSingletonInstance();
		}

		protected override bool ClaimSingletonInstance () {
			if (!base.ClaimSingletonInstance())
				return false;
			if (instance != null && instance != this) {
				base.ReleaseSingletonInstance();
				return false;
			}
			else {
				instance = this as T;
				return true;
			}
		}

		protected override bool ClaimSingletonInstanceOrDestroySelf (bool makeIndestructible = false) {
			if (!ClaimSingletonInstance()) {
				Destroy(this);
				return false;
			}
			else {
				if (makeIndestructible)
					DontDestroyOnLoad(this);
				return true;
			}
		}

		protected override bool ReleaseSingletonInstance () {
			base.ReleaseSingletonInstance();
			if (instance == this) {
				instance = null;
				return true;
			}
			else
				return false;
		}
	}
}