using UnityEngine;

namespace SharedUnityMischief.Lifecycle {
	public abstract class AnimatedEntity<T, U> : Entity where U : EntityAnimator<T> {
		public T state => animator.state;

		protected U animator;

		protected override void Awake () {
			base.Awake();
			animator = GetComponent<U>();
		}
	}
}