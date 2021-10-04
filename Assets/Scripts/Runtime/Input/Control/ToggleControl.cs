using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public class ToggleControl : ControlMonoBehaviour, IToggleControl {
		[Header("Inputs")]
		[SerializeField] private ButtonControl toggleButton;
		[SerializeField] private ButtonControl toggleOnButton;
		[SerializeField] private ButtonControl toggleOffButton;

		[Header("Settings")]
		[SerializeField] private bool startOn = false;

		public bool isOn => hasInitialized ? _isOn : startOn;
		public bool justToggled { get; private set; } = false;
		public bool justToggledOn { get; private set; } = false;
		public bool justToggledOff { get; private set; } = false;

		public Action<bool> onToggle { get; set; }

		private bool _isOn = false;
		private bool hasInitialized = false;

		private void Awake () {
			_isOn = startOn;
			hasInitialized = true;
		}

		private void Update () {
			justToggled = false;
			justToggledOn = false;
			justToggledOff = false;
			if ((toggleButton != null && toggleButton.justPressed) ||
				(toggleOnButton != null && toggleOnButton.justPressed && !isOn) ||
				(toggleOffButton != null && toggleOffButton.justPressed && isOn))
				Toggle();
		}

		public void Toggle (bool triggerEvents = true) {
			_isOn = !_isOn;
			justToggled = true;
			if (_isOn)
				justToggledOn = true;
			else
				justToggledOff = true;
			if (triggerEvents)
				onToggle?.Invoke(_isOn);
		}

		public void Toggle (bool isOn, bool triggerEvents = true) {
			if (isOn != _isOn)
				Toggle(triggerEvents);
		}
	}
}