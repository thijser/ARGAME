// Decompiled with JetBrains decompiler
// Type: Meta.DeviceInfo
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;

namespace Meta
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct DeviceInfo
  {
    public int colorHeight;
    public int colorWidth;
    public int depthHeight;
    public int depthWidth;
    public bool streamingColor;
    public bool streamingDepth;
    public float depthFps;
    public float colorFps;
    public CameraModel cameraModel;
    public IMUModel imuModel;
  }
}
