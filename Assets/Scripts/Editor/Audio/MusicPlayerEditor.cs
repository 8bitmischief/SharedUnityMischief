using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Audio {
	[CustomEditor(typeof(MusicPlayer), true)]
	public class MusicPlayerEditor : Editor {
		private double timeToPlayFrom = 0.0;

		public override bool RequiresConstantRepaint () => Application.isPlaying;

		public override void OnInspectorGUI () {
			MusicPlayer musicPlayer = (MusicPlayer) target;

			DrawDefaultInspector();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();
			timeToPlayFrom = EditorGUILayout.DoubleField(timeToPlayFrom);
			if (GUILayout.Button("Play"))
				musicPlayer.Play(timeToPlayFrom);
			if (GUILayout.Button("Stop"))
				musicPlayer.Stop();
			if (GUILayout.Button("Reimport"))
				musicPlayer.LoadMusicData();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Pause"))
				musicPlayer.Pause();
			if (GUILayout.Button("Resume"))
				musicPlayer.Resume();
			if (GUILayout.Button("Rewind"))
				musicPlayer.Rewind();
			if (GUILayout.Button("Fast Forward"))
				musicPlayer.FastForward();
			GUILayout.EndHorizontal();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Music Playback", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Is Playing");
			EditorGUILayout.Toggle(musicPlayer.isPlaying);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("State");
			EditorGUILayout.TextField(musicPlayer.state.ToString());
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
			EditorGUILayout.LabelField("Time To Next State");
			EditorGUILayout.TextField(musicPlayer.timeUntilNextState.ToString("0.00"));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Percent Completed");
			EditorGUILayout.TextField((100.0 * musicPlayer.percentCompleted).ToString("0.0") + "%");
			GUILayout.EndHorizontal();

			GUI.enabled = wasEnabled;
		}
	}
}