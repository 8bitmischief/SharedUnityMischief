using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	[RequireComponent(typeof(Animator))]
	public abstract class EntityAnimator : EntityComponent {
		// In order to properly trigger events, we want to overshoot each frame a tiny bit
		// This variable controlss how much each frame gets overshot and undershot
		protected static readonly float updateFudgeTime = UpdateLoop.timePerUpdate / 100f;

		[SerializeField] protected Vector3 rootMotionProgress = Vector3.zero;

		public abstract string stateName { get; }
		public abstract float timeInState { get; protected set; }
		public abstract int framesInState { get; protected set; }
		public abstract float animationTime { get; protected set; }
		public abstract float animationDuration { get; protected set; }
		public abstract float percentAnimationCompleted { get; protected set; }
		public abstract bool isAnimationLooping { get; protected set; }
		public abstract bool hasAnimationLooped { get; protected set; }
		public abstract int animationFrame { get; protected set; }
		public abstract int animationFrameDuration { get; protected set; }
		public abstract float percentInterpolated { get; protected set; }
		public abstract Vector3 authoredRootMotion { get; protected set; }
		public abstract Vector3 programmaticRootMotion { get; protected set; }
		public abstract Vector3 programmaticRootMotionProgress { get; protected set; }

		public enum ProgrammaticRootMotionType {
			None = 0,
			UseRootMotionProgress = 1,
			UseAuthoredRootMotion = 2
		}
	}

	public abstract class EntityAnimator<T> : EntityAnimator {
		public T state { get; private set; }
		public override string stateName => state.ToString();
		public override float timeInState { get; protected set; } = 0f;
		public override int framesInState { get; protected set; } = 0;
		public override float animationTime { get; protected set; } = 0f;
		public override float animationDuration { get; protected set; } = 0f;
		public override float percentAnimationCompleted { get; protected set; } = 0f;
		public override bool isAnimationLooping { get; protected set; } = false;
		public override bool hasAnimationLooped { get; protected set; } = false;
		public override int animationFrame { get; protected set; } = 0;
		public override int animationFrameDuration { get; protected set; } = 0;
		public override float percentInterpolated { get; protected set; } = 0f;
		public override Vector3 authoredRootMotion { get; protected set; } = Vector3.zero;
		public override Vector3 programmaticRootMotion { get; protected set; } = Vector3.zero;
		public override Vector3 programmaticRootMotionProgress { get; protected set; } = Vector3.zero;

		public Action<T> onEnterState;
		public Action<T> onLeaveState;
		public Action<T, T> onChangeState;

		private Animator animator;
		private List<AnimationEvent> triggeredEvents = new List<AnimationEvent>();
		private bool didStartNewAnimation = false;
		private bool undoAuthoredRootMotion = false;
		private Vector3 authoredRootMotionTraveledSoFar = Vector3.zero;
		private Vector3 programmaticRootMotionTraveledSoFar = Vector3.zero;
		private Vector3 rootMotionForTriggeredAnimation = Vector3.zero;
		private ProgrammaticRootMotionType xProgrammaticRootMotion = ProgrammaticRootMotionType.None;
		private ProgrammaticRootMotionType yProgrammaticRootMotion = ProgrammaticRootMotionType.None;
		private ProgrammaticRootMotionType zProgrammaticRootMotion = ProgrammaticRootMotionType.None;

		protected virtual void Awake () {
			animator = GetComponent<Animator>();
			animator.speed = 0f;
			UpdateAnimator(updateFudgeTime);
		}

		public override void UpdateState () {
			if (UpdateLoop.I.isInterpolating)
				InterpolateAnimation();
			else {
				AdvanceToNextFrame();
				// Handle animation events that were triggered by moving into the next frame
				foreach (AnimationEvent evt in triggeredEvents)
					OnAnimationEvent(evt);
				triggeredEvents.Clear();
			}
		}

		public void TriggerAnimationStart (EntityAnimation<T> animation, AnimatorStateInfo stateInfo) {
			didStartNewAnimation = true;
			T prevState = state;
			// Leave the previous state
			onLeaveState?.Invoke(state);
			// Enter the new state
			state = animation.state;
			timeInState = 0f;
			framesInState = 0;
			undoAuthoredRootMotion = animation.undoAuthoredRootMotion;
			authoredRootMotion = animation.authoredRootMotion;
			authoredRootMotionTraveledSoFar = Vector3.zero;
			programmaticRootMotion = rootMotionForTriggeredAnimation;
			if (!undoAuthoredRootMotion)
				programmaticRootMotion -= Vector3.Scale(authoredRootMotion, transform.localScale);
			programmaticRootMotionTraveledSoFar = Vector3.zero;
			programmaticRootMotionProgress = Vector3.zero;
			xProgrammaticRootMotion = animation.xRootMotion;
			yProgrammaticRootMotion = animation.yRootMotion;
			zProgrammaticRootMotion = animation.zRootMotion;
			RefreshAnimationState(stateInfo);
			onEnterState?.Invoke(state);
			// Trigger change states
			onChangeState?.Invoke(state, prevState);
		}

		public void Trigger (int hash) => Trigger(hash, Vector3.zero, false);

		public void Trigger (int hash, Vector3 rootMotion, bool isTargetPosition = false) {
			rootMotionForTriggeredAnimation = rootMotion;
			if (isTargetPosition)
				rootMotionForTriggeredAnimation -= transform.position;
			animator.SetTrigger(hash);
			UpdateAnimator(0f);
			rootMotionForTriggeredAnimation = Vector3.zero;
			if (didStartNewAnimation)
				InterpolateAnimation();
		}

		public void SetRootMotion (Vector3 rootMotion, bool isTargetPosition = false) {
			programmaticRootMotion = rootMotion;
			if (isTargetPosition)
				programmaticRootMotion -= transform.position;
			if (!undoAuthoredRootMotion)
				programmaticRootMotion -= Vector3.Scale(authoredRootMotion, transform.localScale);
		}

		protected virtual void OnAnimationEvent (AnimationEvent evt) {}

		private void AdvanceToNextFrame () {
			// Progress the animation all the way to the next frame + a little bit beyond it (the fudge amount)
			float deltaTime = UpdateLoop.timePerUpdate - (animationTime % UpdateLoop.timePerUpdate) + updateFudgeTime;
			if (deltaTime < 2 * updateFudgeTime)
				deltaTime = 2 * updateFudgeTime;
			timeInState += deltaTime;
			framesInState++;
			UpdateAnimator(deltaTime);
		}

		private void InterpolateAnimation () {
			// Figure out how far we need to advance to get to the right point between frames
			float targetTimeBetweenFrames = UpdateLoop.timePerUpdate * UpdateLoop.I.percentNextUpdateInterpolated;
			if (targetTimeBetweenFrames > UpdateLoop.timePerUpdate - updateFudgeTime)
				targetTimeBetweenFrames = UpdateLoop.timePerUpdate - updateFudgeTime;
			float currentTimeBetweenFrames = animationTime % UpdateLoop.timePerUpdate;
			float deltaTime = targetTimeBetweenFrames - currentTimeBetweenFrames;
			if (deltaTime >= UpdateLoop.timePerUpdate / 100f) {
				timeInState += deltaTime;
				UpdateAnimator(deltaTime);
			}
		}

		private void UpdateAnimator (float deltaTime) {
			Vector3 prevPosition = transform.position;
			didStartNewAnimation = false;
			// Have to do a silly trick to update the animator manually
			animator.speed = 1f;
			animator.Update(deltaTime);
			animator.speed = 0f;
			if (!didStartNewAnimation)
				RefreshAnimationState();
			// Calculate the authored root motion that's been traveled so far
			Vector3 changeInPosition = transform.position - prevPosition;
			Vector3 unscaledChangeInPosition = new Vector3(
				changeInPosition.x * (transform.localScale.x == 0f ? 0f : 1f / transform.localScale.x),
				changeInPosition.y * (transform.localScale.y == 0f ? 0f : 1f / transform.localScale.y),
				changeInPosition.z * (transform.localScale.z == 0f ? 0f : 1f / transform.localScale.z));
			authoredRootMotionTraveledSoFar += unscaledChangeInPosition;
			// Use that to calculate programmatic root motion
			programmaticRootMotionProgress = new Vector3(
				CalculateRootMotionComponent(xProgrammaticRootMotion, authoredRootMotionTraveledSoFar.x, authoredRootMotion.x, rootMotionProgress.x),
				CalculateRootMotionComponent(yProgrammaticRootMotion, authoredRootMotionTraveledSoFar.y, authoredRootMotion.y, rootMotionProgress.y),
				CalculateRootMotionComponent(zProgrammaticRootMotion, authoredRootMotionTraveledSoFar.z, authoredRootMotion.z, rootMotionProgress.z));
			Vector3 prevProgrammaticRootMotionTraveledSoFar = programmaticRootMotionTraveledSoFar;
			programmaticRootMotionTraveledSoFar = Vector3.Scale(programmaticRootMotion, programmaticRootMotionProgress);
			// Apply root motion
			if (undoAuthoredRootMotion)
				transform.position = prevPosition + programmaticRootMotionTraveledSoFar - prevProgrammaticRootMotionTraveledSoFar;
			else
				transform.position += programmaticRootMotionTraveledSoFar - prevProgrammaticRootMotionTraveledSoFar;
		}

		private float CalculateRootMotionComponent (ProgrammaticRootMotionType type, float authoredRootMotionTraveledSoFar, float authoredRootMotion, float rootMotionProgress) {
			switch (type) {
				case ProgrammaticRootMotionType.UseAuthoredRootMotion:
					return authoredRootMotion == 0f ? 0f : authoredRootMotionTraveledSoFar / authoredRootMotion;
				case ProgrammaticRootMotionType.UseRootMotionProgress:
					return rootMotionProgress;
				default:
					return 0f;
			}
		}

		private bool RefreshAnimationState () => RefreshAnimationState(animator.GetCurrentAnimatorStateInfo(0));

		private bool RefreshAnimationState (AnimatorStateInfo stateInfo) {
			if (!float.IsInfinity(stateInfo.length)) {
				isAnimationLooping = stateInfo.loop;
				hasAnimationLooped = isAnimationLooping && stateInfo.normalizedTime >= 1f;
				animationDuration = stateInfo.length;
				percentAnimationCompleted = (isAnimationLooping ? stateInfo.normalizedTime % 1f : stateInfo.normalizedTime);
				animationTime = percentAnimationCompleted * animationDuration;
				animationFrame = Mathf.FloorToInt(animationTime / UpdateLoop.timePerUpdate);
				animationFrameDuration = Mathf.FloorToInt(animationDuration / UpdateLoop.timePerUpdate + 0.01f);
				percentInterpolated = (animationTime % UpdateLoop.timePerUpdate) / UpdateLoop.timePerUpdate;
				return true;
			}
			else
				return false;
		}

		private void ANIMATION_TriggerAnimationEvent (AnimationEvent evt) {
			triggeredEvents.Add(evt);
		}
	}

	public abstract class EntityAnimator<T, U> : EntityAnimator<U> where T : Entity {
		private T _typedEntity;
		protected new T entity {
			get {
				if (_typedEntity == null)
					_typedEntity = GetComponentInParent<T>();
				return _typedEntity;
			}
		}
	}
}