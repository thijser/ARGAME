// Decompiled with JetBrains decompiler
// Type: Meta.PointCloudData
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;

namespace Meta
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct PointCloudData
  {
    public IntPtr vertices;
    public int size;

    public void Init()
    {
      GCHandle gcHandle = GCHandle.Alloc((object) new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3], GCHandleType.Pinned);
      this.vertices = gcHandle.AddrOfPinnedObject();
      gcHandle.Free();
    }
  }
}
