using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class LookControl : ControlMonoBehaviour, ILookControl {
		[Header("Inputs")]
		[SerializeField] private InputAction mouseInput;
		[SerializeField] private InputAction buttonInput;

		[Header("Settings")]
		public Vector2 mouseSensitivity = Vector2.one;
		public Vector2 buttonSensitivity = Vector2.one;
		public AnimationCurve buttonPressureCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);
		public bool isMouseLookEnabled { get; private set; } = false;
		public bool isUsingMouseLook { get; private set; } = false;

		public Action onStartUsingMouseLook { get; set; }
		public Action onStopUsingMouseLook { get; set; }

		private int numMouseUpdatesToSkip = 0;

		private void Awake () {
			RegisterInput(mouseInput);
			RegisterInput(buttonInput);
		}

		private void Update () {
			Vector2 mouseVector = mouseInput?.ReadValue<Vector2>() ?? Vector2.zero;
			Vector2 buttonVector = buttonInput?.ReadValue<Vector2>() ?? Vector2.zero;
			if (isMouseLookEnabled && buttonVector.sqrMagnitude <= 0f && numMouseUpdatesToSkip == 0) {
				vector = CalculateLookVector(mouseVector, mouseSensitivity);
				if (vector.sqrMagnitude > 0f && !isUsingMouseLook) {
					isUsingMouseLook = true;
					onStartUsingMouseLook?.Invoke();
				}
			}
			else {
				vector = CalculateLookVector(buttonVector, buttonSensitivity, buttonPressureCurve);
				if (vector.sqrMagnitude > 0f && isUsingMouseLook) {
					isUsingMouseLook = false;
					onStopUsingMouseLook?.Invoke();
				}
			}
			if (mouseVector.sqrMagnitude > 0f && numMouseUpdatesToSkip > 0)
				numMouseUpdatesToSkip--;
		}

		public void EnableMouseLook () {
			isMouseLookEnabled = true;
			numMouseUpdatesToSkip = 4;
		}

		public void DisableMouseLook () {
			isMouseLookEnabled = false;
		}

		private Vector2 CalculateLookVector (Vector2 vector, Vector2 sensitivity, AnimationCurve curve = null) {
			if (vector.x != 0f || vector.y != 0f) {
				if (curve != null) {
					float magnitude = vector.magnitude;
					vector *= curve.Evaluate(Mathf.Clamp01(magnitude)) / magnitude;
				}
				vector.Scale(sensitivity);
			}
			return vector;
		}
	}
}