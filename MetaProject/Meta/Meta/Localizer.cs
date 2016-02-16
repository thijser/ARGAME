// Decompiled with JetBrains decompiler
// Type: Meta.Localizer
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public abstract class Localizer : MonoBehaviour
  {
    protected GameObject _targetGO;

    public GameObject targetGO
    {
      get
      {
        return this._targetGO;
      }
      set
      {
        this._targetGO = value;
      }
    }

    protected Localizer()
    {
      base.\u002Ector();
    }

    private void OnEnable()
    {
      if (!Object.op_Equality((Object) this._targetGO, (Object) null))
        return;
      this.SetDefaultTargetGO();
    }

    protected void SetDefaultTargetGO()
    {
      if (!Object.op_Inequality((Object) MetaCore.Instance, (Object) null))
        return;
      this._targetGO = MetaCore.Instance.getMetaFrame();
    }

    public virtual void ResetLocalizer()
    {
    }

    public Quaternion GetRotation()
    {
      return this._targetGO.get_transform().get_rotation();
    }

    public Vector3 GetPosition()
    {
      return this._targetGO.get_transform().get_position();
    }
  }
}
