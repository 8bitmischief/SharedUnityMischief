using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface IFloatControl : IControl {
		float value { get; }
	}
}