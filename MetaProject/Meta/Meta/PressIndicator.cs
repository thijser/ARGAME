// Decompiled with JetBrains decompiler
// Type: Meta.PressIndicator
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class PressIndicator : MonoBehaviour
  {
    public PressIndicator()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      ((Component) this).get_gameObject().set_layer(LayerMask.NameToLayer("HUD"));
      ((Component) this).get_transform().set_localScale(new Vector3(1.0 / 1000.0, 1.0 / 1000.0, 1.0 / 1000.0));
      LeanTween.alpha(((Component) this).get_gameObject(), 0.0f, 0.75f);
      LeanTween.scale(((Component) this).get_gameObject(), new Vector3(3.0 / 1000.0, 3.0 / 1000.0, 3.0 / 1000.0), 0.5f);
      Object.Destroy((Object) ((Component) this).get_gameObject(), 1f);
    }

    private void Update()
    {
    }
  }
}
