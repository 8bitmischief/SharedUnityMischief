using System;
using UnityEngine;

namespace SharedUnityMischief.Input.Control
{
	public interface ILookControl : IControl
	{
		Vector2 vector { get; }
		bool isMouseLookEnabled { get; }
		bool isUsingMouseLook { get; }
		event Action onStartUsingMouseLook;
		event Action onStopUsingMouseLook;
		void EnableMouseLook();
		void DisableMouseLook();
	}
}