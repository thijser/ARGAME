// Decompiled with JetBrains decompiler
// Type: Meta.MetaSingleton`1
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

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
        if (Object.op_Equality((Object) (object) MetaSingleton<T>.m_Instance, (Object) null))
        {
          MetaSingleton<T>.m_Instance = (T) Object.FindObjectOfType<T>();
          if (Object.op_Inequality((Object) (object) MetaSingleton<T>.m_Instance, (Object) null))
            MetaSingleton<T>.m_Instance.Init();
        }
        return MetaSingleton<T>.m_Instance;
      }
    }

    protected MetaSingleton()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (Object.op_Equality((Object) (object) MetaSingleton<T>.m_Instance, (Object) null))
      {
        MetaSingleton<T>.m_Instance = this as T;
        MetaSingleton<T>.m_Instance.Init();
      }
      else
      {
        if (!Object.op_Inequality((Object) (object) MetaSingleton<T>.m_Instance, (Object) this))
          return;
        Debug.LogError((object) "A singleton already exists! Destroying new one.");
        Object.Destroy((Object) this);
      }
    }

    public virtual void Init()
    {
    }

    private void OnApplicationQuit()
    {
      MetaSingleton<T>.m_Instance = (T) null;
    }

    private void OnEnable()
    {
    }
  }
}
