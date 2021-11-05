using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Animation {
	[CustomEditor(typeof(EnumStateMachineAnimator), true)]
	public class EnumStateMachineAnimatorEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			EnumStateMachineAnimator animator = (EnumStateMachineAnimator) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.TextField("State", animator.stateName);
			EditorGUILayout.FloatField("Time In State", animator.timeInState);
		}
	}
}