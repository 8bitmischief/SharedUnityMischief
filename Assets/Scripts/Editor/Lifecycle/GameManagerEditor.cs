using System;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(GameManager), true)]
	public class GameManagerEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			GameManager manager = (GameManager) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Manager State", EditorStyles.boldLabel);
			EditorGUILayout.FloatField("Time", manager.time);
			EditorGUILayout.IntField("Frame", manager.frame);
			EditorGUILayout.TextField("Percent Interpolated", String.Format("{0:0.00%}", manager.percentNextUpdateInterpolated));

			GUI.enabled = wasEnabled;
		}
	}
}