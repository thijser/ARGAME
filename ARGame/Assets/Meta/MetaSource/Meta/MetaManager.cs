using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Meta
{
	internal class MetaManager : MetaSingleton<MetaManager>
	{
		private List<Transform>[] _interactables = new List<Transform>[7];

		[SerializeField]
		private GameObject canvasObject;

		[SerializeField]
		private GameObject markerObject;

		public List<Transform> grabbables
		{
			get
			{
				return this._interactables[0];
			}
		}

		public List<Transform> pinchables
		{
			get
			{
				return this._interactables[1];
			}
		}

		public List<Transform> touchables
		{
			get
			{
				return this._interactables[2];
			}
		}

		public List<Transform> pointables
		{
			get
			{
				return this._interactables[3];
			}
		}

		public List<Transform> gazeables
		{
			get
			{
				return this._interactables[4];
			}
		}

		public List<Transform> markerTargets
		{
			get
			{
				return this._interactables[5];
			}
		}

		public List<Transform> canvasTargets
		{
			get
			{
				return this._interactables[6];
			}
		}

		public List<Transform> GetInteractables(InteractableType interacableType)
		{
			return this._interactables[(int)interacableType];
		}

		private void Awake()
		{
			for (int i = 0; i < 7; i++)
			{
				this._interactables[i] = new List<Transform>();
			}
		}

		private void Start()
		{
			this.ToggleCanvas();
			this.ToggleMarkers();
		}

		private void Update()
		{
		}

		private void OnLevelWasLoaded(int level)
		{
		}

		public static object InvokeMethod(string methodName, object[] parameters, object obj)
		{
			MethodInfo method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
			{
				method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			}
			if (method != null)
			{
				ParameterInfo[] parameters2 = method.GetParameters();
				if (parameters == null)
				{
					return method.Invoke(obj, null);
				}
				if (parameters != null && parameters.Length == parameters2.Length)
				{
					return method.Invoke(obj, parameters);
				}
				Debug.LogWarning(methodName + " was unable to be invoked!");
			}
			return null;
		}

		public static object GetPrivateField(string fieldName, object obj)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			return field.GetValue(obj);
		}

		public static void SetPrivateField(string fieldName, object newValue, object obj)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			field.SetValue(obj, newValue);
		}

		public static object GetPrivateProperty(string propertyName, object obj)
		{
			PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (property == null)
			{
				property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			}
			return property.GetValue(obj, null);
		}

		public static void SetPrivateProperty(string propertyName, object newValue, object obj)
		{
			PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (property == null)
			{
				property = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			}
			property.SetValue(obj, newValue, null);
		}

		private static bool AreObjectsEqual(object obj1, object obj2)
		{
			return obj1 != null && obj2 != null && obj1.GetType() == obj2.GetType() && obj1.GetHashCode() == obj2.GetHashCode();
		}

		public void AddRemoveInteractable(InteractableType interactable, Transform obj, bool add)
		{
			if (this._interactables[(int)interactable] != null)
			{
				if (add && !this._interactables[(int)interactable].Contains(obj))
				{
					this._interactables[(int)interactable].Add(obj);
				}
				else if (!add && this._interactables[(int)interactable].Contains(obj))
				{
					this._interactables[(int)interactable].Remove(obj);
				}
				if (interactable == InteractableType.CanvasTarget)
				{
					this.ToggleCanvas();
				}
				else if (interactable == InteractableType.MarkerTarget)
				{
					this.ToggleMarkers();
				}
			}
		}

		public void AddInteractable(InteractableType interactable, Transform obj)
		{
			this.AddRemoveInteractable(interactable, obj, true);
		}

		public void RemoveInteractable(InteractableType interactable, Transform obj)
		{
			this.AddRemoveInteractable(interactable, obj, false);
		}

		private void ToggleCanvas()
		{
			if (this.canvasObject != null)
			{
				if (this.canvasTargets.Count > 0)
				{
					this.canvasObject.SetActive(true);
				}
				else
				{
					this.canvasObject.SetActive(false);
				}
			}
		}

		private void ToggleMarkers()
		{
			if (this.markerObject != null)
			{
				if (this.markerTargets.Count > 0)
				{
					this.markerObject.SetActive(true);
				}
				else
				{
					this.markerObject.SetActive(false);
				}
			}
		}
	}
}
