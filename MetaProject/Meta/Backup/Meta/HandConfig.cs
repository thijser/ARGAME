// Decompiled with JetBrains decompiler
// Type: Meta.HandConfig
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  [Serializable]
  internal class HandConfig
  {
    [SerializeField]
    [HideInInspector]
    internal bool _enableMeshRandD;
    internal int _maxHandVertices;
    [Range(10f, 500f)]
    [SerializeField]
    private int _minDepth;
    [Range(800f, 2000f)]
    [SerializeField]
    private int _maxDepth;
    [Range(50f, 250f)]
    [SerializeField]
    private int _minConfidence;
    [SerializeField]
    [Range(500f, 3000f)]
    private double _areaLimit;
    [Range(0.0f, 20f)]
    [SerializeField]
    private float _handVelocity;
    [Range(-10f, 10f)]
    [SerializeField]
    private float _grabThreshold;
    [Range(1f, 5f)]
    [SerializeField]
    [HideInInspector]
    private int _swipeMinFrames;
    [SerializeField]
    [HideInInspector]
    [Range(10f, 30f)]
    private int _swipeMaxFrames;
    [SerializeField]
    private bool _debug;
    [HideInInspector]
    [SerializeField]
    private bool _fingertips;
    [SerializeField]
    private bool _kalman;

    public int minDepth
    {
      get
      {
        return this._minDepth;
      }
      set
      {
        if (value == this._minDepth)
          return;
        this._minDepth = value <= 500 ? (value >= 10 ? value : 10) : 500;
        this.SetAllParameters();
      }
    }

    public int maxDepth
    {
      get
      {
        return this._maxDepth;
      }
      set
      {
        if (value == this._maxDepth)
          return;
        this._maxDepth = value <= 2000 ? (value >= 800 ? value : 800) : 2000;
        this.SetAllParameters();
      }
    }

    public int minConfidence
    {
      get
      {
        return this._minConfidence;
      }
      set
      {
        if (value == this._minConfidence)
          return;
        this._minConfidence = value <= 250 ? (value >= 50 ? value : 50) : 250;
        this.SetAllParameters();
      }
    }

    public double areaLimit
    {
      get
      {
        return this._areaLimit;
      }
      set
      {
        if (value == this._areaLimit)
          return;
        this._areaLimit = value <= 3000.0 ? (value >= 500.0 ? value : 500.0) : 3000.0;
        this.SetAllParameters();
      }
    }

    public float handVelocity
    {
      get
      {
        return this._handVelocity;
      }
      set
      {
        if ((double) value == (double) this._handVelocity)
          return;
        this._handVelocity = (double) value <= 0.0 ? ((double) value >= 20.0 ? value : 20f) : 0.0f;
        this.SetAllParameters();
      }
    }

    public float grabThreshold
    {
      get
      {
        return this._grabThreshold;
      }
      set
      {
        if ((double) value == (double) this._grabThreshold)
          return;
        this._grabThreshold = (double) value <= -10.0 ? ((double) value >= 10.0 ? value : 10f) : -10f;
        this.SetAllParameters();
      }
    }

    public int swipeMinFrames
    {
      get
      {
        return this._swipeMinFrames;
      }
      set
      {
        if (value == this._swipeMinFrames)
          return;
        this._swipeMinFrames = value <= 1 ? (value >= 5 ? value : 5) : 1;
        this.SetAllParameters();
      }
    }

    public int swipeMaxFrames
    {
      get
      {
        return this._swipeMaxFrames;
      }
      set
      {
        if (value == this._swipeMaxFrames)
          return;
        this._swipeMaxFrames = value <= 10 ? (value >= 30 ? value : 30) : 10;
        this.SetAllParameters();
      }
    }

    public bool debug
    {
      get
      {
        return this._debug;
      }
      set
      {
        if (this._debug == value)
          return;
        if (!value)
        {
          this._debug = false;
          this.SetAllParameters();
        }
        else
        {
          this._debug = true;
          this.SetAllParameters();
        }
      }
    }

    public bool fingertips
    {
      get
      {
        return this._fingertips;
      }
      set
      {
        if (!value)
        {
          this._fingertips = false;
          this.SetAllParameters();
        }
        else
        {
          this._fingertips = true;
          this.SetAllParameters();
        }
      }
    }

    public bool kalman
    {
      get
      {
        return this._kalman;
      }
      set
      {
        if (this._kalman == value)
          return;
        if (!value)
        {
          this._kalman = false;
          this.SetAllParameters();
        }
        else
        {
          this._kalman = true;
          this.SetAllParameters();
        }
      }
    }

    public HandConfig()
    {
      this._minDepth = 100;
      this._maxDepth = 800;
      this._minConfidence = 100;
      this._areaLimit = 2000.0;
      this._handVelocity = 10f;
      this._grabThreshold = -10f;
      this._swipeMinFrames = 3;
      this._swipeMaxFrames = 20;
      this._debug = false;
      this._fingertips = false;
      this._kalman = true;
      this._enableMeshRandD = false;
      this._maxHandVertices = 35000;
    }

    [DllImport("MetaVisionDLL", EntryPoint = "setHandMinDepth")]
    private static extern void SetHandMinDepth(short minDepth);

    [DllImport("MetaVisionDLL", EntryPoint = "setHandMaxDepth")]
    private static extern void SetHandMaxDepth(short maxDepth);

    [DllImport("MetaVisionDLL", EntryPoint = "setHandMinConfidence")]
    private static extern void SetHandMinConfidence(short minConfidence);

    [DllImport("MetaVisionDLL", EntryPoint = "setHandMinConfidence")]
    private static extern void SetHandAreaLimit(short areaLimit);

    [DllImport("MetaVisionDLL", EntryPoint = "setHandVelocity")]
    private static extern void SetHandVelocity(float velocity);

    [DllImport("MetaVisionDLL", EntryPoint = "setGrabThreshold")]
    private static extern void SetGrabThreshold(float grabThreshold);

    [DllImport("MetaVisionDLL", EntryPoint = "setHandVelocity")]
    private static extern void SetSwipeMinFrames(int frames);

    [DllImport("MetaVisionDLL", EntryPoint = "setHandVelocity")]
    private static extern void SetSwipeMaxFrames(int frames);

    [DllImport("MetaVisionDLL", EntryPoint = "enableDebugMode")]
    private static extern void EnableDebugMode();

    [DllImport("MetaVisionDLL", EntryPoint = "disableDebugMode")]
    private static extern void DisableDebugMode();

    [DllImport("MetaVisionDLL", EntryPoint = "enableFingerTips")]
    private static extern void EnableFingerTips();

    [DllImport("MetaVisionDLL", EntryPoint = "disableFingerTips")]
    private static extern void DisableFingerTips();

    [DllImport("MetaVisionDLL", EntryPoint = "enableKalman")]
    private static extern void EnableKalman();

    [DllImport("MetaVisionDLL", EntryPoint = "disableKalman")]
    private static extern void DisableKalman();

    [DllImport("MetaVisionDLL", EntryPoint = "setHandParameters")]
    private static extern void SetHandParameters(ushort minDepth, ushort maxDepth, ushort minConfidence, float areaLimit, float velocity, float grabThreshBuffer, int min_frames, int max_frames, int max_hand_vertices, bool showDebugWindows, bool enableFingertips, bool enableKalman, bool enableHandCloud, bool enableHandMesh);

    internal void SetAllParameters()
    {
      HandConfig.SetHandParameters((ushort) this.minDepth, (ushort) this.maxDepth, (ushort) this.minConfidence, (float) (ushort) this.areaLimit, this.handVelocity, this.grabThreshold, this.swipeMinFrames, this.swipeMaxFrames, this._maxHandVertices, this.debug, this.fingertips, this.kalman, true, true);
    }
  }
}
