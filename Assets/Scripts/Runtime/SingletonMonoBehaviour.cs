using System;
using UnityEngine;

namespace SharedUnityMischief
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		protected static T _instance;

		public static T I
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T>();
					if (_instance == null)
					{
						throw new Exception($"Could not find {typeof(T)} singleton in the scene hierarchy");
					}
				}
				return _instance;
			}
		}
		public static bool hasInstance => _instance != null;

		[Header("Singleton Config")]
		[SerializeField] private bool _indestructible = false;

		protected virtual void Awake()
		{
			ClaimSingletonInstanceOrDestroySelf(_indestructible);
		}

		protected virtual void OnDestroy()
		{
			ReleaseSingletonInstance();
		}

		protected virtual bool ClaimSingletonInstance()
		{
			if (_instance != null && _instance != this)
			{
				return false;
			}
			else
			{
				_instance = this as T;
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
			if (_instance == this)
			{
				_instance = null;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}