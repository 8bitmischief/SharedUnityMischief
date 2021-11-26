using UnityEngine;

namespace SharedUnityMischief.Entities.Animated
{
	public abstract class AnimatedEntity<T, U> : Entity where U : EntityAnimator<T>
	{
		protected U _animator;

		public T state => _animator != null ? _animator.state : default(T);
		public float timeInState => _animator?.timeInState ?? 0f;
		public int framesInState => _animator?.framesInState ?? 0;

		protected override void Awake()
		{
			base.Awake();
			_animator = GetComponent<U>();
		}

		protected virtual void OnEnable()
		{
			_animator.onEnterState += OnEnterState;
			_animator.onLeaveState += OnLeaveState;
			_animator.onChangeState += OnChangeState;
		}

		protected virtual void OnDisable()
		{
			_animator.onEnterState -= OnEnterState;
			_animator.onLeaveState -= OnLeaveState;
			_animator.onChangeState -= OnChangeState;
		}

		protected virtual void OnEnterState(T state) {}
		protected virtual void OnLeaveState(T state) {}
		protected virtual void OnChangeState(T state, T prevState) {}
	}
}