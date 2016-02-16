// Decompiled with JetBrains decompiler
// Type: Meta.MetaPlugin
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.IO;
using UnityEngine;

namespace Meta
{
  public class MetaPlugin
  {
    static MetaPlugin()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
      string str;
      if (Application.get_isEditor())
        str = Environment.CurrentDirectory + (object) Path.DirectorySeparatorChar + "Assets" + (string) (object) Path.DirectorySeparatorChar + "Plugins" + (string) (object) Path.DirectorySeparatorChar + "x86";
      else
        str = Application.get_dataPath() + (object) Path.DirectorySeparatorChar + "Plugins";
      if (environmentVariable.Contains(str))
        return;
      Environment.SetEnvironmentVariable("PATH", str + (object) Path.PathSeparator + environmentVariable, EnvironmentVariableTarget.Process);
    }

    public static void Load()
    {
    }
  }
}
