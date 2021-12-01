using System;
using System.Collections.Generic;
using UnityEngine;
using SharedUnityMischief.Lifecycle;

namespace SharedUnityMischief.Entities.Animated
{
	[RequireComponent(typeof(Animator))]
	public abstract class EntityAnimator : EntityComponent
	{
		// In order to properly trigger events, we want to overshoot each frame a tiny bit
		// This variable controls how much each frame gets overshot and undershot
		protected static readonly float UpdateFudgeTime = UpdateLoop.TimePerUpdate / 100f;
		protected static readonly int ResetHash = Animator.StringToHash("Reset");

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

	public abstract class EntityAnimator<TAnimation> : EntityAnimator
	{
		[SerializeField] protected Vector3 _rootMotionProgress = Vector3.zero;
		private TAnimation _animation;
		private Animator _animator;
		private List<AnimationEvent> _triggeredEvents = new List<AnimationEvent>();
		private bool _didStartNewAnimation = false;
		private bool _undoAuthoredRootMotion = false;
		private Vector3 _authoredRootMotionTraveledSoFar = Vector3.zero;
		private Vector3 _programmaticRootMotionTraveledSoFar = Vector3.zero;
		private Vector3 _rootMotionForTriggeredAnimation = Vector3.zero;
		private float _totalAnimationTime = 0f;
		private int _totalAnimationFrames = 0;
		private float _animationTime = 0f;
		private float _animationDuration = 0f;
		private float _percentAnimationCompleted = 0f;
		private bool _hasAnimationCompleted = false;
		private bool _isAnimationLooping = false;
		private bool _hasAnimationLooped = false;
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
		public new TAnimation animation => _animation;
		public override string animationName => _animation.ToString();
		public override float totalAnimationTime => _totalAnimationTime;
		public override int totalAnimationFrames => _totalAnimationFrames;
		public override float animationTime => _animationTime;
		public override float animationDuration => _animationDuration;
		public override float percentAnimationCompleted => _percentAnimationCompleted;
		public override bool hasAnimationCompleted => _hasAnimationCompleted;
		public override bool isAnimationLooping => _isAnimationLooping;
		public override bool hasAnimationLooped => _hasAnimationLooped;
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
			_animator.speed = 0f;
			UpdateAnimator(UpdateFudgeTime);
		}

		public override void ResetComponent()
		{
			foreach (AnimatorControllerParameter param in _animator.parameters)
			{
				if (param.name == "Reset")
				{
					Trigger(ResetHash);
				}
			}
			_animation = default(TAnimation);
			_totalAnimationTime = 0f;
			_totalAnimationFrames = 0;
			_animationTime = 0f;
			_animationDuration = 0f;
			_percentAnimationCompleted = 0f;
			_hasAnimationCompleted = false;
			_isAnimationLooping = false;
			_hasAnimationLooped = false;
			_animationFrame = 0;
			_animationFrameDuration = 0;
			_percentAnimationInterpolated = 0f;
			_animationSpeed = 1.00f;
			_authoredRootMotion = Vector3.zero;
			_programmaticRootMotion = Vector3.zero;
			_programmaticRootMotionProgress = Vector3.zero;
			_didStartNewAnimation = false;
			_undoAuthoredRootMotion = false;
			_authoredRootMotionTraveledSoFar = Vector3.zero;
			_programmaticRootMotionTraveledSoFar = Vector3.zero;
			_rootMotionForTriggeredAnimation = Vector3.zero;
			_xProgrammaticRootMotion = ProgrammaticRootMotionType.None;
			_yProgrammaticRootMotion = ProgrammaticRootMotionType.None;
			_zProgrammaticRootMotion = ProgrammaticRootMotionType.None;
			_triggeredEvents.Clear();
		}

		public override void UpdateState()
		{
			if (_animationSpeed != 1.00f)
			{
				AdvanceNonStandardSpeed();
				if (!UpdateLoop.I.isInterpolating)
				{
					TriggerEvents();
				}
			}
			else if (UpdateLoop.I.isInterpolating)
			{
				InterpolateAnimation();
			}
			else
			{
				AdvanceToNextFrame();
				TriggerEvents();
			}
		}

		public void TriggerAnimationStart(EntityAnimation<TAnimation> animation, AnimatorStateInfo stateInfo)
		{
			_didStartNewAnimation = true;
			TAnimation prevAnimation = _animation;
			// End the previous animation
			OnEndAnimation(_animation);
			onEndAnimation?.Invoke(_animation);
			// Start the new animation
			_animation = animation.animation;
			_totalAnimationTime = 0f;
			_totalAnimationFrames = 0;
			_undoAuthoredRootMotion = animation.undoAuthoredRootMotion;
			_authoredRootMotion = animation.authoredRootMotion;
			_authoredRootMotionTraveledSoFar = Vector3.zero;
			_programmaticRootMotion = _rootMotionForTriggeredAnimation;
			if (!_undoAuthoredRootMotion)
			{
				_programmaticRootMotion -= Vector3.Scale(_authoredRootMotion, transform.localScale);
			}
			_programmaticRootMotionTraveledSoFar = Vector3.zero;
			_programmaticRootMotionProgress = Vector3.zero;
			_xProgrammaticRootMotion = animation.xRootMotion;
			_yProgrammaticRootMotion = animation.yRootMotion;
			_zProgrammaticRootMotion = animation.zRootMotion;
			RefreshAnimationState(stateInfo);
			OnStartAnimation(_animation);
			onStartAnimation?.Invoke(_animation);
			// Trigger animation changes
			OnChangeAnimation(_animation, prevAnimation);
			onChangeAnimation?.Invoke(_animation, prevAnimation);
		}

		protected void Trigger(int hash) => Trigger(hash, Vector3.zero, false);

		protected void Trigger(int hash, Vector3 rootMotion, bool isTargetPosition = true)
		{
			_rootMotionForTriggeredAnimation = rootMotion;
			if (isTargetPosition)
			{
				_rootMotionForTriggeredAnimation -= transform.position;
			}
			_animator.SetTrigger(hash);
			UpdateAnimator(Mathf.Epsilon);
			_rootMotionForTriggeredAnimation = Vector3.zero;
			if (_didStartNewAnimation)
			{
				InterpolateAnimation();
			}
		}

		public void SetRootMotion(Vector3 rootMotion, bool isTargetPosition = true)
		{
			_programmaticRootMotion = rootMotion;
			if (isTargetPosition)
			{
				_programmaticRootMotion -= transform.position;
			}
			if (!_undoAuthoredRootMotion)
			{
				_programmaticRootMotion -= Vector3.Scale(_authoredRootMotion, transform.localScale);
			}
		}

		protected virtual void OnStartAnimation(TAnimation animation) {}
		protected virtual void OnEndAnimation(TAnimation animation) {}
		protected virtual void OnChangeAnimation(TAnimation animation, TAnimation prevAnimation) {}
		protected virtual void OnAnimationEvent(AnimationEvent evt) {}

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
				InterpolateAnimation();
			}
		}

		private void AdvanceNonStandardSpeed()
		{
			_totalAnimationTime += UpdateLoop.I.deltaTime;
			_totalAnimationFrames = Mathf.FloorToInt(_totalAnimationTime / UpdateLoop.TimePerUpdate);
			float deltaTime = UpdateLoop.I.deltaTime * _animationSpeed;
			UpdateAnimator(deltaTime);
		}

		private void InterpolateAnimation(int allowedRecursions = 3)
		{
			// Figure out how far we need to advance to get to the right point between frames
			float targetTimeBetweenFrames = UpdateLoop.TimePerUpdate * UpdateLoop.I.percentNextUpdateInterpolated;
			if (targetTimeBetweenFrames > UpdateLoop.TimePerUpdate - UpdateFudgeTime)
			{
				targetTimeBetweenFrames = UpdateLoop.TimePerUpdate - UpdateFudgeTime;
			}
			float currentTimeBetweenFrames = _animationTime % UpdateLoop.TimePerUpdate;
			float deltaTime = targetTimeBetweenFrames - currentTimeBetweenFrames;
			if (deltaTime >= UpdateLoop.TimePerUpdate / 100f)
			{
				_totalAnimationTime += deltaTime;
				UpdateAnimator(deltaTime);
				if (_didStartNewAnimation && allowedRecursions > 0)
				{
					InterpolateAnimation(allowedRecursions - 1);
				}
			}
		}

		private void TriggerEvents()
		{
			// Handle animation events that were triggered by moving into the next frame
			foreach (AnimationEvent evt in _triggeredEvents)
			{
				OnAnimationEvent(evt);
			}
			_triggeredEvents.Clear();
		}

		private void UpdateAnimator(float deltaTime)
		{
			Vector3 prevPosition = transform.position;
			_didStartNewAnimation = false;
			// Have to do a silly trick to update the animator manually
			_animator.speed = 1f;
			_animator.Update(deltaTime);
			_animator.speed = 0f;
			if (!_didStartNewAnimation)
			{
				RefreshAnimationState();
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
			Vector3 prevProgrammaticRootMotionTraveledSoFar = _programmaticRootMotionTraveledSoFar;
			_programmaticRootMotionTraveledSoFar = Vector3.Scale(_programmaticRootMotion, _programmaticRootMotionProgress);
			// Apply root motion
			if (_undoAuthoredRootMotion)
			{
				transform.position = prevPosition + _programmaticRootMotionTraveledSoFar - prevProgrammaticRootMotionTraveledSoFar;
			}
			else
			{
				transform.position += _programmaticRootMotionTraveledSoFar - prevProgrammaticRootMotionTraveledSoFar;
			}
		}

		private float CalculateRootMotionComponent(ProgrammaticRootMotionType type, float authoredRootMotionTraveledSoFar, float authoredRootMotion, float rootMotionProgress)
		{
			switch (type)
			{
				case ProgrammaticRootMotionType.UseAuthoredRootMotion:
					return authoredRootMotion == 0f ? 0f : authoredRootMotionTraveledSoFar / authoredRootMotion;
				case ProgrammaticRootMotionType.UseRootMotionProgress:
					return rootMotionProgress;
				default:
					return 0f;
			}
		}

		private bool RefreshAnimationState() => RefreshAnimationState(_animator.GetCurrentAnimatorStateInfo(0));

		private bool RefreshAnimationState(AnimatorStateInfo stateInfo)
		{
			if (!float.IsInfinity(stateInfo.length))
			{
				_isAnimationLooping = stateInfo.loop;
				_hasAnimationLooped = _isAnimationLooping && stateInfo.normalizedTime >= 1f;
				_animationDuration = stateInfo.length;
				_percentAnimationCompleted = _isAnimationLooping ? stateInfo.normalizedTime % 1f : Mathf.Min(stateInfo.normalizedTime, 1f);
				_animationTime = _percentAnimationCompleted * _animationDuration;
				_animationFrame = Mathf.FloorToInt(_animationTime / UpdateLoop.TimePerUpdate);
				_animationFrameDuration = Mathf.FloorToInt(_animationDuration / UpdateLoop.TimePerUpdate + 0.01f);
				if (!_isAnimationLooping)
				{
					_animationFrame = Mathf.Min(_animationFrame, _animationFrameDuration - 1);
				}
				_percentAnimationInterpolated = (_animationTime % UpdateLoop.TimePerUpdate) / UpdateLoop.TimePerUpdate;
				_hasAnimationCompleted = !_isAnimationLooping && _animationFrame == _animationFrameDuration - 1;
				return true;
			}
			else
			{
				return false;
			}
		}

		private void ANIMATION_TriggerAnimationEvent(AnimationEvent evt)
		{
			_triggeredEvents.Add(evt);
		}
	}

	public abstract class EntityAnimator<TEntity, TAnimation> : EntityAnimator<TAnimation> where TEntity : Entity
	{
		protected new TEntity entity
		{
			get
			{
				if (_typedEntity == null)
				{
					_typedEntity = GetComponentInParent<TEntity>();
				}
				return _typedEntity;
			}
		}
		private TEntity _typedEntity = null;
	}
}