// Decompiled with JetBrains decompiler
// Type: Meta.MetaCamera
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class MetaCamera : RenderingCameraManagerBase
  {
    [SerializeField]
    private CameraType m_cameraMode = CameraType.Stereo;
    private CameraType _cameraModePrev = CameraType.Stereo;
    [SerializeField]
    private bool m_allowRealTimePoiUpdate = true;
    private bool _allowRealTimePoiUpdatePrev = true;
    [SerializeField]
    private Vector3 m_pointOfInterest = new Vector3(0.0f, 0.0f, 0.45f);
    private Vector3 _pointOfInterestPrev = new Vector3(0.0f, 0.0f, 0.45f);
    [SerializeField]
    private bool m_allowRealTimeSettingsUpdate;
    private bool _allowRealTimeSettingsUpdatePrev;
    [SerializeField]
    private bool m_allowKeyboardControl;
    private bool _allowKeyboardControlPrev;
    [SerializeField]
    private bool m_allowExternalVergenceControl;
    private bool _allowExternalVergenceControlPrev;
    [SerializeField]
    private DeviceType m_device;
    private DeviceType _devicePrev;
    [SerializeField]
    private DeviceSettings m_cameraProfile;
    private DeviceSettings _cameraProfilePrev;
    [SerializeField]
    private GameObject m_objectOfInterest;
    private GameObject _objectOfInterestPrev;

    internal static CameraType GetCameraMode()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.CameraMode;
    }

    internal static void SetCameraMode(CameraType value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.CameraMode = value;
      if (value == CameraType.Monocular)
      {
        ScreenCursor.SetMouseCursorVisibility(true);
        if (MetaSingleton<MetaMouse>.Instance.enableMetaMouse)
          return;
        HandsInputModule.processMouse = true;
      }
      else
      {
        ScreenCursor.SetMouseCursorVisibility(false);
        HandsInputModule.processMouse = false;
      }
    }

    internal static bool GetAllowRealTimeSettingsUpdate()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.AllowRealTimeSettingsUpdate;
    }

    internal static void SetAllowRealTimeSettingsUpdate(bool value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.AllowRealTimeSettingsUpdate = value;
    }

    internal static bool GetAllowRealTimePoiUpdate()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.AllowRealTimePoiUpdate;
    }

    internal static void SetAllowRealTimePoiUpdate(bool value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.AllowRealTimePoiUpdate = value;
    }

    internal static bool GetAllowKeyboardControl()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.AllowKeyboardControl;
    }

    internal static void SetAllowKeyboardControl(bool value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.AllowKeyboardControl = value;
    }

    internal static bool GetAllowExternalVergenceControl()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.AllowExternalVergenceControl;
    }

    internal static void SetAllowExternalVergenceControl(bool value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.AllowExternalVergenceControl = value;
    }

    internal static Vector3 GetPointOfInterest()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.PointOfInterest;
    }

    internal static void SetPointOfInterest(Vector3 value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.PointOfInterest = value;
    }

    internal static DeviceType GetDevice()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.Device;
    }

    internal static DeviceSettings GetCameraProfile()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.CameraProfile;
    }

    internal static void SetCameraProfile(DeviceSettings value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.CameraProfile = value;
    }

    internal static bool GetFOVExpanded()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded;
    }

    internal static void SetFOVExpanded(bool value)
    {
      MetaSingleton<RenderingCameraManagerBase>.Instance.fovExpanded = value;
    }

    internal static GameObject GetObjectOfInterest()
    {
      return MetaSingleton<RenderingCameraManagerBase>.Instance.ObjectOfInterest();
    }

    public static bool IsVisible(Bounds bounds)
    {
      if (MetaCamera.GetCameraMode() == CameraType.Monocular)
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.get_main()), bounds);
      if (!Object.op_Inequality((Object) GameObject.Find("MetaCameraLeft"), (Object) null) || !Object.op_Inequality((Object) GameObject.Find("MetaCameraRight"), (Object) null))
        return false;
      if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes((Camera) GameObject.Find("MetaCameraLeft").GetComponent<Camera>()), bounds))
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes((Camera) GameObject.Find("MetaCameraRight").GetComponent<Camera>()), bounds);
      return true;
    }

    private void InitCameraMode()
    {
      this.CameraMode = this._cameraModePrev = this.m_cameraMode;
    }

    private void UpdateCameraMode()
    {
      if (this._cameraModePrev != this.m_cameraMode)
      {
        this.CameraMode = this._cameraModePrev = this.m_cameraMode;
      }
      else
      {
        if (this.CameraMode == this.m_cameraMode)
          return;
        this._cameraModePrev = this.m_cameraMode = this.CameraMode;
      }
    }

    private void InitAllowRealTimeSettingsUpdate()
    {
      this.AllowRealTimeSettingsUpdate = this._allowRealTimeSettingsUpdatePrev = this.m_allowRealTimeSettingsUpdate;
    }

    private void UpdateAllowRealTimeSettingsUpdate()
    {
      if (this._allowRealTimeSettingsUpdatePrev != this.m_allowRealTimeSettingsUpdate)
      {
        this.AllowRealTimeSettingsUpdate = this._allowRealTimeSettingsUpdatePrev = this.m_allowRealTimeSettingsUpdate;
      }
      else
      {
        if (this.AllowRealTimeSettingsUpdate == this.m_allowRealTimeSettingsUpdate)
          return;
        this._allowRealTimeSettingsUpdatePrev = this.m_allowRealTimeSettingsUpdate = this.AllowRealTimeSettingsUpdate;
      }
    }

    private void InitAllowRealTimePoiUpdate()
    {
      this.AllowRealTimePoiUpdate = this._allowRealTimePoiUpdatePrev = this.m_allowRealTimePoiUpdate;
    }

    private void UpdateAllowRealTimePoiUpdate()
    {
      if (this._allowRealTimePoiUpdatePrev != this.m_allowRealTimePoiUpdate)
      {
        this.AllowRealTimePoiUpdate = this._allowRealTimePoiUpdatePrev = this.m_allowRealTimePoiUpdate;
      }
      else
      {
        if (this.AllowRealTimePoiUpdate == this.m_allowRealTimePoiUpdate)
          return;
        this._allowRealTimePoiUpdatePrev = this.m_allowRealTimePoiUpdate = this.AllowRealTimePoiUpdate;
      }
    }

    private void InitAllowKeyboardControl()
    {
      this.AllowKeyboardControl = this._allowKeyboardControlPrev = this.m_allowKeyboardControl;
    }

    private void UpdateAllowKeyboardControl()
    {
      if (this._allowKeyboardControlPrev != this.m_allowKeyboardControl)
      {
        this.AllowKeyboardControl = this._allowKeyboardControlPrev = this.m_allowKeyboardControl;
      }
      else
      {
        if (this.AllowKeyboardControl == this.m_allowKeyboardControl)
          return;
        this._allowKeyboardControlPrev = this.m_allowKeyboardControl = this.AllowKeyboardControl;
      }
    }

    private void InitAllowExternalVergenceControl()
    {
      this.AllowExternalVergenceControl = this._allowExternalVergenceControlPrev = this.m_allowExternalVergenceControl;
    }

    private void UpdateAllowExternalVergenceControl()
    {
      if (this._allowExternalVergenceControlPrev != this.m_allowExternalVergenceControl)
      {
        this.AllowExternalVergenceControl = this._allowExternalVergenceControlPrev = this.m_allowExternalVergenceControl;
      }
      else
      {
        if (this.AllowExternalVergenceControl == this.m_allowExternalVergenceControl)
          return;
        this._allowExternalVergenceControlPrev = this.m_allowExternalVergenceControl = this.AllowExternalVergenceControl;
      }
    }

    private void InitPointOfInterest()
    {
      this.PointOfInterest = this._pointOfInterestPrev = this.m_pointOfInterest;
    }

    private void UpdatePointOfInterest()
    {
      if (Vector3.op_Inequality(this._pointOfInterestPrev, this.m_pointOfInterest))
      {
        this.PointOfInterest = this._pointOfInterestPrev = this.m_pointOfInterest;
      }
      else
      {
        if (!Vector3.op_Inequality(this.PointOfInterest, this.m_pointOfInterest))
          return;
        this._pointOfInterestPrev = this.m_pointOfInterest = this.PointOfInterest;
      }
    }

    private void InitDevice()
    {
      this._devicePrev = this.m_device = this.Device;
    }

    private void UpdateDevice()
    {
      if (this._devicePrev != this.m_device)
      {
        this._devicePrev = this.m_device = this.Device;
      }
      else
      {
        if (this.Device == this.m_device)
          return;
        this._devicePrev = this.m_device = this.Device;
      }
    }

    private void InitCameraProfile()
    {
      this.CameraProfile = this._cameraProfilePrev = this.m_cameraProfile;
    }

    private void UpdateCameraProfile()
    {
      if (Object.op_Inequality((Object) this._cameraProfilePrev, (Object) this.m_cameraProfile))
      {
        this.CameraProfile = this._cameraProfilePrev = this.m_cameraProfile;
      }
      else
      {
        if (!Object.op_Inequality((Object) this.CameraProfile, (Object) this.m_cameraProfile))
          return;
        this._cameraProfilePrev = this.m_cameraProfile = this.CameraProfile;
      }
    }

    private void InitObjectOfInterest()
    {
      this._objectOfInterestPrev = this.m_objectOfInterest = this.ObjectOfInterest();
    }

    private void UpdateObjectOfInterest()
    {
      if (Object.op_Inequality((Object) this._objectOfInterestPrev, (Object) this.m_objectOfInterest))
      {
        this._objectOfInterestPrev = this.m_objectOfInterest = this.ObjectOfInterest();
      }
      else
      {
        if (!Object.op_Inequality((Object) this.ObjectOfInterest(), (Object) this.m_objectOfInterest))
          return;
        this._objectOfInterestPrev = this.m_objectOfInterest = this.ObjectOfInterest();
      }
    }

    private void InitUserProfileValues()
    {
      if (Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance, (Object) null))
      {
        if (Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true), (Object) null))
        {
          this.m_cameraProfile = MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true);
          this.UpdateCameraProfile();
        }
        this.m_leftEyeOffset = MetaSingleton<MetaUserSettingsManager>.Instance.GetEyeOffset(true);
        this.m_rightEyeOffset = MetaSingleton<MetaUserSettingsManager>.Instance.GetEyeOffset(false);
      }
      this.m_renderingProfile = MetaSingleton<MetaUserSettingsManager>.Instance.GetRenderingProfile();
      this.fovExpanded = MetaSingleton<MetaUserSettingsManager>.Instance.GetFOVSetting();
    }

    protected override void Start()
    {
      this.InitCameraMode();
      this.InitAllowRealTimeSettingsUpdate();
      this.InitAllowRealTimePoiUpdate();
      this.InitAllowKeyboardControl();
      this.InitAllowExternalVergenceControl();
      this.InitPointOfInterest();
      this.InitDevice();
      this.InitCameraProfile();
      this.InitObjectOfInterest();
      this.InitUserProfileValues();
      base.Start();
    }

    protected override void Update()
    {
      this.UpdateCameraMode();
      this.UpdateAllowRealTimeSettingsUpdate();
      this.UpdateAllowRealTimePoiUpdate();
      this.UpdateAllowKeyboardControl();
      this.UpdateAllowExternalVergenceControl();
      this.UpdatePointOfInterest();
      this.UpdateDevice();
      this.UpdateCameraProfile();
      this.UpdateObjectOfInterest();
      base.Update();
    }
  }
}
