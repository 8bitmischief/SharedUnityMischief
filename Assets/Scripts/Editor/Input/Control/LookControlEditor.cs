using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control {
	[CustomEditor(typeof(LookControl), true)]
	public class LookControlEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			LookControl control = (LookControl) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Vector2Field("Vector", control.vector);
			EditorGUILayout.Toggle("Is Mouse Look Enabled", control.isMouseLookEnabled);
			EditorGUILayout.Toggle("Is Using Mouse Look", control.isUsingMouseLook);

			GUI.enabled = wasEnabled;
		}
	}
}