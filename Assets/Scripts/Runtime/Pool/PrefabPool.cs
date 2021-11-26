using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Pool
{
	[Serializable]
	public class PrefabPool<T> : IDisposable where T : MonoBehaviour, IPoolable
	{
		[SerializeField] private T _prefab;
		[SerializeField] private bool _collectionCheck = false;
		[SerializeField] private int _defaultCapacity = 0;
		[SerializeField] private int _maxSize = -1;
		private int _numInstances = 0;
		private Stack<T> _availableInstances { get; set; } = new Stack<T>();

		public T prefab => _prefab;
		public int numInstances => _numInstances;
		public int numAvailableInstances => _availableInstances.Count;

		public void Prewarm() => Prewarm(_defaultCapacity);

		public void Prewarm(int numInstancesToCreate)
		{
			for (int i = 0; i < numInstancesToCreate; i++)
			{
				Deposit(CreateInstance());
			}
		}

		public T Withdraw()
		{
			if (_availableInstances.Count == 0)
			{
				if (_maxSize >= 0 && numInstances > _maxSize)
				{
					return null;
				}
				else
				{
					return CreateInstance();
				}
			}
			else
			{
				return WithdrawInstance();
			}
		}

		public T Withdraw(Vector3 position)
		{
			T instance = Withdraw();
			instance.transform.position = position;
			return instance;
		}

		public T Withdraw(Vector3 position, Quaternion rotation)
		{
			T instance = Withdraw(position);
			instance.transform.rotation = rotation;
			return instance;
		}

		public void Deposit(T instance) => DepositInstance(instance);

		public void Dispose()
		{
			foreach (T instance in _availableInstances)
			{
				DestroyInstance(instance);
			}
		}

		private T CreateInstance()
		{
			if (prefab == null)
			{
				throw new Exception("Cannot instantiate null prefab in PrefabPool");
			}
			_numInstances++;
			// Create and prepare the instance
			T instance = GameObject.Instantiate(prefab);
			instance.name = prefab.name;
			instance.DepositToPool = () => {
				if (this == null)
				{
					return false;
				}
				else
				{
					Deposit(instance);
					return true;
				}
			};
			return instance;
		}

		private T WithdrawInstance()
		{
			T instance = _availableInstances.Pop();
			instance.OnWithdrawFromPool();
			return instance;
		}

		private void DepositInstance(T instance)
		{
			if (!_collectionCheck || !_availableInstances.Contains(instance))
			{
				_availableInstances.Push(instance);
				instance.OnDepositToPool();
			}
		}

		private void DestroyInstance(T instance)
		{
			if (instance != null)
			{
				GameObject.Destroy(instance.gameObject);
			}
		}
	}
}