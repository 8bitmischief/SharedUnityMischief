using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Entities
{
	public abstract class Entity : EntityComponent, IPoolable
	{
		[Header("Entity Config")]
		[SerializeField] private bool _appendSpawnIndexToName = false;
		private EntityManager _entityManager;
		private List<EntityComponent> _components;
		private bool _isSpawned = false;
		private bool _isScheduledToSpawn = false;
		private bool _isScheduledToDespawn = false;

		public EntityManager entityManager => _entityManager;
		public bool appendSpawnIndexToName => _appendSpawnIndexToName;
		public Func<bool> DepositToPool { get; set; }
		public bool isPooled => DepositToPool != null;
		public bool isSpawned => _isSpawned;
		public bool isScheduledToSpawn { get => _isScheduledToSpawn; set => _isScheduledToSpawn = value; }
		public bool isScheduledToDespawn { get => _isScheduledToDespawn; set => _isScheduledToDespawn = value; }
		public override Entity entity => this;
		public override int componentUpdateOrder => EntityComponent.EntityUpdateOrder;

		protected virtual void Awake()
		{
			_components = GetComponentsInChildren<EntityComponent>(true).ToList();
			_components.Sort((a, b) => a.componentUpdateOrder - b.componentUpdateOrder);
		}

		public bool Spawn(EntityManager entityManager)
		{
			if (!_isSpawned)
			{
				_entityManager = entityManager;
				_isSpawned = true;
				_isScheduledToSpawn = false;
				foreach (EntityComponent component in _components)
					if (component.isActiveAndEnabled)
						component.OnSpawn();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void EarlyUpdateEntityState()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.EarlyUpdateState();
		}

		public void UpdateEntityState()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.UpdateState();
		}

		public void LateUpdateEntityState()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.LateUpdateState();
		}

		public bool Despawn()
		{
			if (_isSpawned)
			{
				_isSpawned = false;
				_isScheduledToDespawn = false;
				foreach (EntityComponent component in _components)
					if (component.isActiveAndEnabled)
						component.OnDespawn();
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool DepositToPoolOrDestroy()
		{
			if (isPooled && DepositToPool())
			{
				return true;
			}
			else
			{
				Destroy(gameObject);
				return false;
			}
		}

		public bool DepositToPoolOrDeactivate()
		{
			if (isPooled && DepositToPool())
			{
				return true;
			}
			else
			{
				gameObject.SetActive(false);
				return false;
			}
		}

		public virtual void OnWithdrawFromPool()
		{
			gameObject.SetActive(true);
			foreach (EntityComponent component in _components)
				component.ResetComponent();
		}

		public virtual void OnDepositToPool()
		{
			gameObject.SetActive(false);
		}

		protected T SpawnEntityFromPool<T>(PrefabPool<T> pool) where T : Entity => _entityManager.SpawnEntityFromPool<T>(pool);
		protected T SpawnEntityFromPool<T>(PrefabPool<T> pool, Vector3 position) where T : Entity => _entityManager.SpawnEntityFromPool<T>(pool, position);
		protected T SpawnEntityFromPool<T>(PrefabPool<T> pool, Vector3 position, Quaternion rotation) where T : Entity => _entityManager.SpawnEntityFromPool<T>(pool, position, rotation);
		protected T SpawnEntityFromPool<T>(PrefabPoolMonoBehaviour<T> pool) where T : Entity => _entityManager.SpawnEntityFromPool<T>(pool);
		protected T SpawnEntityFromPool<T>(PrefabPoolMonoBehaviour<T> pool, Vector3 position) where T : Entity => _entityManager.SpawnEntityFromPool<T>(pool, position);
		protected T SpawnEntityFromPool<T>(PrefabPoolMonoBehaviour<T> pool, Vector3 position, Quaternion rotation) where T : Entity => _entityManager.SpawnEntityFromPool<T>(pool, position, rotation);
		protected T SpawnEntityFromGameObject<T>(T entity) where T : Entity => _entityManager.SpawnEntityFromGameObject<T>(entity);
		protected T SpawnEntityFromGameObject<T>(T entity, Vector3 position) where T : Entity => _entityManager.SpawnEntityFromGameObject<T>(entity, position);
		protected T SpawnEntityFromGameObject<T>(T entity, Vector3 position, Quaternion rotation) where T : Entity => _entityManager.SpawnEntityFromGameObject<T>(entity, position, rotation);
		protected T SpawnEntityFromPrefab<T>(T entityPrefab) where T : Entity => _entityManager.SpawnEntityFromPrefab<T>(entityPrefab);
		protected T SpawnEntityFromPrefab<T>(T entityPrefab, Vector3 position) where T : Entity => _entityManager.SpawnEntityFromPrefab<T>(entityPrefab, position);
		protected T SpawnEntityFromPrefab<T>(T entityPrefab, Vector3 position, Quaternion rotation) where T : Entity => _entityManager.SpawnEntityFromPrefab<T>(entityPrefab, position, rotation);
		protected void DespawnEntity(Entity entity) => _entityManager.DespawnEntity(entity);
	}
}