using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface IButtonControl : IControl {
		bool justPressed { get; }
		bool isHeld { get; }
		bool justReleased { get; }
		float holdDuration { get; }
		Action onPress { get; set; }
		Action onRelease { get; set; }
		bool ConsumePress ();
		bool ConsumeRelease ();
	}
}