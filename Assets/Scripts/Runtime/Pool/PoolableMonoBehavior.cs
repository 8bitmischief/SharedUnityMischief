using System;
using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public abstract class PoolableMonoBehavior : MonoBehaviour, IPoolable
	{
		public Func<bool> DepositToPool { get; set; } = null;

		public bool IsPooled() => DepositToPool != null;

		public bool DepositToPoolOrDestroy()
		{
			if (IsPooled() && DepositToPool())
			{
				return true;
			}
			else
			{
				Destroy(gameObject);
				return false;
			}
		}

		public bool DepositToPoolOrDeactivate()
		{
			if (IsPooled() && DepositToPool())
			{
				return true;
			}
			else
			{
				gameObject.SetActive(false);
				return false;
			}
		}

		public virtual void OnWithdrawFromPool()
		{
			gameObject.SetActive(true);
		}

		public virtual void OnDepositToPool()
		{
			gameObject.SetActive(false);
		}
	}
}