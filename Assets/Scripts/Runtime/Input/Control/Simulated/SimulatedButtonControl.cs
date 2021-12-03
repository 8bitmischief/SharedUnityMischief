using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control
{
	[DefaultExecutionOrder(-73)]
	public class SimulatedButtonControl : MonoBehaviour, IButtonControl
	{
		[SerializeField] private ButtonControl _control;
		private SimulatedControlMode _mode = SimulatedControlMode.PassThrough;
		private bool _simulatedJustPressed = false;
		private bool _simulatedIsHeld = false;
		private bool _simulatedJustReleased = false;
		private float _simulatedHoldDuration = 0f;

		public SimulatedControlMode mode
		{
			get => _mode;
			set
			{
				if (_mode != value)
				{
					_mode = value;
					if (_mode == SimulatedControlMode.Simulate)
					{
						_simulatedJustPressed = false;
						_simulatedIsHeld = _control.isHeld;
						_simulatedJustReleased = false;
						_simulatedHoldDuration = _control.holdDuration;
					}
				}
			}
		}
		public bool isActuated => mode == SimulatedControlMode.Simulate ? _simulatedJustPressed || _simulatedIsHeld || _simulatedJustReleased : _control.isActuated;
		public bool justPressed => mode == SimulatedControlMode.Simulate ? _simulatedJustPressed : _control.justPressed;
		public bool isHeld => mode == SimulatedControlMode.Simulate ? _simulatedIsHeld : _control.isHeld;
		public bool justReleased => mode == SimulatedControlMode.Simulate ? _simulatedJustReleased : _control.justReleased;
		public float holdDuration => mode == SimulatedControlMode.Simulate ? _simulatedHoldDuration : _control.holdDuration;
		public float amountHeldDown => mode == SimulatedControlMode.Simulate ? (_simulatedIsHeld ? 1f : 0f) : _control.amountHeldDown;

		public event Action onPress;
		public event Action onRelease;

		private void OnEnable()
		{
			_control.onPress += OnPress;
			_control.onRelease += OnRelease;
		}

		private void OnDisable()
		{
			_control.onPress -= OnPress;
			_control.onRelease -= OnRelease;
		}

		public void SimulateUpdate() => SimulateUpdate(Time.deltaTime);

		public void SimulateUpdate(float deltaTime)
		{
			if (_mode == SimulatedControlMode.Simulate)
			{
				_simulatedJustPressed = false;
				_simulatedJustReleased = false;
				if (_simulatedIsHeld)
				{
					_simulatedHoldDuration += deltaTime;
				}
				if (!_simulatedIsHeld && _control.isHeld)
				{
					_simulatedIsHeld = true;
					_simulatedJustPressed = true;
					_simulatedHoldDuration = 0f;
					onPress?.Invoke();
				}
				else if (_simulatedIsHeld && !_control.isHeld)
				{
					_simulatedIsHeld = false;
					_simulatedJustReleased = true;
					_simulatedHoldDuration = 0f;
					onRelease?.Invoke();
				}
			}
		}

		public void ConsumeInstantaneousInputs()
		{
			if (_mode == SimulatedControlMode.Simulate)
			{
				_simulatedJustPressed = false;
				_simulatedJustReleased = false;
			}
			else
			{
				_control.ConsumeInstantaneousInputs();
			}
		}

		public bool ConsumePress()
		{
			if (_mode == SimulatedControlMode.Simulate)
			{
				if (_simulatedJustPressed)
				{
					_simulatedJustPressed = false;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return _control.ConsumePress();
			}
		}

		public bool ConsumeRelease()
		{
			if (_mode == SimulatedControlMode.Simulate)
			{
				if (_simulatedJustReleased)
				{
					_simulatedJustReleased = false;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return _control.ConsumeRelease();
			}
		}

		private void OnPress()
		{
			if (_mode != SimulatedControlMode.Simulate)
			{
				onPress?.Invoke();
			}
		}

		private void OnRelease()
		{
			if (_mode != SimulatedControlMode.Simulate)
			{
				onRelease?.Invoke();
			}
		}
	}
}