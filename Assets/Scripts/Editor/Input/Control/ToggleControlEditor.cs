using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control {
	[CustomEditor(typeof(ToggleControl), true)]
	public class ToggleControlEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		private float lastToggleTime = -999f;

		private void OnEnable () {
			ToggleControl control = (ToggleControl) target;
			control.onToggle += OnToggle;
		}

		private void OnDisable () {
			ToggleControl control = (ToggleControl) target;
			control.onToggle += OnToggle;
		}

		public override void OnInspectorGUI () {
			ToggleControl control = (ToggleControl) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Toggle("Is On", control.isOn);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
			float toggleHighlight = 1f - Math.Map(Time.time - lastToggleTime, 0.1f, 0.4f, 0f, 1f, true);
			EditorGUILayout.ColorField("Toggle", new Color(toggleHighlight, toggleHighlight, 0.0f, 1.0f));

			GUI.enabled = wasEnabled;
		}

		private void OnToggle (bool isOn) {
			lastToggleTime = Time.time;
		}
	}
}