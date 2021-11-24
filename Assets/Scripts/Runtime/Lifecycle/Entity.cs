using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Lifecycle {
	public abstract class Entity : EntityComponent, IPoolable {
		[Header("Entity Config")]
		[SerializeField] public bool appendSpawnIndexToName = false;

		public override Entity entity => this;
		public override int componentUpdateOrder => EntityComponent.entityUpdateOrder;

		public bool isSpawned { get; private set; } = false;
		public bool scheduledToSpawn { get; set; } = false;
		public bool scheduledToDespawn { get; set; } = false;
		public bool isPooled => DepositToPool != null;

		public Func<bool> DepositToPool { get; set; } = null;

		private List<EntityComponent> components;

		protected virtual void Awake () {
			components = GetComponentsInChildren<EntityComponent>().ToList();
			components.Sort((a, b) => a.componentUpdateOrder - b.componentUpdateOrder);
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

		public bool DepositToPoolOrDestroy () {
			if (isPooled && DepositToPool())
				return true;
			else {
				Destroy(gameObject);
				return false;
			}
		}

		public bool DepositToPoolOrDeactivate () {
			if (isPooled && DepositToPool())
				return true;
			else {
				gameObject.SetActive(false);
				return false;
			}
		}

		public virtual void OnWithdrawFromPool () {
			gameObject.SetActive(true);
			foreach (EntityComponent component in components)
				component.ResetComponent();
		}

		public virtual void OnDepositToPool () {
			gameObject.SetActive(false);
		}
	}
}