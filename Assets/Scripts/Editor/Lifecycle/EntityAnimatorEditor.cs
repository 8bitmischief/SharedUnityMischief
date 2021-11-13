using System;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(EntityAnimator), true)]
	public class EntityAnimatorEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		private bool showAnimationDetails = true;

		protected override void DrawState () {
			EntityAnimator animator = (EntityAnimator) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Animator State", EditorStyles.boldLabel);
			EditorGUILayout.TextField("State", animator.stateName);
			EditorGUILayout.FloatField("Time In State", animator.timeInState);
			EditorGUILayout.IntField("Frames In State", animator.framesInState);

			showAnimationDetails = EditorGUILayout.Foldout(showAnimationDetails, "Animation");
			if (showAnimationDetails) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.FloatField("Time", animator.animationTime);
				EditorGUILayout.FloatField(animator.animationDuration, GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.IntField("Frame", animator.animationFrame);
				EditorGUILayout.IntField(animator.animationFrameDuration, GUILayout.Width(50));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.TextField("Percent Completed", String.Format("{0:0.00%}", animator.percentAnimationCompleted));
				EditorGUILayout.Toggle("Has Completed", animator.hasAnimationCompleted);
				EditorGUILayout.Toggle("Is Looping", animator.isAnimationLooping);
				EditorGUILayout.Toggle("Has Looped", animator.hasAnimationLooped);
				EditorGUILayout.TextField("Percent Into Frame", String.Format("{0:0.00%}", animator.percentInterpolated));
				EditorGUILayout.Vector3Field("Root Motion", animator.programmaticRootMotion);
				EditorGUILayout.Vector3Field("Root Motion Progress", animator.programmaticRootMotionProgress);
			}
		}
	}
}