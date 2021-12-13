using UnityEngine;

namespace SharedUnityMischief.Entities
{
	public interface ISpawner
	{
		void OnSpawnChild(Entity entity);
	}
}