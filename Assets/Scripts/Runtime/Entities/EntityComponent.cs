using UnityEngine;

namespace SharedUnityMischief.Entities
{
	public abstract class EntityComponent : MonoBehaviour, IEntityComponent
	{
		public static readonly int defaultComponentUpdateOrder = 0;
		public static readonly int animatorUpdateOrder = -100;
		public static readonly int entityUpdateOrder = 100;
		public static readonly int controllerUpdateOrder = 200;

		public virtual int componentUpdateOrder => defaultComponentUpdateOrder;

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
		private Entity _entity;

		public virtual void ResetComponent() {}
		public virtual void OnSpawn() {}
		public virtual void EarlyUpdateState() {}
		public virtual void UpdateState() {}
		public virtual void LateUpdateState() {}
		public virtual void OnDespawn() {}
	}

	public abstract class EntityComponent<T> : EntityComponent where T : Entity
	{
		public new T entity
		{
			get
			{
				if (typedEntity == null)
				{
					typedEntity = GetComponentInParent<T>();
				}
				return typedEntity;
			}
		}
		private T typedEntity;
	}
}
