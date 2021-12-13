using UnityEngine;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Entities
{
	public abstract class EntityComponent : MonoBehaviour, IEntityComponent
	{
		public const int DefaultComponentUpdateOrder = 0;
		public const int AnimatorUpdateOrder = -100;
		public const int EntityUpdateOrder = 100;
		public const int ControllerUpdateOrder = 200;

		private Entity _entity;

		public virtual Entity entity
		{
			get
			{
				if (_entity == null)
					_entity = GetComponentInParent<Entity>();
				return _entity;
			}
		}
		public virtual EntityManager entityManager => entity.entityManager;
		public virtual int componentUpdateOrder => DefaultComponentUpdateOrder;

		public virtual void ResetComponent() {}
		public virtual void OnSpawn() {}
		public virtual void EarlyUpdateState() {}
		public virtual void UpdateState() {}
		public virtual void CheckInteractions() {}
		public virtual void LateUpdateState() {}
		public virtual void Render() {}
		public virtual void OnDespawn() {}

		protected Entity SpawnEntityFromPool(PrefabPool pool) => entityManager.SpawnEntityFromPool(pool);
		protected Entity SpawnEntityFromPool(PrefabPool pool, Vector3 position) => entityManager.SpawnEntityFromPool(pool, position);
		protected Entity SpawnEntityFromPool(PrefabPool pool, Vector3 position, Quaternion rotation) => entityManager.SpawnEntityFromPool(pool, position, rotation);
		protected T SpawnEntityFromPool<T>(PrefabPool pool) where T : Entity => entityManager.SpawnEntityFromPool<T>(pool);
		protected T SpawnEntityFromPool<T>(PrefabPool pool, Vector3 position) where T : Entity => entityManager.SpawnEntityFromPool<T>(pool, position);
		protected T SpawnEntityFromPool<T>(PrefabPool pool, Vector3 position, Quaternion rotation) where T : Entity => entityManager.SpawnEntityFromPool<T>(pool, position, rotation);
		protected Entity SpawnEntityFromPool(PrefabPoolMonoBehaviour pool) => entityManager.SpawnEntityFromPool(pool);
		protected Entity SpawnEntityFromPool(PrefabPoolMonoBehaviour pool, Vector3 position) => entityManager.SpawnEntityFromPool(pool, position);
		protected Entity SpawnEntityFromPool(PrefabPoolMonoBehaviour pool, Vector3 position, Quaternion rotation) => entityManager.SpawnEntityFromPool(pool, position, rotation);
		protected T SpawnEntityFromPool<T>(PrefabPoolMonoBehaviour pool) where T : Entity => entityManager.SpawnEntityFromPool<T>(pool);
		protected T SpawnEntityFromPool<T>(PrefabPoolMonoBehaviour pool, Vector3 position) where T : Entity => entityManager.SpawnEntityFromPool<T>(pool, position);
		protected T SpawnEntityFromPool<T>(PrefabPoolMonoBehaviour pool, Vector3 position, Quaternion rotation) where T : Entity => entityManager.SpawnEntityFromPool<T>(pool, position, rotation);
		protected Entity SpawnEntityFromGameObject(Entity entity) => entityManager.SpawnEntityFromGameObject(entity);
		protected Entity SpawnEntityFromGameObject(Entity entity, Vector3 position) => entityManager.SpawnEntityFromGameObject(entity, position);
		protected Entity SpawnEntityFromGameObject(Entity entity, Vector3 position, Quaternion rotation) => entityManager.SpawnEntityFromGameObject(entity, position, rotation);
		protected T SpawnEntityFromGameObject<T>(T entity) where T : Entity => entityManager.SpawnEntityFromGameObject<T>(entity);
		protected T SpawnEntityFromGameObject<T>(T entity, Vector3 position) where T : Entity => entityManager.SpawnEntityFromGameObject<T>(entity, position);
		protected T SpawnEntityFromGameObject<T>(T entity, Vector3 position, Quaternion rotation) where T : Entity => entityManager.SpawnEntityFromGameObject<T>(entity, position, rotation);
		protected T SpawnEntityFromPrefab<T>(T entityPrefab) where T : Entity => entityManager.SpawnEntityFromPrefab<T>(entityPrefab);
		protected T SpawnEntityFromPrefab<T>(T entityPrefab, Vector3 position) where T : Entity => entityManager.SpawnEntityFromPrefab<T>(entityPrefab, position);
		protected T SpawnEntityFromPrefab<T>(T entityPrefab, Vector3 position, Quaternion rotation) where T : Entity => entityManager.SpawnEntityFromPrefab<T>(entityPrefab, position, rotation);
		protected Entity SpawnEntityFromPrefab(Entity entityPrefab) => entityManager.SpawnEntityFromPrefab(entityPrefab);
		protected Entity SpawnEntityFromPrefab(Entity entityPrefab, Vector3 position) => entityManager.SpawnEntityFromPrefab(entityPrefab, position);
		protected Entity SpawnEntityFromPrefab(Entity entityPrefab, Vector3 position, Quaternion rotation) => entityManager.SpawnEntityFromPrefab(entityPrefab, position, rotation);

		protected void DespawnEntity(Entity entity) => entityManager.DespawnEntity(entity);
	}

	public abstract class EntityComponent<T> : EntityComponent where T : Entity
	{
		private T _typedEntity;

		public new T entity
		{
			get
			{
				if (_typedEntity == null)
				{
					_typedEntity = GetComponentInParent<T>();
				}
				return _typedEntity;
			}
		}
	}
}