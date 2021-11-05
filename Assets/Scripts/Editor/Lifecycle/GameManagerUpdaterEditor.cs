using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(GameManagerUpdater), true)]
	public class GameManagerUpdaterEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		private float manualAdvanceTime = GameManager.timePerUpdate;

		protected override void DrawControls () {
			GameManagerUpdater updater = (GameManagerUpdater) target;

			// Show controls for manually manipulating the game's lifecycle
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
			if (GUILayout.Button(updater.isUpdating ? "Pause" : "Resume")) {
				if (updater.isUpdating)
					updater.Pause();
				else
					updater.Resume();
			}
			EditorGUILayout.BeginHorizontal();
			manualAdvanceTime = EditorGUILayout.FloatField(manualAdvanceTime);
			if (GUILayout.Button("Update")) {
				if (updater.isUpdating)
					updater.Pause();
				updater.Advance(manualAdvanceTime);
			}
			if (GUILayout.Button("Next Frame")) {
				if (updater.isUpdating)
					updater.Pause();
				updater.AdvanceOneFrame();
			}
			GUILayout.EndHorizontal();
		}
	}
}