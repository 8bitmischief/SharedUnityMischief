using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class AnimatedEntity<T, U> : Entity where U : EntityAnimator<T> {
		public T state => animator.state;

		protected U animator;

		protected override void Awake () {
			base.Awake();
			animator = GetComponent<U>();
		}

		protected virtual void OnEnable () {
			animator.onEnterState += OnEnterState;
			animator.onLeaveState += OnLeaveState;
			animator.onChangeState += OnChangeState;
		}

		protected virtual void OnDisable () {
			animator.onEnterState -= OnEnterState;
			animator.onLeaveState -= OnLeaveState;
			animator.onChangeState -= OnChangeState;
		}

		protected virtual void OnEnterState (T state) {}
		protected virtual void OnLeaveState (T state) {}
		protected virtual void OnChangeState (T state, T prevState) {}
	}
}