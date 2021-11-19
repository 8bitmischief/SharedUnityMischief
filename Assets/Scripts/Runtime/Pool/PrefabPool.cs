using UnityEngine;
using UnityEngine.Pool;

namespace SharedUnityMischief.Pool {
	public abstract class PrefabPool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable {
		[Header("Pool Config")]
		[SerializeField] private T prefab;
		[SerializeField] private bool collectionCheck = false;
		[SerializeField] private int defaultCapacity = 10;
		[SerializeField] private int maxSize = 10000;

		private ObjectPool<T> pool;

		private void Awake () {
			pool = new ObjectPool<T>(
				CreateInstance, WithdrawInstance, DepositInstance, DestroyInstance,
				collectionCheck, defaultCapacity, maxSize);
		}

		public T Withdraw () => pool.Get();

		public T Withdraw (Vector3 position) {
			T instance = Withdraw();
			instance.transform.position = position;
			return instance;
		}

		public T Withdraw (Vector3 position, Quaternion rotation) {
			T instance = Withdraw(position);
			instance.transform.rotation = rotation;
			return instance;
		}

		private T CreateInstance () {
			T instance = Instantiate(prefab);
			instance.DepositToPool = () => pool.Release(instance);
			return instance;
		}

		private void WithdrawInstance (T instance) {
			instance.OnWithdrawFromPool();
		}

		private void DepositInstance (T instance) {
			instance.OnDepositToPool();
		}

		private void DestroyInstance (T instance) {
			Destroy(instance);
		}
	}
}