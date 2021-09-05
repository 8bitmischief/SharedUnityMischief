using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	[Serializable]
	public class MusicData {
		public AudioClip audioClip = null;
		public List<Bar> bars = new List<Bar>();
		public List<Event> events = new List<Event>();

		[Serializable]
		public class Bar {
			public double startTime = 0.0;
			public int numBars = 1;
			public double barDuration = 1.0;
			public int beatsPerBar = 4;
		}

		[Serializable]
		public class Event {
			public string eventName = null;
			public double startBar = 1.0;
			public double durationInBars = 1.0;
			public string pattern = null;
			public int numRepititions = 1;
		}
	}
}