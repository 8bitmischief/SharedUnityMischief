using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Audio {
	[Serializable]
	public class MusicData {
		public AudioClip audioClip = null;
		public List<MusicBarData> bars = new List<MusicBarData>();
	}

	[Serializable]
	public class MusicBarData {
		public double startTime = 0.0;
		public double timePerBar = 1.0;
		public int numBars = 1;
		public int beatsPerBar = 4;
	}
}