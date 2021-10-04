using System;
using UnityEngine;

namespace SharedUnityMischief {
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;
		public static T I {
			get {
				if (instance == null)
					instance = UnityEngine.Object.FindObjectOfType<T>();
				if (instance == null)
					throw new Exception($"Could not find {typeof(T)} singleton in the scene hierarchy");
				return instance;
			}
		}

		protected bool ClaimSingletonInstance () {
			if (instance != null && instance != this)
				return false;
			else {
				instance = this as T;
				return true;
			}
		}

		protected bool ReleaseSingletonInstance () {
			if (instance == this) {
				instance = null;
				return false;
			}
			else
				return false;
		}
	}
}