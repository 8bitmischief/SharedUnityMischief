using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	public class MusicSchedule {
		public MusicData musicData { get; private set; }
		public List<Bar> bars { get; private set; } = new List<Bar>();
		public List<Beat> beats { get; private set; } = new List<Beat>();
		public List<MusicEvent> events { get; private set; } = new List<MusicEvent>();

		public MusicSchedule (MusicData musicData) {
			this.musicData = musicData;
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
			foreach (MusicEventData evtData in musicData.events) {
				Bar bar = bars[Mathf.FloorToInt((float) evtData.startBar) - 1];
				for (int i = 0; i < evtData.numRepititions; i++) {
					double startTime = bar.time + bar.duration * evtData.durationInBars * ((double) i) + bar.duration * (evtData.startBar % 1);
					if (evtData.pattern.Length == 0)
						events.Add(new MusicEvent {
							eventName = evtData.eventName,
							time = startTime + bar.duration
						});
					else {
						for (int j = 0; j < evtData.pattern.Length; j++) {
							char c = evtData.pattern[j];
							if (c == 'X') {
								events.Add(new MusicEvent {
									eventName = evtData.eventName,
									time = startTime + bar.duration * evtData.durationInBars * ((double) j) / ((double) evtData.pattern.Length)
								});
							}
						}
					}
				}
			}
			events.Sort((a, b) => {
				if (a.time < b.time)
					return -1;
				else if (a.time > b.time)
					return 1;
				else
					return 0;
			});
		}

		public class Bar {
			public int barNumber = 1;
			public double time = 0.0;
			public double duration = 1.0;
		}

		public class Beat {
			public Bar bar = null;
			public int beatNumber = 1;
			public double time = 0.0;
		}

		public class MusicEvent {
			public string eventName = "";
			public double time = 0.0;
		}
	}
}