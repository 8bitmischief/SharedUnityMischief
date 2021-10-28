using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Animation {
	[CustomEditor(typeof(EnumStateMachineAnimator), true)]
	public class EnumStateMachineAnimatorEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			EnumStateMachineAnimator animator = (EnumStateMachineAnimator) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.TextField("State", animator.stateName);
			EditorGUILayout.FloatField("Time In State", animator.timeInState);

			GUI.enabled = wasEnabled;
		}
	}
}