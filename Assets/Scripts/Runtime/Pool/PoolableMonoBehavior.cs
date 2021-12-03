using System;
using UnityEngine;

namespace SharedUnityMischief.Pool
{
	public abstract class PoolableMonoBehaviour : MonoBehaviour, IPoolable
	{
		public Func<bool> DepositToPool { get; set; }
		public bool isPooled => DepositToPool != null;

		public bool DepositToPoolOrDestroy()
		{
			if (isPooled && DepositToPool())
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
			if (isPooled && DepositToPool())
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