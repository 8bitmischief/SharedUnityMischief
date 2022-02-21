using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public class PrefabPoolComponent : MonoBehaviour, IPool
	{
		[SerializeField] private PrefabPool _pool;

		public GameObject prefab => _pool.prefab;
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

		public GameObject Withdraw() => _pool.Withdraw(transform.position, transform.rotation);
		public GameObject Withdraw(Vector3 position) => _pool.Withdraw(position, Quaternion.identity);
		public GameObject Withdraw(Vector3 position, Quaternion rotation) => _pool.Withdraw(position, rotation);
		public T Withdraw<T>() where T : MonoBehaviour => _pool.Withdraw<T>(transform.position, transform.rotation);
		public T Withdraw<T>(Vector3 position) where T : MonoBehaviour => _pool.Withdraw<T>(position, Quaternion.identity);
		public T Withdraw<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour => _pool.Withdraw<T>(position, rotation);
		public void Deposit(GameObject instance) => _pool.Deposit(instance);
	}
}