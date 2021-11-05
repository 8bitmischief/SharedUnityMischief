using System;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(UpdateLoop), true)]
	public class UpdateLoopEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			UpdateLoop manager = (UpdateLoop) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Update Loop State", EditorStyles.boldLabel);
			EditorGUILayout.FloatField("Time", manager.time);
			EditorGUILayout.IntField("Frame", manager.frame);
			EditorGUILayout.TextField("Percent Interpolated", String.Format("{0:0.00%}", manager.percentNextUpdateInterpolated));
		}
	}
}