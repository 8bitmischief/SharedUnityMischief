using System;
using UnityEngine;
using SharedUnityMischief.Lifecycle;

namespace SharedUnityMischief.Entities.Animated
{
	[RequireComponent(typeof(Animator))]
	public abstract class EntityAnimator : EntityComponent
	{
		protected abstract Animator animator { get; }
		public abstract string animationName { get; }
		public abstract float totalAnimationTime { get; }
		public abstract int totalAnimationFrames { get; }
		public abstract float animationTime { get; }
		public abstract float animationDuration { get; }
		public abstract float percentAnimationCompleted { get; }
		public abstract bool hasAnimationCompleted { get; }
		public abstract bool isAnimationLooping { get; }
		public abstract bool hasAnimationLooped { get; }
		public abstract int numAnimationLoops { get; }
		public abstract int animationFrame { get; }
		public abstract int animationFrameDuration { get; }
		public abstract float percentAnimationInterpolated { get; }
		public abstract float animationSpeed { get; set; }
		public abstract Vector3 authoredRootMotion { get; }
		public abstract Vector3 programmaticRootMotion { get; }
		public abstract Vector3 programmaticRootMotionProgress { get; }
		public bool willFinishAnimationNextFrame => !isAnimationLooping && animationFrame == animationFrameDuration - 1;
		public bool willLoopAnimationNextFrame => isAnimationLooping && animationFrame == animationFrameDuration - 1;
		public override int componentUpdateOrder => EntityComponent.AnimatorUpdateOrder;

		public enum ProgrammaticRootMotionType
		{
			None = 0,
			UseRootMotionProgress = 1,
			UseAuthoredRootMotion = 2
		}
	}

	[RequireComponent(typeof(Animator))]
	public abstract class EntityAnimator<TAnimation> : EntityAnimator
	{
		// In order to properly trigger events, we want to overshoot each frame a tiny bit
		// This variable controls how much each frame gets overshot and undershot
		private static readonly float UpdateFudgeTime = UpdateLoop.TimePerUpdate / 100f;

		[SerializeField] protected Vector3 _rootMotionProgress = Vector3.zero;
		private TAnimation _animation;
		private Animator _animator;
		private bool _didStartNewAnimation = false;
		private bool _skipFirstFrame = false;
		private Vector3 _authoredRootMotionTraveledSoFar = Vector3.zero;
		private Vector3 _programmaticRootMotionTraveledSoFar = Vector3.zero;
		private Vector3 _rootMotionForTriggeredAnimation = Vector3.zero;
		private bool _shouldApplyRootMotionToTriggeredAnimation = false;
		private float _totalAnimationTime = 0f;
		private int _totalAnimationFrames = 0;
		private float _animationTime = 0f;
		private float _animationDuration = 0f;
		private float _percentAnimationCompleted = 0f;
		private bool _hasAnimationCompleted = false;
		private bool _isAnimationLooping = false;
		private int _numAnimationLoops = 0;
		private int _animationFrame = 0;
		private int _animationFrameDuration = 0;
		private float _percentAnimationInterpolated = 0f;
		private float _animationSpeed = 1.00f;
		private Vector3 _authoredRootMotion = Vector3.zero;
		private Vector3 _programmaticRootMotion = Vector3.zero;
		private Vector3 _programmaticRootMotionProgress = Vector3.zero;
		private ProgrammaticRootMotionType _xProgrammaticRootMotion = ProgrammaticRootMotionType.None;
		private ProgrammaticRootMotionType _yProgrammaticRootMotion = ProgrammaticRootMotionType.None;
		private ProgrammaticRootMotionType _zProgrammaticRootMotion = ProgrammaticRootMotionType.None;

		protected override Animator animator => _animator;
		#pragma warning disable CS0109 // Ignore "does not hide an accessible member" warnings
		public new TAnimation animation => _animation;
		#pragma warning restore CS0109
		public override string animationName => _animation?.ToString() ?? "null";
		public override float totalAnimationTime => _totalAnimationTime;
		public override int totalAnimationFrames => _totalAnimationFrames;
		public override float animationTime => _animationTime;
		public override float animationDuration => _animationDuration;
		public override float percentAnimationCompleted => _percentAnimationCompleted;
		public override bool hasAnimationCompleted => _hasAnimationCompleted;
		public override bool isAnimationLooping => _isAnimationLooping;
		public override bool hasAnimationLooped => numAnimationLoops > 0;
		public override int numAnimationLoops => _numAnimationLoops;
		public override int animationFrame => _animationFrame;
		public override int animationFrameDuration => _animationFrameDuration;
		public override float percentAnimationInterpolated => _percentAnimationInterpolated;
		public override float animationSpeed { get => _animationSpeed; set => _animationSpeed = value; }
		public override Vector3 authoredRootMotion => _authoredRootMotion;
		public override Vector3 programmaticRootMotion => _programmaticRootMotion;
		public override Vector3 programmaticRootMotionProgress => _programmaticRootMotionProgress;

		public event Action<TAnimation> onStartAnimation;
		public event Action<TAnimation> onEndAnimation;
		public event Action<TAnimation, TAnimation> onChangeAnimation;

		protected virtual void Awake()
		{
			_animator = GetComponent<Animator>();
			// Immediately pause the animator since we'll be hijacking how it updates
			_animator.speed = 0f;
			// Manually update the animator a teensy bit to get it on track
			UpdateAnimator(UpdateFudgeTime);
		}

		public override void ResetComponent()
		{
			// Reset the animator (if it provides a trigger for it)
			foreach (AnimatorControllerParameter param in _animator.parameters)
			{
				if (param.name == "Reset")
					Trigger("Reset");
			}
			_animation = default(TAnimation);
			_totalAnimationTime = 0f;
			_totalAnimationFrames = 0;
			_animationTime = 0f;
			_animationDuration = 0f;
			_percentAnimationCompleted = 0f;
			_hasAnimationCompleted = false;
			_isAnimationLooping = false;
			_numAnimationLoops = 0;
			_animationFrame = 0;
			_animationFrameDuration = 0;
			_percentAnimationInterpolated = 0f;
			_animationSpeed = 1.00f;
			_authoredRootMotion = Vector3.zero;
			_programmaticRootMotion = Vector3.zero;
			_programmaticRootMotionProgress = Vector3.zero;
			_didStartNewAnimation = false;
			_skipFirstFrame = false;
			_authoredRootMotionTraveledSoFar = Vector3.zero;
			_programmaticRootMotionTraveledSoFar = Vector3.zero;
			_rootMotionForTriggeredAnimation = Vector3.zero;
			_xProgrammaticRootMotion = ProgrammaticRootMotionType.None;
			_yProgrammaticRootMotion = ProgrammaticRootMotionType.None;
			_zProgrammaticRootMotion = ProgrammaticRootMotionType.None;
		}

		public override void UpdateState()
		{
			if (_animationSpeed != 1.00f)
				AdvanceNonStandardSpeed();
			else if (UpdateLoop.I.isInterpolating)
				InterpolateAnimation();
			else
				AdvanceToNextFrame();
		}

		public void OnAnimationStart(EntityAnimation<TAnimation> animation, AnimatorStateInfo stateInfo)
		{
			// OnAnimationStart is called when a new animation starts, likely as a result of a call to UpdateAnimator
			_didStartNewAnimation = true;
			_skipFirstFrame = animation.skipFirstFrame;
			TAnimation prevAnimation = _animation;
			// Trigger OnEndAnimation
			OnEndAnimation(_animation);
			onEndAnimation?.Invoke(_animation);
			// Set up everything for the new animation
			_animation = animation.animation;
			_authoredRootMotion = animation.authoredRootMotion;
			_xProgrammaticRootMotion = animation.xRootMotion;
			_yProgrammaticRootMotion = animation.yRootMotion;
			_zProgrammaticRootMotion = animation.zRootMotion;
			_totalAnimationTime = 0f;
			_totalAnimationFrames = 0;
			if (_shouldApplyRootMotionToTriggeredAnimation)
				_programmaticRootMotion = _rootMotionForTriggeredAnimation - Vector3.Scale(_authoredRootMotion, transform.localScale);
			else
				_programmaticRootMotion = Vector3.zero;
			_authoredRootMotionTraveledSoFar = Vector3.zero;
			_programmaticRootMotionTraveledSoFar = Vector3.zero;
			_programmaticRootMotionProgress = Vector3.zero;
			RefreshAnimationState(stateInfo);
			// Trigger OnStartAnimation
			OnStartAnimation(_animation);
			onStartAnimation?.Invoke(_animation);
			// Trigger OnChangeAnimation
			OnChangeAnimation(_animation, prevAnimation);
			onChangeAnimation?.Invoke(_animation, prevAnimation);
		}

		public void SetRootMotion(Vector3 rootMotion, bool isRelativeRootMotion = true)
		{
			_programmaticRootMotion = rootMotion - Vector3.Scale(_authoredRootMotion, transform.localScale);
			if (!isRelativeRootMotion)
				_programmaticRootMotion -= transform.position;
		}

		protected void Trigger(string trigger) => Trigger(Animator.StringToHash(trigger));
		protected void Trigger(int hash)
		{
			_shouldApplyRootMotionToTriggeredAnimation = false;
			SetTriggerAndCheckForTriggeredAnimation(hash);
		}

		protected void Trigger(string trigger, Vector3 rootMotion, bool isRelativeRootMotion = true) => Trigger(Animator.StringToHash(trigger), rootMotion, isRelativeRootMotion);
		protected void Trigger(int hash, Vector3 rootMotion, bool isRelativeRootMotion = true)
		{
			// Store the root motion that'll get applied to whatever animation begins as a result of the trigger
			_shouldApplyRootMotionToTriggeredAnimation = true;
			_rootMotionForTriggeredAnimation = rootMotion;
			if (!isRelativeRootMotion)
				_rootMotionForTriggeredAnimation -= transform.position;
			SetTriggerAndCheckForTriggeredAnimation(hash);
		}

		protected void SkipToNextFrame()
		{
			AdvanceToNextFrame();
		}

		protected virtual void OnStartAnimation(TAnimation animation) {}
		protected virtual void OnEndAnimation(TAnimation animation) {}
		protected virtual void OnChangeAnimation(TAnimation animation, TAnimation prevAnimation) {}

		private void SetTriggerAndCheckForTriggeredAnimation(int hash)
		{
			// Trigger the trigger
			_animator.SetTrigger(hash);
			// Advance the animator the teeniest bit in order to trigger StateMachineBehaviour.OnStateEnter
			UpdateAnimator(Mathf.Epsilon);
			_shouldApplyRootMotionToTriggeredAnimation = false;
			_rootMotionForTriggeredAnimation = Vector3.zero;
			// The call to UpdateAnimator resulted in a new animation starting!
			if (_didStartNewAnimation)
			{
				if (_skipFirstFrame)
					UpdateAnimator(UpdateLoop.TimePerUpdate);
				InterpolateAnimation();
			}
		}

		private void AdvanceToNextFrame()
		{
			// Progress the animation all the way to the next frame + a little bit beyond it (the fudge amount)
			float deltaTime = UpdateLoop.TimePerUpdate - (_animationTime % UpdateLoop.TimePerUpdate) + UpdateFudgeTime;
			deltaTime = Mathf.Max(deltaTime, 2 * UpdateFudgeTime);
			_totalAnimationTime += deltaTime;
			_totalAnimationFrames++;
			UpdateAnimator(deltaTime);
			if (_didStartNewAnimation)
			{
				if (_skipFirstFrame)
					UpdateAnimator(UpdateLoop.TimePerUpdate);
				InterpolateAnimation();
			}
		}

		private void AdvanceNonStandardSpeed()
		{
			// Total animation time + frames intentionally not scaled with animation speed
			_totalAnimationTime += UpdateLoop.I.deltaTime;
			_totalAnimationFrames = Mathf.FloorToInt(_totalAnimationTime / UpdateLoop.TimePerUpdate);
			float deltaTime = UpdateLoop.I.deltaTime * _animationSpeed;
			UpdateAnimator(UpdateLoop.I.deltaTime * _animationSpeed);
			if (_didStartNewAnimation)
			{
				if (_skipFirstFrame)
					UpdateAnimator(UpdateLoop.TimePerUpdate * _animationSpeed);
			}
		}

		private void InterpolateAnimation(int allowedRecursions = 3)
		{
			// Figure out how far we need to advance to get to the right point between frames
			float targetTimeBetweenFrames = Mathf.Min(UpdateLoop.TimePerUpdate * UpdateLoop.I.percentNextUpdateInterpolated, UpdateLoop.TimePerUpdate - UpdateFudgeTime);
			float currentTimeBetweenFrames = _animationTime % UpdateLoop.TimePerUpdate;
			float deltaTime = targetTimeBetweenFrames - currentTimeBetweenFrames;
			if (deltaTime >= UpdateLoop.TimePerUpdate / 100f)
			{
				_totalAnimationTime += deltaTime;
				// Update the animator
				UpdateAnimator(deltaTime);
				// Updating the animator resulted in a new animation being triggered!
				// This is unexpected since we're just interpolating, but technically possible
				if (_didStartNewAnimation)
				{
					Debug.LogWarning($"EntityAnimator.InterpolateAnimation resulted in {name} beginning a new animation: {_animation}! This is unexpected but not impossible");
					if (_skipFirstFrame)
						UpdateAnimator(UpdateLoop.TimePerUpdate);
					// Interpolate the new animation to the correct spot
					if (allowedRecursions > 0)
						InterpolateAnimation(allowedRecursions - 1);
				}
			}
		}

		private void UpdateAnimator(float deltaTime)
		{
			Vector3 prevPosition = transform.position;
			Vector3 prevAuthoredRootMotion = _authoredRootMotion;
			Vector3 prevAuthoredRootMotionTraveledSoFar = _authoredRootMotionTraveledSoFar;
			Vector3 prevProgrammaticRootMotion = _programmaticRootMotion;
			Vector3 prevProgrammaticRootMotionTraveledSoFar = _programmaticRootMotionTraveledSoFar;
			bool wasAnimationCompleted = _hasAnimationCompleted;
			_didStartNewAnimation = false;
			_skipFirstFrame = false;
			// Have to do a silly trick to update the animator manually
			_animator.speed = 1f;
			_animator.Update(deltaTime);
			_animator.speed = 0f;
			// When a new animation starts, we're given an AnimatorStateInfo that we can refresh the animation state with
			// So if no new animation started, that means we have to refresh the animation state manually
			if (!_didStartNewAnimation)
			{
				RefreshAnimationState();
			}
			else if (wasAnimationCompleted)
			{
				// We finished an animation so apply any remaining root motion
				Vector3 authoredRootMotionLeftUntraveled = prevAuthoredRootMotion - prevAuthoredRootMotionTraveledSoFar;
				Vector3 programmaticRootMotionLeftUntraveled = prevProgrammaticRootMotion - prevProgrammaticRootMotionTraveledSoFar;
				Vector3 rootMotionLeftUntraveled = programmaticRootMotionLeftUntraveled + new Vector3(
					authoredRootMotionLeftUntraveled.x * (transform.localScale.x == 0f ? 0f : 1f / transform.localScale.x),
					authoredRootMotionLeftUntraveled.y * (transform.localScale.y == 0f ? 0f : 1f / transform.localScale.y),
					authoredRootMotionLeftUntraveled.z * (transform.localScale.z == 0f ? 0f : 1f / transform.localScale.z));
				transform.position += rootMotionLeftUntraveled;
				prevPosition = transform.position;
			}
			// Calculate the authored root motion that's been traveled so far
			Vector3 changeInPosition = transform.position - prevPosition;
			Vector3 unscaledChangeInPosition = new Vector3(
				changeInPosition.x * (transform.localScale.x == 0f ? 0f : 1f / transform.localScale.x),
				changeInPosition.y * (transform.localScale.y == 0f ? 0f : 1f / transform.localScale.y),
				changeInPosition.z * (transform.localScale.z == 0f ? 0f : 1f / transform.localScale.z));
			_authoredRootMotionTraveledSoFar += unscaledChangeInPosition;
			// Use that to calculate programmatic root motion
			_programmaticRootMotionProgress = new Vector3(
				CalculateRootMotionComponent(_xProgrammaticRootMotion, _authoredRootMotionTraveledSoFar.x, _authoredRootMotion.x, _rootMotionProgress.x),
				CalculateRootMotionComponent(_yProgrammaticRootMotion, _authoredRootMotionTraveledSoFar.y, _authoredRootMotion.y, _rootMotionProgress.y),
				CalculateRootMotionComponent(_zProgrammaticRootMotion, _authoredRootMotionTraveledSoFar.z, _authoredRootMotion.z, _rootMotionProgress.z));
			Vector3 currProgrammaticRootMotionTraveledSoFar = _programmaticRootMotionTraveledSoFar;
			_programmaticRootMotionTraveledSoFar = Vector3.Scale(_programmaticRootMotion, _programmaticRootMotionProgress);
			// Apply programmatic root motion
			transform.position += _programmaticRootMotionTraveledSoFar - currProgrammaticRootMotionTraveledSoFar;
		}

		private float CalculateRootMotionComponent(ProgrammaticRootMotionType type, float authoredRootMotionTraveledSoFar, float authoredRootMotion, float rootMotionProgress)
		{
			float rootMotion;
			switch (type)
			{
				case ProgrammaticRootMotionType.UseAuthoredRootMotion:
					rootMotion = authoredRootMotion == 0f ? 0f : authoredRootMotionTraveledSoFar / authoredRootMotion;
					break;
				case ProgrammaticRootMotionType.UseRootMotionProgress:
					rootMotion = rootMotionProgress;
					break;
				default:
					rootMotion = 0f;
					break;
			}
			// We add _numAnimation loops to get the root motion to keep applying for looping animations
			return rootMotion + ((float) _numAnimationLoops);
		}

		private bool RefreshAnimationState() => RefreshAnimationState(_animator.GetCurrentAnimatorStateInfo(0));
		private bool RefreshAnimationState(AnimatorStateInfo stateInfo)
		{
			// Sometimes Unity returns an invalid state info, still trying to figure out when exactly that happens
			if (!float.IsInfinity(stateInfo.length))
			{
				_isAnimationLooping = stateInfo.loop;
				_numAnimationLoops = _isAnimationLooping ? Mathf.FloorToInt(stateInfo.normalizedTime) : 0;
				_animationDuration = stateInfo.length;
				_percentAnimationCompleted = _isAnimationLooping ? stateInfo.normalizedTime % 1f : Mathf.Min(stateInfo.normalizedTime, 1f);
				_animationTime = _percentAnimationCompleted * _animationDuration;
				_animationFrame = Mathf.FloorToInt(_animationTime / UpdateLoop.TimePerUpdate);
				_animationFrameDuration = Mathf.FloorToInt(_animationDuration / UpdateLoop.TimePerUpdate + 0.01f);
				if (!_isAnimationLooping)
					_animationFrame = Mathf.Min(_animationFrame, _animationFrameDuration - 1);
				_percentAnimationInterpolated = (_animationTime % UpdateLoop.TimePerUpdate) / UpdateLoop.TimePerUpdate;
				_hasAnimationCompleted = !_isAnimationLooping && _animationFrame == _animationFrameDuration - 1;
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	[RequireComponent(typeof(Animator))]
	public abstract class EntityAnimator<TEntity, TAnimation> : EntityAnimator<TAnimation> where TEntity : Entity
	{
		protected new TEntity entity
		{
			get
			{
				if (_typedEntity == null)
					_typedEntity = GetComponentInParent<TEntity>();
				return _typedEntity;
			}
		}
		private TEntity _typedEntity = null;
	}
}