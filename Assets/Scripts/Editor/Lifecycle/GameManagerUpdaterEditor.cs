using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Lifecycle {
	[CustomEditor(typeof(GameManagerUpdater), true)]
	public class GameManagerUpdaterEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		private float manualAdvanceTime = 1f / 60f;

		public override void OnInspectorGUI () {
			GameManagerUpdater updater = (GameManagerUpdater) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = Application.isPlaying;

			// Show controls for manually manipulating the game's lifecycle
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

			GUI.enabled = wasEnabled;
		}
	}
}