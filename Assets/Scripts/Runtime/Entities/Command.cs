using UnityEngine;

namespace SharedUnityMischief.Entities
{
	public abstract class Command
	{
		private bool _hasStarted;

		public bool hasStarted { get => _hasStarted; set => _hasStarted = value; }

		public virtual void Reset() {}
		public virtual void Start() {}
		public virtual void Update() {}
		public virtual void End() {}
		public abstract bool IsDone();
	}

	public abstract class Command<TEntity> : Command where TEntity : Entity
	{
		private TEntity _entity;

		public TEntity entity { get => _entity; set => _entity = value; }
	}
}