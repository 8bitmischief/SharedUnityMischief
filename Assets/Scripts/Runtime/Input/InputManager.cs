using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input {
	[DefaultExecutionOrder(-80)]
	public class InputManager : MonoBehaviour {
		protected virtual void Update () {
			InputSystem.Update();
		}
	}
}