using UnityEngine;

namespace SharedUnityMischief
{
	public class Billboard : MonoBehaviour
	{
		[SerializeField] private bool _xRotation = true;
		[SerializeField] private bool _yRotation = false;
		[SerializeField] private bool _zRotation = false;
		private Camera _camera;

		private void Awake()
		{
			_camera = Camera.main;
		}

		private void LateUpdate()
		{
			Vector3 rotation = transform.eulerAngles;
			Vector3 cameraRotation = _camera.transform.eulerAngles;
			transform.eulerAngles = new Vector3(
				_xRotation ? cameraRotation.x : rotation.x,
				_yRotation ? cameraRotation.y : rotation.y,
				_zRotation ? cameraRotation.z : rotation.z);
		}
	}
}