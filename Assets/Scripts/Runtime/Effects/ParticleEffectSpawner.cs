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

		public ParticleEffect SpawnParticleEffect()
		{
			ParticleEffect effect = _pool.Withdraw<ParticleEffect>(transform.position, transform.rotation);
			effect.Play(true);
			return effect;
		}
	}
}