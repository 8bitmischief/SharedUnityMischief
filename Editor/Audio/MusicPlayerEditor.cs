using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Audio {
	[CustomEditor(typeof(MusicPlayer), true)]
	public class MusicPlayerEditor : Editor {
		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			MusicPlayer musicPlayer = (MusicPlayer) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Music Playback", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Is Playing");
			EditorGUILayout.Toggle(musicPlayer.isPlaying);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Duration");
			EditorGUILayout.TextField(musicPlayer.duration.ToString("0.00"));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Time");
			EditorGUILayout.TextField(musicPlayer.time.ToString("0.00"));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Percent Completed");
			EditorGUILayout.TextField((100.0 * musicPlayer.percentCompleted).ToString("0.0") + "%");
			GUILayout.EndHorizontal();

			GUI.enabled = wasEnabled;
		}
	}
}