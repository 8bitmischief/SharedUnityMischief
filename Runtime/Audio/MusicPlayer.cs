using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	public class MusicPlayer : MonoBehaviour {
		private static readonly double INITIAL_PLAY_DELAY = 1.5;

		[SerializeField] private AudioSource audioSource;
		[SerializeField] private MusicDataScriptableObject musicDataContainer;
		[SerializeField] private bool playOnAwake = false;

		public MusicData musicData { get; private set; }
		public double duration { get; private set; } = 0.0;
		public double time {
			get {
				double time = AudioSettings.dspTime;
				if (time < playTime)
					return startTime;
				else if (time > playTime + duration)
					return duration;
				else
					return startTime + time - playTime;
			}
		}
		public int bar => nextBarIndex <= 0 ? 0 : bars[nextBarIndex - 1].barNumber;
		public int beat => nextBeatIndex <= 0 ? 0 : beats[nextBeatIndex - 1].beatNumber;
		public double percentCompleted => duration <= 0.0 ? 0.0 : time / duration;
		public bool isPlaying {
			get {
				if (audioClip == null)
					return false;
				double time = AudioSettings.dspTime;
				if (time < playTime || time > playTime + duration - startTime)
					return false;
				else
					return true;
			}
		}

		public Action onStartBar;
		public Action onBeat;
		public Action<string> onMusicEvent;

		private AudioClip audioClip => musicData?.audioClip ?? null;
		private double startTime = 0.0;
		private double playTime = 0.0;
		private int nextBarIndex = 0;
		private int nextBeatIndex = 0;
		private int nextMusicEventIndex = 0;
		private List<Bar> bars = new List<Bar>();
		private List<Beat> beats = new List<Beat>();
		private List<MusicEvent> musicEvents = new List<MusicEvent>();

		private void Awake () {
			if (playOnAwake && musicDataContainer != null)
				Play(musicDataContainer.data, 0.0, INITIAL_PLAY_DELAY);
		}

		private void Update () {
			if (isPlaying) {
				// Check for bars
				for (int i = nextBarIndex; i < bars.Count; i++) {
					if (time >= bars[nextBarIndex].time) {
						nextBarIndex++;
						onStartBar?.Invoke();
					}
					else
						break;
				}
				// Check for beats
				for (int i = nextBeatIndex; i < beats.Count; i++) {
					if (time >= beats[nextBeatIndex].time) {
						nextBeatIndex++;
						onBeat?.Invoke();
					}
					else
						break;
				}
				// Check for music events
				for (int i = nextMusicEventIndex; i < musicEvents.Count; i++) {
					if (time >= musicEvents[nextMusicEventIndex].time) {
						nextMusicEventIndex++;
						onMusicEvent?.Invoke(musicEvents[nextMusicEventIndex - 1].eventName);
					}
					else
						break;
				}
			}
		}

		public void Play (MusicData musicData, double time = 0.0, double delay = 0.0) {
			// Set properties
			this.musicData = musicData;
			duration = (double) audioClip.samples / audioClip.frequency;

			// Calculate the times of all bars and beats
			bars.Clear();
			beats.Clear();
			foreach (MusicBarData barData in musicData.bars) {
				for (int i = 0; i < barData.numBars; i++) {
					Bar bar = new Bar {
						time = barData.startTime + barData.barDuration * i,
						duration = barData.barDuration
					};
					bars.Add(bar);
					for (int j = 0; j < barData.beatsPerBar; j++)
						beats.Add(new Beat {
							bar = bar,
							beatNumber = j + 1,
							time = barData.startTime + barData.barDuration * i + barData.barDuration / ((double) barData.beatsPerBar) * j
						});
				}
			}
			bars.Sort((a, b) => {
				if (a.time < b.time)
					return -1;
				else if (a.time > b.time)
					return 1;
				else
					return 0;
			});
			for (int i = 0; i < bars.Count; i++)
				bars[i].barNumber = i + 1;
			beats.Sort((a, b) => {
				if (a.time < b.time)
					return -1;
				else if (a.time > b.time)
					return 1;
				else
					return 0;
			});

			// Calculate the times of all music events
			musicEvents.Clear();
			foreach (MusicEventData evtData in musicData.events) {
				Bar bar = bars[Mathf.FloorToInt((float) evtData.startBar) - 1];
				for (int i = 0; i < evtData.numRepititions; i++) {
					double startTime = bar.time + bar.duration * evtData.durationInBars * ((double) i) + bar.duration * (evtData.startBar % 1);
					if (evtData.pattern.Length == 0)
						musicEvents.Add(new MusicEvent {
							eventName = evtData.eventName,
							time = startTime + bar.duration
						});
					else {
						for (int j = 0; j < evtData.pattern.Length; j++) {
							char c = evtData.pattern[j];
							if (c == 'X') {
								musicEvents.Add(new MusicEvent {
									eventName = evtData.eventName,
									time = startTime + bar.duration * evtData.durationInBars * ((double) j) / ((double) evtData.pattern.Length)
								});
							}
						}
					}
				}
			}
			musicEvents.Sort((a, b) => {
				if (a.time < b.time)
					return -1;
				else if (a.time > b.time)
					return 1;
				else
					return 0;
			});

			// Play the music
			Play(time, delay);
		}

		public void Play (double time = 0.0, double delay = 0.0) {
			if (audioClip != null) {
				playTime = AudioSettings.dspTime + delay;
				startTime = time;
				// Skip forward
				nextMusicEventIndex = 0;
				nextBarIndex = 0;
				nextBeatIndex = 0;
				for (int i = nextBarIndex; i < bars.Count; i++) {
					if (time > bars[nextBarIndex].time)
						nextBarIndex++;
					else
						break;
				}
				for (int i = nextBeatIndex; i < beats.Count; i++) {
					if (time > beats[nextBeatIndex].time)
						nextBeatIndex++;
					else
						break;
				}
				for (int i = nextMusicEventIndex; i < musicEvents.Count; i++) {
					if (time > musicEvents[nextMusicEventIndex].time)
						nextMusicEventIndex++;
					else
						break;
				}
				// Actually play the music
				audioSource.Stop();
				audioSource.clip = audioClip;
				audioSource.loop = false;
				audioSource.time = (float) time;
				audioSource.PlayScheduled(playTime);
			}
		}

		private class Bar {
			public int barNumber = 1;
			public double time = 0.0;
			public double duration = 1.0;
		}

		private class Beat {
			public Bar bar = null;
			public int beatNumber = 1;
			public double time = 0.0;
		}

		private class MusicEvent {
			public string eventName = "";
			public double time = 0.0;
		}
	}
}