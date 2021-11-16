using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class Entity : EntityComponent {
		public virtual bool appendSpawnIndexToName => false;

		public bool isSpawned { get; private set; } = false;
		public bool scheduledToSpawn { get; set; } = false;
		public bool scheduledToDespawn { get; set; } = false;

		private EntityComponent[] components;
		
		protected virtual void Awake () {
			components = GetComponentsInChildren<EntityComponent>();
		}

		public bool Spawn () {
			if (!isSpawned) {
				isSpawned = true;
				scheduledToSpawn = false;
				foreach (EntityComponent component in components)
					component.OnSpawn();
				return true;
			}
			else
				return false;
		}

		public void EarlyUpdateEntityState () {
			foreach (EntityComponent component in components)
				component.EarlyUpdateState();
		}

		public void UpdateEntityState () {
			foreach (EntityComponent component in components)
				component.UpdateState();
		}

		public void LateUpdateEntityState () {
			foreach (EntityComponent component in components)
				component.LateUpdateState();
		}

		public bool Despawn () {
			if (isSpawned) {
				isSpawned = false;
				scheduledToDespawn = false;
				foreach (EntityComponent component in components)
					component.OnDespawn();
				return true;
			}
			else
				return false;
		}
	}
}