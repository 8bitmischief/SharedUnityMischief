using System;
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
		[SerializeField] private bool playOnStart = false;
		[SerializeField] private double initialPlayDelay = 1.5;

		public Action onLoadMusicData;
		public Action onWillPlay;
		public Action onPlay;
		public Action onWillPause;
		public Action onPause;
		public Action onWillResume;
		public Action onResume;
		public Action onStop;
		public Action onDone;

		public MusicData musicData { get; private set; }
		public State state { get; private set; } = State.Idle;
		public double duration { get; private set; } = 0.0;
		public double time {
			get {
				double audioTime = AudioSettings.dspTime;
				switch (state) {
					case State.ScheduledToPlay:
						return startFromPlaybackTime;
					case State.Playing:
						if (startFromPlaybackTime + audioTime - startPlayingAudioTime > duration)
							return duration;
						else
							return startFromPlaybackTime + audioTime - startPlayingAudioTime;
					case State.Done:
						return duration;
					case State.Pausing:
					case State.Paused:
					case State.Resuming:
						return pausedPlaybackTime;
					default:
						return 0.0;
				}
			}
		}
		public double timeUntilNextState {
			get {
				double audioTime = AudioSettings.dspTime;
				switch (state) {
					case State.ScheduledToPlay:
						return startPlayingAudioTime - audioTime;
					case State.Playing:
						return duration - time;
					case State.Pausing:
					case State.Resuming:
						return fadeEndAudioTime - audioTime;
					default:
						return 0.0;
				}
			}
		}
		public double percentCompleted => duration <= 0.0 ? 0.0 : time / duration;
		public bool isPlaying => state == State.Playing;

		private AudioClip audioClip = null;
		private double startFromPlaybackTime = 0.0;
		private double startPlayingAudioTime = 0.0;
		private double pausedPlaybackTime = 0.0;
		private double fadeStartAudioTime = 0.0;
		private double fadeEndAudioTime = 0.0;

		private void Start () {
			if (playOnStart && musicDataContainer != null)
				Play(musicDataContainer.data, 0.0, initialPlayDelay);
		}

		private void Update () {
			double audioTime = AudioSettings.dspTime;

			// Transition states
			if (state == State.Pausing && audioTime >= fadeEndAudioTime) {
				state = State.Paused;
				onPause?.Invoke();
				audioMixer.SetFloat(audioMixerVolumeParamName, 0f);
			}
			if (state == State.Resuming && audioTime >= fadeEndAudioTime) {
				state = State.Playing;
				onPlay?.Invoke();
				audioMixer.SetFloat(audioMixerVolumeParamName, 0f);
			}
			if (state == State.ScheduledToPlay && audioTime >= startPlayingAudioTime) {
				state = State.Playing;
				onPlay?.Invoke();
				audioMixer.SetFloat(audioMixerVolumeParamName, 0f);
			}

			// Fade out the volume
			if ((state == State.Pausing || state == State.Resuming) && fadeStartAudioTime <= audioTime && audioTime <= fadeEndAudioTime) {
				float t = (float) ((audioTime - fadeStartAudioTime) / (fadeEndAudioTime - fadeStartAudioTime));
				if (state == State.Pausing)
					audioMixer.SetFloat(audioMixerVolumeParamName, FADE_LEVEL * t);
				if (state == State.Resuming)
					audioMixer.SetFloat(audioMixerVolumeParamName, FADE_LEVEL * (1f - t));
			}

			// Finish playback
			if (state == State.Playing && time >= duration) {
				state = State.Done;
				onDone?.Invoke();
			}
		}

		public void LoadMusicData (MusicData musicData = null) {
			Stop();
			if (musicData != null)
				this.musicData = musicData;
			audioClip = this.musicData.audioClip;
			duration = (double) audioClip.samples / audioClip.frequency;
			onLoadMusicData?.Invoke();
		}

		public void Play (MusicData musicData, double time = 0.0, double delay = 0.25) {
			LoadMusicData(musicData);
			Play(time, delay);
		}

		public void Play (double time = 0.0, double delay = 0.25) {
			if (SchedulePlayback(time, delay)) {
				state = State.ScheduledToPlay;
				onWillPlay?.Invoke();
			}
		}

		public void Stop () {
			state = State.Idle;
			audioSource.Stop();
			onStop?.Invoke();
		}

		public void Pause (double fadeDuration = 0.5) {
			if (state == State.Playing || state == State.ScheduledToPlay || state == State.Resuming) {
				double audioTime = AudioSettings.dspTime;
				pausedPlaybackTime = time;
				state = State.Pausing;
				if (fadeDuration > duration - time)
					fadeDuration = duration - time;
				fadeStartAudioTime = audioTime;
				fadeEndAudioTime = audioTime + fadeDuration;
				audioSource.SetScheduledEndTime(audioTime + fadeDuration);
				onWillPause?.Invoke();
			}
		}

		public void Resume (double fadeDuration = 0.5, double delay = 0.25) {
			if (state == State.Pausing || state == State.Paused) {
				double audioTime = AudioSettings.dspTime;
				state = State.Resuming;
				audioMixer.SetFloat(audioMixerVolumeParamName, FADE_LEVEL);
				if (fadeDuration > pausedPlaybackTime)
					fadeDuration = pausedPlaybackTime;
				fadeStartAudioTime = audioTime + delay;
				fadeEndAudioTime = audioTime + delay + fadeDuration;
				SchedulePlayback(pausedPlaybackTime - fadeDuration, delay);
				onWillResume?.Invoke();
			}
		}

		public void Rewind (double duration = 5.0, double delay = 0.25) {
			if (SchedulePlayback(time - duration, delay)) {
				state = State.ScheduledToPlay;
				onWillPlay?.Invoke();
			}
		}

		public void FastForward (double duration = 5.0, double delay = 0.25) {
			if (SchedulePlayback(time + duration, delay)) {
				state = State.ScheduledToPlay;
				onWillPlay?.Invoke();
			}
		}

		private bool SchedulePlayback (double time = 0.0, double delay = 0.25) {
			if (audioClip == null)
				return false;

			if (time < 0.0)
				time = 0.0;
			else if (time > duration)
				time = duration;

			audioSource.Stop();

			// Actually play the music
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

		public enum State {
			None = 0,
			Idle = 1,
			ScheduledToPlay = 2,
			Playing = 3,
			Pausing = 4,
			Paused = 5,
			Resuming = 6,
			Done = 7
		}
	}
}