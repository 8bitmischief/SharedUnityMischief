using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Pool
{
	[Serializable]
	public class PrefabPool : IPool, IDisposable
	{
		[SerializeField] private GameObject _prefab;
		[SerializeField] private bool _isActualPrefab = false;
		[SerializeField, ShowIfBool("_isActualPrefab", false)] private bool _disablePrefab = false;
		[SerializeField] private bool _collectionCheck = false;
		[SerializeField] private int _defaultCapacity = 0;
		[SerializeField] private bool _hasMaxCapacity = false;
		[SerializeField, ShowIfBool("_hasMaxCapacity")] private int _maxCapacity = 32;
		private int _numInstances = 0;
		private Queue<GameObject> _availableInstances { get; set; } = new Queue<GameObject>();

		public GameObject prefab => _prefab;
		public bool isActualPrefab => _isActualPrefab;
		public int numInstances => _numInstances;
		public int numAvailableInstances => _availableInstances.Count;

		public void Prewarm() => Prewarm(_defaultCapacity);
		public void Prewarm(int numInstancesToCreate)
		{
			if (!_isActualPrefab && _disablePrefab && _prefab != null)
				_prefab.SetActive(false);
			for (int i = 0; i < numInstancesToCreate; i++)
				Deposit(CreateInstance());
		}

		public GameObject Withdraw()
		{
			GameObject instance = CreateOrWithdrawInstance();
			if (instance != null)
			{
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnWithdrawFromPool();
			}
			return instance;
		}
		public GameObject Withdraw(Transform parent)
		{
			GameObject instance = CreateOrWithdrawInstance(parent);
			if (instance != null)
			{
				instance.transform.SetParent(parent);
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnWithdrawFromPool();
			}
			return instance;
		}
		public GameObject Withdraw(Vector3 position)
		{
			GameObject instance = CreateOrWithdrawInstance();
			if (instance != null)
			{
				instance.transform.position = position;
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnWithdrawFromPool();
			}
			return instance;
		}
		public GameObject Withdraw(Transform parent, Vector3 position)
		{
			GameObject instance = CreateOrWithdrawInstance(parent);
			if (instance != null)
			{
				instance.transform.SetParent(parent);
				instance.transform.position = position;
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnWithdrawFromPool();
			}
			return instance;
		}
		public GameObject Withdraw(Vector3 position, Quaternion rotation)
		{
			GameObject instance = CreateOrWithdrawInstance();
			if (instance != null)
			{
				instance.transform.position = position;
				instance.transform.rotation = rotation;
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnWithdrawFromPool();
			}
			return instance;
		}
		public GameObject Withdraw(Transform parent, Vector3 position, Quaternion rotation)
		{
			GameObject instance = CreateOrWithdrawInstance(parent);
			if (instance != null)
			{
				instance.transform.SetParent(parent);
				instance.transform.position = position;
				instance.transform.rotation = rotation;
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnWithdrawFromPool();
			}
			return instance;
		}

		public T Withdraw<T>() where T : MonoBehaviour => GetComponent<T>(Withdraw());
		public T Withdraw<T>(Transform parent) where T : MonoBehaviour => GetComponent<T>(Withdraw(parent));
		public T Withdraw<T>(Vector3 position) where T : MonoBehaviour => GetComponent<T>(Withdraw(position));
		public T Withdraw<T>(Transform parent, Vector3 position) where T : MonoBehaviour => GetComponent<T>(Withdraw(parent, position));
		public T Withdraw<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour => GetComponent<T>(Withdraw(position, rotation));
		public T Withdraw<T>(Transform parent, Vector3 position, Quaternion rotation) where T : MonoBehaviour => GetComponent<T>(Withdraw(parent, position, rotation));

		public void Deposit(GameObject instance) => DepositInstance(instance);

		public void Dispose()
		{
			foreach (GameObject instance in _availableInstances)
				DestroyInstance(instance);
		}

		private GameObject CreateInstance(Transform parent = null)
		{
			if (prefab == null)
				throw new Exception("Cannot instantiate null prefab in PrefabPool");
			_numInstances++;
			// Create and prepare the instance
			GameObject instance = GameObject.Instantiate(prefab, parent);
			instance.name = prefab.name;
			instance.gameObject.SetActive(true);
			if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
			{
				poolableInstance.DepositToPool = () => {
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
			}
			return instance;
		}

		private GameObject WithdrawInstance() => _availableInstances.Dequeue();

		private GameObject CreateOrWithdrawInstance(Transform parent = null)
		{
			if (_availableInstances.Count == 0)
			{
				if (_hasMaxCapacity && _maxCapacity >= 0 && numInstances >= _maxCapacity)
					return null;
				else
					return CreateInstance(parent);
			}
			else
			{
				return WithdrawInstance();
			}
		}

		private void DepositInstance(GameObject instance)
		{
			if (!_collectionCheck || !_availableInstances.Contains(instance))
			{
				_availableInstances.Enqueue(instance);
				if (instance.TryGetComponent<IPoolable>(out IPoolable poolableInstance))
					poolableInstance.OnDepositToPool();
			}
		}

		private void DestroyInstance(GameObject instance)
		{
			if (instance != null)
				GameObject.Destroy(instance.gameObject);
		}

		private T GetComponent<T>(GameObject instance) where T : MonoBehaviour
		{
			if (instance.TryGetComponent<T>(out T component))
				return component;
			else
				throw new Exception($"Prefab {prefab.name} does not have a {typeof(T).Name} component");
		}
	}
}