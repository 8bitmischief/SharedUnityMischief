using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Pool {
	[Serializable]
	public class PrefabPool<T> : IDisposable where T : MonoBehaviour, IPoolable {
		[SerializeField] public T prefab;
		[SerializeField] private bool collectionCheck = false;
		[SerializeField] private int maxSize = -1;

		public int numInstances { get; private set; } = 0;
		public int numAvailableInstances => availableInstances.Count;

		private Stack<T> availableInstances = new Stack<T>();

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

		public void Dispose () {
			foreach (T instance in availableInstances)
				DestroyInstance(instance);
		}

		private T CreateInstance () {
			if (prefab == null)
				throw new Exception("Cannot instantiate null prefab in PrefabPool");
			numInstances++;
			T instance = GameObject.Instantiate(prefab);
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
			if (instance != null)
				GameObject.Destroy(instance.gameObject);
		}
	}
}