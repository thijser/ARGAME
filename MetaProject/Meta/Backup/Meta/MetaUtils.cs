// Decompiled with JetBrains decompiler
// Type: Meta.MetaUtils
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal static class MetaUtils
  {
    public static void QuitApp()
    {
      Application.Quit();
    }

    public static Rect Vector4toRect(Vector4 v)
    {
      Rect rect;
      // ISSUE: explicit reference operation
      ((Rect) @rect).\u002Ector((float) v.x, (float) v.y, (float) v.z, (float) v.w);
      return rect;
    }

    public static Vector4 RectToVector4(Rect r)
    {
      Vector4 vector4;
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      ((Vector4) @vector4).\u002Ector(((Rect) @r).get_x(), ((Rect) @r).get_y(), ((Rect) @r).get_width(), ((Rect) @r).get_height());
      return vector4;
    }

    public static void FloatToVector3(float[] data, ref Vector3 vector)
    {
      // ISSUE: explicit reference operation
      ((Vector3) @vector).Set(data[0], data[1], data[2]);
    }

    public static Vector3 FloatToVector3(float[] data)
    {
      return new Vector3(data[0], data[1], data[2]);
    }

    public static void FloatToVector2(float[] data, ref Vector2 vector)
    {
      // ISSUE: explicit reference operation
      ((Vector2) @vector).Set(data[0], data[1]);
    }

    public static Vector2 FloatToVector2(float[] data)
    {
      return new Vector2(data[0], data[1]);
    }
  }
}
