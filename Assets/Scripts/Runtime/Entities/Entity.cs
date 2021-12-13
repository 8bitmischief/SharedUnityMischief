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

		public override Entity entity => this;
		public override EntityManager entityManager => _entityManager;
		public bool appendSpawnIndexToName => _appendSpawnIndexToName;
		public Func<bool> DepositToPool { get; set; }
		public bool isPooled => DepositToPool != null;
		public bool isSpawned => _isSpawned;
		public bool isScheduledToSpawn { get => _isScheduledToSpawn; set => _isScheduledToSpawn = value; }
		public bool isScheduledToDespawn { get => _isScheduledToDespawn; set => _isScheduledToDespawn = value; }
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

		public void DoEarlyUpdateState()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.EarlyUpdateState();
		}

		public void DoUpdateState()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.UpdateState();
		}

		public void DoCheckInteractions()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.CheckInteractions();
		}

		public void DoLateUpdateState()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.LateUpdateState();
		}

		public void DoRender()
		{
			foreach (EntityComponent component in _components)
				if (component.isActiveAndEnabled)
					component.Render();
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
	}
}