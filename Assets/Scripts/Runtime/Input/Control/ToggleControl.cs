using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control
{
	[DefaultExecutionOrder(-74)]
	public class ToggleControl : ControlMonoBehaviour, IToggleControl
	{
		[Header("Inputs")]
		[SerializeField] private ButtonControl toggleButton;
		[SerializeField] private ButtonControl toggleOnButton;
		[SerializeField] private ButtonControl toggleOffButton;
		[Header("Settings")]
		[SerializeField] private bool startOn = false;

		public bool isOn { get; private set; } = false;
		public bool justToggledOn { get; private set; } = false;
		public bool justToggledOff { get; private set; } = false;
		public bool justToggled => justToggledOn || justToggledOff;
		public override bool isActuated => justToggled;

		public event Action<bool> onToggle;

		private void Awake()
		{
			isOn = startOn;
		}

		private void Update()
		{
			justToggledOn = false;
			justToggledOff = false;
			if ((toggleButton != null && toggleButton.justPressed) ||
				(toggleOnButton != null && toggleOnButton.justPressed && !isOn) ||
				(toggleOffButton != null && toggleOffButton.justPressed && isOn))
			{
				Toggle();
			}
		}

		public void Toggle(bool triggerEvents = true)
		{
			isOn = !isOn;
			if (isOn)
			{
				justToggledOn = true;
			}
			else
			{
				justToggledOff = true;
			}
			if (triggerEvents)
			{
				onToggle?.Invoke(isOn);
			}
		}

		public void Toggle(bool isOn, bool triggerEvents = true)
		{
			if (isOn != this.isOn)
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
			justToggledOn = false;
			justToggledOff = false;
		}
	}
}