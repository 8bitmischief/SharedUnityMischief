using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class DoubleAxisControl : ControlMonoBehaviour, IDoubleAxisControl {
		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);

		[Header("Inputs")]
		[SerializeField] private InputAction vectorInput;

		private void Awake () {
			RegisterInput(vectorInput);
		}

		private void Update () {
			vector = vectorInput.ReadValue<Vector2>();
		}
	}
}