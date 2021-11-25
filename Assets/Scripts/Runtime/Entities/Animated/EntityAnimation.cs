using UnityEngine;

namespace SharedUnityMischief.Entities.Animated
{
	public abstract class EntityAnimation<T> : StateMachineBehaviour
	{
		[Header("Animation")]
		public T state;
		public Vector3 authoredRootMotion = Vector3.zero;
		public bool undoAuthoredRootMotion = false;
		[Header("Programmatic Root Motion")]
		public EntityAnimator.ProgrammaticRootMotionType xRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		public EntityAnimator.ProgrammaticRootMotionType yRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;
		public EntityAnimator.ProgrammaticRootMotionType zRootMotion = EntityAnimator.ProgrammaticRootMotionType.UseRootMotionProgress;

		private EntityAnimator<T> animator;
		public string stateName => state.ToString();

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
