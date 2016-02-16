// Decompiled with JetBrains decompiler
// Type: Meta.CanvasTracker
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  internal class CanvasTracker : MetaSingleton<CanvasTracker>, IMetaEventReceiver
  {
    private List<CanvasTarget> m_canvasTargets = new List<CanvasTarget>();
    internal bool m_keyboardControlled = true;
    private float modelCorrectionThetaDegrees = -270f;
    [HideInInspector]
    internal bool m_debugMode;
    [SerializeField]
    private bool m_IsValid;
    [SerializeField]
    private bool m_RectanglesDetected;
    internal Matrix4x4 m_transformation_matrix_local;
    internal Matrix4x4 m_transformation_matrix_global;
    [SerializeField]
    private bool m_TargetLocked;
    private static bool m_IsInitialized;
    private int m_numFrames;
    private bool m_mirror;

    internal static List<CanvasTarget> CanvasTargets
    {
      get
      {
        return MetaSingleton<CanvasTracker>.Instance.m_canvasTargets;
      }
    }

    internal bool IsValid
    {
      get
      {
        return this.m_IsValid;
      }
    }

    internal bool RectanglesDetected
    {
      get
      {
        return this.m_RectanglesDetected;
      }
    }

    internal bool TargetLocked
    {
      get
      {
        return this.m_TargetLocked;
      }
    }

    [DllImport("MetaVisionDLL", EntryPoint = "registerCanvasTracker")]
    protected static extern void RegisterDLL();

    [DllImport("MetaVisionDLL", EntryPoint = "InitPlaneTracker")]
    protected static extern void InitDLL();

    [DllImport("MetaVisionDLL", EntryPoint = "ReleasePlaneTracker")]
    protected static extern void ReleaseDLL();

    [DllImport("MetaVisionDLL", EntryPoint = "GetRectangleTransform")]
    protected static extern void GetRectangleTransformDLL(ref mMatrix transformation_matrix);

    [DllImport("MetaVisionDLL", EntryPoint = "SetTargetLock")]
    protected static extern void SetTargetLockDLL(bool value);

    [DllImport("MetaVisionDLL", EntryPoint = "GetTargetLock")]
    protected static extern bool GetTargetLockDLL();

    [DllImport("MetaVisionDLL", EntryPoint = "GetRectanglesDetected")]
    protected static extern bool GetRectanglesDetectedDLL();

    public void MetaInit()
    {
      CanvasTracker.RegisterDLL();
    }

    public void MetaUpdate()
    {
    }

    public void MetaOnDestroy()
    {
    }

    public void MetaLateUpdate()
    {
    }

    private void Start()
    {
      if (MetaCore.Instance.m_depthSensorConnected)
      {
        this.InitializeTracker();
        this.m_IsValid = false;
      }
      else
        ((Behaviour) this).set_enabled(false);
    }

    private void Update()
    {
      this.GetData();
      this.KeyboardControl();
    }

    private void OnDestroy()
    {
      if (!((Behaviour) this).get_enabled())
        return;
      this.Release();
    }

    private void InitializeTracker()
    {
      if (CanvasTracker.m_IsInitialized)
        return;
      CanvasTracker.m_IsInitialized = true;
      CanvasTracker.InitDLL();
    }

    private void KeyboardControl()
    {
      if (!this.m_keyboardControlled)
        return;
      if (Input.GetKeyDown((KeyCode) 108))
      {
        CanvasTracker.SetTargetLockDLL(true);
        if (!this.m_debugMode)
          return;
        Debug.Log(!this.m_TargetLocked ? (object) "Target UNLOCKED" : (object) "Target LOCKED");
      }
      else
      {
        if (!Input.GetKeyDown((KeyCode) 107))
          return;
        CanvasTracker.SetTargetLockDLL(false);
        if (!this.m_debugMode)
          return;
        Debug.Log(!this.m_TargetLocked ? (object) "Target UNLOCKED" : (object) "Target LOCKED");
      }
    }

    internal void LockTarget()
    {
      CanvasTracker.SetTargetLockDLL(true);
    }

    internal void UnlockTarget()
    {
      CanvasTracker.SetTargetLockDLL(false);
    }

    private void Release()
    {
      CanvasTracker.ReleaseDLL();
      this.m_IsValid = false;
      CanvasTracker.m_IsInitialized = false;
    }

    private void GetData()
    {
      this.m_RectanglesDetected = CanvasTracker.GetRectanglesDetectedDLL();
      mMatrix transformation_matrix = new mMatrix();
      CanvasTracker.GetRectangleTransformDLL(ref transformation_matrix);
      this.m_TargetLocked = CanvasTracker.GetTargetLockDLL();
      if (mMatrix.IsNaN(transformation_matrix))
      {
        this.m_IsValid = false;
        if (!this.m_debugMode)
          return;
        Debug.Log((object) "NAN DETECTED in the fetched matrix");
      }
      else
      {
        this.m_IsValid = true;
        if (this.m_IsValid)
          this.m_numFrames = this.m_numFrames <= 1000 ? this.m_numFrames + 1 : 100;
        Matrix4x4 matrix4x4_1 = (Matrix4x4) null;
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(0, transformation_matrix.m00);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(4, transformation_matrix.m10);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(8, transformation_matrix.m20);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(12, transformation_matrix.m30);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(1, transformation_matrix.m01);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(5, transformation_matrix.m11);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(9, transformation_matrix.m21);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(13, transformation_matrix.m31);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(2, transformation_matrix.m02);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(6, transformation_matrix.m12);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(10, transformation_matrix.m22);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(14, transformation_matrix.m32);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(3, transformation_matrix.m03);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(7, transformation_matrix.m13);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(11, transformation_matrix.m23);
        // ISSUE: explicit reference operation
        ((Matrix4x4) @matrix4x4_1).set_Item(15, transformation_matrix.m33);
        // ISSUE: explicit reference operation
        matrix4x4_1 = ((Matrix4x4) @matrix4x4_1).get_transpose();
        float num = Math.PI / 180.0 * this.modelCorrectionThetaDegrees;
        Matrix4x4 matrix4x4_2 = (Matrix4x4) null;
        matrix4x4_2.m00 = (__Null) 1.0;
        matrix4x4_2.m11 = (__Null) (double) Mathf.Cos(num);
        matrix4x4_2.m22 = (__Null) (double) Mathf.Cos(num);
        matrix4x4_2.m33 = (__Null) 1.0;
        matrix4x4_2.m12 = (__Null) (double) Mathf.Sin(num);
        matrix4x4_2.m21 = (__Null) -(double) Mathf.Sin(num);
        matrix4x4_1 = Matrix4x4.op_Multiply(matrix4x4_1, matrix4x4_2);
        Matrix4x4 matrix4x4_3 = (Matrix4x4) null;
        matrix4x4_3.m00 = (__Null) -1.0;
        matrix4x4_3.m11 = (__Null) 1.0;
        matrix4x4_3.m22 = (__Null) 1.0;
        matrix4x4_3.m33 = (__Null) 1.0;
        if (this.m_mirror)
          matrix4x4_1 = Matrix4x4.op_Multiply(matrix4x4_3, matrix4x4_1);
        this.m_transformation_matrix_local = matrix4x4_1;
        Matrix4x4 matrix4x4_4 = (Matrix4x4) null;
        matrix4x4_4.m00 = (__Null) 1.0;
        matrix4x4_4.m11 = (__Null) 1.0;
        matrix4x4_4.m22 = (__Null) -1.0;
        matrix4x4_4.m33 = (__Null) 1.0;
        this.m_transformation_matrix_global = Matrix4x4.op_Multiply(((Component) this).get_transform().get_localToWorldMatrix(), this.m_transformation_matrix_local);
      }
    }

    internal static Vector3 GetPosition()
    {
      return Matrix4x4Extensions.PositionFromMatrix(MetaSingleton<CanvasTracker>.Instance.m_transformation_matrix_global);
    }

    internal static Quaternion GetRotation()
    {
      return Matrix4x4Extensions.QuaternionFromMatrix(MetaSingleton<CanvasTracker>.Instance.m_transformation_matrix_global);
    }

    internal static Vector3 GetScale()
    {
      return Matrix4x4Extensions.ScaleFromMatrix(MetaSingleton<CanvasTracker>.Instance.m_transformation_matrix_global);
    }

    internal static void AddTarget(CanvasTarget pTarget)
    {
      if (Object.op_Inequality((Object) MetaSingleton<CanvasTracker>.Instance, (Object) null))
        MetaSingleton<CanvasTracker>.Instance.m_canvasTargets.Add(pTarget);
      else
        Debug.LogWarning((object) "CanvasTracker.AddTarget can't find an Instance yet.");
    }
  }
}
