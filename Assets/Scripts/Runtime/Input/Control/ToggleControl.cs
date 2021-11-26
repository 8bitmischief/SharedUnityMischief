using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control
{
	[DefaultExecutionOrder(-74)]
	public class ToggleControl : ControlMonoBehaviour, IToggleControl
	{
		[Header("Inputs")]
		[SerializeField] private ButtonControl _toggleButton;
		[SerializeField] private ButtonControl _toggleOnButton;
		[SerializeField] private ButtonControl _toggleOffButton;
		[Header("Settings")]
		[SerializeField] private bool _startOn = false;
		private bool _isOn = false;
		private bool _justToggledOn = false;
		private bool _justToggledOff = false;

		public bool isOn => _isOn;
		public bool justToggledOn => _justToggledOn;
		public bool justToggledOff => _justToggledOff;
		public bool justToggled => _justToggledOn || _justToggledOff;
		public override bool isActuated => _justToggledOn || _justToggledOff;

		public event Action<bool> onToggle;

		private void Awake()
		{
			_isOn = _startOn;
		}

		private void Update()
		{
			_justToggledOn = false;
			_justToggledOff = false;
			if ((_toggleButton != null && _toggleButton.justPressed) ||
				(_toggleOnButton != null && _toggleOnButton.justPressed && !_isOn) ||
				(_toggleOffButton != null && _toggleOffButton.justPressed && _isOn))
			{
				Toggle();
			}
		}

		public void Toggle(bool triggerEvents = true)
		{
			_isOn = !_isOn;
			if (_isOn)
			{
				_justToggledOn = true;
			}
			else
			{
				_justToggledOff = true;
			}
			if (triggerEvents)
			{
				onToggle?.Invoke(_isOn);
			}
		}

		public void Toggle(bool isOn, bool triggerEvents = true)
		{
			if (isOn != _isOn)
			{
				Toggle(triggerEvents);
			}
		}

		public void ToggleOn(bool triggerEvents = true)
			=> Toggle(true, triggerEvents);

		public void ToggleOff(bool triggerEvents = true)
			=> Toggle(false, triggerEvents);

		public override void ConsumeInstantaneousInputs()
		{
			_justToggledOn = false;
			_justToggledOff = false;
		}
	}
}