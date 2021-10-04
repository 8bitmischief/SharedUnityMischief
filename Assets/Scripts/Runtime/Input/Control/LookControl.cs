using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class LookControl : ILookControl {
		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);
		public bool isMouseLookEnabled { get; private set; } = false;

		private InputAction buttonLookAction;
		private InputAction mouseLookAction;

		public LookControl (InputAction buttonLookAction, InputAction mouseLookAction = null) {
			this.buttonLookAction = buttonLookAction;
			this.mouseLookAction = mouseLookAction;
		}

		public void Update () {
			Vector2 buttonLookVector = buttonLookAction.ReadValue<Vector2>();
			if (isMouseLookEnabled && mouseLookAction != null && buttonLookVector.sqrMagnitude <= 0.0f)
				vector = mouseLookAction.ReadValue<Vector2>();
			else
				vector = buttonLookVector;
		}

		public void EnableMouseLook () {
			isMouseLookEnabled = true;
		}

		public void DisableMouseLook () {
			isMouseLookEnabled = false;
		}
	}
}