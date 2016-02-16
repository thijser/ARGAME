// Decompiled with JetBrains decompiler
// Type: Meta.IMUMotionData
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  internal class IMUMotionData
  {
    private Vector3 _smoothedGravity = (Vector3) null;
    private float _lambdaGravity = 0.5f;
    private bool _firstGravity = true;
    private IMUMotionData.MotionSensorData motionSensorData;
    private Quaternion _orientation;
    private Quaternion _correctionOrientation;
    private Vector3 _correctionVector;
    private Vector3 _fusedAngle;
    private Vector3 _setAngle;
    private Vector3 _accelerometerValues;
    private Vector3 _magnetometerValues;
    private Vector3 _gyroscopeValues;

    public Quaternion Orientation
    {
      get
      {
        return this._orientation;
      }
    }

    public Quaternion CorrectionOrientation
    {
      get
      {
        return this._correctionOrientation;
      }
    }

    public Vector3 CorrectionVector
    {
      get
      {
        return this._correctionVector;
      }
      set
      {
        this._correctionVector = value;
      }
    }

    public Vector3 FusedAngle
    {
      get
      {
        return this._fusedAngle;
      }
    }

    public Vector3 SetAngle
    {
      get
      {
        return this._setAngle;
      }
    }

    public Vector3 AccelerometerValues
    {
      get
      {
        return this._accelerometerValues;
      }
    }

    public Vector3 MagnetometerValues
    {
      get
      {
        return this._magnetometerValues;
      }
    }

    public Vector3 GyroscopeValues
    {
      get
      {
        return this._gyroscopeValues;
      }
    }

    public Vector3 SmoothedGravity
    {
      get
      {
        return this._smoothedGravity;
      }
    }

    public float LambdaGravity
    {
      get
      {
        return this._lambdaGravity;
      }
      set
      {
        this._lambdaGravity = Math.Min(1f, Math.Max(value, 1.0 / 1000.0));
      }
    }

    public Vector3 Gravity
    {
      get
      {
        Vector3 vector3 = (Vector3) null;
        double num1 = this.motionSensorData.accelerometerValues[0];
        double num2 = this.motionSensorData.accelerometerValues[1];
        double num3 = this.motionSensorData.accelerometerValues[2];
        double num4 = Math.Sqrt(num2 * num2 + num3 * num3);
        double num5 = num3 >= 0.0 ? 1.0 : -1.0;
        double num6 = Math.Abs(num3) >= 1E-06 ? num3 : num5 * 1E-06;
        double num7 = Math.Atan(num2 / num6);
        double num8 = Math.Atan(num1 / (num4 + 1E-06));
        double num9 = Math.Cos(num8);
        double num10 = Math.Sin(num8);
        double num11 = Math.Cos(num7);
        double num12 = Math.Sin(num7);
        ((Vector3) @vector3).set_Item(0, (float) num10);
        ((Vector3) @vector3).set_Item(1, -(float) (num11 * num9));
        ((Vector3) @vector3).set_Item(2, (float) (num9 * num12));
        ((Vector3) @vector3).Normalize();
        return vector3;
      }
    }

    public IMUMotionData()
    {
      this.motionSensorData.accelerometerValues = new double[3];
      this.motionSensorData.gyroscopeValues = new double[3];
      this.motionSensorData.magnetometerValues = new double[3];
      this.motionSensorData.orientation = new double[3];
      this.motionSensorData.fusedAngle = new double[3];
      this.InitDataStructs();
    }

    [DllImport("MetaVisionDLL", EntryPoint = "getMotionSensorData")]
    internal static extern void GetMotionSensorData(ref IMUMotionData.MotionSensorData motionSensorData);

    private void InitDataStructs()
    {
      this._orientation = this._correctionOrientation = Quaternion.get_identity();
      this._correctionVector = this._fusedAngle = this._setAngle = this._accelerometerValues = this._magnetometerValues = this._gyroscopeValues = Vector3.get_zero();
      for (int index = 0; index < 3; ++index)
        this.motionSensorData.accelerometerValues[index] = this.motionSensorData.gyroscopeValues[index] = this.motionSensorData.magnetometerValues[index] = this.motionSensorData.orientation[index] = this.motionSensorData.fusedAngle[index] = 0.0;
      this._firstGravity = true;
    }

    public void Update()
    {
      IMUMotionData.GetMotionSensorData(ref this.motionSensorData);
      this._orientation.w = (__Null) this.motionSensorData.orientation[0];
      this._orientation.x = (__Null) this.motionSensorData.orientation[1];
      this._orientation.y = (__Null) -this.motionSensorData.orientation[3];
      this._orientation.z = (__Null) this.motionSensorData.orientation[2];
      if (MetaCore.Instance.imuModel == IMUModel.MPU9150HID)
      {
        this._fusedAngle.x = (__Null) (this.motionSensorData.fusedAngle[0] * 180.0 / Math.PI);
        this._fusedAngle.y = (__Null) (this.motionSensorData.fusedAngle[2] * 180.0 / Math.PI);
        this._fusedAngle.z = (__Null) -(this.motionSensorData.fusedAngle[1] * 180.0 / Math.PI);
      }
      else
      {
        this._fusedAngle.x = (__Null) (this.motionSensorData.fusedAngle[0] * 180.0 / Math.PI);
        this._fusedAngle.y = (__Null) -(this.motionSensorData.fusedAngle[2] * 180.0 / Math.PI);
        this._fusedAngle.z = (__Null) (this.motionSensorData.fusedAngle[1] * 180.0 / Math.PI);
      }
      this._accelerometerValues.x = (__Null) this.motionSensorData.accelerometerValues[0];
      this._accelerometerValues.y = (__Null) -this.motionSensorData.accelerometerValues[2];
      this._accelerometerValues.z = (__Null) this.motionSensorData.accelerometerValues[1];
      this._gyroscopeValues.x = (__Null) this.motionSensorData.gyroscopeValues[0];
      this._gyroscopeValues.y = (__Null) -this.motionSensorData.gyroscopeValues[2];
      this._gyroscopeValues.z = (__Null) this.motionSensorData.gyroscopeValues[1];
      this._magnetometerValues.x = (__Null) this.motionSensorData.magnetometerValues[0];
      this._magnetometerValues.y = (__Null) -this.motionSensorData.magnetometerValues[2];
      this._magnetometerValues.z = (__Null) this.motionSensorData.magnetometerValues[1];
      if (!IMUMotionData.isNormalizedQuaternionValid(this.Compute()))
        return;
      if (this._firstGravity)
      {
        this._firstGravity = false;
        this._smoothedGravity = this.Gravity;
      }
      else
        this._smoothedGravity = Vector3.op_Division(Vector3.op_Addition(Vector3.op_Multiply(this._lambdaGravity, this._smoothedGravity), this.Gravity), 1f + this._lambdaGravity);
    }

    internal Quaternion Compute()
    {
      this._setAngle = Vector3.op_Subtraction(this._fusedAngle, this._correctionVector);
      return Quaternion.Euler(this._setAngle);
    }

    public void Reset()
    {
      this._correctionOrientation = !IMUMotionData.isNormalizedQuaternionValid(this._orientation) ? Quaternion.get_identity() : this._orientation;
      this._correctionVector = this._fusedAngle;
      this._firstGravity = true;
    }

    internal static bool isNormalizedQuaternionValid(Quaternion Q)
    {
      return (double) Mathf.Abs((float) Math.Sqrt((double) (Q.x * Q.x + Q.y * Q.y + Q.z * Q.z + Q.w * Q.w)) - 1f) < 0.100000001490116;
    }

    internal static bool isQuaternionValid(Quaternion Q)
    {
      return (double) Mathf.Abs((float) Math.Sqrt((double) (Q.x * Q.x + Q.y * Q.y + Q.z * Q.z + Q.w * Q.w))) > 9.99999997475243E-07;
    }

    public bool LatchIMU(ref Quaternion oVirt2Gravity)
    {
      if (!IMUMotionData.isNormalizedQuaternionValid(this.Compute()))
        return false;
      Vector3 smoothedGravity = this.SmoothedGravity;
      // ISSUE: explicit reference operation
      ((Vector3) @smoothedGravity).Normalize();
      Vector3 vector3_1;
      // ISSUE: explicit reference operation
      ((Vector3) @vector3_1).\u002Ector(0.0f, -1f, 0.0f);
      Vector3 vector3_2 = Vector3.Cross(vector3_1, smoothedGravity);
      // ISSUE: explicit reference operation
      if ((double) ((Vector3) @vector3_2).get_sqrMagnitude() < 1E-07)
        return false;
      Vector3 vector3_3 = Vector3.Cross(smoothedGravity, vector3_2);
      // ISSUE: explicit reference operation
      ((Vector3) @vector3_3).Normalize();
      double num = Math.Acos((double) Vector3.Dot(vector3_1, smoothedGravity)) * 57.2957795130823;
      // ISSUE: explicit reference operation
      ((Vector3) @vector3_2).Normalize();
      oVirt2Gravity = Quaternion.AngleAxis((float) num, vector3_2);
      return true;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct MotionSensorData
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] gyroscopeValues;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] accelerometerValues;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] magnetometerValues;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public double[] fusedAngle;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public double[] orientation;
      private double timestamp;
    }
  }
}
