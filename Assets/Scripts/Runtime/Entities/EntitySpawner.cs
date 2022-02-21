using System;
using UnityEngine;
using SharedUnityMischief.Lifecycle;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Entities
{
	public class EntitySpawner : EntitySpawner<Entity> {}

	public class EntitySpawner<TEntity> : EntityComponent where TEntity : Entity
	{
		[SerializeField] private PrefabPoolComponent _pool;

		[Header("Spawner Config")]
		[SerializeField] private bool _spawn = false;
		[SerializeField] private bool _spawnOnEnable = false;
		private ISpawner _spawnerEntity;
		private bool _wasTriggered = false;

		public event Action<TEntity> onSpawnChildEntity;

		private void Awake()
		{
			if (entity is ISpawner)
				_spawnerEntity = entity as ISpawner;
		}

		private void OnEnable()
		{
			if (_spawnOnEnable)
				SpawnChildEntity();
		}

		public override void UpdateState()
		{
			if (!UpdateLoop.I.isInterpolating)
			{
				if (_spawn && !_wasTriggered)
					SpawnChildEntity();
				_wasTriggered = _spawn;
			}
		}

		public TEntity SpawnChildEntity()
		{
			TEntity entity = SpawnEntityFromPool<TEntity>(_pool, transform.position, transform.rotation);
			OnSpawnChildEntity(entity);
			if (_spawnerEntity != null)
				_spawnerEntity.OnSpawnChildEntity(entity);
			onSpawnChildEntity?.Invoke(entity);
			return entity;
		}

		protected virtual void OnSpawnChildEntity(TEntity entity) {}
	}
}