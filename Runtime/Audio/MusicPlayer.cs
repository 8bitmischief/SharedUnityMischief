using UnityEngine;

namespace SharedUnityMischief.Audio {
	public class MusicPlayer : MonoBehaviour {
		private static readonly double PLAY_DELAY = 0.5;

		[SerializeField] private AudioSource audioSource;
		[SerializeField] private MusicDataScriptableObject musicDataContainer;
		[SerializeField] private bool playOnAwake = false;

		public MusicData musicData { get; private set; }
		private AudioClip audioClip => musicData?.audioClip ?? null;
		private double startTime = 0.0;
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

		private void Awake () {
			if (playOnAwake && musicDataContainer != null)
				Play(musicDataContainer.data);
		}

		public void Play (MusicData musicData) {
			// Set properties
			this.musicData = musicData;
			duration = (double) audioClip.samples / audioClip.frequency;
			startTime = AudioSettings.dspTime + PLAY_DELAY;

			// Play the music
			audioSource.clip = audioClip;
			audioSource.PlayScheduled(startTime);
		}
	}
}