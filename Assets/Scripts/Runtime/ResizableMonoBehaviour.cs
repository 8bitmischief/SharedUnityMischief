using UnityEngine;

namespace SharedUnityMischief
{
	public abstract class ResizableMonoBehaviour : MonoBehaviour
	{
		private static readonly float MaxResizeDelayTime = 0.2f;
		private static readonly float ResizeDebounceTime = 0.05f;

		[SerializeField] private Camera _camera;
		private Vector2 _resolution = Vector2.zero;
		private float _fieldOfView = 0f;
		private float _timeUntilResize = 0f;
		private float _debouncedTimeUntilResize = 0f;

		#pragma warning disable CS0109 // Ignore "does not hide an accessible member" warnings
		protected new Camera camera => _camera;
		#pragma warning restore CS0109
		protected Vector2 resolution => _resolution;

		protected virtual void Update()
		{
			// Check for resolution or field of view changes
			if (_resolution.x != Screen.width || _resolution.y != Screen.height || (_camera != null && _camera.fieldOfView != _fieldOfView))
			{
				_resolution = new Vector2(Screen.width, Screen.height);
				_debouncedTimeUntilResize = ResizeDebounceTime;
				if (_timeUntilResize == 0)
					_timeUntilResize = MaxResizeDelayTime;
			}
			// Debounce the OnResize() callback
			if (_timeUntilResize > 0f || _debouncedTimeUntilResize > 0f)
			{
				_timeUntilResize -= Time.deltaTime;
				_debouncedTimeUntilResize -= Time.deltaTime;
				if (_timeUntilResize <= 0f || _debouncedTimeUntilResize <= 0f)
				{
					_timeUntilResize = 0f;
					_debouncedTimeUntilResize = 0f;
					OnResize();
				}
			}
		}

		protected abstract void OnResize();
	}
}