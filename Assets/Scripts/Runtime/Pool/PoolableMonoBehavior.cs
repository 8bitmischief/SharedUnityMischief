using System;
using UnityEngine;

namespace SharedUnityMischief.Pool {
	public abstract class PoolableMonoBehavior : MonoBehaviour, IPoolable {
		public Action DepositToPool { get; set; } = null;

		public void DepositOrDestroy () {
			if (DepositToPool != null)
				DepositToPool.Invoke();
			else
				Destroy(gameObject);
		}

		public virtual void OnWithdrawFromPool () {
			gameObject.SetActive(true);
		}

		public virtual void OnDepositToPool () {
			gameObject.SetActive(false);
		}
	}
}