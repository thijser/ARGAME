using UnityEngine;

namespace Meta
{
	public abstract class MetaSingleton<T> : MonoBehaviour where T : MetaSingleton<T>
	{
		private static T m_Instance;

		public static T Instance
		{
			get
			{
				if (MetaSingleton<T>.m_Instance == null)
				{
					MetaSingleton<T>.m_Instance = UnityEngine.Object.FindObjectOfType<T>();
					if (MetaSingleton<T>.m_Instance != null)
					{
						MetaSingleton<T>.m_Instance.Init();
					}
				}
				return MetaSingleton<T>.m_Instance;
			}
		}

		private void Awake()
		{
			if (MetaSingleton<T>.m_Instance == null)
			{
				MetaSingleton<T>.m_Instance = (this as T);
				MetaSingleton<T>.m_Instance.Init();
			}
			else if (MetaSingleton<T>.m_Instance != this)
			{
				Debug.LogError("A singleton already exists! Destroying new one.");
				Object.Destroy(this);
			}
		}

		public virtual void Init()
		{
		}

		private void OnApplicationQuit()
		{
			MetaSingleton<T>.m_Instance = (T)((object)null);
		}

		private void OnEnable()
		{
		}
	}
}
