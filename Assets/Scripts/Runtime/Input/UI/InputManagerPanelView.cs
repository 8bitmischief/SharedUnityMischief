using UnityEngine;

namespace SharedUnityMischief.Input.UI {
	public class InputManagerPanelView : MonoBehaviour {
		[SerializeField] private DeviceListView deviceList;

		private InputManager inputManager;

		private void OnDestroy () {
			if (inputManager != null)
				inputManager.onDeviceConnect -= OnDeviceConnect;
		}

		public void SetInputManager (InputManager inputManager) {
			this.inputManager = inputManager;
			foreach (Device device in inputManager.devices)
				deviceList.AddDevice(device);
			inputManager.onDeviceConnect += OnDeviceConnect;
		}

		private void OnDeviceConnect (Device device) {
			deviceList.AddDevice(device);
		}
	}
}