using UnityEngine;
using SharedUnityMischief.Entities;
using SharedUnityMischief.Lifecycle;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects
{
	public class ParticleEffectSpawner : EntityComponent
	{
		[SerializeField] private PrefabPoolMonoBehaviour _pool;

		[Header("Spawner Config")]
		[SerializeField] private bool _spawn = false;
		[SerializeField] private bool _spawnOnEnable = false;

		private bool _wasTriggered = false;

		private void OnEnable()
		{
			if (_spawnOnEnable)
				SpawnParticleEffect();
		}

		public override void UpdateState()
		{
			if (!UpdateLoop.I.isInterpolating)
			{
				if (_spawn && !_wasTriggered)
					SpawnParticleEffect();
				_wasTriggered = _spawn;
			}
		}

		public ParticleEffect SpawnParticleEffect() => SpawnParticleEffect(transform.position, transform.rotation);
		public ParticleEffect SpawnParticleEffect(Vector3 position) => SpawnParticleEffect(position, transform.rotation);
		public ParticleEffect SpawnParticleEffect(Vector3 position, Quaternion rotation)
		{
			ParticleEffect effect = _pool.Withdraw<ParticleEffect>(position, rotation);
			effect.Play(true);
			return effect;
		}
	}
}