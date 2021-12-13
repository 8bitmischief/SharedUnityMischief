using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public class PrefabPoolMonoBehaviour : MonoBehaviour
	{
		[SerializeField] private PrefabPool _pool;

		public PrefabPool pool => _pool;

		private void Start()
		{
			pool.Prewarm();
		}

		private void OnDestroy()
		{
			pool.Dispose();
		}
	}
}