using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input.Control
{
	public class LookControl : ControlMonoBehaviour, ILookControl
	{
		[Header("Inputs")]
		[SerializeField] private InputAction mouseInput;
		[SerializeField] private InputAction buttonInput;

		[Header("Settings")]
		public Vector2 mouseSensitivity = Vector2.one;
		public Vector2 nonMouseSensitivity = Vector2.one;
		public AnimationCurve nonMousePressureSensitivity = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public Vector2 vector { get; private set; } = new Vector2(0f, 0f);
		public bool isMouseLookEnabled { get; private set; } = false;
		public bool isUsingMouseLook { get; private set; } = false;
		public override bool isActuated => vector.x != 0f || vector.y != 0f;
		public Vector2 recentAverageVector => recentVectors.Aggregate<Vector2>((a, b) => a + b) / recentVectors.Length;

		public event Action onStartUsingMouseLook;
		public event Action onStopUsingMouseLook;

		private int numMouseUpdatesToSkip = 0;
		private Vector2[] recentVectors = new Vector2[60];
		private int nextVectorIndex = 0;

		private void Awake()
		{
			RegisterInput(mouseInput);
			RegisterInput(buttonInput);
		}

		private void Update()
		{
			Vector2 mouseVector = mouseInput?.ReadValue<Vector2>() ?? Vector2.zero;
			Vector2 buttonVector = buttonInput?.ReadValue<Vector2>() ?? Vector2.zero;
			if (isMouseLookEnabled && buttonVector.sqrMagnitude <= 0f && numMouseUpdatesToSkip == 0)
			{
				vector = CalculateLookVector(mouseVector, mouseSensitivity, null, true);
				if (vector.sqrMagnitude > 0f && !isUsingMouseLook)
				{
					isUsingMouseLook = true;
					onStartUsingMouseLook?.Invoke();
				}
			}
			else
			{
				vector = CalculateLookVector(buttonVector, nonMouseSensitivity, nonMousePressureSensitivity, false);
				if (vector.sqrMagnitude > 0f && isUsingMouseLook)
				{
					isUsingMouseLook = false;
					onStopUsingMouseLook?.Invoke();
				}
			}
			if (mouseVector.sqrMagnitude > 0f && numMouseUpdatesToSkip > 0)
			{
				numMouseUpdatesToSkip--;
			}
			recentVectors[nextVectorIndex] = vector;
			nextVectorIndex = (nextVectorIndex + 1) % recentVectors.Length;
		}

		public void EnableMouseLook()
		{
			if (!isMouseLookEnabled)
			{
				isMouseLookEnabled = true;
				numMouseUpdatesToSkip = 4;
			}
		}

		public void DisableMouseLook()
		{
			isMouseLookEnabled = false;
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