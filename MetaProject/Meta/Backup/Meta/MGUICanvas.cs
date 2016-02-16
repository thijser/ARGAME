// Decompiled with JetBrains decompiler
// Type: Meta.MGUICanvas
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class MGUICanvas : MonoBehaviour
  {
    public MGUICanvas()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      ((Canvas) ((Component) this).GetComponent<Canvas>()).set_worldCamera(Camera.get_main());
    }

    private void Update()
    {
    }
  }
}
