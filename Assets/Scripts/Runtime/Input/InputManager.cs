using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input {
	public class InputManager : IDisposable {
		public List<Device> devices { get; private set; }

		public Action<Device> onDeviceConnect;

		public InputManager () {
			devices = new List<Device>();
			foreach (InputDevice device in InputSystem.devices)
				AddInputDevice(device);
			InputSystem.onDeviceChange += OnInputDeviceChange;
		}

		public void Dispose () {
			InputSystem.onDeviceChange -= OnInputDeviceChange;
		}

		private void OnInputDeviceChange (InputDevice inputDevice, InputDeviceChange change) {
			switch (change) {
				case InputDeviceChange.Added:
					AddInputDevice(inputDevice);
					break;
				case InputDeviceChange.Removed:
					RemoveInputDevice(inputDevice);
					break;
			}
		}

		private void AddInputDevice (InputDevice inputDevice) {
			// Check to see if this is a device that's been reconnected
			foreach (Device device in devices) {
				if (!device.isConnected && device.IsDefiniteMatchForInputDevice(inputDevice)) {
					device.Reconnect(inputDevice);
					return;
				}
			}
			foreach (Device device in devices) {
				Device plausibleDevice = null;
				if (!device.isConnected && device.IsPlausibleMatchForInputDevice(inputDevice)) {
					if (plausibleDevice == null || plausibleDevice.timeSinceLastConnected > device.timeSinceLastConnected)
						plausibleDevice = device;
				}
				if (plausibleDevice != null) {
					plausibleDevice.Reconnect(inputDevice);
					return;
				}
			}
			// Assume this is a totally new device
			Device newDevice = new Device(inputDevice);
			devices.Add(newDevice);
			onDeviceConnect?.Invoke(newDevice);
		}

		private void RemoveInputDevice (InputDevice inputDevice) {
			foreach (Device device in devices) {
				if (device.IsDefiniteMatchForInputDevice(inputDevice)) {
					if (device.isConnected)
						device.Disconnect();
					return;
				}
			}
		}
	}
}