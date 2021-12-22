using UnityEngine;

namespace SharedUnityMischief.Entities.Animated
{
	public abstract class EntityAnimation<TAnimation> : StateMachineBehaviour
	{
		[Header("Animation")]
		[SerializeField] private TAnimation _animation;
		[SerializeField] private Vector3 _authoredRootMotion = Vector3.zero;
		[SerializeField] private bool _skipFirstFrame = false;
		[Header("Programmatic Root Motion")]
		[SerializeField] private EntityAnimator.ProgrammaticRootMotionType _xRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		[SerializeField] private EntityAnimator.ProgrammaticRootMotionType _yRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		[SerializeField] private EntityAnimator.ProgrammaticRootMotionType _zRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		private EntityAnimator<TAnimation> _animator;

		public TAnimation animation => _animation;
		public string animationName => _animation.ToString();
		public Vector3 authoredRootMotion => _authoredRootMotion;
		public bool skipFirstFrame => _skipFirstFrame;
		public EntityAnimator.ProgrammaticRootMotionType xRootMotion => _xRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType yRootMotion => _yRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType zRootMotion => _zRootMotion;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_animator == null)
				_animator = animator.GetComponent<EntityAnimator<TAnimation>>();
			_animator.OnAnimationStart(this, stateInfo);
		}
	}
}