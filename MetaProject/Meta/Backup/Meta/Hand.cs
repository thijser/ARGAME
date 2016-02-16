// Decompiled with JetBrains decompiler
// Type: Meta.Hand
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
  public class Hand
  {
    private List<Finger> _fingers = new List<Finger>();
    private CppGestureData[] cachedGestures = new CppGestureData[10];
    private MetaGesture prevGesture = MetaGesture.NONE;
    private bool _correctedCoordinateSystem;
    private bool _isValid;
    private Palm _palm;
    private Pointer _pointer;
    private Pointer _rightMostPoint;
    private Pointer _leftMostPoint;
    private float _angle;
    private Gesture _gesture;
    private float _handOpenness;

    public bool correctedCoordinateSystem
    {
      get
      {
        return this._correctedCoordinateSystem;
      }
      internal set
      {
        this._correctedCoordinateSystem = value;
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
      }
    }

    internal List<Finger> fingers
    {
      get
      {
        return this._fingers;
      }
      set
      {
        this._fingers = value;
      }
    }

    public Palm palm
    {
      get
      {
        return this._palm;
      }
      internal set
      {
        this._palm = value;
      }
    }

    public Pointer pointer
    {
      get
      {
        return this._pointer;
      }
      internal set
      {
        this._pointer = value;
      }
    }

    internal Pointer rightMostPoint
    {
      get
      {
        return this._rightMostPoint;
      }
      set
      {
        this._rightMostPoint = value;
      }
    }

    internal Pointer leftMostPoint
    {
      get
      {
        return this._leftMostPoint;
      }
      set
      {
        this._leftMostPoint = value;
      }
    }

    internal float angle
    {
      get
      {
        return this._angle;
      }
      set
      {
        this._angle = value;
      }
    }

    public Gesture gesture
    {
      get
      {
        return this._gesture;
      }
      internal set
      {
        this._gesture = value;
      }
    }

    public float handOpenness
    {
      get
      {
        return this._handOpenness;
      }
      internal set
      {
        this._handOpenness = value;
      }
    }

    public Hand()
    {
      for (int index = 0; index < 5; ++index)
        this._fingers.Add(new Finger());
      this._palm = new Palm();
      this._gesture = new Gesture();
      this._pointer = new Pointer();
      this._rightMostPoint = new Pointer();
      this._leftMostPoint = new Pointer();
    }

    internal void LocalToWorldCoordinate(Transform parentTransform)
    {
      if (this._correctedCoordinateSystem)
        return;
      this._palm.CorrectCoordinates(parentTransform);
      this._pointer.CorrectCoordinates(parentTransform);
      this._rightMostPoint.CorrectCoordinates(parentTransform);
      this._leftMostPoint.CorrectCoordinates(parentTransform);
      this._gesture.LocalToWorldCoordinate(parentTransform);
      for (int index = 0; index < 5; ++index)
        this._fingers[index].CorrectCoordinates(parentTransform);
      this._correctedCoordinateSystem = true;
    }

    private CppGestureData SmoothGesture(CppGestureData newGesture)
    {
      for (int index = 0; index < this.cachedGestures.Length - 1; ++index)
        this.cachedGestures[index] = this.cachedGestures[index + 1];
      this.cachedGestures[this.cachedGestures.Length - 1] = newGesture;
      Dictionary<MetaGesture, int> dictionary1 = new Dictionary<MetaGesture, int>();
      for (int index1 = 0; index1 < this.cachedGestures.Length; ++index1)
      {
        if (dictionary1.ContainsKey(this.cachedGestures[index1].manipulationGesture))
        {
          Dictionary<MetaGesture, int> dictionary2;
          MetaGesture index2;
          (dictionary2 = dictionary1)[index2 = this.cachedGestures[index1].manipulationGesture] = dictionary2[index2] + 1;
        }
        else
          dictionary1.Add(this.cachedGestures[index1].manipulationGesture, 1);
      }
      int num = 0;
      MetaGesture metaGesture = newGesture.manipulationGesture;
      using (Dictionary<MetaGesture, int>.Enumerator enumerator = dictionary1.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<MetaGesture, int> current = enumerator.Current;
          if (current.Value > num)
          {
            num = current.Value;
            metaGesture = current.Key;
          }
        }
      }
      if (num > 5)
      {
        newGesture.manipulationGesture = metaGesture;
        this.prevGesture = metaGesture;
      }
      else
        newGesture.manipulationGesture = this.prevGesture;
      return newGesture;
    }

    internal void Update(CppHandData cppHand)
    {
      this._correctedCoordinateSystem = false;
      this._isValid = cppHand.valid;
      this._angle = (float) cppHand.angle;
      this._handOpenness = cppHand.handOpenness;
      this._pointer.localPosition = MetaUtils.FloatToVector3(cppHand.top);
      this._pointer.isValid = cppHand.valid;
      this._rightMostPoint.localPosition = MetaUtils.FloatToVector3(cppHand.right);
      this._rightMostPoint.isValid = cppHand.valid;
      this._leftMostPoint.localPosition = MetaUtils.FloatToVector3(cppHand.left);
      this._leftMostPoint.isValid = cppHand.valid;
      this._palm.localPosition = MetaUtils.FloatToVector3(cppHand.center);
      this._palm.isValid = cppHand.valid;
      this._palm.radius = (float) cppHand.palm.radius;
      this._palm.normal = MetaUtils.FloatToVector3(cppHand.palm.normalVector);
      this._palm.orientations = MetaUtils.FloatToVector2(cppHand.palm.orientationAngles);
      this._palm.SetOrientation(this._angle);
      CppGestureData cppGesture = this.SmoothGesture(cppHand.gesture);
      if (this.gesture.type == cppGesture.manipulationGesture)
      {
        this._gesture.Update(cppGesture);
      }
      else
      {
        switch (cppGesture.manipulationGesture)
        {
          case MetaGesture.GRAB:
            this._gesture = (Gesture) new GrabGesture(cppGesture);
            break;
          case MetaGesture.PINCH:
            this._gesture = (Gesture) new PinchGesture(cppGesture);
            break;
          default:
            this._gesture = new Gesture(cppGesture);
            break;
        }
      }
      for (int index = 0; index < cppHand.fingers.Length; ++index)
      {
        this._fingers[index].localPosition = MetaUtils.FloatToVector3(cppHand.fingers[index].location);
        this._fingers[index].isValid = cppHand.fingers[index].found;
      }
    }
  }
}
