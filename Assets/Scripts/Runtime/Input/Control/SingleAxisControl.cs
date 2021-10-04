using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class SingleAxisControl : ControlMonoBehaviour, ISingleAxisControl {
		public float value { get; private set; } = 0f;

		[Header("Inputs")]
		[SerializeField] private InputAction positiveInput;
		[SerializeField] private InputAction negativeInput;

		private void Awake () {
			RegisterInput(positiveInput);
			RegisterInput(negativeInput);
		}

		private void Update () {
			value = (positiveInput?.ReadValue<float>() ?? 0f) - (negativeInput?.ReadValue<float>() ?? 0f);
		}
	}
}