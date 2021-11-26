using UnityEngine;

namespace SharedUnityMischief.Lifecycle
{
	public abstract class UpdateLoop : SingletonMonoBehaviour<UpdateLoop>
	{
		public const int UpdatesPerSecond = 60;
		public const float TimePerUpdate = 1f / 60f;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetSingletonClass() => _instance = null;

		[Header("Update Loop Config")]
		[SerializeField] private bool _updateAutomatically = true;
		[SerializeField, Range(0, 2)] private float _timeScale = 1.00f;
		private float _time = 0f;
		private int _frame = 0;
		private float _deltaTime = 0f;
		private bool _isInterpolating = false;
		private bool _isPaused = false;
		private float _interpolatedTime = 0f;
		private float _leftoverInterpolationTime = 0f;

		public bool updateAutomatically => _updateAutomatically;
		public float time => _time;
		public int frame => _frame;
		public float deltaTime => _deltaTime;
		public bool isInterpolating => _isInterpolating;
		public bool isPaused => _isPaused;
		public float percentNextUpdateInterpolated => _interpolatedTime / TimePerUpdate;

		private void Update()
		{
			if (_updateAutomatically)
			{
				Advance();
			}
		}

		public virtual void Pause() => _isPaused = true;

		public virtual void Resume() => _isPaused = false;

		public void Advance(bool ignorePause = false) => Advance(UnityEngine.Time.deltaTime, ignorePause);

		public void Advance(float deltaTime, bool ignorePause = false)
		{
			deltaTime *= _timeScale;
			if (ignorePause || !_isPaused)
			{
				// We haven't advanced enough to count for a full frame, so just interpolate a bit more
				if (_leftoverInterpolationTime + deltaTime < TimePerUpdate)
				{
					UpdateState(deltaTime, true);
					_leftoverInterpolationTime += deltaTime;
				}
				// We've adanced to the next frame (and possibly beyond)
				else
				{
					// Get to the next frame
					UpdateState(TimePerUpdate - _leftoverInterpolationTime, false);
					float unusedDeltaTime = deltaTime - (TimePerUpdate - _leftoverInterpolationTime);
					// Keep advancing frames for as long as we have unused delta time left
					while (unusedDeltaTime >= TimePerUpdate)
					{
						unusedDeltaTime -= TimePerUpdate;
						UpdateState(TimePerUpdate, false);
					}
					// If we have a bit of leftover time, interpolate forward a bit
					if (unusedDeltaTime > 0f)
					{
						_leftoverInterpolationTime = unusedDeltaTime;
						UpdateState(unusedDeltaTime, true);
					}
					else
					{
						_leftoverInterpolationTime = 0f;
					}
				}
			}
		}

		public void AdvanceOneFrame(bool ignorePause = false)
		{
			if (ignorePause || !_isPaused)
			{
				// Advance just far enough to get to the next frame
				UpdateState(TimePerUpdate - _leftoverInterpolationTime, false);
				_leftoverInterpolationTime = 0f;
			}
		}

		protected virtual void UpdateState() {}

		private void UpdateState(float deltaTime, bool isInterpolating)
		{
			// Set all public variables
			_time += deltaTime;
			_deltaTime = deltaTime;
			_isInterpolating = isInterpolating;
			if (isInterpolating)
			{
				_interpolatedTime += deltaTime;
			}
			else
			{
				_frame++;
				_interpolatedTime = 0f;
			}
			// Actually update the game state
			UpdateState();
		}
	}
}