using UnityEngine;

namespace SharedUnityMischief.Entities
{
	public interface IEntityComponent
	{
		int componentUpdateOrder { get; }
		Entity entity { get; }
		void ResetComponent();
		void OnSpawn();
		void EarlyUpdateState();
		void UpdateState();
		void CheckInteractions();
		void LateUpdateState();
		void Render();
		void OnDespawn();
	}
}
