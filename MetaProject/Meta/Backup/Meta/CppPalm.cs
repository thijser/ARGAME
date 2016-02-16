// Decompiled with JetBrains decompiler
// Type: Meta.CppPalm
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;

namespace Meta
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct CppPalm
  {
    public int radius;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public float[] orientationAngles;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] normalVector;

    public void Init()
    {
      this.radius = 0;
      this.orientationAngles = new float[3];
      this.normalVector = new float[3];
    }
  }
}
