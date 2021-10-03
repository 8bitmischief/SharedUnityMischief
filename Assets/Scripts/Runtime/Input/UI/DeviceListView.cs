using UnityEngine;

namespace SharedUnityMischief.Input.UI {
	public class DeviceListView : MonoBehaviour {
		[SerializeField] private DevicePanelView devicePanelTemplate;

		private void Awake () {
			devicePanelTemplate.gameObject.SetActive(false);
		}

		public void AddDevice (Device device) {
			DevicePanelView devicePanel = Object.Instantiate(devicePanelTemplate, transform);
			devicePanel.gameObject.SetActive(true);
			devicePanel.SetDevice(device);
		}
	}
}
