// Decompiled with JetBrains decompiler
// Type: Meta.HandEntity
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class HandEntity
  {
    protected Vector3 _position;
    protected Quaternion _orientation;
    protected Vector3 _scale;
    protected bool _isValid;

    public Vector3 position
    {
      get
      {
        return this._position;
      }
      internal set
      {
        this._position = value;
      }
    }

    public Quaternion orientation
    {
      get
      {
        return this._orientation;
      }
      internal set
      {
        this._orientation = value;
      }
    }

    public Vector3 scale
    {
      get
      {
        return this._scale;
      }
      internal set
      {
        this._scale = value;
      }
    }

    public bool isValid
    {
      get
      {
        return this._isValid;
      }
      internal set
      {
        this._isValid = value;
        if (this._isValid)
          return;
        ((Vector3) @this._position).Set(0.0f, 0.0f, 0.0f);
        this._orientation = Quaternion.get_identity();
        ((Vector3) @this._scale).Set(1f, 1f, 1f);
      }
    }

    protected object Clone()
    {
      return this.MemberwiseClone();
    }

    internal void CopyTo(ref HandEntity HandEntity)
    {
      HandEntity = (HandEntity) this.Clone();
    }
  }
}
