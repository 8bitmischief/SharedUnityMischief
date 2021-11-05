using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class Entity : EntityComponent {
		private EntityComponent[] components;
		
		protected virtual void Awake () {
			components = GetComponentsInChildren<EntityComponent>();
		}

		public void UpdateEntityState () {
			foreach (EntityComponent component in components)
				component.UpdateState();
		}
	}
}