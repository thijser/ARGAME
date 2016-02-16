// Decompiled with JetBrains decompiler
// Type: Meta.MeshData
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;

namespace Meta
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  internal struct MeshData
  {
    public bool valid;
    public IntPtr vertices;
    public IntPtr normals;
    public IntPtr intensities;
    public IntPtr triangles;
    public IntPtr spatial_conf;
    public IntPtr temporal_conf;
    public int vertices_length;
    public int normals_length;
    public int triangles_length;
    public int tangents_length;

    public int GetVerticesLength()
    {
      return this.vertices_length;
    }

    public int GetTrianglesLength()
    {
      return this.triangles_length;
    }

    public int GetNormalsLength()
    {
      return this.normals_length;
    }
  }
}
