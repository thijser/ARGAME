// Decompiled with JetBrains decompiler
// Type: Meta.HandGameEntity
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class HandGameEntity : HandEntity
  {
    private float _rayCastSpread = 5f;
    private float _rayCastMinConfidence = 15f;
    private GameObject _objectOfInterest;
    private GameObject _unityGameObject;

    public GameObject objectOfInterest
    {
      get
      {
        return this._objectOfInterest;
      }
    }

    public GameObject gameObject
    {
      get
      {
        return this._unityGameObject;
      }
    }

    public new Vector3 position
    {
      get
      {
        return this._unityGameObject.get_transform().get_parent().TransformPoint(this._position);
      }
    }

    public Vector3 localPosition
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

    public new Quaternion orientation
    {
      get
      {
        return Quaternion.op_Multiply(this._orientation, Quaternion.Inverse(this._unityGameObject.get_transform().get_parent().get_rotation()));
      }
    }

    public Quaternion localOrientation
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

    public float rayCastSpread
    {
      get
      {
        return this._rayCastSpread;
      }
      internal set
      {
        if ((double) value == (double) this._rayCastSpread)
          return;
        if ((double) value > 45.0)
          this._rayCastSpread = 45f;
        else if ((double) value < 5.0)
          this._rayCastSpread = 5f;
        else
          this._rayCastSpread = value;
      }
    }

    public float rayCastMinConfidence
    {
      get
      {
        return this._rayCastMinConfidence;
      }
      internal set
      {
        if ((double) value == (double) this._rayCastMinConfidence)
          return;
        if ((double) value > 90.0)
          this._rayCastMinConfidence = 90f;
        else if ((double) value < 15.0)
          this._rayCastMinConfidence = 15f;
        else
          this._rayCastMinConfidence = value;
      }
    }

    internal void InstantiateObject(GameObject prefab, string name, Transform parentTransform)
    {
      this._unityGameObject = Object.Instantiate((Object) prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.get_identity()) as GameObject;
      this._unityGameObject.get_transform().set_parent(parentTransform);
      this._unityGameObject.set_layer(31);
      ((Object) this._unityGameObject).set_name(name);
    }

    internal void SetTransform(Vector3 _position, Quaternion _orientation, Vector3 _scale)
    {
      this._unityGameObject.get_transform().set_position(_position);
      this._unityGameObject.get_transform().set_localRotation(_orientation);
      this._unityGameObject.get_transform().set_localScale(_scale);
      this._unityGameObject.SetActive(this._isValid);
    }

    internal void MultiRayCast(LayerMask layers)
    {
      Vector3 position = this._unityGameObject.get_transform().get_position();
      Vector3 direction = Vector3.op_Subtraction(position, ((Component) Camera.get_main()).get_transform().get_position());
      this._objectOfInterest = MultiRaycast.MostHit(MultiRaycast.MultiRayCast(position, direction, 5, 5, this._rayCastSpread, layers, false));
    }

    internal void CopyTo(ref HandGameEntity HandEntity)
    {
      HandEntity = (HandGameEntity) this.Clone();
    }

    internal void CorrectCoordinates(Transform parentTransform)
    {
      // ISSUE: explicit reference operation
      ((Vector3) @this._position).Set((float) -this._position.x, (float) this._position.y, (float) this._position.z);
    }
  }
}
