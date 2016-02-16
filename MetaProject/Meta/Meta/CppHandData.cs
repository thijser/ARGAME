// Decompiled with JetBrains decompiler
// Type: Meta.CppHandData
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;

namespace Meta
{
  internal struct CppHandData
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] top;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] left;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] right;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] center;
    public bool valid;
    public int angle;
    public CppPalm palm;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public CppFinger[] fingers;
    public CppGestureData gesture;
    public float handOpenness;

    public void Init()
    {
      this.top = new float[3];
      this.left = new float[3];
      this.right = new float[3];
      this.center = new float[3];
      this.valid = false;
      this.angle = 0;
      this.palm = new CppPalm();
      this.palm.Init();
      this.fingers = new CppFinger[5];
      for (int index = 0; index < 5; ++index)
        this.fingers[index].Init();
      this.gesture = new CppGestureData();
      this.gesture.Init();
      this.handOpenness = 0.0f;
    }
  }
}
