// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.SetPropertyUtility
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace UnityEngine.UI
{
  internal static class SetPropertyUtility
  {
    public static bool SetColor(ref Color currentValue, Color newValue)
    {
      if (currentValue.r == newValue.r && currentValue.g == newValue.g && (currentValue.b == newValue.b && currentValue.a == newValue.a))
        return false;
      currentValue = newValue;
      return true;
    }

    public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
    {
      if (currentValue.Equals((object) newValue))
        return false;
      currentValue = newValue;
      return true;
    }

    public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
    {
      if ((object) currentValue == null && (object) newValue == null || (object) currentValue != null && currentValue.Equals((object) newValue))
        return false;
      currentValue = newValue;
      return true;
    }
  }
}
