using UnityEngine;

namespace SharedUnityMischief
{
	[RequireComponent(typeof(Camera))]
	public class ScrollableCamera : MonoBehaviour
	{
		[SerializeField] private Vector2 _scroll = Vector2.zero;
		private Camera _camera;
		private Vector2 _lastRenderedScroll;
		private Vector2 _lastRenderedScreenSize;
		private float _lastRenderedFieldOfView;

		public Vector2 scroll { get => _scroll; set => _scroll = value; }

		private void Awake()
		{
			_camera = GetComponent<Camera>();
			ApplyScrollToProjectionMatrix();
			_lastRenderedScroll = scroll;
			_lastRenderedScreenSize = new Vector2(Screen.width, Screen.height);
			_lastRenderedFieldOfView = _camera.fieldOfView;
		}

		private void LateUpdate()
		{
			// Only update the camera's projection matrix if something's changed
			if (_lastRenderedScroll != scroll ||
				_lastRenderedScreenSize.x != Screen.width ||
				_lastRenderedScreenSize.y != Screen.height ||
				_lastRenderedFieldOfView != _camera.fieldOfView)
			{
				// Reset the projection matrix so any automatic field of view changes can take effect
				if (_lastRenderedScreenSize.x != Screen.width ||
					_lastRenderedScreenSize.y != Screen.height ||
					_lastRenderedFieldOfView != _camera.fieldOfView)
				{
					_camera.ResetProjectionMatrix();
				}
				// Refresh the camera's projection matrix
				ApplyScrollToProjectionMatrix();
				_lastRenderedScroll = scroll;
				_lastRenderedScreenSize = new Vector2(Screen.width, Screen.height);
				_lastRenderedFieldOfView = _camera.fieldOfView;
			}
		}

		private void ApplyScrollToProjectionMatrix()
		{
			Matrix4x4 matrix = _camera.projectionMatrix;
			// Do some perspective math to scroll the camera around
			var w = 2f * _camera.nearClipPlane / matrix.m00;
			var h = 2f * _camera.nearClipPlane / matrix.m11;
			var left = -w / 2f + scroll.x / 100f;
			var right = left + w;
			var bottom = -h / 2f + scroll.y / 100f;
			var top = bottom + h;
			float near = _camera.nearClipPlane;
			float far = _camera.farClipPlane;
			float x = (2f * near) / (right - left);
			float y = (2f * near) / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(far + near) / (far - near);
			float d = -(2f * far * near) / (far - near);
			float e = -1f;
			// Generate and set a new projection matrix
			matrix[0,0] = x;	matrix[0,1] = 0f;	matrix[0,2] = a;	matrix[0,3] = 0f;
			matrix[1,0] = 0f;	matrix[1,1] = y;	matrix[1,2] = b;	matrix[1,3] = 0f;
			matrix[2,0] = 0f;	matrix[2,1] = 0f;	matrix[2,2] = c;	matrix[2,3] = d;
			matrix[3,0] = 0f;	matrix[3,1] = 0f;	matrix[3,2] = e;	matrix[3,3] = 0f;
			_camera.projectionMatrix = matrix;
		}
	}
}