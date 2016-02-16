// Decompiled with JetBrains decompiler
// Type: Meta.RenderingCameraManagerBase
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class RenderingCameraManagerBase : MetaSingleton<RenderingCameraManagerBase>
  {
    private CameraType _cameraMode = CameraType.Stereo;
    [Range(0.0f, 100f)]
    private float m_screenInteraxialDistance = 60f;
    [SerializeField]
    internal Vector3 m_leftEyeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField]
    internal Vector3 m_rightEyeOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Range(-30f, 30f)]
    private float m_sensorScreenAngle = 12f;
    private float m_customAspectRatio = 1f;
    [Range(0.01f, 0.03f)]
    private float m_screenWidth = 7.0 / 400.0;
    [Range(0.01f, 0.03f)]
    private float m_screenHeight = 0.015f;
    private bool _allowRealTimePoiUpdate = true;
    private Vector3 _pointOfInterest = Vector3.get_forward();
    [Range(0.0f, 100f)]
    private float m_eyeInteraxialDistance = 64f;
    [Range(0.0f, 0.04f)]
    private float m_eyeballRadius = 0.024f;
    [Range(0.0f, 0.1f)]
    private float m_nearPlaneDistance = 0.02f;
    [Range(0.01f, 50f)]
    private float m_farPlaneDistance = 30f;
    private Vector4 m_leftCustomRect = new Vector4(0.0f, 0.0f, 0.5f, 1f);
    private Vector4 m_rightCustomRect = new Vector4(0.5f, 0.0f, 1f, 1f);
    internal bool m_useExperimentalRendering = true;
    private float widescaleHNearScaling = 0.3f;
    private float widescaleHFarScaling = 0.4f;
    private DeviceSettings[] m_AvailableCameraProfiles;
    private bool m_UseProfileLoader;
    private DeviceSettings _cameraProfile;
    private Vector3 m_centerPosition;
    private Vector3 m_centerRotation;
    private Transform m_metaDepthSensor;
    private GameObject m_mainCamera;
    private GameObject m_leftCamera;
    private GameObject m_rightCamera;
    private Camera m_mainHUDCamera;
    private Camera m_leftHUDCamera;
    private Camera m_rightHUDCamera;
    private Camera m_mainLoadingCamera;
    private Camera m_leftLoadingCamera;
    private Camera m_rightLoadingCamera;
    private DeviceType _device;
    [Range(-20f, 20f)]
    private float m_horizontalScreenOffset;
    [Range(-20f, 20f)]
    private float m_verticalScreenOffset;
    private Vector3 m_sensorScreenOffset;
    private bool m_useCustomAspectRatio;
    private bool _allowRealTimeSettingsUpdate;
    private bool _allowKeyboardControl;
    private bool _allowExternalVergenceControl;
    private LayerMask _selectedLayersForMainCamera;
    private float _hNear;
    private float _hFar;
    private float _xNearLeft;
    private float _xFarLeft;
    private float _physicalSize;
    private float _screenWidth;
    private float _screenHeight;
    private float _physicalSpaceBetween;
    private float _worldNearDepth;
    private float _desiredNearPoint;
    private float _farPlaneDistance;
    private float _xNearRightOffset;
    private float _xFarRightOffset;
    private float _yNearLeft;
    private float _yFarLeft;
    private float _yNearRight;
    private float _yFarRight;
    [SerializeField]
    internal RenderingSettings m_renderingProfile;
    [SerializeField]
    internal bool fovExpanded;
    [SerializeField]
    internal bool widescaleMode;
    private GameObject _objectOfInterestTest;
    [SerializeField]
    internal GameObject _objectOfInterestTestPrev;

    protected Vector3 CenterPosition
    {
      get
      {
        return this.m_centerPosition;
      }
    }

    protected Vector3 CenterRotation
    {
      get
      {
        return this.m_centerRotation;
      }
    }

    protected DeviceSettings CameraProfile
    {
      get
      {
        return this._cameraProfile;
      }
      set
      {
        this._cameraProfile = value;
      }
    }

    internal DeviceType Device
    {
      get
      {
        return this._device;
      }
    }

    protected bool AllowRealTimeSettingsUpdate
    {
      get
      {
        return this._allowRealTimeSettingsUpdate;
      }
      set
      {
        this._allowRealTimeSettingsUpdate = value;
      }
    }

    protected bool AllowRealTimePoiUpdate
    {
      get
      {
        return this._allowRealTimePoiUpdate;
      }
      set
      {
        this._allowRealTimePoiUpdate = value;
      }
    }

    protected bool AllowKeyboardControl
    {
      get
      {
        return this._allowKeyboardControl;
      }
      set
      {
        this._allowKeyboardControl = value;
      }
    }

    protected bool AllowExternalVergenceControl
    {
      get
      {
        return this._allowExternalVergenceControl;
      }
      set
      {
        this._allowExternalVergenceControl = value;
      }
    }

    protected Vector3 PointOfInterest
    {
      get
      {
        return this._pointOfInterest;
      }
      set
      {
        this._pointOfInterest = value;
      }
    }

    internal CameraType CameraMode
    {
      get
      {
        return this._cameraMode;
      }
      set
      {
        this._cameraMode = value;
      }
    }

    internal float CalibrationValue
    {
      set
      {
        if ((double) value > 0.375)
          value = 0.375f;
        if ((double) value < -0.25)
          value = -0.25f;
        this.m_renderingProfile.m_xNearLeft = (float) (250.0 + (double) value * 1.0 * 400.0);
        this.m_renderingProfile.m_xFarLeft = (float) (126.599998474121 + (double) value * 0.558229982852936 * 400.0);
      }
    }

    internal void TranslateDisplayTo(float x, float y)
    {
      this.m_renderingProfile.m_yNearRight = y;
      this.m_renderingProfile.m_yFarRight = y;
      this.m_renderingProfile.m_yNearLeft = y;
      this.m_renderingProfile.m_yFarLeft = y;
      this.m_renderingProfile.m_xNearRightOffset = x;
      this.m_renderingProfile.m_xFarRightOffset = x;
    }

    internal void SaveRenderingProfile()
    {
      if (Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance, (Object) null) && Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile, (Object) null))
      {
        MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile.m_renderingProfile = this.m_renderingProfile;
        if (MetaSingleton<MetaUserSettingsManager>.Instance.SaveUserProfile())
          Debug.Log((object) "Rendering profile saved to user profile.");
        else
          Debug.Log((object) "Rendering profile could not saved.");
      }
      else
        Debug.Log((object) "MetaUserSettingsManager not present.");
    }

    protected virtual void Start()
    {
      this.LoadSettings();
      this.InitStereoCameras();
      this.SetupCameraAspectRatio();
      this.UpdateCameraMatrices();
      this._selectedLayersForMainCamera = LayerMask.op_Implicit(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_cullingMask());
    }

    protected virtual void Update()
    {
      if (this._allowRealTimeSettingsUpdate)
        this.LoadVariablesFromProfile();
      if (this._allowKeyboardControl)
        this.HandleKeyboardInput();
      if (this._cameraMode == CameraType.Monocular)
        this.SetMonocularMode();
      else if (this._cameraMode == CameraType.Stereo)
      {
        this.SetStereoMode();
        if (this._allowRealTimePoiUpdate)
        {
          if (this.m_useExperimentalRendering)
            this.SetupLocalDeviceTransformationsFromDesires();
          else
            this.SetupLocalDeviceTransformations();
          this.UpdateCameraMatrices();
        }
      }
      this.UpdatePublicAPI();
      this.ObjectOfInterest();
    }

    private void LoadVariablesFromProfile()
    {
      if (!Object.op_Implicit((Object) this._cameraProfile))
        Debug.LogError((object) "No camera profile selected for loading settings.");
      this._device = this._cameraProfile.m_device;
      this.m_screenInteraxialDistance = this._cameraProfile.m_screenInteraxialDistance;
      this.m_horizontalScreenOffset = this._cameraProfile.m_horizontalScreenOffset;
      this.m_verticalScreenOffset = this._cameraProfile.m_verticalScreenOffset;
      this.m_sensorScreenOffset = this._cameraProfile.m_sensorScreenOffset;
      this.m_sensorScreenAngle = this._cameraProfile.m_sensorScreenAngle;
      this.m_useCustomAspectRatio = this._cameraProfile.m_useCustomAspectRatio;
      this.m_customAspectRatio = this._cameraProfile.m_customAspectRatio;
      this.m_screenWidth = this._cameraProfile.m_screenWidth;
      this.m_screenHeight = this._cameraProfile.m_screenHeight;
      this.m_eyeInteraxialDistance = this._cameraProfile.m_eyeInteraxialDistance;
      this.m_eyeballRadius = this._cameraProfile.m_eyeballRadius;
      this.m_nearPlaneDistance = this._cameraProfile.m_nearPlaneDistance;
      this.m_farPlaneDistance = this._cameraProfile.m_farPlaneDistance;
      if (this.m_useExperimentalRendering && Object.op_Equality((Object) this.m_renderingProfile, (Object) null))
      {
        Debug.LogError((object) "No rendering profile for loading settings.");
        this.m_useExperimentalRendering = false;
      }
      else
      {
        if (!this.m_useExperimentalRendering)
          return;
        this._hNear = this.m_renderingProfile.m_hNear;
        this._hFar = this.m_renderingProfile.m_hFar;
        this._xNearLeft = this.m_renderingProfile.m_xNearLeft;
        this._xFarLeft = this.m_renderingProfile.m_xFarLeft;
        this._physicalSize = this.m_renderingProfile.m_physicalSize;
        this._screenWidth = this.m_renderingProfile.m_screenWidth;
        this._screenHeight = this.m_renderingProfile.m_screenHeight;
        this._physicalSpaceBetween = this.m_renderingProfile.m_physicalSpaceBetween;
        this._worldNearDepth = this.m_renderingProfile.m_worldNearDepth;
        this._desiredNearPoint = this.m_renderingProfile.m_desiredNearPoint;
        this._farPlaneDistance = this.m_renderingProfile.m_farPlaneDistance;
        this._xNearRightOffset = this.m_renderingProfile.m_xNearRightOffset;
        this._xFarRightOffset = this.m_renderingProfile.m_xFarRightOffset;
        this._yNearLeft = this.m_renderingProfile.m_yNearLeft;
        this._yFarLeft = this.m_renderingProfile.m_yFarLeft;
        this._yNearRight = this.m_renderingProfile.m_yNearRight;
        this._yFarRight = this.m_renderingProfile.m_yFarRight;
      }
    }

    private void LoadSettings()
    {
      if (this.m_UseProfileLoader)
      {
        if (this.m_AvailableCameraProfiles.Length > 0)
        {
          if (Object.op_Implicit((Object) this.m_AvailableCameraProfiles[0]))
          {
            this._cameraProfile = new DeviceSettings();
            this.m_AvailableCameraProfiles[0].DeepCopyTo(this._cameraProfile);
            this._cameraProfile.m_ProfileName = this._cameraProfile.m_ProfileName + "_modified";
          }
          else
            this._cameraProfile = new DeviceSettings();
        }
        else
          this._cameraProfile = new DeviceSettings();
      }
      this.LoadVariablesFromProfile();
    }

    private void HandleKeyboardInput()
    {
      if (Input.GetKeyDown((KeyCode) 285))
        this._cameraMode = CameraType.Stereo;
      if (!Input.GetKeyDown((KeyCode) 286))
        return;
      this._cameraMode = CameraType.Monocular;
    }

    private void SetStereoMode()
    {
      ((Camera) this.m_mainCamera.GetComponent<Camera>()).set_cullingMask(0);
      ((Component) this.m_mainHUDCamera).get_gameObject().SetActive(false);
      ((Component) this.m_mainLoadingCamera).get_gameObject().SetActive(false);
      this.m_leftCamera.SetActive(true);
      this.m_rightCamera.SetActive(true);
    }

    private void SetMonocularMode()
    {
      ((Camera) this.m_mainCamera.GetComponent<Camera>()).set_cullingMask(LayerMask.op_Implicit(this._selectedLayersForMainCamera));
      ((Component) this.m_mainHUDCamera).get_gameObject().SetActive(true);
      ((Component) this.m_mainLoadingCamera).get_gameObject().SetActive(true);
      this.m_leftCamera.SetActive(false);
      this.m_rightCamera.SetActive(false);
    }

    private Matrix4x4 CalculateProjectionMatrixFromDesires(bool isLeftCam)
    {
      if (this._cameraMode != CameraType.Stereo)
        return ((Camera) ((Component) this).GetComponent<Camera>()).get_projectionMatrix();
      Matrix4x4 projectionMatrix = (Matrix4x4) null;
      Vector3 eyeTransform = (Vector3) null;
      this.GetCameraInformationFromDesires(isLeftCam, ref projectionMatrix, ref eyeTransform);
      return projectionMatrix;
    }

    private void GetCameraInformationPure(bool isLeftCam, ref Matrix4x4 projectionMatrix, ref Vector3 eyeTransform, float xNear, float yNear, float xFar, float yFar, float hNear, float hFar, float sNear, float sFar, float physicalSpaceBetween, float worldNearDepth, float screenWidth, float screenHeight, float desiredNearPoint, float farPlaneDistance)
    {
      float num1 = hNear / hFar;
      float num2 = sNear / hNear;
      float num3 = sFar / hFar;
      float num4 = -xNear * num2;
      float num5 = -yNear * num2;
      float num6 = -xFar * num3;
      float num7 = -yFar * num3;
      float num8 = (float) ((double) sNear * (double) physicalSpaceBetween / ((double) num1 * (double) sFar - (double) sNear));
      float num9 = num8 / physicalSpaceBetween;
      float num10 = num4 + num9 * (num4 - num6);
      float num11 = num5 + num9 * (num5 - num7);
      // ISSUE: explicit reference operation
      ((Vector3) @eyeTransform).\u002Ector(num10, num11, worldNearDepth - num8);
      float num12 = sNear * screenHeight / hNear;
      float num13 = screenWidth / screenHeight * num12;
      float num14 = desiredNearPoint / num8;
      float num15 = num12 * num14;
      float num16 = num13 * num14;
      float num17 = (num4 - num10) * num14;
      float num18 = (num5 - num11) * num14;
      float top = num18 + 0.5f * num15;
      float bottom = num18 - 0.5f * num15;
      float right = num17 + 0.5f * num16;
      float left = num17 - 0.5f * num16;
      projectionMatrix = this.CalculateOffCenterProjectionMatrix(left, right, bottom, top, this._desiredNearPoint, farPlaneDistance);
    }

    private void GetCameraInformationFromDesires(bool isLeftCam, ref Matrix4x4 projectionMatrix, ref Vector3 eyeTransform)
    {
      double num1;
      float num2 = (float) (num1 = 0.0);
      float hNear;
      float hFar;
      float xNear;
      float xFar;
      float yNear;
      float yFar;
      if (!this.fovExpanded)
      {
        hNear = this._hNear;
        hFar = this._hFar;
        if (isLeftCam)
        {
          xNear = this._xNearLeft + this._xNearRightOffset;
          xFar = this._xFarLeft + this._xFarRightOffset;
          yNear = this._yNearLeft;
          yFar = this._yFarLeft;
        }
        else
        {
          xNear = -1f * this._xNearLeft + this._xNearRightOffset;
          xFar = -1f * this._xFarLeft + this._xFarRightOffset;
          yNear = this._yNearRight;
          yFar = this._yFarRight;
        }
      }
      else
      {
        float num3 = 0.7394822f;
        float num4 = -36.3843f;
        hNear = 746f;
        hFar = 408.6f;
        float num5 = hNear / this._hNear;
        float num6 = 0.4761905f;
        float num7 = 5.942857f;
        if (isLeftCam)
        {
          xNear = (float) ((double) this._xNearLeft * (double) num3 + (double) num4 + (double) this._xNearRightOffset * (double) num5);
          xFar = (float) ((double) this._xFarLeft * (double) num6 + (double) num7 + (double) this._xFarRightOffset * (double) num5);
          yNear = this._yNearLeft * num5;
          yFar = this._yFarLeft * num5;
        }
        else
        {
          xNear = (float) (-1.0 * ((double) this._xNearLeft * (double) num3 + (double) num4) + (double) this._xNearRightOffset * (double) num5);
          xFar = (float) (-1.0 * ((double) this._xFarLeft * (double) num6 + (double) num7) + (double) this._xFarRightOffset * (double) num5);
          yNear = this._yNearRight * num5;
          yFar = this._yFarRight * num5;
        }
      }
      if (this.widescaleMode)
      {
        hNear *= this.widescaleHNearScaling;
        hFar *= this.widescaleHFarScaling;
      }
      this.GetCameraInformationPure(isLeftCam, ref projectionMatrix, ref eyeTransform, xNear, yNear, xFar, yFar, hNear, hFar, this._physicalSize, this._physicalSize, this._physicalSpaceBetween, this._worldNearDepth, this._screenWidth, this._screenHeight, this._desiredNearPoint, this._farPlaneDistance);
    }

    private Vector3 GetEyeTransformFromDesires(bool isLeftCam)
    {
      Matrix4x4 projectionMatrix = (Matrix4x4) null;
      Vector3 eyeTransform = (Vector3) null;
      this.GetCameraInformationFromDesires(isLeftCam, ref projectionMatrix, ref eyeTransform);
      return eyeTransform;
    }

    private Matrix4x4 CalculateProjectionMatrixFromSettings(bool isLeftCam)
    {
      if (this._cameraMode != CameraType.Stereo)
        return ((Camera) ((Component) this).GetComponent<Camera>()).get_projectionMatrix();
      float num1 = !this.m_useCustomAspectRatio ? this.m_screenWidth / this.m_screenHeight : this.m_customAspectRatio;
      float num2 = this.m_screenHeight / 2f;
      float num3 = (float) (((double) this.m_eyeInteraxialDistance - (double) this.m_screenInteraxialDistance) / 2000.0);
      float left;
      float right;
      if (isLeftCam)
      {
        Vector3 vector3 = Vector3.op_Subtraction(this._pointOfInterest, new Vector3((float) (-(double) this.m_eyeInteraxialDistance / 2000.0), 0.0f, 0.0f));
        float num4 = (float) (vector3.x * (double) this.m_eyeballRadius / (vector3.z + (double) this.m_eyeballRadius + (double) this.m_nearPlaneDistance));
        left = (float) (-(double) num1 * (double) num2 - (double) num4 + (double) this.m_horizontalScreenOffset / 1000.0) + num3;
        right = (float) ((double) num1 * (double) num2 - (double) num4 + (double) this.m_horizontalScreenOffset / 1000.0) + num3;
      }
      else
      {
        Vector3 vector3 = Vector3.op_Subtraction(this._pointOfInterest, new Vector3(this.m_eyeInteraxialDistance / 2000f, 0.0f, 0.0f));
        float num4 = (float) (vector3.x * (double) this.m_eyeballRadius / (vector3.z + (double) this.m_eyeballRadius + (double) this.m_nearPlaneDistance));
        left = (float) (-(double) num1 * (double) num2 - (double) num4 + (double) this.m_horizontalScreenOffset / 1000.0) - num3;
        right = (float) ((double) num1 * (double) num2 - (double) num4 + (double) this.m_horizontalScreenOffset / 1000.0) - num3;
      }
      float top = num2 + this.m_verticalScreenOffset / 1000f;
      float bottom = (float) (-(double) num2 + (double) this.m_verticalScreenOffset / 1000.0);
      return this.CalculateOffCenterProjectionMatrix(left, right, bottom, top, this.m_nearPlaneDistance, this.m_farPlaneDistance);
    }

    private Matrix4x4 CalculateOffCenterProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
    {
      float num1 = (float) (2.0 * (double) near / ((double) right - (double) left));
      float num2 = (float) (2.0 * (double) near / ((double) top - (double) bottom));
      float num3 = (float) (((double) right + (double) left) / ((double) right - (double) left));
      float num4 = (float) (((double) top + (double) bottom) / ((double) top - (double) bottom));
      float num5 = (float) (-((double) far + (double) near) / ((double) far - (double) near));
      float num6 = (float) (-(2.0 * (double) far * (double) near) / ((double) far - (double) near));
      float num7 = -1f;
      Matrix4x4 matrix4x4 = (Matrix4x4) null;
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(0, 0, num1);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(0, 1, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(0, 2, num3);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(0, 3, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(1, 0, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(1, 1, num2);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(1, 2, num4);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(1, 3, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(2, 0, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(2, 1, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(2, 2, num5);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(2, 3, num6);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(3, 0, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(3, 1, 0.0f);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(3, 2, num7);
      // ISSUE: explicit reference operation
      ((Matrix4x4) @matrix4x4).set_Item(3, 3, 0.0f);
      return matrix4x4;
    }

    private void InitStereoCameras()
    {
      this.AttachCameraGameObjects();
      this.SetupCameraAspectRatio();
      this.SetupCullingMask();
      if (this.m_useExperimentalRendering)
        this.SetupLocalDeviceTransformationsFromDesires();
      else
        this.SetupLocalDeviceTransformations();
    }

    private void AttachCameraGameObjects()
    {
      this.m_mainCamera = TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "MetaCameraMono");
      this.m_leftCamera = TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "StereoCameras/MetaCameraLeft");
      ((Camera) this.m_leftCamera.GetComponent<Camera>()).CopyFrom((Camera) this.m_mainCamera.GetComponent<Camera>());
      this.m_rightCamera = TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "StereoCameras/MetaCameraRight");
      ((Camera) this.m_rightCamera.GetComponent<Camera>()).CopyFrom((Camera) this.m_mainCamera.GetComponent<Camera>());
      this.m_mainHUDCamera = (Camera) TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "MetaCameraMono/HudCamMono").GetComponent<Camera>();
      this.m_leftHUDCamera = (Camera) TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "StereoCameras/MetaCameraLeft/HudCamLeft").GetComponent<Camera>();
      this.m_rightHUDCamera = (Camera) TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "StereoCameras/MetaCameraRight/HudCamRight").GetComponent<Camera>();
      this.m_mainLoadingCamera = (Camera) TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "MetaCameraMono/LoadingCamMono").GetComponent<Camera>();
      this.m_leftLoadingCamera = (Camera) TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "StereoCameras/MetaCameraLeft/LoadingCamLeft").GetComponent<Camera>();
      this.m_rightLoadingCamera = (Camera) TransformExtensions.FindChildGameObjectOrDie(((Component) this).get_transform(), "StereoCameras/MetaCameraRight/LoadingCamRight").GetComponent<Camera>();
      this.m_metaDepthSensor = TransformExtensions.FindChildGameObjectOrDie(((Component) ((Component) ((Component) this).get_transform().get_parent()).get_transform().get_parent()).get_transform(), "MetaDepthSensor").get_transform();
    }

    private void SetupCullingMask()
    {
      M0 component1 = this.m_leftCamera.GetComponent<Camera>();
      int num1 = ((Camera) component1).get_cullingMask() | 2048;
      ((Camera) component1).set_cullingMask(num1);
      M0 component2 = this.m_leftCamera.GetComponent<Camera>();
      int num2 = ((Camera) component2).get_cullingMask() & -4097;
      ((Camera) component2).set_cullingMask(num2);
      M0 component3 = this.m_rightCamera.GetComponent<Camera>();
      int num3 = ((Camera) component3).get_cullingMask() & -2049;
      ((Camera) component3).set_cullingMask(num3);
      M0 component4 = this.m_rightCamera.GetComponent<Camera>();
      int num4 = ((Camera) component4).get_cullingMask() | 4096;
      ((Camera) component4).set_cullingMask(num4);
    }

    private void SetupCameraAspectRatio()
    {
      ((Camera) this.m_leftCamera.GetComponent<Camera>()).set_rect(MetaUtils.Vector4toRect(this.m_leftCustomRect));
      ((Camera) this.m_rightCamera.GetComponent<Camera>()).set_rect(MetaUtils.Vector4toRect(this.m_rightCustomRect));
      this.m_leftCustomRect = MetaUtils.RectToVector4(((Camera) this.m_leftCamera.GetComponent<Camera>()).get_rect());
      this.m_rightCustomRect = MetaUtils.RectToVector4(((Camera) this.m_rightCamera.GetComponent<Camera>()).get_rect());
      this.fixCameraAspect();
      if (Object.op_Implicit((Object) this.m_leftHUDCamera))
      {
        int cullingMask = this.m_leftHUDCamera.get_cullingMask();
        this.m_leftHUDCamera.CopyFrom((Camera) this.m_leftCamera.GetComponent<Camera>());
        this.m_leftHUDCamera.set_cullingMask(cullingMask);
        this.m_leftHUDCamera.set_depth(3f);
        this.m_leftHUDCamera.set_clearFlags((CameraClearFlags) 3);
      }
      if (Object.op_Implicit((Object) this.m_leftLoadingCamera))
      {
        int cullingMask = this.m_leftLoadingCamera.get_cullingMask();
        this.m_leftLoadingCamera.CopyFrom((Camera) this.m_leftCamera.GetComponent<Camera>());
        this.m_leftLoadingCamera.set_cullingMask(cullingMask);
        this.m_leftLoadingCamera.set_depth(4f);
        this.m_leftLoadingCamera.set_clearFlags((CameraClearFlags) 3);
      }
      if (Object.op_Inequality((Object) this.m_rightHUDCamera, (Object) null))
      {
        int cullingMask = this.m_rightHUDCamera.get_cullingMask();
        this.m_rightHUDCamera.CopyFrom((Camera) this.m_rightCamera.GetComponent<Camera>());
        this.m_rightHUDCamera.set_cullingMask(cullingMask);
        this.m_rightHUDCamera.set_depth(3f);
        this.m_rightHUDCamera.set_clearFlags((CameraClearFlags) 3);
      }
      if (!Object.op_Inequality((Object) this.m_rightLoadingCamera, (Object) null))
        return;
      int cullingMask1 = this.m_rightLoadingCamera.get_cullingMask();
      this.m_rightLoadingCamera.CopyFrom((Camera) this.m_rightCamera.GetComponent<Camera>());
      this.m_rightLoadingCamera.set_cullingMask(cullingMask1);
      this.m_rightLoadingCamera.set_depth(3f);
      this.m_rightLoadingCamera.set_clearFlags((CameraClearFlags) 3);
    }

    private void fixCameraAspect()
    {
      ((Camera) this.m_mainCamera.GetComponent<Camera>()).ResetAspect();
      M0 component = this.m_mainCamera.GetComponent<Camera>();
      double num1 = (double) ((Camera) component).get_aspect();
      Rect rect1 = ((Camera) this.m_leftCamera.GetComponent<Camera>()).get_rect();
      // ISSUE: explicit reference operation
      double num2 = (double) ((Rect) @rect1).get_width() * 2.0;
      Rect rect2 = ((Camera) this.m_leftCamera.GetComponent<Camera>()).get_rect();
      // ISSUE: explicit reference operation
      double num3 = (double) ((Rect) @rect2).get_height();
      double num4 = num2 / num3;
      double num5 = num1 * num4;
      ((Camera) component).set_aspect((float) num5);
      ((Camera) this.m_leftCamera.GetComponent<Camera>()).set_aspect(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_aspect());
      ((Camera) this.m_rightCamera.GetComponent<Camera>()).set_aspect(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_aspect());
      ((Camera) ((Component) this.m_leftHUDCamera).GetComponent<Camera>()).set_aspect(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_aspect());
      ((Camera) ((Component) this.m_rightHUDCamera).GetComponent<Camera>()).set_aspect(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_aspect());
      ((Camera) ((Component) this.m_leftLoadingCamera).GetComponent<Camera>()).set_aspect(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_aspect());
      ((Camera) ((Component) this.m_rightLoadingCamera).GetComponent<Camera>()).set_aspect(((Camera) this.m_mainCamera.GetComponent<Camera>()).get_aspect());
    }

    private void SetupLocalDeviceTransformationsFromDesires()
    {
      ((Component) this.m_metaDepthSensor).get_transform().set_localPosition(new Vector3((float) -this.m_sensorScreenOffset.x, (float) -this.m_sensorScreenOffset.y, (float) -this.m_sensorScreenOffset.z));
      ((Component) this.m_metaDepthSensor).get_transform().set_localRotation(Quaternion.Euler(-this.m_sensorScreenAngle, 0.0f, 0.0f));
      this.m_leftCamera.get_transform().set_localPosition(this.GetEyeTransformFromDesires(true));
      this.m_rightCamera.get_transform().set_localPosition(this.GetEyeTransformFromDesires(false));
      this.m_mainCamera.get_transform().set_localPosition(new Vector3(0.0f, 0.0f, -this.m_nearPlaneDistance));
    }

    private void SetupLocalDeviceTransformations()
    {
      ((Component) this.m_metaDepthSensor).get_transform().set_localPosition(new Vector3((float) -this.m_sensorScreenOffset.x, (float) -this.m_sensorScreenOffset.y, (float) -this.m_sensorScreenOffset.z));
      ((Component) this.m_metaDepthSensor).get_transform().set_localRotation(Quaternion.Euler(-this.m_sensorScreenAngle, 0.0f, 0.0f));
      this.m_leftCamera.get_transform().set_localPosition(new Vector3((float) (-(double) this.m_eyeInteraxialDistance / 2000.0), 0.0f, -this.m_nearPlaneDistance));
      this.m_rightCamera.get_transform().set_localPosition(new Vector3(this.m_eyeInteraxialDistance / 2000f, 0.0f, -this.m_nearPlaneDistance));
      this.m_mainCamera.get_transform().set_localPosition(new Vector3(0.0f, 0.0f, -this.m_nearPlaneDistance));
      if (!Vector3.op_Inequality(this.m_leftEyeOffset, Vector3.get_zero()) && !Vector3.op_Inequality(this.m_rightEyeOffset, Vector3.get_zero()))
        return;
      Transform transform1 = this.m_leftCamera.get_transform();
      Vector3 vector3_1 = Vector3.op_Addition(transform1.get_position(), this.m_leftEyeOffset);
      transform1.set_position(vector3_1);
      Transform transform2 = this.m_rightCamera.get_transform();
      Vector3 vector3_2 = Vector3.op_Addition(transform2.get_position(), this.m_rightEyeOffset);
      transform2.set_position(vector3_2);
    }

    private void UpdateCameraMatrices()
    {
      if (this._cameraMode != CameraType.Stereo)
        return;
      this.RegisterProjectionMatrices();
    }

    private void RegisterProjectionMatrices()
    {
      if (this.m_useExperimentalRendering)
      {
        ((Camera) this.m_leftCamera.GetComponent<Camera>()).set_projectionMatrix(this.CalculateProjectionMatrixFromDesires(true));
        ((Camera) this.m_rightCamera.GetComponent<Camera>()).set_projectionMatrix(this.CalculateProjectionMatrixFromDesires(false));
      }
      else
      {
        ((Camera) this.m_leftCamera.GetComponent<Camera>()).set_projectionMatrix(this.CalculateProjectionMatrixFromSettings(true));
        ((Camera) this.m_rightCamera.GetComponent<Camera>()).set_projectionMatrix(this.CalculateProjectionMatrixFromSettings(false));
      }
      if (Object.op_Inequality((Object) this.m_leftHUDCamera, (Object) null) && Object.op_Inequality((Object) this.m_rightHUDCamera, (Object) null))
      {
        this.m_leftHUDCamera.set_projectionMatrix(((Camera) this.m_leftCamera.GetComponent<Camera>()).get_projectionMatrix());
        this.m_rightHUDCamera.set_projectionMatrix(((Camera) this.m_rightCamera.GetComponent<Camera>()).get_projectionMatrix());
      }
      if (!Object.op_Inequality((Object) this.m_leftLoadingCamera, (Object) null) || !Object.op_Inequality((Object) this.m_rightLoadingCamera, (Object) null))
        return;
      this.m_leftLoadingCamera.set_projectionMatrix(((Camera) this.m_leftCamera.GetComponent<Camera>()).get_projectionMatrix());
      this.m_rightLoadingCamera.set_projectionMatrix(((Camera) this.m_rightCamera.GetComponent<Camera>()).get_projectionMatrix());
    }

    private void UpdatePublicAPI()
    {
      this.m_centerPosition = Vector3.op_Division(Vector3.op_Addition(this.m_leftCamera.get_transform().get_position(), this.m_rightCamera.get_transform().get_position()), 2f);
      this.m_centerRotation = new Vector3((float) this.m_leftCamera.get_transform().get_rotation().x, (float) this.m_leftCamera.get_transform().get_rotation().y, (float) this.m_leftCamera.get_transform().get_rotation().z);
    }

    internal GameObject ObjectOfInterest()
    {
      GameObject gameObject = MetaSingleton<Gaze>.Instance.objectOfInterest;
      if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Equality((Object) this._objectOfInterestTestPrev, (Object) null) && Object.op_Equality((Object) this._objectOfInterestTest, (Object) null))
        this._objectOfInterestTest = gameObject;
      else if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) this._objectOfInterestTest, (Object) null) && Object.op_Inequality((Object) this._objectOfInterestTest, (Object) gameObject))
      {
        this._objectOfInterestTestPrev = this._objectOfInterestTest;
        this._objectOfInterestTest = gameObject;
      }
      else if (Object.op_Equality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) this._objectOfInterestTest, (Object) null))
        this._objectOfInterestTestPrev = this._objectOfInterestTest;
      return gameObject;
    }

    internal GameObject GetLastObjectOfInterest()
    {
      return this._objectOfInterestTestPrev;
    }
  }
}
