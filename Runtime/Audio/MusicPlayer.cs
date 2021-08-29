using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	public class MusicPlayer : MonoBehaviour {
		private static readonly double PLAY_DELAY = 1.0;

		[SerializeField] private AudioSource audioSource;
		[SerializeField] private MusicDataScriptableObject musicDataContainer;
		[SerializeField] private bool playOnAwake = false;

		public MusicData musicData { get; private set; }
		public double duration { get; private set; } = 0.0;
		public double time {
			get {
				double time = AudioSettings.dspTime;
				if (time < startTime)
					return 0.0;
				else if (time > startTime + duration)
					return duration;
				else
					return time - startTime;
			}
		}
		public int bar { get; private set; } = 0;
		public int beat { get; private set; } = 0;
		public double percentCompleted => duration <= 0.0 ? 0.0 : time / duration;
		public bool isPlaying {
			get {
				if (audioClip == null)
					return false;
				double time = AudioSettings.dspTime;
				if (time < startTime || time > startTime + duration)
					return false;
				else
					return true;
			}
		}

		public Action onStartBar;
		public Action onBeat;

		private AudioClip audioClip => musicData?.audioClip ?? null;
		private double startTime = 0.0;
		private int nextMusicEventIndex = 0;
		private List<MusicEvent> musicEvents = new List<MusicEvent>();

		private void Awake () {
			if (playOnAwake && musicDataContainer != null)
				Play(musicDataContainer.data);
		}

		private void Update () {
			if (isPlaying && nextMusicEventIndex < musicEvents.Count) {
				for (int i = nextMusicEventIndex; i < musicEvents.Count; i++) {
					MusicEvent evt = musicEvents[nextMusicEventIndex];
					if (time >= evt.time) {
						switch (evt.type) {
							case MusicEventType.StartBar:
								bar++;
								beat = 0;
								onStartBar?.Invoke();
								break;
						}
						switch (evt.type) {
							case MusicEventType.Beat:
								beat++;
								onBeat?.Invoke();
								break;
						}
						nextMusicEventIndex++;
					}
					else
						break;
				}
			}
		}

		public void Play (MusicData musicData) {
			// Set properties
			this.musicData = musicData;
			duration = (double) audioClip.samples / audioClip.frequency;
			startTime = AudioSettings.dspTime + PLAY_DELAY;
			bar = 0;

			// Calculate all future music events
			nextMusicEventIndex = 0;
			musicEvents.Clear();
			foreach (MusicBarData barData in musicData.bars) {
				for (int i = 0; i < barData.numBars; i++) {
					musicEvents.Add(new MusicEvent {
						type = MusicEventType.StartBar,
						time = barData.startTime + barData.timePerBar * i
					});
					for (int j = 0; j < barData.beatsPerBar; j++) {
						musicEvents.Add(new MusicEvent {
							type = MusicEventType.Beat,
							time = barData.startTime + barData.timePerBar * i + barData.timePerBar / ((double) barData.beatsPerBar) * j
						});
					}
				}
			}

			// Sort music events by when they'll happen
			musicEvents.Sort((a, b) => {
				if (a.time < b.time)
					return -1;
				else if (a.time > b.time)
					return 1;
				else
					return 0;
			});

			// Play the music
			audioSource.clip = audioClip;
			audioSource.PlayScheduled(startTime);
		}

		private class MusicEvent {
			public MusicEventType type = MusicEventType.None;
			public double time = 0.0;
		}

		private enum MusicEventType {
			None = 0,
			StartBar = 1,
			Beat = 2
		}
	}
}