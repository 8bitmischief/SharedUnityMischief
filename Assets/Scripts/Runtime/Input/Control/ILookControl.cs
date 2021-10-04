using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface ILookControl : IControl {
		Vector2 vector { get; }
		bool isMouseLookEnabled { get; }
		bool isUsingMouseLook { get; }
		void EnableMouseLook ();
		void DisableMouseLook ();
		Action onStartUsingMouseLook { get; set; }
		Action onStopUsingMouseLook { get; set; }
	}
}