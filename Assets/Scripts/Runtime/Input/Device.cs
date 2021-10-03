using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input {
	public class Device {
		public bool isConnected { get; private set; }
		public string name => inputDevice.ToString();
		public double timeSinceLastConnected => isConnected ? 0.0 : Time.time - timeDisconnected;
		public bool hasReconnected { get; private set; }

		public Action onReconnect;
		public Action onDisconnect;

		private InputDevice inputDevice;
		private double timeDisconnected;

		public Device (InputDevice inputDevice) {
			this.inputDevice = inputDevice;
			isConnected = true;
			timeDisconnected = Time.time;
			hasReconnected = false;
		}

		public void Reconnect (InputDevice inputDevice = null) {
			if (inputDevice != null)
				this.inputDevice = inputDevice;
			if (!isConnected) {
				isConnected = true;
				hasReconnected = true;
				onReconnect?.Invoke();
			}
		}

		public void Disconnect () {
			if (isConnected) {
				isConnected = false;
				timeDisconnected = Time.time;
				onDisconnect?.Invoke();
			}
		}

		public bool IsDefiniteMatchForInputDevice (InputDevice inputDevice) {
			return inputDevice == this.inputDevice;
		}

		public bool IsPlausibleMatchForInputDevice (InputDevice inputDevice) {
			return false;
		}
	}
}
