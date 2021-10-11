using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control {
	[CustomEditor(typeof(Vector2Control), true)]
	public class DoubleAxisControlEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			Vector2Control control = (Vector2Control) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Vector2Field("Vector", control.vector);

			GUI.enabled = wasEnabled;
		}
	}
}