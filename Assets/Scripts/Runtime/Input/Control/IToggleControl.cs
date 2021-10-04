using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface IToggleControl : IControl {
		bool isOn { get; }
		bool justToggled { get; }
		bool justToggledOn { get; }
		bool justToggledOff { get; }
		Action<bool> onToggle { get; set; }
		void Toggle(bool triggerEvents = true);
		void Toggle(bool isOn, bool triggerEvents = true);
	}
}