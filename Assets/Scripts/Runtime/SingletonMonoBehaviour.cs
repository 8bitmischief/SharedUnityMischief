using System;
using UnityEngine;

namespace SharedUnityMischief
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		protected static T instance;

		public static T I
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<T>();
					if (instance == null)
					{
						throw new Exception($"Could not find {typeof(T)} singleton in the scene hierarchy");
					}
				}
				return instance;
			}
		}
		public static bool hasInstance => instance != null;

		[Header("Singleton Config")]
		[SerializeField] private bool indestructible = false;

		protected virtual void Awake()
		{
			ClaimSingletonInstanceOrDestroySelf(indestructible);
		}

		protected virtual void OnDestroy()
		{
			ReleaseSingletonInstance();
		}

		protected virtual bool ClaimSingletonInstance()
		{
			if (instance != null && instance != this)
			{
				return false;
			}
			else
			{
				instance = this as T;
				return true;
			}
		}

		protected virtual bool ClaimSingletonInstanceOrDestroySelf(bool makeIndestructible = false)
		{
			if (!ClaimSingletonInstance())
			{
				Destroy(this);
				return false;
			}
			else
			{
				if (makeIndestructible)
				{
					DontDestroyOnLoad(this);
				}
				return true;
			}
		}

		protected virtual bool ReleaseSingletonInstance()
		{
			if (instance == this)
			{
				instance = null;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}