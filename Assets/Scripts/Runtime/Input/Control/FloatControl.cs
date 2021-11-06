using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class FloatControl : ControlMonoBehaviour, IFloatControl {
		[Header("Inputs")]
		[SerializeField] private InputAction positiveInput;
		[SerializeField] private InputAction negativeInput;

		public override bool isActuated => value != 0f;
		public float value { get; private set; } = 0f;

		private void Awake () {
			RegisterInput(positiveInput);
			RegisterInput(negativeInput);
		}

		private void Update () {
			value = (positiveInput?.ReadValue<float>() ?? 0f) - (negativeInput?.ReadValue<float>() ?? 0f);
		}

		public override void ConsumeInstantaneousInputs () {}
	}
}