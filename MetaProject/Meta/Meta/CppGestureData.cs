// Decompiled with JetBrains decompiler
// Type: Meta.CppGestureData
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;

namespace Meta
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct CppGestureData
  {
    public bool valid;
    public MetaGesture manipulationGesture;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] gesturePoint;

    public void Init()
    {
      this.gesturePoint = new float[3];
      this.manipulationGesture = MetaGesture.OPEN;
      this.valid = false;
    }
  }
}
