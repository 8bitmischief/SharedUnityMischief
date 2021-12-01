using UnityEngine;

namespace SharedUnityMischief.Entities.Animated
{
	public abstract class AnimatedEntity<TAnimator, TAnimation> : Entity where TAnimator : EntityAnimator<TAnimation>
	{
		private TAnimator _animator;

		public TAnimator animator => _animator;
		protected new TAnimation animation => _animator != null ? _animator.animation : default(TAnimation);
		protected float totalAnimationTime => _animator?.totalAnimationTime ?? 0f;
		protected int totalAnimationFrames => _animator?.totalAnimationFrames ?? 0;
		protected float animationTime => _animator?.animationTime ?? 0f;
		protected float animationDuration => _animator?.animationDuration ?? 0f;
		protected float percentAnimationCompleted => _animator?.percentAnimationCompleted ?? 0f;
		protected bool hasAnimationCompleted => _animator?.hasAnimationCompleted ?? false;
		protected bool isAnimationLooping => _animator?.isAnimationLooping ?? false;
		protected bool hasAnimationLooped => _animator?.hasAnimationLooped ?? false;
		protected int animationFrame => _animator?.animationFrame ?? 0;
		protected int animationFrameDuration => _animator?.animationFrameDuration ?? 0;
		protected float percentAnimationInterpolated => _animator?.percentAnimationInterpolated ?? 0f;
		protected float animationSpeed
		{
			get => _animator?.animationSpeed ?? 1f;
			set
			{
				if (_animator != null)
				{
					_animator.animationSpeed = value;
				}
			}
		}
		protected Vector3 authoredRootMotion => _animator?.authoredRootMotion ?? Vector3.zero;
		protected Vector3 programmaticRootMotion => _animator?.programmaticRootMotion ?? Vector3.zero;
		protected Vector3 programmaticRootMotionProgress => _animator?.programmaticRootMotionProgress ?? Vector3.zero;

		protected override void Awake()
		{
			base.Awake();
			_animator = GetComponent<TAnimator>();
		}

		protected virtual void OnEnable()
		{
			_animator.onStartAnimation += OnStartAnimation;
			_animator.onEndAnimation += OnEndAnimation;
			_animator.onChangeAnimation += OnChangeAnimation;
		}

		protected virtual void OnDisable()
		{
			_animator.onStartAnimation -= OnStartAnimation;
			_animator.onEndAnimation -= OnEndAnimation;
			_animator.onChangeAnimation -= OnChangeAnimation;
		}

		protected virtual void OnStartAnimation(TAnimation animation) {}
		protected virtual void OnEndAnimation(TAnimation animation) {}
		protected virtual void OnChangeAnimation(TAnimation animation, TAnimation prevAnimation) {}
	}
}