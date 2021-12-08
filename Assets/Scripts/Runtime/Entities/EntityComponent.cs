using UnityEngine;

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
				{
					_entity = GetComponentInParent<Entity>();
				}
				return _entity;
			}
		}
		public virtual int componentUpdateOrder => DefaultComponentUpdateOrder;

		public virtual void ResetComponent() {}
		public virtual void OnSpawn() {}
		public virtual void EarlyUpdateState() {}
		public virtual void UpdateState() {}
		public virtual void CheckInteractions() {}
		public virtual void LateUpdateState() {}
		public virtual void Render() {}
		public virtual void OnDespawn() {}
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