// Decompiled with JetBrains decompiler
// Type: Meta.HandObjects
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using UnityEngine;

namespace Meta
{
  [Serializable]
  internal class HandObjects
  {
    [SerializeField]
    private GameObject m_colliderPrefab;

    public void InitHandGO(ref Hand[] hands, Transform parentTransform)
    {
      GameObject gameObject = new GameObject();
      gameObject.get_transform().set_parent(parentTransform);
      ((Object) gameObject).set_name("HandObjects");
      gameObject.get_transform().set_localPosition(Vector3.get_zero());
      gameObject.get_transform().set_localRotation(Quaternion.get_identity());
      for (int index1 = 0; index1 < 2; ++index1)
      {
        string name1 = (string) (object) (HandType) index1 + (object) "HandtopLocation";
        hands[index1].pointer.InstantiateObject(this.m_colliderPrefab, name1, gameObject.get_transform());
        string name2 = (string) (object) (HandType) index1 + (object) "HandleftLocation";
        hands[index1].leftMostPoint.InstantiateObject(this.m_colliderPrefab, name2, gameObject.get_transform());
        string name3 = (string) (object) (HandType) index1 + (object) "HandrightLocation";
        hands[index1].rightMostPoint.InstantiateObject(this.m_colliderPrefab, name3, gameObject.get_transform());
        string name4 = (string) (object) (HandType) index1 + (object) "HandpalmLocation";
        hands[index1].palm.InstantiateObject(this.m_colliderPrefab, name4, gameObject.get_transform());
        for (int index2 = 0; index2 < 5; ++index2)
        {
          string name5 = string.Concat(new object[4]
          {
            (object) (HandType) index1,
            (object) "Hand",
            (object) (FingerTypes) index2,
            (object) "FingerLocation"
          });
          hands[index1].fingers[index2].InstantiateObject(this.m_colliderPrefab, name5, gameObject.get_transform());
        }
      }
    }

    public void UpdateHandGO(ref Hand[] hands)
    {
      Vector3 _scale;
      // ISSUE: explicit reference operation
      ((Vector3) @_scale).\u002Ector(1f, 1f, 1f);
      for (int index1 = 0; index1 < 2; ++index1)
      {
        hands[index1].pointer.SetTransform(hands[index1].pointer.position, Quaternion.get_identity(), _scale);
        hands[index1].leftMostPoint.SetTransform(hands[index1].leftMostPoint.position, Quaternion.get_identity(), _scale);
        hands[index1].rightMostPoint.SetTransform(hands[index1].rightMostPoint.position, Quaternion.get_identity(), _scale);
        hands[index1].palm.SetTransform(hands[index1].palm.position, hands[index1].palm.localOrientation, _scale);
        for (int index2 = 0; index2 < 5; ++index2)
          hands[index1].fingers[index2].SetTransform(hands[index1].fingers[index2].position, Quaternion.get_identity(), _scale);
        LayerMask layers = LayerMask.op_Implicit(-1);
        hands[index1].pointer.MultiRayCast(layers);
        if (hands[index1].gesture.type == MetaGesture.OPEN)
          hands[index1].palm.MultiRayCast(layers);
      }
    }

    public void DestroyHandGO()
    {
    }
  }
}
