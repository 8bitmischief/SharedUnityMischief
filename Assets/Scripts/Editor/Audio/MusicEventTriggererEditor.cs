using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Audio {
	[CustomEditor(typeof(MusicScheduler), true)]
	public class MusicSchedulerEditor : Editor {
		private double lastStartBarTime = 0.0;
		private double lastBeatTime = 0.0;
		List<string> musicEventNames = new List<string>();
		private Dictionary<string, double> lastMusicEventTimes = new Dictionary<string, double>();

		private void OnEnable () {
			MusicScheduler musicEvents = (MusicScheduler) target;
			musicEvents.onStartBar += OnStartBar;
			musicEvents.onBeat += OnBeat;
			musicEvents.onMusicEvent += OnMusicEvent;
		}

		private void OnDisable () {
			MusicScheduler musicEvents = (MusicScheduler) target;
			musicEvents.onStartBar -= OnStartBar;
			musicEvents.onBeat -= OnBeat;
			musicEvents.onMusicEvent -= OnMusicEvent;
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
			MusicScheduler musicEvents = (MusicScheduler) target;

			DrawDefaultInspector();

			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Music Events", EditorStyles.boldLabel);

			float startBarHighlight = Mathf.Clamp01(1.25f * (1.0f - (float) (AudioSettings.dspTime - lastStartBarTime) / 0.5f));
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Bar");
			EditorGUILayout.IntField(musicEvents.bar);
			EditorGUILayout.ColorField(new Color(startBarHighlight, 0.0f, 0.0f, 1.0f));
			GUILayout.EndHorizontal();

			float beatHighlight = Mathf.Clamp01(1.25f * (1.0f - (float) (AudioSettings.dspTime - lastBeatTime) / 0.25f));
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Beat");
			EditorGUILayout.IntField(musicEvents.beat);
			EditorGUILayout.ColorField(new Color(0.0f, 0.0f, beatHighlight, 1.0f));
			GUILayout.EndHorizontal();

			foreach (string eventName in musicEventNames) {
				float highlight = Mathf.Clamp01(1.25f * (1.0f - (float) (AudioSettings.dspTime - lastMusicEventTimes[eventName]) / 0.25f));
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(eventName);
				EditorGUILayout.ColorField(new Color(0.0f, highlight, 0.0f, 1.0f));
				GUILayout.EndHorizontal();
			}

			GUI.enabled = wasEnabled;
		}
	}
}