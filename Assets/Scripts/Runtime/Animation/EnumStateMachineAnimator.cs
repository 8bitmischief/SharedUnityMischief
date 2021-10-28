using System;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief.Animation {
	[DefaultExecutionOrder(-1)]
	[RequireComponent(typeof(Animator))]
	public class EnumStateMachineAnimator<T> : EnumStateMachineAnimator {
		public T state { get; private set; }
		public override float timeInState { get; protected set; } = 0f;

		public override string stateName => state.ToString();

		public Action<T, T> onChangeState;

		protected Animator animator;

		private Dictionary<string, T> stateLookup = new Dictionary<string, T>();

		protected void Awake () {
			animator = GetComponent<Animator>();
			foreach (T value in Enum.GetValues(typeof(T)))
				stateLookup.Add(value.ToString(), value);
		}

		protected void Update () {
			timeInState += Time.deltaTime;
			RefreshState();
		}

		protected bool RefreshState () {
			T prevState = state;
			AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			foreach (KeyValuePair<string, T> entry in stateLookup) {
				if (stateInfo.IsName(entry.Key) || stateInfo.IsTag(entry.Key)) {
					if (state.Equals(entry.Value))
						return false;
					else {
						state = entry.Value;
						onChangeState?.Invoke(state, prevState);
						timeInState = 0f;
						return true;
					}
				}
			}
			if (state.Equals(default(T)))
				return false;
			else {
				state = default(T);
				onChangeState?.Invoke(state, prevState);
				timeInState = 0f;
				return true;
			}
		}

		protected bool Trigger (int trigger) {
			animator.SetTrigger(trigger);
			return RefreshState();
		}
	}

	public abstract class EnumStateMachineAnimator : MonoBehaviour {
		public abstract string stateName { get; }
		public abstract float timeInState { get; protected set; }
	}
}