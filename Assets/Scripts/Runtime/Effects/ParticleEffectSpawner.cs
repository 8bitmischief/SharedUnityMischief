using UnityEngine;
using SharedUnityMischief.Entities;
using SharedUnityMischief.Lifecycle;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects
{
	[ExecuteInEditMode]
	public class ParticleEffectSpawner : EntityComponent
	{
		[SerializeField] private PrefabPoolComponent _pool;

		[Header("Spawner Config")]
		[SerializeField] private bool _spawn = false;
		[SerializeField] private bool _spawnOnEnable = false;
		[SerializeField] private bool _playInEditMode = true;
		private bool _wasTriggered = false;

		private void OnEnable()
		{
			if (_spawnOnEnable)
			{
				if (Application.isPlaying)
					SpawnParticleEffect();
				else if (_playInEditMode)
					TriggerParticleEffect();
			}
		}

		private void Update()
		{
			if (!Application.isPlaying && _playInEditMode)
			{
				if (_spawn && !_wasTriggered)
					TriggerParticleEffect();
				_wasTriggered = _spawn;
			}
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
			effect.transform.localScale = _pool.prefab.transform.lossyScale;
			effect.PlayOnceThenDestroy();
			return effect;
		}

		private void TriggerParticleEffect()
		{
			if (_pool.prefab != null && !_pool.isActualPrefab)
				_pool.prefab.GetComponent<ParticleEffect>().Play();
		}
	}
}