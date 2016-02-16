// Decompiled with JetBrains decompiler
// Type: Meta.Matrix4x4Extensions
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal static class Matrix4x4Extensions
  {
    public static Vector3 ScaleFromMatrix(this Matrix4x4 m)
    {
      Vector3 zero = Vector3.get_zero();
      for (int index = 0; index < 3; ++index)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector3& local = @zero;
        int num1 = index;
        // ISSUE: explicit reference operation
        Vector3 vector3 = Vector4.op_Implicit(((Matrix4x4) @m).GetColumn(index));
        // ISSUE: explicit reference operation
        double num2 = (double) ((Vector3) @vector3).get_magnitude();
        ((Vector3) local).set_Item(num1, (float) num2);
      }
      return zero;
    }

    public static Quaternion QuaternionFromMatrix(this Matrix4x4 m)
    {
      Vector3 vector3 = Matrix4x4Extensions.ScaleFromMatrix(m);
      for (int index = 0; index < 3; ++index)
      {
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Matrix4x4) @m).SetColumn(index, Vector4.op_Division(((Matrix4x4) @m).GetColumn(index), ((Vector3) @vector3).get_Item(index)));
      }
      Quaternion quaternion = (Quaternion) null;
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      quaternion.w = (__Null) ((double) Mathf.Sqrt(Mathf.Max(0.0f, 1f + ((Matrix4x4) @m).get_Item(0, 0) + ((Matrix4x4) @m).get_Item(1, 1) + ((Matrix4x4) @m).get_Item(2, 2))) / 2.0);
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      quaternion.x = (__Null) ((double) Mathf.Sqrt(Mathf.Max(0.0f, 1f + ((Matrix4x4) @m).get_Item(0, 0) - ((Matrix4x4) @m).get_Item(1, 1) - ((Matrix4x4) @m).get_Item(2, 2))) / 2.0);
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      quaternion.y = (__Null) ((double) Mathf.Sqrt(Mathf.Max(0.0f, 1f - ((Matrix4x4) @m).get_Item(0, 0) + ((Matrix4x4) @m).get_Item(1, 1) - ((Matrix4x4) @m).get_Item(2, 2))) / 2.0);
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      quaternion.z = (__Null) ((double) Mathf.Sqrt(Mathf.Max(0.0f, 1f - ((Matrix4x4) @m).get_Item(0, 0) - ((Matrix4x4) @m).get_Item(1, 1) + ((Matrix4x4) @m).get_Item(2, 2))) / 2.0);
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Quaternion& local1 = @quaternion;
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      double num1 = (^local1).x * (double) Mathf.Sign((float) (quaternion.x * ((double) ((Matrix4x4) @m).get_Item(2, 1) - (double) ((Matrix4x4) @m).get_Item(1, 2))));
      // ISSUE: explicit reference operation
      (^local1).x = (__Null) num1;
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Quaternion& local2 = @quaternion;
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      double num2 = (^local2).y * (double) Mathf.Sign((float) (quaternion.y * ((double) ((Matrix4x4) @m).get_Item(0, 2) - (double) ((Matrix4x4) @m).get_Item(2, 0))));
      // ISSUE: explicit reference operation
      (^local2).y = (__Null) num2;
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Quaternion& local3 = @quaternion;
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      double num3 = (^local3).z * (double) Mathf.Sign((float) (quaternion.z * ((double) ((Matrix4x4) @m).get_Item(1, 0) - (double) ((Matrix4x4) @m).get_Item(0, 1))));
      // ISSUE: explicit reference operation
      (^local3).z = (__Null) num3;
      return quaternion;
    }

    public static Vector3 PositionFromMatrix(this Matrix4x4 m)
    {
      // ISSUE: explicit reference operation
      return Vector4.op_Implicit(((Matrix4x4) @m).GetColumn(3));
    }
  }
}
