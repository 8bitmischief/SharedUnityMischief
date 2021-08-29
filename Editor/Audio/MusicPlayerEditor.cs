using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Audio {
	[CustomEditor(typeof(MusicPlayer), true)]
	public class MusicPlayerEditor : Editor {
		private double lastStartBarTime = 0.0;
		private double lastBeatTime = 0.0;

		private void OnEnable () {
			MusicPlayer musicPlayer = (MusicPlayer) target;
			musicPlayer.onStartBar += OnStartBar;
			musicPlayer.onBeat += OnBeat;
		}

		private void OnDisable () {
			MusicPlayer musicPlayer = (MusicPlayer) target;
			musicPlayer.onStartBar -= OnStartBar;
			musicPlayer.onBeat -= OnBeat;
		}

		private void OnStartBar () {
			lastStartBarTime = AudioSettings.dspTime;
		}

		private void OnBeat () {
			lastBeatTime = AudioSettings.dspTime;
		}

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

			float startBarHighlight = Mathf.Clamp01(1.25f * (1.0f - (float) (AudioSettings.dspTime - lastStartBarTime) / 0.5f));
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Bar");
			EditorGUILayout.IntField(musicPlayer.bar);
			EditorGUILayout.ColorField(new Color(startBarHighlight, 0.0f, 0.0f, 1.0f));
			GUILayout.EndHorizontal();

			float beatHighlight = Mathf.Clamp01(1.25f * (1.0f - (float) (AudioSettings.dspTime - lastBeatTime) / 0.25f));
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Beat");
			EditorGUILayout.IntField(musicPlayer.beat);
			EditorGUILayout.ColorField(new Color(0.0f, 0.0f, beatHighlight, 1.0f));
			GUILayout.EndHorizontal();

			GUI.enabled = wasEnabled;
		}
	}
}