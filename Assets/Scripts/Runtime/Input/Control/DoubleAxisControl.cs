using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public class DoubleAxisControl : IDoubleAxisControl {
		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);

		private InputAction action;

		public DoubleAxisControl (InputAction action) {
			this.action = action;
		}

		public void Update () {
			vector = action.ReadValue<Vector2>();
		}
	}
}