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

		public T state => _state;
		public string stateName => _state.ToString();
		public Vector3 authoredRootMotion => _authoredRootMotion;
		public bool undoAuthoredRootMotion => _undoAuthoredRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType xRootMotion => _xRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType yRootMotion => _yRootMotion;
		public EntityAnimator.ProgrammaticRootMotionType zRootMotion => _zRootMotion;

		private EntityAnimator<T> animator;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.animator == null)
			{
				this.animator = animator.GetComponent<EntityAnimator<T>>();
			}
			this.animator.TriggerAnimationStart(this, stateInfo);
		}
	}
}
