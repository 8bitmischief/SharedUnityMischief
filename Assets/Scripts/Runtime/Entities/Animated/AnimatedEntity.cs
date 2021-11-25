using UnityEngine;

namespace SharedUnityMischief.Entities.Animated
{
	public abstract class AnimatedEntity<T, U> : Entity where U : EntityAnimator<T>
	{
		protected U animator;

		public T state => animator != null ? animator.state : default(T);
		public float timeInState => animator?.timeInState ?? 0f;
		public int framesInState => animator?.framesInState ?? 0;

		protected override void Awake()
		{
			base.Awake();
			animator = GetComponent<U>();
		}

		protected virtual void OnEnable()
		{
			animator.onEnterState += OnEnterState;
			animator.onLeaveState += OnLeaveState;
			animator.onChangeState += OnChangeState;
		}

		protected virtual void OnDisable()
		{
			animator.onEnterState -= OnEnterState;
			animator.onLeaveState -= OnLeaveState;
			animator.onChangeState -= OnChangeState;
		}

		protected virtual void OnEnterState(T state) {}
		protected virtual void OnLeaveState(T state) {}
		protected virtual void OnChangeState(T state, T prevState) {}
	}
}