using UnityEngine;
using UnityEngine.VFX;
using SharedUnityMischief.Pool;

namespace SharedUnityMischief.Effects
{
	public class ParticleEffect : PoolableMonoBehavior
	{
		[SerializeField] private float duration = 1.0f;

		public bool isPlaying { get; private set; } = false;

		private VisualEffect[] visualEffects;
		private float playTime = 0f;
		private EndBehavior endBehavior = EndBehavior.Destroy;

		private void Awake()
		{
			visualEffects = GetComponentsInChildren<VisualEffect>();
		}

		private void Update()
		{
			if (isPlaying)
			{
				playTime += Time.deltaTime;
				if (playTime >= duration)
				{
					if (endBehavior == EndBehavior.Loop)
					{
						PlayEndlessly();
					}
					else
					{
						Stop();
						if (endBehavior == EndBehavior.Destroy)
						{
							DepositToPoolOrDestroy();
						}
					}
				}
			}
		}

		public override void OnWithdrawFromPool() {}

		public override void OnDepositToPool() {}

		public void PlayEndlessly() => Play(EndBehavior.Loop);

		public void Play(bool destroyWhenDonePlaying = true)
			=> Play(destroyWhenDonePlaying ? EndBehavior.Destroy : EndBehavior.None);

		private void Play(EndBehavior endBehavior)
		{
			isPlaying = true;
			playTime = 0f;
			this.endBehavior = endBehavior;
			foreach (VisualEffect visualEffect in visualEffects)
			{
				visualEffect.Play();
			}
		}

		public void Stop()
		{
			isPlaying = false;
			foreach (VisualEffect visualEffect in visualEffects)
			{
				visualEffect.Stop();
			}
		}

		private enum EndBehavior
		{
			None = 0,
			Destroy = 1,
			Loop = 2
		}
	}
}