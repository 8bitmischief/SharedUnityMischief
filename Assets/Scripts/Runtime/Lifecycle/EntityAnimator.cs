using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	[RequireComponent(typeof(Animator))]
	public abstract class EntityAnimator : EntityComponent {
		// In order to properly trigger events, we want to overshoot each frame a tiny bit
		// This variable controls how much each frame gets overshot and undershot
		private static readonly float UPDATE_FUDGE_TIME = UpdateLoop.timePerUpdate / 100f;

		[Header("Root Motion")]
		[SerializeField] protected Vector3 rootMotionProgress = Vector3.zero;

		public abstract string stateName { get; }
		public abstract float timeInState { get; protected set; }
		public abstract int framesInState { get; protected set; }
		public float animationTime { get; private set; } = 0f;
		public float animationDuration { get; private set; } = 0f;
		public float percentAnimationCompleted { get; private set; } = 0f;
		public bool isAnimationLooping { get; private set; } = false;
		public bool hasAnimationLooped { get; private set; } = false;
		public int animationFrame { get; private set; } = 0;
		public int animationFrameDuration { get; private set; } = 0;
		public float percentInterpolated { get; private set; } = 0f;
		public Vector3 rootMotion { get; set; } = Vector3.zero;

		protected Animator animator;
		protected Vector3 rootMotionAppliedSoFar = Vector3.zero;
		private List<AnimationEvent> triggeredEvents = new List<AnimationEvent>();

		protected virtual void Awake () {
			animator = GetComponent<Animator>();
			animator.speed = 0f;
		}

		public override void UpdateState () {
			if (UpdateLoop.I.isInterpolating) {
				if (InterpolateAnimation())
					RefreshAnimationState();
			}
			else {
				AdvanceToNextFrame();
				RefreshAnimationState();
				// Handle animation events that were triggered by moving into the next frame
				foreach (AnimationEvent evt in triggeredEvents)
					OnAnimationEvent(evt);
				triggeredEvents.Clear();
			}
		}

		protected virtual bool RefreshAnimationState (bool applyRootMotion = true) => RefreshAnimationState(animator.GetCurrentAnimatorStateInfo(0), applyRootMotion);

		protected virtual bool RefreshAnimationState (AnimatorStateInfo stateInfo, bool applyRootMotion = true) {
			if (!float.IsInfinity(stateInfo.length)) {
				float prevAnimtionTime = animationTime;
				isAnimationLooping = stateInfo.loop;
				hasAnimationLooped = isAnimationLooping && stateInfo.normalizedTime >= 1f;
				animationDuration = stateInfo.length;
				percentAnimationCompleted = (isAnimationLooping ? stateInfo.normalizedTime % 1f : stateInfo.normalizedTime);
				animationTime = percentAnimationCompleted * animationDuration;
				animationFrame = Mathf.FloorToInt(animationTime / UpdateLoop.timePerUpdate);
				animationFrameDuration = Mathf.FloorToInt(animationDuration / UpdateLoop.timePerUpdate + 0.01f);
				percentInterpolated = (animationTime % UpdateLoop.timePerUpdate) / UpdateLoop.timePerUpdate;
				return prevAnimtionTime != animationTime;
			}
			else
				return false;
		}

		protected virtual void CheckForStateChanges () => CheckForStateChanges(Vector3.zero);

		protected virtual void CheckForStateChanges (Vector3 rootMotion) {
			UpdateAnimator(0f);
			if (RefreshAnimationState(false)) {
				this.rootMotion = rootMotion;
				rootMotionAppliedSoFar = Vector3.zero;
				if (InterpolateAnimation(true))
					RefreshAnimationState();
			}
		}

		protected void Trigger (int hash, bool checkForStateChanges = true) {
			animator.SetTrigger(hash);
			if (checkForStateChanges)
				CheckForStateChanges();
		}

		protected void Trigger (int hash, Vector3 rootMotion) {
			animator.SetTrigger(hash);
			CheckForStateChanges(rootMotion);
		}

		protected virtual void OnAnimationEvent (AnimationEvent evt) {}

		private void TriggerAnimationEvent (AnimationEvent evt) {
			triggeredEvents.Add(evt);
		}

		private void AdvanceToNextFrame () {
			// Progress the animation all the way to the next frame + a little bit beyond it (the fudge amount)
			float deltaTime = UpdateLoop.timePerUpdate - (animationTime % UpdateLoop.timePerUpdate) + UPDATE_FUDGE_TIME;
			if (deltaTime < 2 * UPDATE_FUDGE_TIME)
				deltaTime = 2 * UPDATE_FUDGE_TIME;
			UpdateAnimator(deltaTime);
		}

		private bool InterpolateAnimation (bool applyToTimeInState = false) {
			// Figure out how far we need to advance to get to the right point between frames
			float targetTimeBetweenFrames = UpdateLoop.timePerUpdate * UpdateLoop.I.percentNextUpdateInterpolated;
			if (targetTimeBetweenFrames > UpdateLoop.timePerUpdate - UPDATE_FUDGE_TIME)
				targetTimeBetweenFrames = UpdateLoop.timePerUpdate - UPDATE_FUDGE_TIME;
			float currentTimeBetweenFrames = animationTime % UpdateLoop.timePerUpdate;
			float deltaTime = targetTimeBetweenFrames - currentTimeBetweenFrames;
			if (deltaTime >= UpdateLoop.timePerUpdate / 100f) {
				UpdateAnimator(deltaTime);
				if (applyToTimeInState)
					timeInState += deltaTime;
				return true;
			}
			return false;
		}

		private void UpdateAnimator (float deltaTime) {
			animator.speed = 1f;
			animator.Update(deltaTime);
			animator.speed = 0f;
		}
	}

	public abstract class EntityAnimator<T> : EntityAnimator {
		public T state { get; private set; }
		public override string stateName => state.ToString();
		public override float timeInState { get; protected set; } = 0f;
		public override int framesInState { get; protected set; } = 0;

		public Action<T, T> onChangeState;

		private Dictionary<string, T> stateLookup = new Dictionary<string, T>();
		private bool hasSetInitialState = false;

		protected override void Awake () {
			base.Awake();
			foreach (T value in Enum.GetValues(typeof(T)))
				stateLookup.Add(value.ToString(), value);
		}

		public override void UpdateState () {
			timeInState += UpdateLoop.I.deltaTime;
			if (!UpdateLoop.I.isInterpolating)
				framesInState++;
			base.UpdateState();
		}

		protected override bool RefreshAnimationState (AnimatorStateInfo stateInfo, bool applyRootMotion = true) {
			bool hasAnimationUpdated = base.RefreshAnimationState(stateInfo);
			T prevState = state;
			bool hasChangedState = false;
			bool foundState = false;
			foreach (KeyValuePair<string, T> entry in stateLookup) {
				if (stateInfo.IsName(entry.Key) || stateInfo.IsTag(entry.Key)) {
					if (!state.Equals(entry.Value)) {
						state = entry.Value;
						timeInState = 0f;
						framesInState = 0;
						hasChangedState = true;
						rootMotion = Vector3.zero;
						rootMotionAppliedSoFar = Vector3.zero;
						onChangeState?.Invoke(state, prevState);
					}
					foundState = true;
					break;
				}
			}
			if (!foundState && !state.Equals(default(T))) {
				state = default(T);
				timeInState = 0f;
				framesInState = 0;
				hasChangedState = true;
				rootMotion = Vector3.zero;
				rootMotionAppliedSoFar = Vector3.zero;
				onChangeState?.Invoke(state, prevState);
			}
			if (!hasSetInitialState && !state.Equals(default(T))) {
				hasSetInitialState = true;
				timeInState = animationTime;
				framesInState = animationFrame;
			}
			if (applyRootMotion)
				ApplyRootMotion();
			return hasAnimationUpdated || hasChangedState;
		}

		private void ApplyRootMotion () {
			Vector3 targetRootMotion = Vector3.Scale(rootMotion, rootMotionProgress);
			transform.position += targetRootMotion - rootMotionAppliedSoFar;
			rootMotionAppliedSoFar = targetRootMotion;
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