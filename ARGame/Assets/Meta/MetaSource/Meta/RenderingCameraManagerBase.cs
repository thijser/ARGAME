using System;
using UnityEngine;

namespace Meta
{
	public class RenderingCameraManagerBase : MetaSingleton<RenderingCameraManagerBase>
	{
		private DeviceSettings[] m_AvailableCameraProfiles;

		private bool m_UseProfileLoader;

		private DeviceSettings _CameraProfile;

		private Vector3 m_centerPosition;

		private Vector3 m_centerRotation;

		private Transform m_metaDepthSensor;

		private GameObject m_mainCamera;

		private GameObject m_leftCamera;

		private GameObject m_rightCamera;

		private UnityEngine.Camera m_mainHUDCamera;

		private UnityEngine.Camera m_leftHUDCamera;

		private UnityEngine.Camera m_rightHUDCamera;

		private UnityEngine.Camera m_mainLoadingCamera;

		private UnityEngine.Camera m_leftLoadingCamera;

		private UnityEngine.Camera m_rightLoadingCamera;

		private DeviceType _device;

		private CameraType _CameraMode = CameraType.Stereo;

		[Range(0f, 100f)]
		private float m_screenInteraxialDistance = 60f;

		[Range(-20f, 20f)]
		private float m_horizontalScreenOffset;

		[Range(-20f, 20f)]
		private float m_verticalScreenOffset;

		[SerializeField]
		internal Vector3 m_leftEyeOffset = new Vector3(0f, 0f, 0f);

		[SerializeField]
		internal Vector3 m_rightEyeOffset = new Vector3(0f, 0f, 0f);

		private Vector3 m_sensorScreenOffset;

		[Range(-30f, 30f)]
		private float m_sensorScreenAngle = 12f;

		private bool m_useCustomAspectRatio;

		private float m_customAspectRatio = 1f;

		[Range(0.01f, 0.03f)]
		private float m_screenWidth = 0.0175f;

		[Range(0.01f, 0.03f)]
		private float m_screenHeight = 0.015f;

		private bool _allowRealTimeSettingsUpdate;

		private bool _allowRealTimePoiUpdate = true;

		private bool _allowKeyboardControl;

		private bool _allowExternalVergenceControl;

		private Vector3 _pointOfInterest = Vector3.forward;

		[Range(0f, 100f)]
		private float m_eyeInteraxialDistance = 64f;

		[Range(0f, 0.04f)]
		private float m_eyeballRadius = 0.024f;

		[Range(0f, 0.1f)]
		private float m_nearPlaneDistance = 0.02f;

		[Range(0.01f, 50f)]
		private float m_farPlaneDistance = 30f;

		private Vector4 m_leftCustomRect = new Vector4(0f, 0f, 0.5f, 1f);

		private Vector4 m_rightCustomRect = new Vector4(0.5f, 0f, 1f, 1f);

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

		internal bool m_useExperimentalRendering = true;

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

		private float widescaleHNearScaling = 0.3f;

		private float widescaleHFarScaling = 0.4f;

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
				return this._CameraProfile;
			}
			set
			{
				this._CameraProfile = value;
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
				return this._CameraMode;
			}
			set
			{
				this._CameraMode = value;
			}
		}

		internal float CalibrationValue
		{
			set
			{
				if (value > 0.375f)
				{
					value = 0.375f;
				}
				if (value < -0.25f)
				{
					value = -0.25f;
				}
				this.m_renderingProfile.m_xNearLeft = 250f + value * 1f * 400f;
				this.m_renderingProfile.m_xFarLeft = 126.6f + value * 0.55823f * 400f;
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
			if (MetaSingleton<MetaUserSettingsManager>.Instance != null && MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile != null)
			{
				MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile.m_renderingProfile = this.m_renderingProfile;
				if (MetaSingleton<MetaUserSettingsManager>.Instance.SaveUserProfile())
				{
					Debug.Log("Rendering profile saved to user profile.");
				}
				else
				{
					Debug.Log("Rendering profile could not saved.");
				}
			}
			else
			{
				Debug.Log("MetaUserSettingsManager not present.");
			}
		}

		protected virtual void Start()
		{
			this.LoadSettings();
			this.InitStereoCameras();
			this.SetupCameraAspectRatio();
			this.UpdateCameraMatrices();
			this._selectedLayersForMainCamera = this.m_mainCamera.GetComponent<UnityEngine.Camera>().cullingMask;
		}

		protected virtual void Update()
		{
			if (this._allowRealTimeSettingsUpdate)
			{
				this.LoadVariablesFromProfile();
			}
			if (this._allowKeyboardControl)
			{
				this.HandleKeyboardInput();
			}
			if (this._CameraMode == CameraType.Monocular)
			{
				this.SetMonocularMode();
			}
			else if (this._CameraMode == CameraType.Stereo)
			{
				this.SetStereoMode();
				if (this._allowRealTimePoiUpdate)
				{
					if (this.m_useExperimentalRendering)
					{
						this.SetupLocalDeviceTransformationsFromDesires();
					}
					else
					{
						this.SetupLocalDeviceTransformations();
					}
					this.UpdateCameraMatrices();
				}
			}
			this.UpdatePublicAPI();
			this.ObjectOfInterest();
		}

		private void LoadVariablesFromProfile()
		{
			if (!this._CameraProfile)
			{
				Debug.LogError("No UnityEngine.Camera profile selected for loading settings.");
			}
			this._device = this._CameraProfile.m_device;
			this.m_screenInteraxialDistance = this._CameraProfile.m_screenInteraxialDistance;
			this.m_horizontalScreenOffset = this._CameraProfile.m_horizontalScreenOffset;
			this.m_verticalScreenOffset = this._CameraProfile.m_verticalScreenOffset;
			this.m_sensorScreenOffset = this._CameraProfile.m_sensorScreenOffset;
			this.m_sensorScreenAngle = this._CameraProfile.m_sensorScreenAngle;
			this.m_useCustomAspectRatio = this._CameraProfile.m_useCustomAspectRatio;
			this.m_customAspectRatio = this._CameraProfile.m_customAspectRatio;
			this.m_screenWidth = this._CameraProfile.m_screenWidth;
			this.m_screenHeight = this._CameraProfile.m_screenHeight;
			this.m_eyeInteraxialDistance = this._CameraProfile.m_eyeInteraxialDistance;
			this.m_eyeballRadius = this._CameraProfile.m_eyeballRadius;
			this.m_nearPlaneDistance = this._CameraProfile.m_nearPlaneDistance;
			this.m_farPlaneDistance = this._CameraProfile.m_farPlaneDistance;
			if (this.m_useExperimentalRendering && this.m_renderingProfile == null)
			{
				Debug.LogError("No rendering profile for loading settings.");
				this.m_useExperimentalRendering = false;
			}
			else if (this.m_useExperimentalRendering)
			{
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
					if (this.m_AvailableCameraProfiles[0])
					{
						this._CameraProfile = new DeviceSettings();
						this.m_AvailableCameraProfiles[0].DeepCopyTo(this._CameraProfile);
						this._CameraProfile.m_ProfileName = this._CameraProfile.m_ProfileName + "_modified";
					}
					else
					{
						this._CameraProfile = new DeviceSettings();
					}
				}
				else
				{
					this._CameraProfile = new DeviceSettings();
				}
			}
			this.LoadVariablesFromProfile();
		}

		private void HandleKeyboardInput()
		{
			if (Input.GetKeyDown((KeyCode)285))
			{
				this._CameraMode = CameraType.Stereo;
			}
			if (Input.GetKeyDown((KeyCode)286))
			{
				this._CameraMode = CameraType.Monocular;
			}
		}

		private void SetStereoMode()
		{
			this.m_mainCamera.GetComponent<UnityEngine.Camera>().cullingMask = 0;
			this.m_mainHUDCamera.gameObject.SetActive(false);
			this.m_mainLoadingCamera.gameObject.SetActive(false);
			this.m_leftCamera.SetActive(true);
			this.m_rightCamera.SetActive(true);
		}

		private void SetMonocularMode()
		{
			this.m_mainCamera.GetComponent<UnityEngine.Camera>().cullingMask = this._selectedLayersForMainCamera;
			this.m_mainHUDCamera.gameObject.SetActive(true);
			this.m_mainLoadingCamera.gameObject.SetActive(true);
			this.m_leftCamera.SetActive(false);
			this.m_rightCamera.SetActive(false);
		}

		private Matrix4x4 CalculateProjectionMatrixFromDesires(bool isLeftCam)
		{
			if (this._CameraMode == CameraType.Stereo)
			{
				Matrix4x4 result = default(Matrix4x4);
				Vector3 vector = default(Vector3);
				this.GetCameraInformationFromDesires(isLeftCam, ref result, ref vector);
				return result;
			}
			return base.GetComponent<UnityEngine.Camera>().projectionMatrix;
		}

		private void GetCameraInformationPure(bool isLeftCam, ref Matrix4x4 projectionMatrix, ref Vector3 eyeTransform, float xNear, float yNear, float xFar, float yFar, float hNear, float hFar, float sNear, float sFar, float physicalSpaceBetween, float worldNearDepth, float screenWidth, float screenHeight, float desiredNearPoint, float farPlaneDistance)
		{
			float num = hNear / hFar;
			float num2 = sNear / hNear;
			float num3 = sFar / hFar;
			float num4 = -xNear * num2;
			float num5 = -yNear * num2;
			float num6 = -xFar * num3;
			float num7 = -yFar * num3;
			float num8 = sNear * physicalSpaceBetween / (num * sFar - sNear);
			float num9 = num8 / physicalSpaceBetween;
			float num10 = num4 + num9 * (num4 - num6);
			float num11 = num5 + num9 * (num5 - num7);
			eyeTransform = new Vector3(num10, num11, worldNearDepth - num8);
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
			float num;
			float num2;
			float xNear;
			float xFar;
			float yNear;
			float yFar;
			if (!this.fovExpanded)
			{
				num = this._hNear;
				num2 = this._hFar;
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
				float num4 = -36.384304f;
				num = 746f;
				num2 = 408.6f;
				float num5 = num / this._hNear;
				float num6 = 0.476190478f;
				float num7 = 5.94285727f;
				if (isLeftCam)
				{
					xNear = this._xNearLeft * num3 + num4 + this._xNearRightOffset * num5;
					xFar = this._xFarLeft * num6 + num7 + this._xFarRightOffset * num5;
					yNear = this._yNearLeft * num5;
					yFar = this._yFarLeft * num5;
				}
				else
				{
					xNear = -1f * (this._xNearLeft * num3 + num4) + this._xNearRightOffset * num5;
					xFar = -1f * (this._xFarLeft * num6 + num7) + this._xFarRightOffset * num5;
					yNear = this._yNearRight * num5;
					yFar = this._yFarRight * num5;
				}
			}
			if (this.widescaleMode)
			{
				num *= this.widescaleHNearScaling;
				num2 *= this.widescaleHFarScaling;
			}
			this.GetCameraInformationPure(isLeftCam, ref projectionMatrix, ref eyeTransform, xNear, yNear, xFar, yFar, num, num2, this._physicalSize, this._physicalSize, this._physicalSpaceBetween, this._worldNearDepth, this._screenWidth, this._screenHeight, this._desiredNearPoint, this._farPlaneDistance);
		}

		private Vector3 GetEyeTransformFromDesires(bool isLeftCam)
		{
			Matrix4x4 matrix4x = default(Matrix4x4);
			Vector3 result = default(Vector3);
			this.GetCameraInformationFromDesires(isLeftCam, ref matrix4x, ref result);
			return result;
		}

		private Matrix4x4 CalculateProjectionMatrixFromSettings(bool isLeftCam)
		{
			if (this._CameraMode == CameraType.Stereo)
			{
				float num = (!this.m_useCustomAspectRatio) ? (this.m_screenWidth / this.m_screenHeight) : this.m_customAspectRatio;
				float num2 = this.m_screenHeight / 2f;
				float num3 = (this.m_eyeInteraxialDistance - this.m_screenInteraxialDistance) / 2000f;
				Vector3 vector = default(Vector3);
				float left;
				float right;
				if (isLeftCam)
				{
					vector = this._pointOfInterest - new Vector3(-this.m_eyeInteraxialDistance / 2000f, 0f, 0f);
					float num4 = vector.x * this.m_eyeballRadius / (vector.z + this.m_eyeballRadius + this.m_nearPlaneDistance);
					left = -num * num2 - num4 + this.m_horizontalScreenOffset / 1000f + num3;
					right = num * num2 - num4 + this.m_horizontalScreenOffset / 1000f + num3;
				}
				else
				{
					vector = this._pointOfInterest - new Vector3(this.m_eyeInteraxialDistance / 2000f, 0f, 0f);
					float num5 = vector.x * this.m_eyeballRadius / (vector.z + this.m_eyeballRadius + this.m_nearPlaneDistance);
					left = -num * num2 - num5 + this.m_horizontalScreenOffset / 1000f - num3;
					right = num * num2 - num5 + this.m_horizontalScreenOffset / 1000f - num3;
				}
				float top = num2 + this.m_verticalScreenOffset / 1000f;
				float bottom = -num2 + this.m_verticalScreenOffset / 1000f;
				return this.CalculateOffCenterProjectionMatrix(left, right, bottom, top, this.m_nearPlaneDistance, this.m_farPlaneDistance);
			}
			return base.GetComponent<UnityEngine.Camera>().projectionMatrix;
		}

		private Matrix4x4 CalculateOffCenterProjectionMatrix(float left, float right, float bottom, float top, float near, float far)
		{
			float num = 2f * near / (right - left);
			float num2 = 2f * near / (top - bottom);
			float num3 = (right + left) / (right - left);
			float num4 = (top + bottom) / (top - bottom);
			float num5 = -(far + near) / (far - near);
			float num6 = -(2f * far * near) / (far - near);
			float num7 = -1f;
			Matrix4x4 result = default(Matrix4x4);
			result[0, 0] = num;
			result[0, 1] = 0f;
			result[0, 2] = num3;
			result[0, 3] = 0f;
			result[1, 0] = 0f;
			result[1, 1] = num2;
			result[1, 2] = num4;
			result[1, 3] = 0f;
			result[2, 0] = 0f;
			result[2, 1] = 0f;
			result[2, 2] = num5;
			result[2, 3] = num6;
			result[3, 0] = 0f;
			result[3, 1] = 0f;
			result[3, 2] = num7;
			result[3, 3] = 0f;
			return result;
		}

		private void InitStereoCameras()
		{
			this.AttachCameraGameObjects();
			this.SetupCameraAspectRatio();
			this.SetupCullingMask();
			if (this.m_useExperimentalRendering)
			{
				this.SetupLocalDeviceTransformationsFromDesires();
			}
			else
			{
				this.SetupLocalDeviceTransformations();
			}
		}

		private void AttachCameraGameObjects()
		{
			this.m_mainCamera = base.transform.FindChildGameObjectOrDie("MetaCameraMono");
			this.m_leftCamera = base.transform.FindChildGameObjectOrDie("StereoCameras/MetaCameraLeft");
			this.m_leftCamera.GetComponent<UnityEngine.Camera>().CopyFrom(this.m_mainCamera.GetComponent<UnityEngine.Camera>());
			this.m_rightCamera = base.transform.FindChildGameObjectOrDie("StereoCameras/MetaCameraRight");
			this.m_rightCamera.GetComponent<UnityEngine.Camera>().CopyFrom(this.m_mainCamera.GetComponent<UnityEngine.Camera>());
			this.m_mainHUDCamera = base.transform.FindChildGameObjectOrDie("MetaCameraMono/HudCamMono").GetComponent<UnityEngine.Camera>();
			this.m_leftHUDCamera = base.transform.FindChildGameObjectOrDie("StereoCameras/MetaCameraLeft/HudCamLeft").GetComponent<UnityEngine.Camera>();
			this.m_rightHUDCamera = base.transform.FindChildGameObjectOrDie("StereoCameras/MetaCameraRight/HudCamRight").GetComponent<UnityEngine.Camera>();
			this.m_mainLoadingCamera = base.transform.FindChildGameObjectOrDie("MetaCameraMono/LoadingCamMono").GetComponent<UnityEngine.Camera>();
			this.m_leftLoadingCamera = base.transform.FindChildGameObjectOrDie("StereoCameras/MetaCameraLeft/LoadingCamLeft").GetComponent<UnityEngine.Camera>();
			this.m_rightLoadingCamera = base.transform.FindChildGameObjectOrDie("StereoCameras/MetaCameraRight/LoadingCamRight").GetComponent<UnityEngine.Camera>();
			this.m_metaDepthSensor = base.transform.parent.transform.parent.transform.FindChildGameObjectOrDie("MetaDepthSensor").transform;
		}

		private void SetupCullingMask()
		{
			UnityEngine.Camera expr_0B = this.m_leftCamera.GetComponent<UnityEngine.Camera>();
			expr_0B.cullingMask = expr_0B.cullingMask | 2048;
			UnityEngine.Camera expr_27 = this.m_leftCamera.GetComponent<UnityEngine.Camera>();
			expr_27.cullingMask = expr_27.cullingMask & -4097;
			UnityEngine.Camera expr_43 = this.m_rightCamera.GetComponent<UnityEngine.Camera>();
			expr_43.cullingMask = expr_43.cullingMask & -2049;
			UnityEngine.Camera expr_5F = this.m_rightCamera.GetComponent<UnityEngine.Camera>();
			expr_5F.cullingMask = expr_5F.cullingMask | 4096;
		}

		private void SetupCameraAspectRatio()
		{
			this.m_leftCamera.GetComponent<UnityEngine.Camera>().rect = MetaUtils.Vector4toRect(this.m_leftCustomRect);
			this.m_rightCamera.GetComponent<UnityEngine.Camera>().rect = MetaUtils.Vector4toRect(this.m_rightCustomRect);
			this.m_leftCustomRect = MetaUtils.RectToVector4(this.m_leftCamera.GetComponent<UnityEngine.Camera>().rect);
			this.m_rightCustomRect = MetaUtils.RectToVector4(this.m_rightCamera.GetComponent<UnityEngine.Camera>().rect);
			this.fixCameraAspect();
			if (this.m_leftHUDCamera)
			{
				int cullingMask = this.m_leftHUDCamera.cullingMask;
				this.m_leftHUDCamera.CopyFrom(this.m_leftCamera.GetComponent<UnityEngine.Camera>());
				this.m_leftHUDCamera.cullingMask = cullingMask;
				this.m_leftHUDCamera.depth = 3f;
			}
			if (this.m_leftLoadingCamera)
			{
				int cullingMask2 = this.m_leftLoadingCamera.cullingMask;
				this.m_leftLoadingCamera.CopyFrom(this.m_leftCamera.GetComponent<UnityEngine.Camera>());
				this.m_leftLoadingCamera.cullingMask = cullingMask2;
				this.m_leftLoadingCamera.depth = 4f;
			}
			if (this.m_rightHUDCamera != null)
			{
				int cullingMask3 = this.m_rightHUDCamera.cullingMask;
				this.m_rightHUDCamera.CopyFrom(this.m_rightCamera.GetComponent<UnityEngine.Camera>());
				this.m_rightHUDCamera.cullingMask = cullingMask3;
				this.m_rightHUDCamera.depth = 3f;
			}
			if (this.m_rightLoadingCamera != null)
			{
				int cullingMask4 = this.m_rightLoadingCamera.cullingMask;
				this.m_rightLoadingCamera.CopyFrom(this.m_rightCamera.GetComponent<UnityEngine.Camera>());
				this.m_rightLoadingCamera.cullingMask = cullingMask4;
				this.m_rightLoadingCamera.depth = 3f;
			}
		}

		private void fixCameraAspect()
		{
			this.m_mainCamera.GetComponent<UnityEngine.Camera>().ResetAspect();
			UnityEngine.Camera expr_1B = this.m_mainCamera.GetComponent<UnityEngine.Camera>();
			expr_1B.aspect = expr_1B.aspect * (this.m_leftCamera.GetComponent<UnityEngine.Camera>().rect.width * 2f / this.m_leftCamera.GetComponent<UnityEngine.Camera>().rect.height);
			this.m_leftCamera.GetComponent<UnityEngine.Camera>().aspect = this.m_mainCamera.GetComponent<UnityEngine.Camera>().aspect;
			this.m_rightCamera.GetComponent<UnityEngine.Camera>().aspect = this.m_mainCamera.GetComponent<UnityEngine.Camera>().aspect;
			this.m_leftHUDCamera.GetComponent<UnityEngine.Camera>().aspect = this.m_mainCamera.GetComponent<UnityEngine.Camera>().aspect;
			this.m_rightHUDCamera.GetComponent<UnityEngine.Camera>().aspect = this.m_mainCamera.GetComponent<UnityEngine.Camera>().aspect;
			this.m_leftLoadingCamera.GetComponent<UnityEngine.Camera>().aspect = this.m_mainCamera.GetComponent<UnityEngine.Camera>().aspect;
			this.m_rightLoadingCamera.GetComponent<UnityEngine.Camera>().aspect = this.m_mainCamera.GetComponent<UnityEngine.Camera>().aspect;
		}

		private void SetupLocalDeviceTransformationsFromDesires()
		{
			this.m_metaDepthSensor.transform.localPosition = new Vector3(-this.m_sensorScreenOffset.x, -this.m_sensorScreenOffset.y, -this.m_sensorScreenOffset.z);
			this.m_metaDepthSensor.transform.localRotation = Quaternion.Euler(-this.m_sensorScreenAngle, 0f, 0f);
			this.m_leftCamera.transform.localPosition = this.GetEyeTransformFromDesires(true);
			this.m_rightCamera.transform.localPosition = this.GetEyeTransformFromDesires(false);
			this.m_mainCamera.transform.localPosition = new Vector3(0f, 0f, -this.m_nearPlaneDistance);
		}

		private void SetupLocalDeviceTransformations()
		{
			this.m_metaDepthSensor.transform.localPosition = new Vector3(-this.m_sensorScreenOffset.x, -this.m_sensorScreenOffset.y, -this.m_sensorScreenOffset.z);
			this.m_metaDepthSensor.transform.localRotation = Quaternion.Euler(-this.m_sensorScreenAngle, 0f, 0f);
			this.m_leftCamera.transform.localPosition = new Vector3(-this.m_eyeInteraxialDistance / 2000f, 0f, -this.m_nearPlaneDistance);
			this.m_rightCamera.transform.localPosition = new Vector3(this.m_eyeInteraxialDistance / 2000f, 0f, -this.m_nearPlaneDistance);
			this.m_mainCamera.transform.localPosition = new Vector3(0f, 0f, -this.m_nearPlaneDistance);
			if (this.m_leftEyeOffset != Vector3.zero || this.m_rightEyeOffset != Vector3.zero)
			{
				Transform expr_115 = this.m_leftCamera.transform;
				expr_115.position = expr_115.position + this.m_leftEyeOffset;
				Transform expr_136 = this.m_rightCamera.transform;
				expr_136.position = expr_136.position + this.m_rightEyeOffset;
			}
		}

		private void UpdateCameraMatrices()
		{
			if (this._CameraMode == CameraType.Stereo)
			{
				this.RegisterProjectionMatrices();
			}
		}

		private void RegisterProjectionMatrices()
		{
			if (this.m_useExperimentalRendering)
			{
				this.m_leftCamera.GetComponent<UnityEngine.Camera>().projectionMatrix = this.CalculateProjectionMatrixFromDesires(true);
				this.m_rightCamera.GetComponent<UnityEngine.Camera>().projectionMatrix = this.CalculateProjectionMatrixFromDesires(false);
			}
			else
			{
				this.m_leftCamera.GetComponent<UnityEngine.Camera>().projectionMatrix = this.CalculateProjectionMatrixFromSettings(true);
				this.m_rightCamera.GetComponent<UnityEngine.Camera>().projectionMatrix = this.CalculateProjectionMatrixFromSettings(false);
			}
			if (this.m_leftHUDCamera != null && this.m_rightHUDCamera != null)
			{
				this.m_leftHUDCamera.projectionMatrix = this.m_leftCamera.GetComponent<UnityEngine.Camera>().projectionMatrix;
				this.m_rightHUDCamera.projectionMatrix = this.m_rightCamera.GetComponent<UnityEngine.Camera>().projectionMatrix;
			}
			if (this.m_leftLoadingCamera != null && this.m_rightLoadingCamera != null)
			{
				this.m_leftLoadingCamera.projectionMatrix = this.m_leftCamera.GetComponent<UnityEngine.Camera>().projectionMatrix;
				this.m_rightLoadingCamera.projectionMatrix = this.m_rightCamera.GetComponent<UnityEngine.Camera>().projectionMatrix;
			}
		}

		private void UpdatePublicAPI()
		{
			this.m_centerPosition = (this.m_leftCamera.transform.position + this.m_rightCamera.transform.position) / 2f;
			this.m_centerRotation = new Vector3(this.m_leftCamera.transform.rotation.x, this.m_leftCamera.transform.rotation.y, this.m_leftCamera.transform.rotation.z);
		}

		internal GameObject ObjectOfInterest()
		{
			GameObject objectOfInterest = MetaSingleton<Gaze>.Instance.ObjectOfInterest;
			if (objectOfInterest != null && this._objectOfInterestTestPrev == null && this._objectOfInterestTest == null)
			{
				this._objectOfInterestTest = objectOfInterest;
			}
			else if (objectOfInterest != null && this._objectOfInterestTest != null && this._objectOfInterestTest != objectOfInterest)
			{
				this._objectOfInterestTestPrev = this._objectOfInterestTest;
				this._objectOfInterestTest = objectOfInterest;
			}
			else if (objectOfInterest == null && this._objectOfInterestTest != null)
			{
				this._objectOfInterestTestPrev = this._objectOfInterestTest;
			}
			return objectOfInterest;
		}

		internal GameObject GetLastObjectOfInterest()
		{
			return this._objectOfInterestTestPrev;
		}
	}
}
