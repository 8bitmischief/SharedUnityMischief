using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class Vector2Control : ControlMonoBehaviour, IVector2Control {
		[Header("Inputs")]
		[SerializeField] private InputAction vectorInput;

		[Header("Settings")]
		public Curve pressureSensitivity = Curve.Linear(0f, 1f);

		public override bool isActuated => vector.x != 0f || vector.y != 0f;
		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);

		private void Awake () {
			RegisterInput(vectorInput);
		}

		private void Update () {
			vector = vectorInput.ReadValue<Vector2>();
			if (vector.sqrMagnitude > 0f) {
				float magnitude = vector.magnitude;
				vector *= pressureSensitivity.Evaluate(Mathf.Clamp01(magnitude)) / magnitude;
			}
		}

		public override void ConsumeInstantaneousInputs () {}
	}
}