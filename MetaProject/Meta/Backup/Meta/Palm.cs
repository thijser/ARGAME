// Decompiled with JetBrains decompiler
// Type: Meta.Palm
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class Palm : HandGameEntity
  {
    private float _radius;
    private Vector3 _normal;
    private Vector2 _orientations;

    public float radius
    {
      get
      {
        return this._radius;
      }
      internal set
      {
        this._radius = value;
      }
    }

    internal Vector3 normal
    {
      get
      {
        return this._normal;
      }
      set
      {
        this._normal = value;
      }
    }

    internal Vector2 orientations
    {
      get
      {
        return this._orientations;
      }
      set
      {
        this._orientations = value;
      }
    }

    internal void SetOrientation(float armAngle)
    {
      this._orientation = Quaternion.Euler(new Vector3((float) (-1.0 * this._orientations.x), (float) (-1.0 * (90.0 + this._orientations.y)), armAngle - 90f));
    }

    internal void CopyTo(ref Palm palm)
    {
      palm = (Palm) this.Clone();
    }
  }
}
