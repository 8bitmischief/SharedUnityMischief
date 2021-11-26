using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control
{
	[DefaultExecutionOrder(-75)]
	public abstract class ControlMonoBehaviour : MonoBehaviour, IControl
	{
		private List<InputAction> _inputs = new List<InputAction>();

		public abstract bool isActuated { get; }

		public abstract void ConsumeInstantaneousInputs();

		protected virtual void OnEnable()
		{
			foreach (InputAction input in _inputs)
			{
				input.Enable();
			}
		}

		protected virtual void OnDisable()
		{
			foreach (InputAction input in _inputs)
			{
				input.Disable();
			}
		}

		protected virtual void OnDestroy()
		{
			foreach (InputAction input in _inputs)
			{
				input.Dispose();
			}
		}

		protected void RegisterInput(InputAction input)
		{
			if (input != null)
			{
				_inputs.Add(input);
			}
		}
	}
}