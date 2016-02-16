// Decompiled with JetBrains decompiler
// Type: Meta.MetaWorld
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  [ExecuteInEditMode]
  public class MetaWorld : MonoBehaviour
  {
    public MetaWorld()
    {
      base.\u002Ector();
    }

    private void OnEnable()
    {
      MetaPlugin.Load();
      if (Object.FindObjectsOfType<MetaWorld>().Length <= 1)
        return;
      Object.DestroyImmediate((Object) ((Component) this).get_gameObject());
    }
  }
}
