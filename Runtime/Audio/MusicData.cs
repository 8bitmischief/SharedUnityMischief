using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	[Serializable]
	public class MusicData {
		public AudioClip audioClip = null;
		public List<MusicBarData> bars = new List<MusicBarData>();
		public List<MusicEventData> events = new List<MusicEventData>();
	}

	[Serializable]
	public class MusicBarData {
		public double startTime = 0.0;
		public int numBars = 1;
		public double barDuration = 1.0;
		public int beatsPerBar = 4;
	}

	[Serializable]
	public class MusicEventData {
		public string eventName = "";
		public double startBar = 1.0;
		public double durationInBars = 1.0;
		public string pattern = "X---";
		public int numRepititions = 1;
	}
}