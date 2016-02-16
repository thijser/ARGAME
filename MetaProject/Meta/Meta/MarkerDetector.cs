// Decompiled with JetBrains decompiler
// Type: Meta.MarkerDetector
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  public class MarkerDetector : MetaSingleton<MarkerDetector>, IMetaEventReceiver
  {
    private Vector3 markerOffset = new Vector3(0.0f, 0.0f, 0.0f);
    internal float markerReleaseRange = 0.2f;
    [Range(0.001f, 0.5f)]
    [SerializeField]
    private double _markerSizeMeters = 0.05;
    [SerializeField]
    private bool debug;
    private int _numDetectedMarkers;
    private Dictionary<int, Matrix4x4> markerTransformDict;
    public List<int> updatedMarkerTransforms;
    [HideInInspector]
    private MarkerDetector.CppMarkerDataArray _cppMarkerDataArray;

    public double markerSizeMeters
    {
      get
      {
        return this._markerSizeMeters;
      }
      set
      {
        this._markerSizeMeters = value;
      }
    }

    [Obsolete]
    public void SetMarkerSize(double markerSizeMeters)
    {
      MarkerDetector.SetMarkerSize_(markerSizeMeters);
    }

    public int GetNumberOfVisibleMarkers()
    {
      return this._numDetectedMarkers;
    }

    public static int GetNumberOfMarkersVisible()
    {
      return MetaSingleton<MarkerDetector>.Instance._numDetectedMarkers;
    }

    private void Start()
    {
      MetaCore.Instance.Log("Initialize MarkerDetector");
      this.markerTransformDict = new Dictionary<int, Matrix4x4>();
      this._numDetectedMarkers = 0;
      if (this.markerSizeMeters >= 0.001)
        return;
      this.markerSizeMeters = 0.001;
    }

    public void MetaLateUpdate()
    {
    }

    public void MetaInit()
    {
      this._cppMarkerDataArray = new MarkerDetector.CppMarkerDataArray();
      this._cppMarkerDataArray.cppMarkerData = new MarkerDetector.CppMarkerData[10];
      for (int index = 0; index < 10; ++index)
      {
        this._cppMarkerDataArray.cppMarkerData[index].center2DCoords = new float[2];
        this._cppMarkerDataArray.cppMarkerData[index].corner2DCoords = new float[8];
        this._cppMarkerDataArray.cppMarkerData[index].center3DCoords = new float[3];
        this._cppMarkerDataArray.cppMarkerData[index].corner3DCoords = new float[12];
        this._cppMarkerDataArray.cppMarkerData[index].rotationMatrix = new float[9];
        this._cppMarkerDataArray.cppMarkerData[index].translationVector = new float[3];
        this._cppMarkerDataArray.cppMarkerData[index].transformMatrix = new float[16];
      }
      MarkerDetector.RegisterMarkerDetector_();
      MarkerDetector.SetMarkerSize_(this.markerSizeMeters);
      MarkerDetector.EnableDebugDisplay_(this.debug);
      MarkerDetector.EnableMarkerDetector_();
    }

    public void MetaUpdate()
    {
      if (!((Component) this).get_gameObject().get_activeInHierarchy())
        return;
      this._numDetectedMarkers = MarkerDetector.GetMarkerData_(ref this._cppMarkerDataArray);
      this.updatedMarkerTransforms = new List<int>();
      this.UpdateMarkerTransforms();
    }

    public void MetaOnDestroy()
    {
    }

    [DllImport("MetaVisionDLL", EntryPoint = "registerMarkerDetector")]
    private static extern void RegisterMarkerDetector_();

    [DllImport("MetaVisionDLL", EntryPoint = "enableMarkerDetector")]
    private static extern void EnableMarkerDetector_();

    [DllImport("MetaVisionDLL", EntryPoint = "disableMarkerDetector")]
    private static extern void DisableMarkerDetector_();

    [DllImport("MetaVisionDLL", EntryPoint = "setMarkerSize")]
    private static extern void SetMarkerSize_(double markerSizeMeters);

    [DllImport("MetaVisionDLL", EntryPoint = "getNumberOfVisibleMarkers")]
    private static extern int GetNumberOfVisibleMarkers_();

    [DllImport("MetaVisionDLL", EntryPoint = "getMarkerData")]
    private static extern int GetMarkerData_(ref MarkerDetector.CppMarkerDataArray cppMarkerData);

    [DllImport("MetaVisionDLL", EntryPoint = "enableDebugDisplay")]
    private static extern void EnableDebugDisplay_(bool enable);

    private void UpdateMarkerTransforms()
    {
      int num = Math.Min(this._numDetectedMarkers, 10);
      for (int index = 0; index < num; ++index)
      {
        int key = this._cppMarkerDataArray.cppMarkerData[index].id;
        this.updatedMarkerTransforms.Add(key);
        if (!this.markerTransformDict.ContainsKey(key))
        {
          Matrix4x4 matrix4x4 = (Matrix4x4) null;
          this.markerTransformDict.Add(key, matrix4x4);
        }
        this.markerTransformDict[key] = this.FloatArrToMatrix4_(ref this._cppMarkerDataArray.cppMarkerData[index].transformMatrix);
      }
    }

    private MarkerDetector.MarkerData GetMarkerDataAt(int index)
    {
      return this.CopyCppMarkerDataToMarkerData_(ref this._cppMarkerDataArray.cppMarkerData[index]);
    }

    public void GetMarkerTransform(int markerID, ref Transform newTransform)
    {
      if (this.markerTransformDict == null || !this.markerTransformDict.ContainsKey(markerID))
        return;
      Matrix4x4 m = Matrix4x4.op_Multiply(((Component) this).get_transform().get_localToWorldMatrix(), this.markerTransformDict[markerID]);
      newTransform.set_position(Matrix4x4Extensions.PositionFromMatrix(m));
      newTransform.set_rotation(Matrix4x4Extensions.QuaternionFromMatrix(m));
      newTransform.set_position(Vector3.op_Addition(newTransform.get_position(), this.markerOffset));
      newTransform.Rotate(90f, 0.0f, 0.0f);
    }

    private Vector3 FloatArrToVec3_(ref float[] arr)
    {
      return new Vector3(arr[0], arr[1], arr[2]);
    }

    private Vector3 FloatSubArrToVec3_(ref float[] arr, int startIdx)
    {
      return new Vector3(arr[startIdx], arr[startIdx + 1], arr[startIdx + 2]);
    }

    private Matrix4x4 FloatArrToMatrix4_(ref float[] arr)
    {
      Matrix4x4 matrix4x4 = (Matrix4x4) null;
      for (int index1 = 0; index1 < 4; ++index1)
      {
        for (int index2 = 0; index2 < 4; ++index2)
        {
          // ISSUE: explicit reference operation
          ((Matrix4x4) @matrix4x4).set_Item(index1, index2, arr[index1 * 4 + index2]);
        }
      }
      return matrix4x4;
    }

    private MarkerDetector.MarkerData CopyCppMarkerDataToMarkerData_(ref MarkerDetector.CppMarkerData cppMarkerData)
    {
      MarkerDetector.MarkerData markerData = new MarkerDetector.MarkerData();
      markerData.id = cppMarkerData.id;
      for (int index = 0; index < 4; ++index)
        markerData.cornerCoords[index] = this.FloatSubArrToVec3_(ref cppMarkerData.corner3DCoords, index * 3);
      markerData.transformMatrix = this.FloatArrToMatrix4_(ref cppMarkerData.transformMatrix);
      return markerData;
    }

    private struct MarkerData
    {
      public int id;
      public Vector3[] cornerCoords;
      public Vector3 centerCoords;
      public Matrix4x4 transformMatrix;
    }

    private struct CppMarkerData
    {
      public int id;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public float[] corner2DCoords;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
      public float[] corner3DCoords;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] center2DCoords;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] center3DCoords;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
      public float[] rotationMatrix;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] translationVector;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      public float[] transformMatrix;
    }

    private struct CppMarkerDataArray
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      public MarkerDetector.CppMarkerData[] cppMarkerData;
    }
  }
}
