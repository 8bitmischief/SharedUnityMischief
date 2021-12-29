using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public interface IPool
	{
		MonoBehaviour prefab { get; }
		bool isActualPrefab { get; }
		int numInstances { get; }
		int numAvailableInstances { get; }

		MonoBehaviour Withdraw();
		MonoBehaviour Withdraw(Vector3 position);
		MonoBehaviour Withdraw(Vector3 position, Quaternion rotation);
		T Withdraw<T>() where T : MonoBehaviour;
		T Withdraw<T>(Vector3 position) where T : MonoBehaviour;
		T Withdraw<T>(Vector3 position, Quaternion rotation) where T : MonoBehaviour;

		void Deposit(MonoBehaviour instance);
	}
}