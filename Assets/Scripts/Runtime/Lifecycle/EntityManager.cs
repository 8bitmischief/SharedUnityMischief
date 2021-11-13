using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public class EntityManager : MonoBehaviour {
		public int numEntities => entities.Count;
		public int numEntitiesToSpawn => entitiesToSpawn.Count;
		public int numEntitiesToDespawn => entitiesToDespawn.Count;

		protected List<Entity> entities = new List<Entity>();
		private List<Entity> entitiesToSpawn = new List<Entity>();
		private List<Entity> entitiesToDespawn = new List<Entity>();

		private void Start () {
			foreach (Entity entity in FindObjectsOfType<Entity>())
				ScheduleEntityToSpawn(entity);
			SpawnEntitiesScheduledToSpawn();
		}

		public virtual void UpdateState () {
			UpdateEntities();
			DespawnEntitiesScheduledToDespawn();
			SpawnEntitiesScheduledToSpawn();
			DespawnEntitiesScheduledToDespawn();
		}

		public T SpawnEntityFromPrefab<T> (T entityPrefab) where T : Entity => SpawnEntityFromPrefab(entityPrefab, Vector3.zero);
		public T SpawnEntityFromPrefab<T> (T entityPrefab, Vector3 position) where T : Entity => SpawnEntityFromPrefab(entityPrefab, position, Quaternion.identity);
		public T SpawnEntityFromPrefab<T> (T entityPrefab, Vector3 position, Quaternion rotation) where T : Entity {
			T entity = Instantiate(entityPrefab, position, rotation);
			entity.name = entityPrefab.name;
			entity.gameObject.SetActive(true);
			return ScheduleEntityToSpawn(entity);
		}
		private T ScheduleEntityToSpawn<T> (T entity) where T : Entity {
			if (!entity.scheduledToSpawn && !entity.isSpawned && !entity.scheduledToDespawn) {
				entity.scheduledToSpawn = true;
				entitiesToSpawn.Add(entity);
			}
			return entity;
		}

		public void DespawnEntity (Entity entity) {
			if (entity.scheduledToSpawn) {
				entity.scheduledToSpawn = false;
				entitiesToSpawn.Remove(entity);
			}
			else if (entity.isSpawned && !entity.scheduledToDespawn) {
				entity.scheduledToDespawn = true;
				entitiesToDespawn.Add(entity);
			}
		}

		protected virtual void UpdateEntities () {
			foreach (Entity entity in entities)
				entity.UpdateEntityState();
		}

		protected void SpawnEntitiesScheduledToSpawn () {
			if (entitiesToSpawn.Count > 0) {
				foreach (Entity entity in entitiesToSpawn) {
					entities.Add(entity);
					entity.Spawn();
				}
				entitiesToSpawn.Clear();
			}
		}

		protected void DespawnEntitiesScheduledToDespawn () {
			if (entitiesToDespawn.Count > 0) {
				foreach (Entity entity in entitiesToDespawn)
					entities.Remove(entity);
				foreach (Entity entity in entitiesToDespawn)
					entity.Despawn();
				foreach (Entity entity in entitiesToDespawn)
					Destroy(entity.gameObject);
				entitiesToDespawn.Clear();
			}
		}
	}
}