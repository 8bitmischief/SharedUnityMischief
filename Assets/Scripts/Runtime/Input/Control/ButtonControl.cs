using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class ButtonControl : ControlMonoBehaviour, IButtonControl {
		[Header("Inputs")]
		[SerializeField] private InputAction input;

		public override bool isActuated => justPressed || isHeld || justReleased;
		public bool justPressed { get; private set; } = false;
		public bool isHeld { get; private set; } = false;
		public bool justReleased { get; private set; } = false;
		public float holdDuration => isHeld ? Time.time - timeLastPressed : 0f;
		public float amountHeldDown { get; private set; } = 0f;

		public Action onPress { get; set; }
		public Action onRelease { get; set; }

		private int numPresses = 0;
		private int numReleases = 0;
		private float timeLastPressed = 0f;

		private void Awake () {
			RegisterInput(input);
		}

		protected override void OnEnable () {
			base.OnEnable();
			input.started += OnPress;
			input.canceled += OnRelease;
		}

		private void Update () {
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
			amountHeldDown = input.ReadValue<float>();
			 if (!isHeld && input.phase == InputActionPhase.Performed) {
			 	isHeld = true;
			 	justPressed = true;
			 	timeLastPressed = Time.time;
			 	onPress?.Invoke();
			 }
			 else if (isHeld && (input.phase == InputActionPhase.Waiting || input.phase == InputActionPhase.Disabled)) {
			 	isHeld = false;
			 	justReleased = true;
			 	onRelease?.Invoke();
			 }
		}

		protected override void OnDisable () {
			input.started -= OnPress;
			input.canceled -= OnRelease;
			base.OnDisable();
		}

		private void OnPress (InputAction.CallbackContext context) {
			numPresses++;
		}

		private void OnRelease (InputAction.CallbackContext context) {
			numReleases++;
		}
	}
}