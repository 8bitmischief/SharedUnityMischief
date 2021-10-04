using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface IDoubleAxisControl : IControl {
		Vector2 vector { get; }
	}
}