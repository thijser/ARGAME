// Decompiled with JetBrains decompiler
// Type: Meta.MetaLocalizationHotKey
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class MetaLocalizationHotKey : MonoBehaviour
  {
    private MetaLocalization metaLocalization;

    public MetaLocalizationHotKey()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      if (!Object.op_Equality((Object) this.metaLocalization, (Object) null))
        return;
      this.metaLocalization = (MetaLocalization) ((Component) this).GetComponent<MetaLocalization>();
    }

    private void Update()
    {
      if (Object.op_Equality((Object) this.metaLocalization, (Object) null))
        this.metaLocalization = (MetaLocalization) ((Component) this).GetComponent<MetaLocalization>();
      if (!Object.op_Inequality((Object) this.metaLocalization, (Object) null) || !(MetaSingleton<KeyboardShortcuts>.Instance.recalibrate != string.Empty) || !Input.GetKeyDown(MetaSingleton<KeyboardShortcuts>.Instance.recalibrate))
        return;
      this.metaLocalization.ResetLocalizer();
    }
  }
}
