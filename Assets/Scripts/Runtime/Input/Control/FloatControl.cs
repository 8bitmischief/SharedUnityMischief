using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control
{
	public class FloatControl : ControlMonoBehaviour, IFloatControl
	{
		[Header("Inputs")]
		[SerializeField] private InputAction _positiveInput;
		[SerializeField] private InputAction _negativeInput;
		private float _value = 0f;

		public float value => _value;
		public override bool isActuated => _value != 0f;

		private void Awake()
		{
			RegisterInput(_positiveInput);
			RegisterInput(_negativeInput);
		}

		private void Update()
		{
			_value = (_positiveInput?.ReadValue<float>() ?? 0f) - (_negativeInput?.ReadValue<float>() ?? 0f);
		}

		public override void ConsumeInstantaneousInputs() {}
	}
}