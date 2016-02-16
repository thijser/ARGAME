// Decompiled with JetBrains decompiler
// Type: Meta.HandData
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;

namespace Meta
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct HandData
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    internal float[] top;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    internal float[] left;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    internal float[] right;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    internal float[] center;
    internal HandData.Type type;
    internal HandData.StateData state_data;
    internal bool valid;
    internal int angle;
    internal HandData.Palm palm;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    internal HandData.Finger[] fingers;
    internal MeshData hand_mesh;
    internal PointCloudDataOld hand_point_cloud;

    internal enum Type
    {
      NONE = -1,
      LEFT = 1,
      RIGHT = 2,
    }

    internal struct StateData
    {
      internal HandData.StateData.State state;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      internal float[] pinch_pt;
      internal float grab_value;

      internal enum State
      {
        NONE = -1,
        OPEN = 0,
        CLOSED = 1,
        POINT = 2,
        PINCH = 3,
      }
    }

    internal struct Palm
    {
      internal int radius;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      internal float[] orientation_angles;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      internal float[] norm_vec;
    }

    internal struct Finger
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      internal float[] loc;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      internal float[] dir;
      internal bool found;
    }

    internal enum FingerTypes
    {
      THUMB,
      INDEX,
      MIDDLE,
      RING,
      PINKY,
    }
  }
}
