using System;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	public class MusicEventTriggerer : MonoBehaviour {
		[SerializeField] private MusicPlayer musicPlayer;

		public Action<string> onMusicEvent;
		public Action onStartBar;
		public Action onBeat;

		public MusicSchedule musicSchedule { get; private set; }
		public double time => musicPlayer.time;
		public int bar => nextBarIndex <= 0 ? 0 : musicSchedule.bars[nextBarIndex - 1].barNumber;
		public int beat => nextBeatIndex <= 0 ? 0 : musicSchedule.beats[nextBeatIndex - 1].beatNumber;

		private int nextBarIndex = 0;
		private int nextBeatIndex = 0;
		private int nextMusicEventIndex = 0;

		private void OnEnable () {
			musicPlayer.onLoadMusicData += InitializeMusicSchedule;
			musicPlayer.onWillPlay += SeekToCurrentScheduleTime;
			musicPlayer.onWillResume += SeekToCurrentScheduleTime;
		}

		private void OnDisable () {
			musicPlayer.onLoadMusicData -= InitializeMusicSchedule;
			musicPlayer.onWillPlay -= SeekToCurrentScheduleTime;
			musicPlayer.onWillResume -= SeekToCurrentScheduleTime;
		}

		private void Update () {
			if (musicPlayer.state == MusicPlayer.State.Playing) {
				for (int i = nextBarIndex; i < musicSchedule.bars.Count; i++) {
					if (musicPlayer.time >= musicSchedule.bars[nextBarIndex].time) {
						nextBarIndex++;
						onStartBar?.Invoke();
					}
					else
						break;
				}
				for (int i = nextBeatIndex; i < musicSchedule.beats.Count; i++) {
					if (musicPlayer.time >= musicSchedule.beats[nextBeatIndex].time) {
						nextBeatIndex++;
						onBeat?.Invoke();
					}
					else
						break;
				}
				for (int i = nextMusicEventIndex; i < musicSchedule.events.Count; i++) {
					if (musicPlayer.time >= musicSchedule.events[nextMusicEventIndex].time) {
						nextMusicEventIndex++;
						onMusicEvent?.Invoke(musicSchedule.events[nextMusicEventIndex - 1].eventName);
					}
					else
						break;
				}
			}
		}

		public void InitializeMusicSchedule () {
			nextBarIndex = 0;
			nextBeatIndex = 0;
			nextMusicEventIndex = 0;
			musicSchedule = new MusicSchedule(musicPlayer.musicData);
		}

		private void SeekToCurrentScheduleTime () {
			for (nextBarIndex = 0; nextBarIndex < musicSchedule.events.Count; nextBarIndex++)
				if (musicPlayer.time <= musicSchedule.bars[nextBarIndex].time)
					break;
			for (nextBeatIndex = 0; nextBeatIndex < musicSchedule.events.Count; nextBeatIndex++)
				if (musicPlayer.time <= musicSchedule.beats[nextBeatIndex].time)
					break;
			for (nextMusicEventIndex = 0; nextMusicEventIndex < musicSchedule.events.Count; nextMusicEventIndex++)
				if (musicPlayer.time <= musicSchedule.events[nextMusicEventIndex].time)
					break;
		}
	}
}