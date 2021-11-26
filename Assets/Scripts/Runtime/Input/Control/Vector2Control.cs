using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control
{
	public class Vector2Control : ControlMonoBehaviour, IVector2Control
	{
		[Header("Inputs")]
		[SerializeField] private InputAction _vectorInput;
		[Header("Settings")]
		[SerializeField] private AnimationCurve _pressureSensitivity = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		private Vector2 _vector;

		public Vector2 vector => _vector;
		public override bool isActuated => _vector.x != 0f || _vector.y != 0f;

		private void Awake()
		{
			RegisterInput(_vectorInput);
		}

		private void Update()
		{
			_vector = _vectorInput.ReadValue<Vector2>();
			if (_vector.sqrMagnitude > 0f)
			{
				float magnitude = _vector.magnitude;
				_vector *= _pressureSensitivity.Evaluate(Mathf.Clamp01(magnitude)) / magnitude;
			}
		}

		public override void ConsumeInstantaneousInputs() {}
	}
}