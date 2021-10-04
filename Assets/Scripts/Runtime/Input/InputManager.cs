using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using SharedUnityMischief.Input.Control;

namespace SharedUnityMischief.Input {
	public class InputManager : IDisposable {
		private List<IControl> controls = new List<IControl>();

		public InputManager () {}

		public virtual void Update () {
			InputSystem.Update();
			foreach (IControl control in controls)
				control.Update();
		}

		public virtual void Dispose () {}

		public T RegisterControl<T> (T control) where T : IControl {
			controls.Add(control);
			return control;
		}

		public bool UnregisterControl (IControl control) {
			return controls.Remove(control);
		}
	}
}