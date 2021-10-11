using UnityEngine;

namespace SharedUnityMischief.Input.Control {
	public interface IVector2Control : IControl {
		Vector2 vector { get; }
	}
}