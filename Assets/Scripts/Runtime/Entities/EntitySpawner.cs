using System;
using UnityEngine;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Entities
{
	public class EntitySpawner : EntityComponent
	{
		[SerializeField] private PrefabPoolMonoBehaviour _pool;

		[Header("Spawner Config")]
		[SerializeField] private bool _spawnOnEnable = true;
		private ISpawner _spawnerEntity;

		public event Action<Entity> onSpawnChild;

		private void Start()
		{
			if (entity is ISpawner)
				_spawnerEntity = entity as ISpawner;
		}

		private void OnEnable()
		{
			if (_spawnOnEnable)
				SpawnChild();
		}

		public Entity SpawnChild()
		{
			Entity entity = SpawnEntityFromPool(_pool, transform.position, transform.rotation);
			OnSpawnChild(entity);
			if (_spawnerEntity != null)
				_spawnerEntity.OnSpawnChild(entity);
			onSpawnChild?.Invoke(entity);
			return entity;
		}

		protected virtual void OnSpawnChild(Entity entity) {}
	}
}