using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class ButtonControl : IButtonControl, IDisposable {
		private static readonly float PRESS_THRESHOLD = 0.65f;
		private static readonly float RELEASE_THRESHOLD = 0.25f;

		public bool justPressed { get; private set; } = false;
		public bool isHeld { get; private set; } = false;
		public bool justReleased { get; private set; } = false;
		public float holdDuration => isHeld ? Time.time - timeLastPressed : 0f;

		public Action onPress { get; set; }
		public Action onRelease { get; set; }

		private InputAction action;
		private int numPresses = 0;
		private int numReleases = 0;
		private float timeLastPressed = 0f;

		public ButtonControl (InputAction action) {
			this.action = action;
			action.started += OnPress;
			action.canceled += OnRelease;
		}

		public void Update () {
			justPressed = false;
			justReleased = false;
			// Trigger releases/presses
			while (numPresses > 0 || numReleases > 0) {
				if (isHeld) {
					if (numReleases > 0) {
						numReleases--;
						isHeld = false;
						justReleased = true;
						onRelease?.Invoke();
					}
					else
						break;
				}
				else {
					if (numPresses > 0) {
						numPresses--;
						isHeld = true;
						justPressed = true;
						timeLastPressed = Time.time;
						onPress?.Invoke();
					}
					else
						break;
				}
			}
			numPresses = 0;
			numReleases = 0;
			// Read the current state of the button
			float amountHeldDown = action.ReadValue<float>();
			if (!isHeld && amountHeldDown >= PRESS_THRESHOLD) {
				isHeld = true;
				justPressed = true;
				timeLastPressed = Time.time;
				onPress?.Invoke();
			}
			else if (isHeld && amountHeldDown <= RELEASE_THRESHOLD) {
				isHeld = false;
				justReleased = true;
				onRelease?.Invoke();
			}
		}

		public void Dispose () {
			action.started -= OnPress;
			action.canceled -= OnRelease;
		}

		private void OnPress (InputAction.CallbackContext context) {
			numPresses++;
		}

		private void OnRelease (InputAction.CallbackContext context) {
			numReleases++;
		}
	}
}