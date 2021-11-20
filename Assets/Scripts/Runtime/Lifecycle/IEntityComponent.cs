using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public interface IEntityComponent {
		Entity entity { get; }
		void OnSpawn ();
		void EarlyUpdateState ();
		void UpdateState ();
		void LateUpdateState ();
		void OnDespawn ();
	}
}
