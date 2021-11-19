using System;
using UnityEngine;

namespace SharedUnityMischief.Pool {
	public interface IPoolable {
		Action DepositToPool { get; set; }
		void OnWithdrawFromPool();
		void OnDepositToPool();
	}
}