// Decompiled with JetBrains decompiler
// Type: Meta.IMULocalizer
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  public class IMULocalizer : Localizer
  {
    private bool _resetAtStart = true;
    private IMUMotionData _imuData;
    private Quaternion _imu2Gravity;
    private bool _imu2GravityValid;
    public GameObject gravity_arrow;

    public bool resetAtStart
    {
      get
      {
        return this._resetAtStart;
      }
      set
      {
        this._resetAtStart = value;
      }
    }

    public Vector3 imuOrientation
    {
      get
      {
        if (((Component) this).get_gameObject().get_activeSelf() && ((Behaviour) this).get_enabled())
          return this._imuData.FusedAngle;
        return new Vector3(0.0f, 0.0f, 0.0f);
      }
    }

    public Vector3 localizerOrientation
    {
      get
      {
        if (((Component) this).get_gameObject().get_activeSelf() && ((Behaviour) this).get_enabled())
          return this._imuData.SetAngle;
        return new Vector3(0.0f, 0.0f, 0.0f);
      }
    }

    public Vector3 accelerometerValues
    {
      get
      {
        if (((Component) this).get_gameObject().get_activeSelf() && ((Behaviour) this).get_enabled())
          return this._imuData.AccelerometerValues;
        return new Vector3(0.0f, 0.0f, 0.0f);
      }
    }

    public Vector3 gyroscopeValues
    {
      get
      {
        if (((Component) this).get_gameObject().get_activeSelf() && ((Behaviour) this).get_enabled())
          return this._imuData.GyroscopeValues;
        return new Vector3(0.0f, 0.0f, 0.0f);
      }
    }

    public Vector3 magnetometerValues
    {
      get
      {
        if (((Component) this).get_gameObject().get_activeSelf() && ((Behaviour) this).get_enabled())
          return this._imuData.MagnetometerValues;
        return new Vector3(0.0f, 0.0f, 0.0f);
      }
    }

    [DllImport("MetaVisionDLL", EntryPoint = "isMotionSensorConnected")]
    internal static extern bool IsMotionSensorConnected();

    public bool IsIMUConnected()
    {
      return IMULocalizer.IsMotionSensorConnected();
    }

    private void Start()
    {
      this._imuData = new IMUMotionData();
      this.InitDataStructs();
      if (!this._resetAtStart)
        return;
      this.Invoke("CalibrateAfterDelay", 1f);
    }

    private void InitDataStructs()
    {
      if (!Object.op_Equality((Object) this._targetGO, (Object) null))
        return;
      this.SetDefaultTargetGO();
    }

    private void CalibrateAfterDelay()
    {
      this.ResetLocalizer();
    }

    private void Update()
    {
      this._imuData.Update();
      if (Object.op_Equality((Object) this._targetGO, (Object) null))
        this.SetDefaultTargetGO();
      this.UpdateTargetGOTransform();
    }

    private void UpdateTargetGOTransform()
    {
      if (!this._imu2GravityValid)
        this.LatchIMU();
      this._targetGO.get_transform().set_rotation(!this._imu2GravityValid ? this._imuData.Compute() : Quaternion.op_Multiply(this._imu2Gravity, this._imuData.Compute()));
      Vector3 smoothedGravity = this._imuData.SmoothedGravity;
      // ISSUE: explicit reference operation
      ((Vector3) @smoothedGravity).Normalize();
      Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), Vector3.op_Multiply(10f, smoothedGravity), Color.get_green());
    }

    public bool LatchIMU()
    {
      Quaternion identity = Quaternion.get_identity();
      if (!this._imuData.LatchIMU(ref identity))
        return false;
      this._imu2Gravity = Quaternion.Inverse(identity);
      this._imu2GravityValid = true;
      if (Object.op_Inequality((Object) this.gravity_arrow, (Object) null))
        this.gravity_arrow.get_transform().set_rotation(identity);
      return true;
    }

    internal static bool isQuaternionValid(Quaternion Q)
    {
      return (double) Mathf.Abs((float) Math.Sqrt((double) (Q.x * Q.x + Q.y * Q.y + Q.z * Q.z + Q.w * Q.w)) - 1f) > 0.100000001490116;
    }

    public override void ResetLocalizer()
    {
      this._imuData.Reset();
      this._imu2GravityValid = false;
    }
  }
}
