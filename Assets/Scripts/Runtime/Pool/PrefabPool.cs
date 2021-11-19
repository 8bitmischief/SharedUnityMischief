using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Pool {
	public abstract class PrefabPool : MonoBehaviour {
		public abstract int numInstances { get; protected set; }
		public abstract int numAvailableInstances { get; }
	}
	
	public abstract class PrefabPool<T> : PrefabPool where T : MonoBehaviour, IPoolable {
		[Header("Pool Config")]
		[SerializeField] private T prefab;
		[SerializeField] private bool collectionCheck = false;
		[SerializeField] private int defaultCapacity = 0;
		[SerializeField] private int maxSize = -1;

		public override int numInstances { get; protected set; } = 0;
		public override int numAvailableInstances => availableInstances.Count;

		private Stack<T> availableInstances = new Stack<T>();

		private void Awake () {
			for (int i = 0; i < defaultCapacity; i++)
				DepositInstance(CreateInstance());
		}

		private void OnDestroy () {
			foreach (T instance in availableInstances)
				DestroyInstance(instance);
		}

		public T Withdraw () {
			if (availableInstances.Count == 0) {
				if (maxSize >= 0 && numInstances > maxSize)
					return null;
				else
					return CreateInstance();
			}
			else
				return WithdrawInstance();
		}

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

		public void Deposit (T instance) => DepositInstance(instance);

		private T CreateInstance () {
			numInstances++;
			T instance = Instantiate(prefab);
			instance.DepositToPool = () => {
				if (this == null)
					return false;
				else {
					Deposit(instance);
					return true;
				}
			};
			return instance;
		}

		private T WithdrawInstance () {
			T instance = availableInstances.Pop();
			instance.OnWithdrawFromPool();
			return instance;
		}

		private void DepositInstance (T instance) {
			if (!collectionCheck || !availableInstances.Contains(instance)) {
				availableInstances.Push(instance);
				instance.OnDepositToPool();
			}
		}

		private void DestroyInstance (T instance) {
			Destroy(instance.gameObject);
		}
	}
}