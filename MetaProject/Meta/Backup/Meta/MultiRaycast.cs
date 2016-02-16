// Decompiled with JetBrains decompiler
// Type: Meta.MultiRaycast
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
  public static class MultiRaycast
  {
    public static RaycastHit[] MultiRayCast(Vector3 origin, Vector3 direction, int rows, int raysPerRow, float theta, LayerMask layerMask, bool descend = false)
    {
      // ISSUE: explicit reference operation
      int num1 = ((LayerMask) @layerMask).get_value();
      if (num1 < 0)
        num1 = 0;
      int num2 = ~(num1 | int.MinValue | 4 | 65536);
      if (rows == 0 || raysPerRow == 0)
        return new RaycastHit[0];
      RaycastHit[] raycastHitArray = new RaycastHit[rows * raysPerRow];
      Vector3 up = Vector3.get_up();
      Vector3 vector3_1 = Vector3.Cross(direction, up);
      // ISSUE: explicit reference operation
      if ((double) ((Vector3) @vector3_1).get_magnitude() == 0.0)
      {
        Vector3 right = Vector3.get_right();
        vector3_1 = Vector3.Cross(direction, right);
      }
      Quaternion quaternion = Quaternion.AngleAxis(360f / (float) raysPerRow, direction);
      for (int index1 = 0; index1 < rows; ++index1)
      {
        Vector3 vector3_2 = Quaternion.op_Multiply(Quaternion.AngleAxis((1f + (float) index1) / (float) rows * theta, vector3_1), direction);
        for (int index2 = 0; index2 < raysPerRow; ++index2)
        {
          if (descend)
          {
            RaycastHit[] hits = Physics.RaycastAll(origin, vector3_2, float.PositiveInfinity, num2);
            if (hits.Length != 0)
              raycastHitArray[index1 * raysPerRow + index2] = MultiRaycast.CollidedDescendant(hits);
          }
          else
            Physics.Raycast(origin, vector3_2, ref raycastHitArray[index1 * raysPerRow + index2], float.PositiveInfinity, num2);
          vector3_2 = Quaternion.op_Multiply(quaternion, vector3_2);
        }
      }
      return raycastHitArray;
    }

    public static GameObject MostHit(RaycastHit[] hits)
    {
      Dictionary<Transform, int> dictionary1 = new Dictionary<Transform, int>();
      foreach (RaycastHit raycastHit in hits)
      {
        // ISSUE: explicit reference operation
        if (Object.op_Inequality((Object) ((RaycastHit) @raycastHit).get_collider(), (Object) null))
        {
          // ISSUE: explicit reference operation
          if (!dictionary1.ContainsKey(((RaycastHit) @raycastHit).get_transform()))
          {
            // ISSUE: explicit reference operation
            dictionary1[((RaycastHit) @raycastHit).get_transform()] = 0;
          }
          Dictionary<Transform, int> dictionary2;
          Transform transform;
          // ISSUE: explicit reference operation
          (dictionary2 = dictionary1)[transform = ((RaycastHit) @raycastHit).get_transform()] = dictionary2[transform] + 1;
        }
      }
      Transform transform1 = (Transform) null;
      int num = 0;
      using (Dictionary<Transform, int>.Enumerator enumerator = dictionary1.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<Transform, int> current = enumerator.Current;
          if (current.Value > num)
          {
            transform1 = current.Key;
            num = current.Value;
          }
        }
      }
      if (Object.op_Inequality((Object) transform1, (Object) null))
        return ((Component) transform1).get_gameObject();
      return (GameObject) null;
    }

    public static GameObject MostHitWithWeights(RaycastHit[] hits, float[] rowWeights)
    {
      if (hits.Length % rowWeights.Length != 0)
      {
        Debug.LogError((object) "Invalid number of row weights.");
        return (GameObject) null;
      }
      Dictionary<Transform, float> dictionary1 = new Dictionary<Transform, float>();
      int num1 = 0;
      int index = 0;
      int num2 = hits.Length / rowWeights.Length;
      foreach (RaycastHit raycastHit in hits)
      {
        // ISSUE: explicit reference operation
        if (Object.op_Inequality((Object) ((RaycastHit) @raycastHit).get_collider(), (Object) null))
        {
          // ISSUE: explicit reference operation
          if (!dictionary1.ContainsKey(((RaycastHit) @raycastHit).get_transform()))
          {
            // ISSUE: explicit reference operation
            dictionary1[((RaycastHit) @raycastHit).get_transform()] = 0.0f;
          }
          Dictionary<Transform, float> dictionary2;
          Transform transform;
          // ISSUE: explicit reference operation
          (dictionary2 = dictionary1)[transform = ((RaycastHit) @raycastHit).get_transform()] = dictionary2[transform] + rowWeights[index];
        }
        ++num1;
        if (num1 == num2)
        {
          ++index;
          num1 = 0;
        }
      }
      Transform transform1 = (Transform) null;
      float num3 = 0.0f;
      using (Dictionary<Transform, float>.Enumerator enumerator = dictionary1.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<Transform, float> current = enumerator.Current;
          if ((double) current.Value > (double) num3)
          {
            transform1 = current.Key;
            num3 = current.Value;
          }
        }
      }
      if (Object.op_Inequality((Object) transform1, (Object) null))
        return ((Component) transform1).get_gameObject();
      return (GameObject) null;
    }

    private static RaycastHit CollidedDescendant(RaycastHit[] hits)
    {
      RaycastHit raycastHit = hits[0];
      for (int index = 1; index < hits.Length; ++index)
      {
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        if ((double) ((RaycastHit) @hits[index]).get_distance() >= (double) ((RaycastHit) @raycastHit).get_distance() || ((RaycastHit) @raycastHit).get_transform().IsChildOf(((RaycastHit) @hits[index]).get_transform()))
        {
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          if (((RaycastHit) @hits[index]).get_transform().IsChildOf(((RaycastHit) @raycastHit).get_transform()))
          {
            // ISSUE: explicit reference operation
            Bounds bounds = ((RaycastHit) @raycastHit).get_collider().get_bounds();
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            if (!((Bounds) @bounds).Contains(((RaycastHit) @hits[index]).get_point()))
              continue;
          }
          else
            continue;
        }
        raycastHit = hits[index];
      }
      return raycastHit;
    }
  }
}
