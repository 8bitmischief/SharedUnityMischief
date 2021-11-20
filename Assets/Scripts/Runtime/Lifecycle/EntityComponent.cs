using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class EntityComponent : MonoBehaviour, IEntityComponent {
		private Entity _entity;
		public virtual Entity entity {
			get {
				if (_entity == null)
					_entity = GetComponentInParent<Entity>();
				return _entity;
			}
		}

		public virtual void Reset () {}
		public virtual void OnSpawn () {}
		public virtual void EarlyUpdateState () {}
		public virtual void UpdateState () {}
		public virtual void LateUpdateState () {}
		public virtual void OnDespawn () {}
	}

	public abstract class EntityComponent<T> : EntityComponent where T : Entity {
		private T _typedEntity;
		public new T entity {
			get {
				if (_typedEntity == null)
					_typedEntity = GetComponentInParent<T>();
				return _typedEntity;
			}
		}
	}
}
