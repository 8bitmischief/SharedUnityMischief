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

		[Header("Singleton Config")]
		[SerializeField] protected bool indestructible = true;

		protected virtual void Awake () {
			if (!ClaimSingletonInstanceOrDestroySelf(indestructible))
				return;
		}

		protected virtual void OnDestroy () {
			ReleaseSingletonInstance();
		}

		protected virtual bool ClaimSingletonInstance () {
			if (instance != null && instance != this)
				return false;
			else {
				instance = this as T;
				return true;
			}
		}

		protected virtual bool ClaimSingletonInstanceOrDestroySelf (bool makeIndestructible = false) {
			if (!ClaimSingletonInstance()) {
				Destroy(this);
				return false;
			}
			else {
				if (makeIndestructible)
					DontDestroyOnLoad(this);
				return true;
			}
		}

		protected virtual bool ReleaseSingletonInstance () {
			if (instance == this) {
				instance = null;
				return true;
			}
			else
				return false;
		}
	}
}