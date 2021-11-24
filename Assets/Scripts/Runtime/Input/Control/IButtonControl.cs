using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control
{
	public interface IButtonControl : IControl
	{
		bool justPressed { get; }
		bool isHeld { get; }
		bool justReleased { get; }
		float holdDuration { get; }
		event Action onPress;
		event Action onRelease;
		bool ConsumePress();
		bool ConsumeRelease();
	}
}