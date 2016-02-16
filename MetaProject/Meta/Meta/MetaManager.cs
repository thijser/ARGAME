// Decompiled with JetBrains decompiler
// Type: Meta.MetaManager
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

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
      return this._interactables[(int) interacableType];
    }

    private void Awake()
    {
      for (int index = 0; index < 7; ++index)
        this._interactables[index] = new List<Transform>();
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
      MethodInfo methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic) ?? obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
      if (methodInfo != null)
      {
        ParameterInfo[] parameters1 = methodInfo.GetParameters();
        if (parameters == null)
          return methodInfo.Invoke(obj, (object[]) null);
        if (parameters != null && parameters.Length == parameters1.Length)
          return methodInfo.Invoke(obj, parameters);
        Debug.LogWarning((object) (methodName + " was unable to be invoked!"));
      }
      return (object) null;
    }

    public static object GetPrivateField(string fieldName, object obj)
    {
      return obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
    }

    public static void SetPrivateField(string fieldName, object newValue, object obj)
    {
      obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, newValue);
    }

    public static object GetPrivateProperty(string propertyName, object obj)
    {
      return (obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic) ?? obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)).GetValue(obj, (object[]) null);
    }

    public static void SetPrivateProperty(string propertyName, object newValue, object obj)
    {
      (obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic) ?? obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)).SetValue(obj, newValue, (object[]) null);
    }

    private static bool AreObjectsEqual(object obj1, object obj2)
    {
      return obj1 != null && obj2 != null && (obj1.GetType() == obj2.GetType() && obj1.GetHashCode() == obj2.GetHashCode());
    }

    public void AddRemoveInteractable(InteractableType interactable, Transform obj, bool add)
    {
      if (this._interactables[(int) interactable] == null)
        return;
      if (add && !this._interactables[(int) interactable].Contains(obj))
        this._interactables[(int) interactable].Add(obj);
      else if (!add && this._interactables[(int) interactable].Contains(obj))
        this._interactables[(int) interactable].Remove(obj);
      if (interactable == InteractableType.CanvasTarget)
      {
        this.ToggleCanvas();
      }
      else
      {
        if (interactable != InteractableType.MarkerTarget)
          return;
        this.ToggleMarkers();
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
      if (!Object.op_Inequality((Object) this.canvasObject, (Object) null))
        return;
      if (this.canvasTargets.Count > 0)
        this.canvasObject.SetActive(true);
      else
        this.canvasObject.SetActive(false);
    }

    private void ToggleMarkers()
    {
      if (!Object.op_Inequality((Object) this.markerObject, (Object) null))
        return;
      if (this.markerTargets.Count > 0)
        this.markerObject.SetActive(true);
      else
        this.markerObject.SetActive(false);
    }
  }
}
