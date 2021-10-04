using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class LookControl : ILookControl {
		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);
		public bool isMouseLookEnabled { get; private set; } = false;
		public bool isUsingMouseLook { get; private set; } = false;

		private InputAction buttonLookAction;
		private InputAction mouseLookAction;
		private LookSettings settings = new LookSettings();

		public LookControl (InputAction buttonLookAction, InputAction mouseLookAction = null, LookSettings settings = null) {
			this.buttonLookAction = buttonLookAction;
			this.mouseLookAction = mouseLookAction;
			if (settings != null)
				this.settings = settings;
		}

		public void Update () {
			Vector2 buttonLookVector = buttonLookAction.ReadValue<Vector2>();
			if (isMouseLookEnabled && mouseLookAction != null && buttonLookVector.sqrMagnitude <= 0.0f) {
				vector = CalculateLookVector(mouseLookAction.ReadValue<Vector2>(), settings.mouseLookSensitivity);
				if (vector.sqrMagnitude > 0.0f)
					isUsingMouseLook = true;
			}
			else {
				vector = CalculateLookVector(buttonLookVector, settings.buttonLookSensitivity, settings.buttonLookCurve);
				if (vector.sqrMagnitude > 0.0f)
					isUsingMouseLook = false;
			}
		}

		public void EnableMouseLook () {
			isMouseLookEnabled = true;
		}

		public void DisableMouseLook () {
			isMouseLookEnabled = false;
		}

		public void SetSettings(LookSettings settings) {
			this.settings = settings;
		}

		private Vector2 CalculateLookVector (Vector2 vector, Vector2 sensitivity, AnimationCurve curve = null) {
			if (vector.x != 0.0f || vector.y != 0.0f) {
				if (curve != null) {
					float magnitude = vector.magnitude;
					vector *= curve.Evaluate(Mathf.Clamp01(magnitude)) / magnitude;
				}
				vector.Scale(sensitivity);
			}
			return vector;
		}

		[Serializable]
		public class LookSettings {
			public Vector2 mouseLookSensitivity = Vector2.one;
			public Vector2 buttonLookSensitivity = Vector2.one;
			public AnimationCurve buttonLookCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		}
	}
}