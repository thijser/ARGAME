// Decompiled with JetBrains decompiler
// Type: Meta.Gesture
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class Gesture
  {
    protected bool _isValid;
    protected MetaGesture _type;
    protected Vector3 _position;

    public bool isValid
    {
      get
      {
        return this._isValid;
      }
    }

    public MetaGesture type
    {
      get
      {
        return this._type;
      }
    }

    public Vector3 position
    {
      get
      {
        return this._position;
      }
    }

    public Gesture()
    {
      this._type = MetaGesture.NONE;
      this._position = Vector3.get_zero();
      this._isValid = false;
    }

    internal Gesture(CppGestureData cppGesture)
    {
      this._isValid = cppGesture.valid;
      this._type = cppGesture.manipulationGesture;
      if (cppGesture.gesturePoint != null)
        this._position = MetaUtils.FloatToVector3(cppGesture.gesturePoint);
      else
        this._position = Vector3.get_zero();
    }

    internal void CopyTo(ref Gesture gesture)
    {
      gesture = (Gesture) this.MemberwiseClone();
    }

    internal void LocalToWorldCoordinate(Transform parentTransform)
    {
      // ISSUE: explicit reference operation
      ((Vector3) @this._position).Set((float) -this._position.x, (float) this._position.y, (float) this._position.z);
      this._position = parentTransform.TransformPoint(this._position);
    }

    internal void Update(CppGestureData cppGesture)
    {
      this._type = cppGesture.manipulationGesture;
      this._position = MetaUtils.FloatToVector3(cppGesture.gesturePoint);
      this._isValid = cppGesture.valid;
    }
  }
}
