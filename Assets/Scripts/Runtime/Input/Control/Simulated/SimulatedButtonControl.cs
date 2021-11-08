using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	[DefaultExecutionOrder(-73)]
	public class SimulatedButtonControl : MonoBehaviour, IButtonControl {
		[SerializeField] private ButtonControl control;

		public SimulatedControlMode mode {
			get => _mode;
			set {
				_mode = value;
				if (_mode == SimulatedControlMode.Simulate) {
					simulatedJustPressed = false;
					simulatedIsHeld = control.isHeld;
					simulatedJustReleased = false;
					simulatedHoldDuration = control.holdDuration;
				}
			}
		}

		public bool isActuated => mode == SimulatedControlMode.Simulate ? simulatedJustPressed || simulatedIsHeld || simulatedJustReleased : control.isActuated;
		public bool justPressed => mode == SimulatedControlMode.Simulate ? simulatedJustPressed : control.justPressed;
		public bool isHeld => mode == SimulatedControlMode.Simulate ? simulatedIsHeld : control.isHeld;
		public bool justReleased => mode == SimulatedControlMode.Simulate ? simulatedJustReleased : control.justReleased;
		public float holdDuration => mode == SimulatedControlMode.Simulate ? simulatedHoldDuration : control.holdDuration;
		public float amountHeldDown => mode == SimulatedControlMode.Simulate ? (simulatedIsHeld ? 1f : 0f) : control.amountHeldDown;

		public Action onPress { get; set; }
		public Action onRelease { get; set; }

		private SimulatedControlMode _mode;
		private bool simulatedJustPressed = false;
		private bool simulatedIsHeld = false;
		private bool simulatedJustReleased = false;
		private float simulatedHoldDuration = 0f;

		private void OnEnable () {
			control.onPress += OnPress;
			control.onRelease += OnRelease;
		}

		private void OnDisable () {
			control.onPress -= OnPress;
			control.onRelease -= OnRelease;
		}

		public void SimulateUpdate () => SimulateUpdate(Time.deltaTime);

		public void SimulateUpdate (float deltaTime) {
			if (mode == SimulatedControlMode.Simulate) {
				simulatedJustPressed = false;
				simulatedJustReleased = false;
				if (simulatedIsHeld)
					simulatedHoldDuration += deltaTime;
				if (!simulatedIsHeld && control.isHeld) {
					simulatedIsHeld = true;
					simulatedJustPressed = true;
					simulatedHoldDuration = 0f;
					onPress?.Invoke();
				}
				else if (simulatedIsHeld && !control.isHeld) {
					simulatedIsHeld = false;
					simulatedJustReleased = true;
					simulatedHoldDuration = 0f;
					onRelease?.Invoke();
				}
			}
		}

		public void ConsumeInstantaneousInputs () {
			if (mode == SimulatedControlMode.Simulate) {
				simulatedJustPressed = false;
				simulatedJustReleased = false;
			}
			else
				control.ConsumeInstantaneousInputs();
		}

		public bool ConsumePress () {
			if (mode == SimulatedControlMode.Simulate) {
				if (simulatedJustPressed) {
					simulatedJustPressed = false;
					return true;
				}
				else
					return false;
			}
			else
				return control.ConsumePress();
		}

		public bool ConsumeRelease () {
			if (mode == SimulatedControlMode.Simulate) {
				if (simulatedJustReleased) {
					simulatedJustReleased = false;
					return true;
				}
				else
					return false;
			}
			else
				return control.ConsumeRelease();
		}

		private void OnPress () {
			if (mode != SimulatedControlMode.Simulate)
				onPress?.Invoke();
		}

		private void OnRelease () {
			if (mode != SimulatedControlMode.Simulate)
				onRelease?.Invoke();
		}
	}
}