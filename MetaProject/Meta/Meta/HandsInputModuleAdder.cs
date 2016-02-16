// Decompiled with JetBrains decompiler
// Type: Meta.HandsInputModuleAdder
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class HandsInputModuleAdder : MonoBehaviour
  {
    public HandsInputModuleAdder()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      if (Object.op_Equality((Object) ((Component) this).get_gameObject().GetComponent<HandsInputModule>(), (Object) null))
        ((Component) this).get_gameObject().AddComponent<HandsInputModule>();
      ((Object) this).set_hideFlags((HideFlags) 2);
    }
  }
}
