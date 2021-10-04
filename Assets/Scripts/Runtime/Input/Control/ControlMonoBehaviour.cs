using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control {
	public abstract class ControlMonoBehaviour : MonoBehaviour, IControl {
		private List<InputAction> inputs = new List<InputAction>();

		protected virtual void OnEnable () {
			foreach (InputAction input in inputs)
				input.Enable();
		}

		protected virtual void OnDisable () {
			foreach (InputAction input in inputs)
				input.Disable();
		}

		protected virtual void OnDestroy () {
			foreach (InputAction input in inputs)
				input.Dispose();
		}

		protected void RegisterInput (InputAction input) {
			if (input != null)
				inputs.Add(input);
		}
	}
}