using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control
{
	public class ButtonControl : ControlMonoBehaviour, IButtonControl
	{
		[Header("Inputs")]
		[SerializeField] private InputAction _input;
		private bool _justPressed = false;
		private bool _isHeld = false;
		private bool _justReleased = false;
		private float _amountHeldDown = 0f;
		private int _numPresses = 0;
		private int _numReleases = 0;
		private float timeLastPressed = 0f;

		public bool justPressed => _justPressed;
		public bool isHeld => _isHeld;
		public bool justReleased => _justReleased;
		public float amountHeldDown => _amountHeldDown;
		public float holdDuration => _isHeld ? Time.time - timeLastPressed : 0f;
		public override bool isActuated => _justPressed || _isHeld || _justReleased;

		public event Action onPress;
		public event Action onRelease;

		private void Awake()
		{
			RegisterInput(_input);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_input.started += OnPress;
			_input.canceled += OnRelease;
		}

		private void Update()
		{
			_justPressed = false;
			_justReleased = false;
			// Trigger releases/presses
			while (_numPresses > 0 || _numReleases > 0)
			{
				if (_isHeld)
				{
					if (_numReleases > 0)
					{
						_numReleases--;
						_isHeld = false;
						_justReleased = true;
						onRelease?.Invoke();
					}
					else
					{
						break;
					}
				}
				else
				{
					if (_numPresses > 0)
					{
						_numPresses--;
						_isHeld = true;
						_justPressed = true;
						timeLastPressed = Time.time;
						onPress?.Invoke();
					}
					else
					{
						break;
					}
				}
			}
			_numPresses = 0;
			_numReleases = 0;
			// Read the current state of the button
			_amountHeldDown = _input.ReadValue<float>();
			if (!_isHeld && _input.phase == InputActionPhase.Performed)
			{
				_isHeld = true;
				_justPressed = true;
				timeLastPressed = Time.time;
				onPress?.Invoke();
			}
			else if (_isHeld && (_input.phase == InputActionPhase.Waiting || _input.phase == InputActionPhase.Disabled))
			{
				_isHeld = false;
				_justReleased = true;
				onRelease?.Invoke();
			}
		}

		protected override void OnDisable()
		{
			_input.started -= OnPress;
			_input.canceled -= OnRelease;
			base.OnDisable();
		}

		public override void ConsumeInstantaneousInputs()
		{
			_justPressed = false;
			_justReleased = false;
		}

		public bool ConsumePress()
		{
			if (_justPressed)
			{
				_justPressed = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ConsumeRelease()
		{
			if (_justReleased)
			{
				_justReleased = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		private void OnPress (InputAction.CallbackContext context)
		{
			_numPresses++;
		}

		private void OnRelease (InputAction.CallbackContext context)
		{
			_numReleases++;
		}
	}
}