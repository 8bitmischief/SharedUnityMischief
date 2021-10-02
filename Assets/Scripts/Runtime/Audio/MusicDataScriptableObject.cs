using UnityEngine;

namespace SharedUnityMischief.Audio {
	[CreateAssetMenu(fileName = "MusicData", menuName = "SharedUnityMischief/Audio/MusicData", order = 1)]
	public class MusicDataScriptableObject : ScriptableObject {
		public MusicData data;
	}
}