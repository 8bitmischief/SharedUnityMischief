using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control
{
	public class LookControl : ControlMonoBehaviour, ILookControl
	{
		[Header("Inputs")]
		[SerializeField] private InputAction _mouseInput;
		[SerializeField] private InputAction _buttonInput;
		[Header("Settings")]
		[SerializeField] private Vector2 _mouseSensitivity = Vector2.one;
		[SerializeField] private Vector2 _nonMouseSensitivity = Vector2.one;
		[SerializeField] private AnimationCurve _nonMousePressureSensitivity = AnimationCurve.Linear(0f, 0f, 1f, 1f);
		private Vector2 _vector = Vector2.zero;
		private bool _isMouseLookEnabled = false;
		private bool _isUsingMouseLook = false;
		private int _numMouseUpdatesToSkip = 0;
		private Vector2[] _recentVectors = new Vector2[60];
		private int _nextVectorIndex = 0;

		public Vector2 vector => _vector;
		public bool isMouseLookEnabled => _isMouseLookEnabled;
		public bool isUsingMouseLook => _isUsingMouseLook;
		public int numMouseUpdatesToSkip => _numMouseUpdatesToSkip;
		public override bool isActuated => _vector.x != 0f || _vector.y != 0f;
		public Vector2 recentAverageVector => _recentVectors.Aggregate<Vector2>((a, b) => a + b) / _recentVectors.Length;

		public event Action onStartUsingMouseLook;
		public event Action onStopUsingMouseLook;

		private void Awake()
		{
			RegisterInput(_mouseInput);
			RegisterInput(_buttonInput);
		}

		private void Update()
		{
			Vector2 mouseVector = _mouseInput?.ReadValue<Vector2>() ?? Vector2.zero;
			Vector2 buttonVector = _buttonInput?.ReadValue<Vector2>() ?? Vector2.zero;
			if (_isMouseLookEnabled && buttonVector.sqrMagnitude <= 0f && _numMouseUpdatesToSkip == 0)
			{
				_vector = CalculateLookVector(mouseVector, _mouseSensitivity, null, true);
				if (_vector.sqrMagnitude > 0f && !_isUsingMouseLook)
				{
					_isUsingMouseLook = true;
					onStartUsingMouseLook?.Invoke();
				}
			}
			else
			{
				_vector = CalculateLookVector(buttonVector, _nonMouseSensitivity, _nonMousePressureSensitivity, false);
				if (_vector.sqrMagnitude > 0f && _isUsingMouseLook)
				{
					_isUsingMouseLook = false;
					onStopUsingMouseLook?.Invoke();
				}
			}
			if (mouseVector.sqrMagnitude > 0f && _numMouseUpdatesToSkip > 0)
			{
				_numMouseUpdatesToSkip--;
			}
			_recentVectors[_nextVectorIndex] = _vector;
			_nextVectorIndex = (_nextVectorIndex + 1) % _recentVectors.Length;
		}

		public void EnableMouseLook()
		{
			if (!_isMouseLookEnabled)
			{
				_isMouseLookEnabled = true;
				_numMouseUpdatesToSkip = 4;
			}
		}

		public void DisableMouseLook()
		{
			_isMouseLookEnabled = false;
		}

		public override void ConsumeInstantaneousInputs() {}

		private Vector2 CalculateLookVector(Vector2 vector, Vector2 sensitivity, AnimationCurve sensitivityCurve, bool denormalizeTime)
		{
			if (vector.x != 0f || vector.y != 0f)
			{
				if (sensitivityCurve != null)
				{
					float magnitude = vector.magnitude;
					vector *= sensitivityCurve.Evaluate(Mathf.Clamp01(magnitude)) / magnitude;
				}
				vector.Scale(sensitivity);
				if (denormalizeTime)
				{
					vector /= 60f * Time.deltaTime;
				}
			}
			return vector;
		}
	}
}