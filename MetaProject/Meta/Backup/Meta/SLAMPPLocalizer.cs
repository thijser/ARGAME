// Decompiled with JetBrains decompiler
// Type: Meta.SLAMPPLocalizer
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Meta
{
  internal class SLAMPPLocalizer : Localizer
  {
    private double[] _trans = new double[3];
    private double[] _quat = new double[4];
    private DateTime _timeNotTracking = DateTime.Now;
    public int m_maxNotTracking = 3;
    public bool m_autoRelocalize = true;
    public GameObject gravity_arrow;
    private Quaternion _quaternion;
    private IMUMotionData _imu;
    private Quaternion _imuOrientation;
    private Quaternion _initSlam2World;
    private Quaternion _slam2Gravity;
    [SerializeField]
    private Image slamTimerImage;
    [SerializeField]
    private GameObject slamArrowGO;
    private SLAMPPLocalizer.SLAMLocalizerState _state;
    public bool m_useIMU;

    public SLAMPPLocalizer.SLAMLocalizerState State
    {
      get
      {
        return this._state;
      }
    }

    public bool AreTracking
    {
      get
      {
        return this._state == SLAMPPLocalizer.SLAMLocalizerState.inTracking;
      }
    }

    public int TimeNotTracking
    {
      get
      {
        switch (this._state)
        {
          case SLAMPPLocalizer.SLAMLocalizerState.atStart:
          case SLAMPPLocalizer.SLAMLocalizerState.inTracking:
            return 0;
          default:
            return (DateTime.Now - this._timeNotTracking).Seconds;
        }
      }
    }

    public float TimeNotTrackingFloat
    {
      get
      {
        switch (this._state)
        {
          case SLAMPPLocalizer.SLAMLocalizerState.atStart:
          case SLAMPPLocalizer.SLAMLocalizerState.inTracking:
            return 0.0f;
          default:
            return (float) (DateTime.Now - this._timeNotTracking).TotalSeconds;
        }
      }
    }

    [DllImport("MetaVisionDLL", EntryPoint = "registerSLAM")]
    protected static extern void RegisterSLAM([MarshalAs(UnmanagedType.BStr)] string iSlamName, float iRelThresh);

    [DllImport("MetaVisionDLL", EntryPoint = "enableSLAM")]
    protected static extern void EnableSLAM();

    [DllImport("MetaVisionDLL", EntryPoint = "relocalizeSLAM")]
    protected static extern void RelocalizeSLAM();

    [DllImport("MetaVisionDLL", EntryPoint = "resetSLAM")]
    protected static extern void ResetSLAM();

    [DllImport("MetaVisionDLL", EntryPoint = "cameraSLAMMetaWorldToCamera")]
    protected static extern int CameraSlamMetaWorldToCamera([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

    [DllImport("MetaVisionDLL", EntryPoint = "cameraSLAMToMetaWorld")]
    protected static extern int CameraSlamToMetaWorld([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

    [DllImport("MetaVisionDLL", EntryPoint = "deltaSLAMMetaWorldCamera")]
    protected static extern int DeltaSlamMetaWorldCamera([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

    [DllImport("MetaVisionDLL", EntryPoint = "deltaSLAMMetaWorld")]
    protected static extern int DeltaSlamMetaWorld([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

    public void UpdateSLAMTimer()
    {
      if (this._state == SLAMPPLocalizer.SLAMLocalizerState.inRelocalization)
      {
        ((Behaviour) this.slamTimerImage).set_enabled(true);
        this.slamTimerImage.set_fillAmount(this.TimeNotTrackingFloat / (float) (this.m_maxNotTracking + 1));
        if (this.slamArrowGO.get_activeSelf())
          return;
        this.slamArrowGO.SetActive(true);
      }
      else
      {
        ((Behaviour) this.slamTimerImage).set_enabled(false);
        if (!this.slamArrowGO.get_activeSelf())
          return;
        this.slamArrowGO.SetActive(false);
      }
    }

    private void Start()
    {
      Debug.Log((object) "Starting SLAM");
      SLAMPPLocalizer.RegisterSLAM("slampp", 0.1f);
      this._imuOrientation = Quaternion.get_identity();
      if (!this.m_useIMU)
        return;
      this._imu = new IMUMotionData();
    }

    public void Update()
    {
      if (Object.op_Equality((Object) this._targetGO, (Object) null))
        this.SetDefaultTargetGO();
      this.UpdateTargetGOTransform();
      this.UpdateSLAMTimer();
    }

    public static bool StateInRelocalization(SLAMPPLocalizer.SLAMLocalizerState iState)
    {
      if (iState != SLAMPPLocalizer.SLAMLocalizerState.inRelocalization)
        return iState == SLAMPPLocalizer.SLAMLocalizerState.inError;
      return true;
    }

    public static bool StateAtStart(SLAMPPLocalizer.SLAMLocalizerState iState)
    {
      if (iState != SLAMPPLocalizer.SLAMLocalizerState.atStart)
        return iState == SLAMPPLocalizer.SLAMLocalizerState.waitIMU;
      return true;
    }

    private void StateTransition(SLAMPPLocalizer.SLAMLocalizerState iNewState)
    {
      bool flag = SLAMPPLocalizer.StateInRelocalization(iNewState);
      if (this._state != iNewState && SLAMPPLocalizer.StateInRelocalization(this._state) != flag && flag)
      {
        if (SLAMPPLocalizer.StateAtStart(this._state))
          return;
        Debug.Log((object) "Relocalizing SLAM");
        this._timeNotTracking = DateTime.Now;
      }
      if (this.m_autoRelocalize && flag && this.TimeNotTracking > this.m_maxNotTracking)
      {
        this.ResetLocalizer();
      }
      else
      {
        SLAMPPLocalizer.SLAMLocalizerState iState;
        if (SLAMPPLocalizer.StateAtStart(this._state) && !flag)
        {
          if (this._state == SLAMPPLocalizer.SLAMLocalizerState.waitIMU)
          {
            iState = this.LatchIMU(iNewState);
          }
          else
          {
            iState = this.LatchCurrent(iNewState);
            if (SLAMPPLocalizer.StateInRelocalization(iState))
              this._timeNotTracking = DateTime.Now;
          }
        }
        else
          iState = iNewState;
        this._state = iState;
      }
    }

    internal SLAMPPLocalizer.SLAMLocalizerState LatchCurrent(SLAMPPLocalizer.SLAMLocalizerState iReqState)
    {
      if (!IMUMotionData.isQuaternionValid(this._quaternion))
        return SLAMPPLocalizer.SLAMLocalizerState.inError;
      SLAMPPLocalizer.SLAMLocalizerState slamLocalizerState = iReqState;
      if (this._imu != null)
        slamLocalizerState = this.LatchIMU(iReqState);
      return slamLocalizerState;
    }

    public SLAMPPLocalizer.SLAMLocalizerState LatchIMU(SLAMPPLocalizer.SLAMLocalizerState iReqState)
    {
      Quaternion identity = Quaternion.get_identity();
      if (!this._imu.LatchIMU(ref identity))
        return SLAMPPLocalizer.SLAMLocalizerState.waitIMU;
      this._imuOrientation = identity;
      this._slam2Gravity = Quaternion.Inverse(this._imuOrientation);
      if (Object.op_Inequality((Object) this.gravity_arrow, (Object) null))
        this.gravity_arrow.get_transform().set_rotation(this._imuOrientation);
      return iReqState;
    }

    public void UpdateTargetGOTransform()
    {
      if (this._imu != null)
        this._imu.Update();
      switch (SLAMPPLocalizer.CameraSlamToMetaWorld(this._trans, this._quat))
      {
        case 0:
          this._quaternion = this.From();
          if (this._state != SLAMPPLocalizer.SLAMLocalizerState.waitIMU)
            this._targetGO.get_transform().set_position(new Vector3((float) this._trans[0], (float) this._trans[1], (float) this._trans[2]));
          this.slamArrowGO.get_transform().set_position(Vector3.op_UnaryNegation(this._targetGO.get_transform().get_position()));
          this.StateTransition(SLAMPPLocalizer.SLAMLocalizerState.inTracking);
          this._targetGO.get_transform().set_rotation(Quaternion.op_Multiply(this._slam2Gravity, this._quaternion));
          Vector3 smoothedGravity = this._imu.SmoothedGravity;
          // ISSUE: explicit reference operation
          ((Vector3) @smoothedGravity).Normalize();
          Debug.DrawLine(this._targetGO.get_transform().get_position(), Vector3.op_Multiply(10f, smoothedGravity), Color.get_green());
          break;
        case -1:
          this._targetGO.get_transform().set_rotation(this._imu.Compute());
          this.StateTransition(SLAMPPLocalizer.SLAMLocalizerState.inRelocalization);
          break;
        default:
          this._targetGO.get_transform().set_rotation(this._imu.Compute());
          this.StateTransition(SLAMPPLocalizer.SLAMLocalizerState.inError);
          break;
      }
    }

    public override void ResetLocalizer()
    {
      Debug.Log((object) "Reseting SLAM");
      SLAMPPLocalizer.ResetSLAM();
      this._imu.Reset();
      this._state = SLAMPPLocalizer.SLAMLocalizerState.atStart;
    }

    internal Quaternion From()
    {
      return SLAMPPLocalizer.From(this._quat);
    }

    internal static Quaternion From(double[] iQuat)
    {
      Quaternion quaternion;
      quaternion.x = (__Null) iQuat[0];
      quaternion.y = (__Null) iQuat[1];
      quaternion.z = (__Null) iQuat[2];
      quaternion.w = (__Null) iQuat[3];
      return quaternion;
    }

    public enum SLAMLocalizerState
    {
      atStart,
      waitIMU,
      inError,
      inTracking,
      inRelocalization,
    }
  }
}
