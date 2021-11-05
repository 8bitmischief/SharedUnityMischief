using System;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(GameManager), true)]
	public class GameManagerEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			GameManager manager = (GameManager) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Manager State", EditorStyles.boldLabel);
			EditorGUILayout.FloatField("Time", manager.time);
			EditorGUILayout.IntField("Frame", manager.frame);
			EditorGUILayout.TextField("Percent Interpolated", String.Format("{0:0.00%}", manager.percentNextUpdateInterpolated));
		}
	}
}