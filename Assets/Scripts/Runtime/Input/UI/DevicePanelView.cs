using UnityEngine;
using TMPro;

namespace SharedUnityMischief.Input.UI {
	public class DevicePanelView : MonoBehaviour {
		[SerializeField] private TMP_Text nameText;
		[SerializeField] private TMP_Text statusText;

		private Device device;

		private void Awake () {
			RefreshDeviceText();
		}

		private void OnDestroy () {
			if (device != null) {
				device.onReconnect -= RefreshDeviceText;
				device.onDisconnect -= RefreshDeviceText;
			}
		}

		public void SetDevice (Device device) {
			this.device = device;
			device.onReconnect += RefreshDeviceText;
			device.onDisconnect += RefreshDeviceText;
			RefreshDeviceText();
		}

		private void RefreshDeviceText () {
			if (device == null) {
				nameText.text = "null device";
				statusText.text = "";
			}
			else {
				nameText.text = device.name;
				statusText.text = device.isConnected ? (device.hasReconnected ? "Reconnected" : "Connected") : "Disconnected";
			}
		}
	}
}
