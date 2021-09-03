using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	public class MusicEventTriggerer : MonoBehaviour {
        [SerializeField] private MusicPlayer musicPlayer;

		public Action onStartBar;
		public Action onBeat;
		public Action<string> onMusicEvent;

		public int bar => nextBarIndex <= 0 ? 0 : bars[nextBarIndex - 1].barNumber;
		public int beat => nextBeatIndex <= 0 ? 0 : beats[nextBeatIndex - 1].beatNumber;

		private int nextBarIndex = 0;
		private int nextBeatIndex = 0;
		private int nextMusicEventIndex = 0;
		private List<Bar> bars = new List<Bar>();
		private List<Beat> beats = new List<Beat>();
		private List<MusicEvent> musicEvents = new List<MusicEvent>();

		private void OnEnable () {
			musicPlayer.onLoadMusicData += LoadEvents;
			musicPlayer.onWillPlay += SeekToEventTime;
			musicPlayer.onWillResume += SeekToEventTime;
		}

		private void OnDisable () {
			musicPlayer.onLoadMusicData -= LoadEvents;
			musicPlayer.onWillPlay -= SeekToEventTime;
			musicPlayer.onWillResume -= SeekToEventTime;
		}

		private void Update () {
			if (musicPlayer.state == MusicPlayer.State.Playing) {
				for (int i = nextBarIndex; i < bars.Count; i++) {
					if (musicPlayer.time >= bars[nextBarIndex].time) {
						nextBarIndex++;
						onStartBar?.Invoke();
					}
					else
						break;
				}
				for (int i = nextBeatIndex; i < beats.Count; i++) {
					if (musicPlayer.time >= beats[nextBeatIndex].time) {
						nextBeatIndex++;
						onBeat?.Invoke();
					}
					else
						break;
				}
				for (int i = nextMusicEventIndex; i < musicEvents.Count; i++) {
					if (musicPlayer.time >= musicEvents[nextMusicEventIndex].time) {
						nextMusicEventIndex++;
						onMusicEvent?.Invoke(musicEvents[nextMusicEventIndex - 1].eventName);
					}
					else
						break;
				}
			}
		}

		public void LoadEvents () {
			// Calculate the times of all bars and beats
			nextBarIndex = 0;
			nextBeatIndex = 0;
			bars.Clear();
			beats.Clear();
			foreach (MusicBarData barData in musicPlayer.musicData.bars) {
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
			nextMusicEventIndex = 0;
			musicEvents.Clear();
			foreach (MusicEventData evtData in musicPlayer.musicData.events) {
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
		}

		private void SeekToEventTime () {
			for (nextBarIndex = 0; nextBarIndex < musicEvents.Count; nextBarIndex++)
				if (musicPlayer.time <= bars[nextBarIndex].time)
					break;
			for (nextBeatIndex = 0; nextBeatIndex < musicEvents.Count; nextBeatIndex++)
				if (musicPlayer.time <= beats[nextBeatIndex].time)
					break;
			for (nextMusicEventIndex = 0; nextMusicEventIndex < musicEvents.Count; nextMusicEventIndex++)
				if (musicPlayer.time <= musicEvents[nextMusicEventIndex].time)
					break;
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