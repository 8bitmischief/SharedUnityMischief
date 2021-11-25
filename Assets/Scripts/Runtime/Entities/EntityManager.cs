using System.Collections.Generic;
using UnityEngine;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Entities
{
	public class EntityManager : MonoBehaviour
	{
		protected List<Entity> entities = new List<Entity>();
		private List<Entity> entitiesToSpawn = new List<Entity>();
		private List<Entity> entitiesToDespawn = new List<Entity>();
		private Dictionary<string, int> entitySpawnCounts = new Dictionary<string, int>();
		public int numEntities => entities.Count;
		public int numEntitiesToSpawn => entitiesToSpawn.Count;
		public int numEntitiesToDespawn => entitiesToDespawn.Count;

		private void Start()
		{
			foreach (Entity entity in FindObjectsOfType<Entity>())
			{
				ScheduleEntityToSpawn(entity);
			}
			SpawnEntitiesScheduledToSpawn();
		}

		public virtual void UpdateState()
		{
			UpdateEntities();
			DespawnEntitiesScheduledToDespawn();
			SpawnEntitiesScheduledToSpawn();
			DespawnEntitiesScheduledToDespawn();
		}

		public T SpawnEntityFromPool<T> (PrefabPool<T> pool) where T : Entity => SpawnEntityFromPool(pool, Vector3.zero);
		public T SpawnEntityFromPool<T> (PrefabPool<T> pool, Vector3 position) where T : Entity => SpawnEntityFromPool(pool, position, Quaternion.identity);
		public T SpawnEntityFromPool<T> (PrefabPool<T> pool, Vector3 position, Quaternion rotation) where T : Entity
		{
			T entity = pool.Withdraw();
			RenameEntity(entity, pool.prefab.name);
			return SpawnEntityFromGameObject(entity, position, rotation);
		}

		public T SpawnEntityFromGameObject<T> (T entity) where T : Entity => SpawnEntityFromGameObject(entity, Vector3.zero);
		public T SpawnEntityFromGameObject<T> (T entity, Vector3 position) where T : Entity => SpawnEntityFromGameObject(entity, position, Quaternion.identity);
		public T SpawnEntityFromGameObject<T> (T entity, Vector3 position, Quaternion rotation) where T : Entity
		{
			entity.transform.position = position;
			entity.transform.rotation = rotation;
			entity.gameObject.SetActive(true);
			return ScheduleEntityToSpawn(entity);
		}

		public T SpawnEntityFromPrefab<T> (T entityPrefab) where T : Entity => SpawnEntityFromPrefab(entityPrefab, Vector3.zero);
		public T SpawnEntityFromPrefab<T> (T entityPrefab, Vector3 position) where T : Entity => SpawnEntityFromPrefab(entityPrefab, position, Quaternion.identity);
		public T SpawnEntityFromPrefab<T> (T entityPrefab, Vector3 position, Quaternion rotation) where T : Entity
		{
			T entity = Instantiate(entityPrefab, position, rotation);
			RenameEntity(entity, entityPrefab.name);
			entity.gameObject.SetActive(true);
			return ScheduleEntityToSpawn(entity);
		}

		private void RenameEntity<T> (T entity, string baseName) where T : Entity
		{
			if (entity.appendSpawnIndexToName)
			{
				if (!entitySpawnCounts.ContainsKey(baseName))
				{
					entitySpawnCounts.Add(baseName, 0);
				}
				entitySpawnCounts[baseName]++;
				entity.name = $"{baseName} {entitySpawnCounts[baseName]}";
			}
			else
				entity.name = baseName;
		}

		private T ScheduleEntityToSpawn<T> (T entity) where T : Entity
		{
			if (!entity.isScheduledToSpawn && !entity.isSpawned && !entity.isScheduledToDespawn)
			{
				entity.isScheduledToSpawn = true;
				entitiesToSpawn.Add(entity);
			}
			return entity;
		}

		public void DespawnEntity (Entity entity)
		{
			if (entity.isScheduledToSpawn)
			{
				entity.isScheduledToSpawn = false;
				entitiesToSpawn.Remove(entity);
				entity.DepositToPoolOrDestroy();
			}
			else if (entity.isSpawned && !entity.isScheduledToDespawn)
			{
				entity.isScheduledToDespawn = true;
				entitiesToDespawn.Add(entity);
			}
		}

		protected virtual void UpdateEntities()
		{
			foreach (Entity entity in entities)
			{
				entity.EarlyUpdateEntityState();
			}
			foreach (Entity entity in entities)
			{
				entity.UpdateEntityState();
			}
			foreach (Entity entity in entities)
			{
				entity.LateUpdateEntityState();
			}
		}

		protected void SpawnEntitiesScheduledToSpawn()
		{
			if (entitiesToSpawn.Count > 0)
			{
				foreach (Entity entity in entitiesToSpawn)
				{
					entities.Add(entity);
					entity.gameObject.SetActive(true);
					entity.Spawn();
				}
				entitiesToSpawn.Clear();
			}
		}

		protected void DespawnEntitiesScheduledToDespawn()
		{
			if (entitiesToDespawn.Count > 0)
			{
				foreach (Entity entity in entitiesToDespawn)
				{
					entities.Remove(entity);
				}
				foreach (Entity entity in entitiesToDespawn)
				{
					entity.Despawn();
				}
				foreach (Entity entity in entitiesToDespawn)
				{
					entity.DepositToPoolOrDestroy();
				}
				entitiesToDespawn.Clear();
			}
		}
	}
}