using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class EntityComponent : MonoBehaviour {
		private Entity _entity;
		protected Entity entity {
			get {
				if (_entity == null)
					_entity = GetComponentInParent<Entity>();
				return _entity;
			}
		}

		public virtual void OnSpawn () {}
		public virtual void UpdateState () {}
		public virtual void OnDespawn () {}
	}

	public abstract class EntityComponent<T> : EntityComponent where T : Entity {
		private T _typedEntity;
		protected new T entity {
			get {
				if (_typedEntity == null)
					_typedEntity = GetComponentInParent<T>();
				return _typedEntity;
			}
		}
	}
}
