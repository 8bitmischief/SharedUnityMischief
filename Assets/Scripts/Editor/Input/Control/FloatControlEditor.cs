using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control {
	[CustomEditor(typeof(FloatControl), true)]
	public class FloatControlEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			FloatControl control = (FloatControl) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.FloatField("Value", control.value);

			GUI.enabled = wasEnabled;
		}
	}
}