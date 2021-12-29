using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public class PrefabPoolMonoBehaviour : MonoBehaviour, IPool
	{
		[SerializeField] private PrefabPool _pool;

		public MonoBehaviour prefab => _pool.prefab;
		public bool isActualPrefab => _pool.isActualPrefab;
		public int numInstances => _pool.numInstances;
		public int numAvailableInstances => _pool.numAvailableInstances;

		private void Awake()
		{
			_pool.Prewarm();
		}

		private void OnDestroy()
		{
			_pool.Dispose();
		}

		public MonoBehaviour Withdraw() => _pool.Withdraw();
		public MonoBehaviour Withdraw(Vector3 position) => _pool.Withdraw(position);
		public MonoBehaviour Withdraw(Vector3 position, Quaternion rotation) => _pool.Withdraw(position, rotation);
		public T Withdraw<T>() where T : MonoBehaviour => _pool.Withdraw<T>();
		public T Withdraw<T>(Vector3 position) where T : MonoBehaviour => _pool.Withdraw<T>(position);
		public T Withdraw<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour => _pool.Withdraw<T>(position, rotation);
		public void Deposit(MonoBehaviour instance) => _pool.Deposit(instance);
	}
}