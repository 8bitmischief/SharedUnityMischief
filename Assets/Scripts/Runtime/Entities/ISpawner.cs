using UnityEngine;

namespace SharedUnityMischief.Entities
{
	public interface ISpawner
	{
		void OnSpawnChildEntity(Entity entity);
	}
}