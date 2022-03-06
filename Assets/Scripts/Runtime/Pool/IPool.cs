using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public interface IPool
	{
		GameObject prefab { get; }
		bool isActualPrefab { get; }
		int numInstances { get; }
		int numAvailableInstances { get; }

		GameObject Withdraw();
		GameObject Withdraw(Transform parent);
		GameObject Withdraw(Vector3 position);
		GameObject Withdraw(Transform parent, Vector3 position);
		GameObject Withdraw(Vector3 position, Quaternion rotation);
		GameObject Withdraw(Transform parent, Vector3 position, Quaternion rotation);
		T Withdraw<T>() where T : MonoBehaviour;
		T Withdraw<T>(Transform parent) where T : MonoBehaviour;
		T Withdraw<T>(Vector3 position) where T : MonoBehaviour;
		T Withdraw<T>(Transform parent, Vector3 position) where T : MonoBehaviour;
		T Withdraw<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour;
		T Withdraw<T>(Transform parent, Vector3 position, Quaternion rotation) where T : MonoBehaviour;

		void Deposit(GameObject instance);
	}
}