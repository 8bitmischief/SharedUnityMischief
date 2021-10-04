using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface ISingleAxisControl : IControl {
		float value { get; }
	}
}