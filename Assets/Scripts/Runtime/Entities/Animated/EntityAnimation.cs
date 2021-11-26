using UnityEngine;

namespace SharedUnityMischief.Entities.Animated
{
	public abstract class EntityAnimation<T> : StateMachineBehaviour
	{
		[Header("Animation")]
		[SerializeField] private T _state;
		[SerializeField] private Vector3 _authoredRootMotion = Vector3.zero;
		[SerializeField] private bool _undoAuthoredRootMotion = false;
		[Header("Programmatic Root Motion")]
		[SerializeField] private EntityAnimator.ProgrammaticRootMotionType _xRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		[SerializeField] private EntityAnimator.ProgrammaticRootMotionType _yRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		[SerializeField] private EntityAnimator.ProgrammaticRootMotionType _zRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		private EntityAnimator<T> _animator;

		public T state => _state;
		public string stateName => _state.ToString();
		public Vector3 authoredRootMotion => _authoredRootMotion;
		public bool undoAuthoredRootMotion => _undoAuthoredRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType xRootMotion => _xRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType yRootMotion => _yRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType zRootMotion => _zRootMotion;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_animator == null)
			{
				_animator = animator.GetComponent<EntityAnimator<T>>();
			}
			_animator.TriggerAnimationStart(this, stateInfo);
		}
	}
}
