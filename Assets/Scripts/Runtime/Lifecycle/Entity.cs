using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class Entity : EntityComponent {
		public bool isSpawned { get; private set; } = false;
		public bool scheduledToSpawn = false;
		public bool scheduledToDespawn = false;

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

		public void UpdateEntityState () {
			foreach (EntityComponent component in components)
				component.UpdateState();
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