using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SharedUnityMischief.Audio {
	public class MusicPlayer : MonoBehaviour {
		private readonly float FADE_LEVEL = -40f;

		[SerializeField] private AudioSource audioSource;
		[SerializeField] private AudioMixer audioMixer;
		[SerializeField] private AudioMixerGroup audioMixerGroup;
		[SerializeField] private string audioMixerVolumeParamName;
		[SerializeField] private MusicDataScriptableObject musicDataContainer;
		[SerializeField] private bool playOnAwake = false;
		[SerializeField] private double initialPlayDelay = 1.5;

		public MusicData musicData { get; private set; }
		public MusicPlayerState state { get; private set; } = MusicPlayerState.Idle;
		public double duration { get; private set; } = 0.0;
		public double time {
			get {
				double audioTime = AudioSettings.dspTime;
				switch (state) {
					case MusicPlayerState.ScheduledToPlay:
						return startFromPlaybackTime;
					case MusicPlayerState.Playing:
						return startFromPlaybackTime + audioTime - startPlayingAudioTime;
					case MusicPlayerState.Done:
						return duration;
					case MusicPlayerState.Pausing:
					case MusicPlayerState.Paused:
					case MusicPlayerState.Resuming:
						return pausedPlaybackTime;
					default:
						return 0.0;
				}
			}
		}
		public int bar => nextBarIndex <= 0 ? 0 : bars[nextBarIndex - 1].barNumber;
		public int beat => nextBeatIndex <= 0 ? 0 : beats[nextBeatIndex - 1].beatNumber;
		public double percentCompleted => duration <= 0.0 ? 0.0 : time / duration;
		public bool isPlaying => state == MusicPlayerState.Playing;

		public Action onStartBar;
		public Action onBeat;
		public Action<string> onMusicEvent;

		private AudioClip audioClip = null;
		private double startFromPlaybackTime = 0.0;
		private double startPlayingAudioTime = 0.0;
		private double pausedPlaybackTime = 0.0;
		private double fadeStartAudioTime = 0.0;
		private double fadeEndAudioTime = 0.0;
		private int nextBarIndex = 0;
		private int nextBeatIndex = 0;
		private int nextMusicEventIndex = 0;
		private List<Bar> bars = new List<Bar>();
		private List<Beat> beats = new List<Beat>();
		private List<MusicEvent> musicEvents = new List<MusicEvent>();

		private void Awake () {
			if (playOnAwake && musicDataContainer != null)
				Play(musicDataContainer.data, 0.0, initialPlayDelay);
		}

		private void Update () {
			double audioTime = AudioSettings.dspTime;

			// Transition states
			if (state == MusicPlayerState.Pausing && audioTime >= fadeEndAudioTime) {
				state = MusicPlayerState.Paused;
				audioMixer.SetFloat(audioMixerVolumeParamName, 0f);
			}
			if (state == MusicPlayerState.Resuming && audioTime >= fadeEndAudioTime) {
				state = MusicPlayerState.Playing;
				audioMixer.SetFloat(audioMixerVolumeParamName, 0f);
			}
			if (state == MusicPlayerState.ScheduledToPlay && audioTime >= startPlayingAudioTime) {
				state = MusicPlayerState.Playing;
				audioMixer.SetFloat(audioMixerVolumeParamName, 0f);
			}

			// Fade out the volume
			if (fadeStartAudioTime <= audioTime && audioTime <= fadeEndAudioTime) {
				float t = (float) ((audioTime - fadeStartAudioTime) / (fadeEndAudioTime - fadeStartAudioTime));
				if (state == MusicPlayerState.Pausing)
					audioMixer.SetFloat(audioMixerVolumeParamName, FADE_LEVEL * t);
				if (state == MusicPlayerState.Resuming)
					audioMixer.SetFloat(audioMixerVolumeParamName, FADE_LEVEL * (1f - t));
			}

			// Check for bars, beats, and events
			if (state == MusicPlayerState.Playing) {
				for (int i = nextBarIndex; i < bars.Count; i++) {
					if (time >= bars[nextBarIndex].time) {
						nextBarIndex++;
						onStartBar?.Invoke();
					}
					else
						break;
				}
				for (int i = nextBeatIndex; i < beats.Count; i++) {
					if (time >= beats[nextBeatIndex].time) {
						nextBeatIndex++;
						onBeat?.Invoke();
					}
					else
						break;
				}
				for (int i = nextMusicEventIndex; i < musicEvents.Count; i++) {
					if (time >= musicEvents[nextMusicEventIndex].time) {
						nextMusicEventIndex++;
						onMusicEvent?.Invoke(musicEvents[nextMusicEventIndex - 1].eventName);
					}
					else
						break;
				}

				// Finish playback
				if (time >= duration)
					state = MusicPlayerState.Done;
			}
		}

		public void Play (MusicData musicData, double time = 0.0, double delay = 0.25) {
			ReimportMusicData(musicData);
			Play(time, delay);
		}

		public void Play (double time = 0.0, double delay = 0.25) {
			if (SchedulePlayback(time, delay))
				state = MusicPlayerState.ScheduledToPlay;
		}

		public void Stop () {
			state = MusicPlayerState.Idle;
			audioSource.Stop();
		}

		public void ReimportMusicData (MusicData musicData = null) {
			Stop();

			// Set properties
			if (musicData != null)
				this.musicData = musicData;
			audioClip = this.musicData.audioClip;
			duration = (double) audioClip.samples / audioClip.frequency;

			// Calculate the times of all bars and beats
			nextBarIndex = 0;
			nextBeatIndex = 0;
			bars.Clear();
			beats.Clear();
			foreach (MusicBarData barData in this.musicData.bars) {
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
			foreach (MusicEventData evtData in this.musicData.events) {
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

		public void Pause (double fadeDuration = 0.5) {
			if (state == MusicPlayerState.Playing || state == MusicPlayerState.ScheduledToPlay) {
				double audioTime = AudioSettings.dspTime;
				pausedPlaybackTime = time;
				state = MusicPlayerState.Pausing;
				if (fadeDuration > duration - time)
					fadeDuration = duration - time;
				fadeStartAudioTime = audioTime;
				fadeEndAudioTime = audioTime + fadeDuration;
				audioSource.SetScheduledEndTime(audioTime + fadeDuration);
			}
		}

		public void Resume (double fadeDuration = 0.5, double delay = 0.25) {
			if (state == MusicPlayerState.Pausing || state == MusicPlayerState.Paused) {
				double audioTime = AudioSettings.dspTime;
				state = MusicPlayerState.Resuming;
				audioMixer.SetFloat(audioMixerVolumeParamName, FADE_LEVEL);
				if (fadeDuration > pausedPlaybackTime)
					fadeDuration = pausedPlaybackTime;
				fadeStartAudioTime = audioTime + delay;
				fadeEndAudioTime = audioTime + delay + fadeDuration;
				SchedulePlayback(pausedPlaybackTime - fadeDuration, delay);
			}
		}

		public void Rewind (double duration = 5.0, double delay = 0.25) {
			if (SchedulePlayback(time - duration, delay))
				state = MusicPlayerState.ScheduledToPlay;
		}

		public void FastForward (double duration = 5.0, double delay = 0.25) {
			if (SchedulePlayback(time + duration, delay))
				state = MusicPlayerState.ScheduledToPlay;
		}

		private bool SchedulePlayback (double time = 0.0, double delay = 0.25) {
			if (audioClip == null)
				return false;

			if (time < 0.0)
				time = 0.0;
			else if (time > duration)
				time = duration;

			// Skip forward
			for (nextBarIndex = 0; nextBarIndex < musicEvents.Count; nextBarIndex++)
				if (time <= bars[nextBarIndex].time)
					break;
			for (nextBeatIndex = 0; nextBeatIndex < musicEvents.Count; nextBeatIndex++)
				if (time <= beats[nextBeatIndex].time)
					break;
			for (nextMusicEventIndex = 0; nextMusicEventIndex < musicEvents.Count; nextMusicEventIndex++)
				if (time <= musicEvents[nextMusicEventIndex].time)
					break;

			// Actually play the music
			audioSource.Stop();
			startPlayingAudioTime = AudioSettings.dspTime + delay;
			startFromPlaybackTime = time;
			audioSource.outputAudioMixerGroup = audioMixerGroup;
			audioSource.clip = audioClip;
			audioSource.loop = false;
			audioSource.time = (float) time;
			audioSource.PlayScheduled(startPlayingAudioTime);
			audioSource.SetScheduledEndTime(startPlayingAudioTime + duration - startFromPlaybackTime + 1.0);
			return true;
		}

		public enum MusicPlayerState {
			None = 0,
			Idle = 1,
			ScheduledToPlay = 2,
			Playing = 3,
			Pausing = 4,
			Paused = 5,
			Resuming = 6,
			Done = 7
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