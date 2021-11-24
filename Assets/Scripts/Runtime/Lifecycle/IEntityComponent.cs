using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public interface IEntityComponent {
		int componentUpdateOrder { get; }
		Entity entity { get; }
		void ResetComponent ();
		void OnSpawn ();
		void EarlyUpdateState ();
		void UpdateState ();
		void LateUpdateState ();
		void OnDespawn ();
	}
}
