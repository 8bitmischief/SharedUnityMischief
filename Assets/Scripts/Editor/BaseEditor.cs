using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief
{
	public abstract class BaseEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if (Application.isPlaying)
			{
				bool wasEnabled = GUI.enabled;
				GUI.enabled = true;
				DrawControls();
				GUI.enabled = false;
				DrawState();
				GUI.enabled = wasEnabled;
			}
			else
			{
				bool wasEnabled = GUI.enabled;
				GUI.enabled = true;
				DrawEditModeControls();
				GUI.enabled = false;
				DrawEditModeState();
				GUI.enabled = wasEnabled;
			}
		}

		protected virtual void DrawControls() {}
		protected virtual void DrawEditModeControls() {}
		protected virtual void DrawState() {}
		protected virtual void DrawEditModeState() {}
	}
}