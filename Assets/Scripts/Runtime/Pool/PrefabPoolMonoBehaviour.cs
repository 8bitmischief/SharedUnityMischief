using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public class PrefabPoolMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour, IPoolable
	{
		[SerializeField] private PrefabPool<T> _pool;

		public PrefabPool<T> pool => _pool;

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