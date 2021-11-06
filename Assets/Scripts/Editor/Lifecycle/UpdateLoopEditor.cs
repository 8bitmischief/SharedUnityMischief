using System;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(UpdateLoop), true)]
	public class UpdateLoopEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		private float manualAdvanceTime = UpdateLoop.timePerUpdate;

		protected override void DrawControls () {
			UpdateLoop loop = (UpdateLoop) target;

			// Show controls for manually manipulating the game's lifecycle
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Update Loop Controls", EditorStyles.boldLabel);
			if (GUILayout.Button(loop.isPaused ? "Resume" : "Pause")) {
				if (loop.isPaused)
					loop.Resume();
				else
					loop.Pause();
			}
			EditorGUILayout.BeginHorizontal();
			manualAdvanceTime = EditorGUILayout.FloatField(manualAdvanceTime);
			if (GUILayout.Button("Update")) {
				if (!loop.isPaused)
					loop.Pause();
				loop.Advance(manualAdvanceTime, true);
			}
			if (GUILayout.Button("Next Frame")) {
				if (!loop.isPaused)
					loop.Pause();
				loop.AdvanceOneFrame(true);
			}
			GUILayout.EndHorizontal();
		}

		protected override void DrawState () {
			UpdateLoop loop = (UpdateLoop) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Update Loop State", EditorStyles.boldLabel);
			EditorGUILayout.FloatField("Time", loop.time);
			EditorGUILayout.IntField("Frame", loop.frame);
			EditorGUILayout.TextField("Percent Interpolated", String.Format("{0:0.00%}", loop.percentNextUpdateInterpolated));
		}
	}
}