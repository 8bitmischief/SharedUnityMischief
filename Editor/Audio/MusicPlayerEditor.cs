using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Audio {
	[CustomEditor(typeof(MusicPlayer), true)]
	public class MusicPlayerEditor : Editor {
		private double timeToPlayFrom = 0.0;
		private double lastStartBarTime = 0.0;
		private double lastBeatTime = 0.0;
		List<string> musicEventNames = new List<string>();
		private Dictionary<string, double> lastMusicEventTimes = new Dictionary<string, double>();

		private void OnEnable () {
			MusicPlayer musicPlayer = (MusicPlayer) target;
			musicPlayer.onStartBar += OnStartBar;
			musicPlayer.onBeat += OnBeat;
			musicPlayer.onMusicEvent += OnMusicEvent;
		}

		private void OnDisable () {
			MusicPlayer musicPlayer = (MusicPlayer) target;
			musicPlayer.onStartBar -= OnStartBar;
			musicPlayer.onBeat -= OnBeat;
			musicPlayer.onMusicEvent -= OnMusicEvent;
		}

		private void OnStartBar () {
			lastStartBarTime = AudioSettings.dspTime;
		}

		private void OnBeat () {
			lastBeatTime = AudioSettings.dspTime;
		}

		private void OnMusicEvent (string eventName) {
			if (!musicEventNames.Contains(eventName))
				musicEventNames.Add(eventName);
			lastMusicEventTimes[eventName] = AudioSettings.dspTime;
		}

		public override bool RequiresConstantRepaint () => true;

		public override void OnInspectorGUI () {
			MusicPlayer musicPlayer = (MusicPlayer) target;

			DrawDefaultInspector();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Play From Time");
			timeToPlayFrom = EditorGUILayout.DoubleField(timeToPlayFrom);
			if (GUILayout.Button("Play"))
				musicPlayer.Play(timeToPlayFrom, 0.5);
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

			if (musicEventNames.Count > 0) {
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Music Events", EditorStyles.boldLabel);

				foreach (string eventName in musicEventNames) {
					float highlight = Mathf.Clamp01(1.25f * (1.0f - (float) (AudioSettings.dspTime - lastMusicEventTimes[eventName]) / 0.25f));
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(eventName);
					EditorGUILayout.ColorField(new Color(0.0f, highlight, 0.0f, 1.0f));
					GUILayout.EndHorizontal();
				}
			}

			GUI.enabled = wasEnabled;
		}
	}
}