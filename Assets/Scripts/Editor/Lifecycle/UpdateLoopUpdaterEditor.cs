using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(UpdateLoopUpdater), true)]
	public class UpdateLoopUpdaterEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		private float manualAdvanceTime = UpdateLoop.timePerUpdate;

		protected override void DrawControls () {
			UpdateLoopUpdater updater = (UpdateLoopUpdater) target;

			// Show controls for manually manipulating the game's lifecycle
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
			if (GUILayout.Button(updater.isPaused ? "Resume" : "Pause"))
				updater.isPaused = !updater.isPaused;
			EditorGUILayout.BeginHorizontal();
			manualAdvanceTime = EditorGUILayout.FloatField(manualAdvanceTime);
			if (GUILayout.Button("Update")) {
				if (!updater.isPaused)
					updater.isPaused = true;
				updater.UpdateState(manualAdvanceTime, true);
			}
			if (GUILayout.Button("Next Frame")) {
				if (!updater.isPaused)
					updater.isPaused = true;
				updater.UpdateStateOneFrame(true);
			}
			GUILayout.EndHorizontal();
		}
	}
}