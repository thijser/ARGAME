// Decompiled with JetBrains decompiler
// Type: Meta.ScreenCursor
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Reflection;
using UnityEngine;

namespace Meta
{
  internal static class ScreenCursor
  {
    private static int _version = int.Parse(Application.get_unityVersion()[0].ToString());

    internal static void SetMouseCursorVisibility(bool visibility)
    {
      if (ScreenCursor._version >= 5)
      {
        System.Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
        type.GetProperty("visible").SetValue((object) type, (object) (bool) (visibility ? 1 : 0), (object[]) null);
      }
      else
      {
        System.Type type = Types.GetType("UnityEngine.Screen", "UnityEngine");
        type.GetProperty("showCursor").SetValue((object) type, (object) (bool) (visibility ? 1 : 0), (object[]) null);
      }
    }

    internal static void SetMouseCursorLockState(bool locked)
    {
      if (ScreenCursor._version >= 5)
      {
        System.Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
        PropertyInfo property = type.GetProperty("lockState");
        if (locked)
          property.SetValue((object) type, (object) 0, (object[]) null);
        else
          property.SetValue((object) type, (object) 1, (object[]) null);
      }
      else
      {
        System.Type type = Types.GetType("UnityEngine.Screen", "UnityEngine");
        type.GetProperty("lockCursor").SetValue((object) type, (object) (bool) (locked ? 1 : 0), (object[]) null);
      }
    }

    internal static bool GetMouseCursorVisibility()
    {
      if (ScreenCursor._version >= 5)
      {
        System.Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
        return (bool) type.GetProperty("visible").GetValue((object) type, (object[]) null);
      }
      System.Type type1 = Types.GetType("UnityEngine.Screen", "UnityEngine");
      return (bool) type1.GetProperty("showCursor").GetValue((object) type1, (object[]) null);
    }

    internal static bool GetMouseCursorLockState()
    {
      if (ScreenCursor._version >= 5)
      {
        System.Type type = Types.GetType("UnityEngine.Cursor", "UnityEngine");
        return (int) type.GetProperty("lockState").GetValue((object) type, (object[]) null) != 0;
      }
      System.Type type1 = Types.GetType("UnityEngine.Screen", "UnityEngine");
      return (bool) type1.GetProperty("lockCursor").GetValue((object) type1, (object[]) null);
    }
  }
}
