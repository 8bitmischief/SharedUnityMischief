using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input
{
	[DefaultExecutionOrder(-80)]
	public class InputManager : MonoBehaviour
	{
		[Header("Input System")]
		[SerializeField] private bool updateInputSystem = false;

		protected virtual void Update()
		{
			if (updateInputSystem)
				InputSystem.Update();
		}
	}
}