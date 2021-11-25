using UnityEngine;

namespace SharedUnityMischief.Lifecycle
{
	public abstract class UpdateLoop : SingletonMonoBehaviour<UpdateLoop>
	{
		public const int UpdatesPerSecond = 60;
		public const float TimePerUpdate = 1f / 60f;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetSingletonClass() => instance = null;

		[Header("Update Loop Config")]
		public bool updateAutomatically = true;
		[Range(0, 2)] public float timeScale = 1.00f;

		public float time { get; private set; } = 0f;
		public int frame { get; private set; } = 0;
		public float deltaTime { get; private set; } = 0f;
		public bool isInterpolating { get; private set; } = false;
		public bool isPaused { get; private set; } = false;
		private float interpolatedTime = 0f;
		private float leftoverInterpolationTime = 0f;
		public float percentNextUpdateInterpolated => interpolatedTime / TimePerUpdate;

		private void Update()
		{
			if (updateAutomatically)
			{
				Advance();
			}
		}

		public virtual void Pause() => isPaused = true;

		public virtual void Resume() => isPaused = false;

		public void Advance(bool ignorePause = false) => Advance(UnityEngine.Time.deltaTime, ignorePause);

		public void Advance(float deltaTime, bool ignorePause = false)
		{
			deltaTime *= timeScale;
			if (ignorePause || !isPaused)
			{
				// We haven't advanced enough to count for a full frame, so just interpolate a bit more
				if (leftoverInterpolationTime + deltaTime < TimePerUpdate)
				{
					UpdateState(deltaTime, true);
					leftoverInterpolationTime += deltaTime;
				}
				// We've adanced to the next frame (and possibly beyond)
				else {
					// Get to the next frame
					UpdateState(TimePerUpdate - leftoverInterpolationTime, false);
					float unusedDeltaTime = deltaTime - (TimePerUpdate - leftoverInterpolationTime);
					// Keep advancing frames for as long as we have unused delta time left
					while (unusedDeltaTime >= TimePerUpdate)
					{
						unusedDeltaTime -= TimePerUpdate;
						UpdateState(TimePerUpdate, false);
					}
					// If we have a bit of leftover time, interpolate forward a bit
					if (unusedDeltaTime > 0f)
					{
						leftoverInterpolationTime = unusedDeltaTime;
						UpdateState(unusedDeltaTime, true);
					}
					else
					{
						leftoverInterpolationTime = 0f;
					}
				}
			}
		}

		public void AdvanceOneFrame(bool ignorePause = false)
		{
			if (ignorePause || !isPaused)
			{
				// Advance just far enough to get to the next frame
				UpdateState(TimePerUpdate - leftoverInterpolationTime, false);
				leftoverInterpolationTime = 0f;
			}
		}

		protected virtual void UpdateState() {}

		private void UpdateState(float deltaTime, bool isInterpolating)
		{
			// Set all public variables
			time += deltaTime;
			this.deltaTime = deltaTime;
			this.isInterpolating = isInterpolating;
			if (isInterpolating)
			{
				interpolatedTime += deltaTime;
			}
			else {
				frame++;
				interpolatedTime = 0f;
			}
			// Actually update the game state
			UpdateState();
		}
	}
}