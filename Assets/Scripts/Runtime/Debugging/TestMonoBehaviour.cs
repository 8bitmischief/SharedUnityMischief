using UnityEngine;

namespace SharedUnityMischief.Debugging {
	public class TestMonoBehaviour : MonoBehaviour {
		protected virtual string startupMessage => "TestMonoBehavior started successfully!";

		private void Start() {
			Debug.Log(startupMessage);
		}
	}
}