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
				GUI.enabled = Application.isPlaying;
				DrawControls();
				GUI.enabled = false;
				DrawState();
				GUI.enabled = wasEnabled;
			}
		}

		protected virtual void DrawControls() {}
		protected virtual void DrawState() {}
	}
}