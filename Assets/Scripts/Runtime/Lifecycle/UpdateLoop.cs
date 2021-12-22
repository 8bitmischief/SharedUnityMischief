using System;
using UnityEngine;

namespace SharedUnityMischief.Lifecycle
{
	public abstract class UpdateLoop : SingletonMonoBehaviour<UpdateLoop>
	{
		public const int UpdatesPerSecond = 60;
		public const float TimePerUpdate = 1f / 60f;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetSingletonClass() => ClearInstance();

		[Header("Update Loop Config")]
		[SerializeField] private bool _updateAutomatically = true;
		private float _time = 0f;
		private int _frame = 0;
		private float _deltaTime = 0f;
		private bool _isInterpolating = false;
		private bool _isFinalUpdateThisFrame = false;
		private bool _isPaused = false;
		private float _interpolatedTime = 0f;
		private float _leftoverInterpolationTime = 0f;

		public float time => _time;
		public int frame => _frame;
		public float deltaTime => _deltaTime;
		public bool isInterpolating => _isInterpolating;
		public bool isFinalUpdateThisFrame => _isFinalUpdateThisFrame;
		public bool isPaused => _isPaused;
		public float percentNextUpdateInterpolated => _interpolatedTime / TimePerUpdate;

		public event Action onPreUpdateState;
		public event Action onPostUpdateState;

		protected virtual void Update()
		{
			if (_updateAutomatically)
				Advance();
		}

		public virtual void Pause() => _isPaused = true;

		public virtual void Resume() => _isPaused = false;

		public void Advance(bool ignorePause = false) => Advance(Time.deltaTime, ignorePause);

		public void Advance(float deltaTime, bool ignorePause = false)
		{
			if (ignorePause || !_isPaused)
			{
				// We haven't advanced enough to count for a full frame, so just interpolate a bit more
				if (_leftoverInterpolationTime + deltaTime < TimePerUpdate)
				{
					_isFinalUpdateThisFrame = true;
					UpdateState(deltaTime, true);
					_leftoverInterpolationTime += deltaTime;
				}
				// We've adanced to the next frame (and possibly beyond)
				else
				{
					// Get to the next frame
					float unusedDeltaTime = deltaTime - (TimePerUpdate - _leftoverInterpolationTime);
					_isFinalUpdateThisFrame = unusedDeltaTime <= 0f;
					UpdateState(TimePerUpdate - _leftoverInterpolationTime, false);
					// Keep advancing frames for as long as we have unused delta time left
					while (unusedDeltaTime >= TimePerUpdate)
					{
						unusedDeltaTime -= TimePerUpdate;
						_isFinalUpdateThisFrame = unusedDeltaTime <= 0f;
						UpdateState(TimePerUpdate, false);
					}
					// If we have a bit of leftover time, interpolate forward a bit
					if (unusedDeltaTime > 0f)
					{
						_leftoverInterpolationTime = unusedDeltaTime;
						_isFinalUpdateThisFrame = true;
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
				_isFinalUpdateThisFrame = true;
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
			onPreUpdateState?.Invoke();
			UpdateState();
			onPostUpdateState?.Invoke();
		}
	}
}