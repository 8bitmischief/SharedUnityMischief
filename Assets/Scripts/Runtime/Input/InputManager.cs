using UnityEngine;
using UnityEngine.InputSystem;

namespace SharedUnityMischief.Input {
	public class InputManager : MonoBehaviour {
		protected virtual void Update () {
			InputSystem.Update();
		}
	}
}