using System.Collections.Generic;
using UnityEngine;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Entities
{
	public class EntityManager : MonoBehaviour
	{
		private List<Entity> _entities = new List<Entity>();
		private List<Entity> _entitiesToSpawn = new List<Entity>();
		private List<Entity> _entitiesToDespawn = new List<Entity>();
		private Dictionary<string, int> _entitySpawnCounts = new Dictionary<string, int>();

		public int numEntities => _entities.Count;
		public int numEntitiesToSpawn => _entitiesToSpawn.Count;
		public int numEntitiesToDespawn => _entitiesToDespawn.Count;

		private void Start()
		{
			foreach (Entity entity in FindObjectsOfType<Entity>())
				ScheduleEntityToSpawn(entity);
			SpawnEntitiesScheduledToSpawn();
		}

		public void EarlyUpdateEntities()
		{
			foreach (Entity entity in _entities)
				entity.DoEarlyUpdateState();
		}

		public void UpdateEntities()
		{
			foreach (Entity entity in _entities)
				entity.DoUpdateState();
		}

		public void CheckEntityInteractions()
		{
			foreach (Entity entity in _entities)
				entity.DoCheckInteractions();
		}

		public void LateUpdateEntities()
		{
			foreach (Entity entity in _entities)
				entity.DoLateUpdateState();
		}

		public void RenderEntities()
		{
			foreach (Entity entity in _entities)
				entity.DoRender();
		}

		public void SpawnAndDespawnEntities()
		{
			DespawnEntitiesScheduledToDespawn();
			SpawnEntitiesScheduledToSpawn();
			DespawnEntitiesScheduledToDespawn();
		}

		public Entity SpawnEntityFromPool(IPool pool) => SpawnEntityFromPool<Entity>(pool);
		public Entity SpawnEntityFromPool(IPool pool, Vector3 position) => SpawnEntityFromPool<Entity>(pool, position);
		public Entity SpawnEntityFromPool(IPool pool, Vector3 position, Quaternion rotation) => SpawnEntityFromPool<Entity>(pool, position, rotation);
		public T SpawnEntityFromPool<T>(IPool pool) where T : Entity
		{
			T entity = pool.Withdraw<T>();
			RenameEntity(entity, pool.prefab.name);
			return SpawnEntityFromGameObject(entity);
		}
		public T SpawnEntityFromPool<T>(IPool pool, Vector3 position) where T : Entity
		{
			T entity = pool.Withdraw<T>();
			RenameEntity(entity, pool.prefab.name);
			return SpawnEntityFromGameObject(entity, position);
		}
		public T SpawnEntityFromPool<T>(IPool pool, Vector3 position, Quaternion rotation) where T : Entity
		{
			T entity = pool.Withdraw<T>();
			RenameEntity(entity, pool.prefab.name);
			return SpawnEntityFromGameObject(entity, position, rotation);
		}

		public Entity SpawnEntityFromGameObject(Entity entity) => SpawnEntityFromGameObject<Entity>(entity);
		public Entity SpawnEntityFromGameObject(Entity entity, Vector3 position) => SpawnEntityFromGameObject<Entity>(entity, position);
		public Entity SpawnEntityFromGameObject(Entity entity, Vector3 position, Quaternion rotation) => SpawnEntityFromGameObject<Entity>(entity, position, rotation);
		public T SpawnEntityFromGameObject<T>(T entity) where T : Entity
		{
			return SetUpEntityToSpawn<T>(entity);
		}
		public T SpawnEntityFromGameObject<T>(T entity, Vector3 position) where T : Entity
		{
			entity.transform.position = position;
			return SpawnEntityFromGameObject(entity);
		}
		public T SpawnEntityFromGameObject<T>(T entity, Vector3 position, Quaternion rotation) where T : Entity
		{
			entity.transform.position = position;
			entity.transform.rotation = rotation;
			return SpawnEntityFromGameObject(entity);
		}

		public Entity SpawnEntityFromPrefab(Entity entityPrefab) => SpawnEntityFromPrefab<Entity>(entityPrefab);
		public Entity SpawnEntityFromPrefab(Entity entityPrefab, Vector3 position) => SpawnEntityFromPrefab<Entity>(entityPrefab, position);
		public Entity SpawnEntityFromPrefab(Entity entityPrefab, Vector3 position, Quaternion rotation) => SpawnEntityFromPrefab<Entity>(entityPrefab, position, rotation);
		public T SpawnEntityFromPrefab<T>(T entityPrefab) where T : Entity
		{
			return SetUpEntityToSpawn<T>(Instantiate(entityPrefab), entityPrefab.name);
		}
		public T SpawnEntityFromPrefab<T>(T entityPrefab, Vector3 position) where T : Entity
		{
			return SetUpEntityToSpawn<T>(Instantiate(entityPrefab, position, Quaternion.identity), entityPrefab.name);
		}
		public T SpawnEntityFromPrefab<T>(T entityPrefab, Vector3 position, Quaternion rotation) where T : Entity
		{
			return SetUpEntityToSpawn<T>(Instantiate(entityPrefab, position, rotation), entityPrefab.name);
		}

		private T SetUpEntityToSpawn<T>(T entity, string name = null) where T : Entity
		{
			if (name != null)
				RenameEntity(entity, name);
			entity.gameObject.SetActive(true);
			ScheduleEntityToSpawn(entity);
			return entity;
		}

		private void RenameEntity(Entity entity, string baseName)
		{
			if (entity.appendSpawnIndexToName)
			{
				if (!_entitySpawnCounts.ContainsKey(baseName))
					_entitySpawnCounts.Add(baseName, 0);
				_entitySpawnCounts[baseName]++;
				entity.name = $"{baseName} {_entitySpawnCounts[baseName]}";
			}
			else
				entity.name = baseName;
		}

		private void ScheduleEntityToSpawn(Entity entity)
		{
			if (!entity.isScheduledToSpawn && !entity.isSpawned && !entity.isScheduledToDespawn)
			{
				entity.isScheduledToSpawn = true;
				_entitiesToSpawn.Add(entity);
			}
		}

		public void DespawnEntity(Entity entity)
		{
			if (entity.isScheduledToSpawn)
			{
				entity.isScheduledToSpawn = false;
				_entitiesToSpawn.Remove(entity);
				entity.DepositToPoolOrDestroy();
			}
			else if (entity.isSpawned && !entity.isScheduledToDespawn)
			{
				entity.isScheduledToDespawn = true;
				_entitiesToDespawn.Add(entity);
			}
		}

		protected void SpawnEntitiesScheduledToSpawn()
		{
			foreach (Entity entity in _entitiesToSpawn)
			{
				_entities.Add(entity);
				entity.gameObject.SetActive(true);
				entity.Spawn(this);
			}
			_entitiesToSpawn.Clear();
		}

		protected void DespawnEntitiesScheduledToDespawn()
		{
			foreach (Entity entity in _entitiesToDespawn)
				_entities.Remove(entity);
			foreach (Entity entity in _entitiesToDespawn)
				entity.Despawn();
			foreach (Entity entity in _entitiesToDespawn)
				entity.DepositToPoolOrDestroy();
			_entitiesToDespawn.Clear();
		}
	}
}