using System;
using UnityEngine;

namespace SharedUnityMischief.Pool {
	public interface IPoolable {
		Func<bool> DepositToPool { get; set; }
		void OnWithdrawFromPool();
		void OnDepositToPool();
	}
}