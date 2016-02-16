using System;
using UnityEngine;

namespace Meta
{
	public class MetaCamera : RenderingCameraManagerBase
	{
		[SerializeField]
		private CameraType m_cameraMode = CameraType.Stereo;

		private CameraType _cameraModePrev = CameraType.Stereo;

		[SerializeField]
		private bool m_allowRealTimeSettingsUpdate;

		private bool _allowRealTimeSettingsUpdatePrev;

		[SerializeField]
		private bool m_allowRealTimePoiUpdate = true;

		private bool _allowRealTimePoiUpdatePrev = true;

		[SerializeField]
		private bool m_allowKeyboardControl;

		private bool _allowKeyboardControlPrev;

		[SerializeField]
		private bool m_allowExternalVergenceControl;

		private bool _allowExternalVergenceControlPrev;

		[SerializeField]
		private Vector3 m_pointOfInterest = new Vector3(0f, 0f, 0.45f);

		private Vector3 _pointOfInterestPrev = new Vector3(0f, 0f, 0.45f);

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
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).CameraMode;
		}

		internal static void SetCameraMode(CameraType value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).CameraMode = value;
			if (value == CameraType.Monocular)
			{
				ScreenCursor.SetMouseCursorVisibility(true);
				if (!MetaSingleton<MetaMouse>.Instance.enableMetaMouse)
				{
					HandsInputModule.processMouse = true;
				}
			}
			else
			{
				ScreenCursor.SetMouseCursorVisibility(false);
				HandsInputModule.processMouse = false;
			}
		}

		internal static bool GetAllowRealTimeSettingsUpdate()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowRealTimeSettingsUpdate;
		}

		internal static void SetAllowRealTimeSettingsUpdate(bool value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowRealTimeSettingsUpdate = value;
		}

		internal static bool GetAllowRealTimePoiUpdate()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowRealTimePoiUpdate;
		}

		internal static void SetAllowRealTimePoiUpdate(bool value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowRealTimePoiUpdate = value;
		}

		internal static bool GetAllowKeyboardControl()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowKeyboardControl;
		}

		internal static void SetAllowKeyboardControl(bool value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowKeyboardControl = value;
		}

		internal static bool GetAllowExternalVergenceControl()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowExternalVergenceControl;
		}

		internal static void SetAllowExternalVergenceControl(bool value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).AllowExternalVergenceControl = value;
		}

		internal static Vector3 GetPointOfInterest()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).PointOfInterest;
		}

		internal static void SetPointOfInterest(Vector3 value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).PointOfInterest = value;
		}

		internal static DeviceType GetDevice()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).Device;
		}

		internal static DeviceSettings GetCameraProfile()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).CameraProfile;
		}

		internal static void SetCameraProfile(DeviceSettings value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).CameraProfile = value;
		}

		internal static bool GetFOVExpanded()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).fovExpanded;
		}

		internal static void SetFOVExpanded(bool value)
		{
			((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).fovExpanded = value;
		}

		internal static GameObject GetObjectOfInterest()
		{
			return ((MetaCamera)MetaSingleton<RenderingCameraManagerBase>.Instance).ObjectOfInterest();
		}

		public static bool IsVisible(Bounds bounds)
		{
			if (MetaCamera.GetCameraMode() == CameraType.Monocular)
			{
				return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.get_main()), bounds);
			}
			return GameObject.Find("MetaCameraLeft") != null && GameObject.Find("MetaCameraRight") != null && (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(GameObject.Find("MetaCameraLeft").GetComponent<Camera>()), bounds) || GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(GameObject.Find("MetaCameraRight").GetComponent<Camera>()), bounds));
		}

		private void InitCameraMode()
		{
			base.CameraMode = (this._cameraModePrev = this.m_cameraMode);
		}

		private void UpdateCameraMode()
		{
			if (this._cameraModePrev != this.m_cameraMode)
			{
				base.CameraMode = (this._cameraModePrev = this.m_cameraMode);
			}
			else if (base.CameraMode != this.m_cameraMode)
			{
				this._cameraModePrev = (this.m_cameraMode = base.CameraMode);
			}
		}

		private void InitAllowRealTimeSettingsUpdate()
		{
			base.AllowRealTimeSettingsUpdate = (this._allowRealTimeSettingsUpdatePrev = this.m_allowRealTimeSettingsUpdate);
		}

		private void UpdateAllowRealTimeSettingsUpdate()
		{
			if (this._allowRealTimeSettingsUpdatePrev != this.m_allowRealTimeSettingsUpdate)
			{
				base.AllowRealTimeSettingsUpdate = (this._allowRealTimeSettingsUpdatePrev = this.m_allowRealTimeSettingsUpdate);
			}
			else if (base.AllowRealTimeSettingsUpdate != this.m_allowRealTimeSettingsUpdate)
			{
				this._allowRealTimeSettingsUpdatePrev = (this.m_allowRealTimeSettingsUpdate = base.AllowRealTimeSettingsUpdate);
			}
		}

		private void InitAllowRealTimePoiUpdate()
		{
			base.AllowRealTimePoiUpdate = (this._allowRealTimePoiUpdatePrev = this.m_allowRealTimePoiUpdate);
		}

		private void UpdateAllowRealTimePoiUpdate()
		{
			if (this._allowRealTimePoiUpdatePrev != this.m_allowRealTimePoiUpdate)
			{
				base.AllowRealTimePoiUpdate = (this._allowRealTimePoiUpdatePrev = this.m_allowRealTimePoiUpdate);
			}
			else if (base.AllowRealTimePoiUpdate != this.m_allowRealTimePoiUpdate)
			{
				this._allowRealTimePoiUpdatePrev = (this.m_allowRealTimePoiUpdate = base.AllowRealTimePoiUpdate);
			}
		}

		private void InitAllowKeyboardControl()
		{
			base.AllowKeyboardControl = (this._allowKeyboardControlPrev = this.m_allowKeyboardControl);
		}

		private void UpdateAllowKeyboardControl()
		{
			if (this._allowKeyboardControlPrev != this.m_allowKeyboardControl)
			{
				base.AllowKeyboardControl = (this._allowKeyboardControlPrev = this.m_allowKeyboardControl);
			}
			else if (base.AllowKeyboardControl != this.m_allowKeyboardControl)
			{
				this._allowKeyboardControlPrev = (this.m_allowKeyboardControl = base.AllowKeyboardControl);
			}
		}

		private void InitAllowExternalVergenceControl()
		{
			base.AllowExternalVergenceControl = (this._allowExternalVergenceControlPrev = this.m_allowExternalVergenceControl);
		}

		private void UpdateAllowExternalVergenceControl()
		{
			if (this._allowExternalVergenceControlPrev != this.m_allowExternalVergenceControl)
			{
				base.AllowExternalVergenceControl = (this._allowExternalVergenceControlPrev = this.m_allowExternalVergenceControl);
			}
			else if (base.AllowExternalVergenceControl != this.m_allowExternalVergenceControl)
			{
				this._allowExternalVergenceControlPrev = (this.m_allowExternalVergenceControl = base.AllowExternalVergenceControl);
			}
		}

		private void InitPointOfInterest()
		{
			base.PointOfInterest = (this._pointOfInterestPrev = this.m_pointOfInterest);
		}

		private void UpdatePointOfInterest()
		{
			if (this._pointOfInterestPrev != this.m_pointOfInterest)
			{
				base.PointOfInterest = (this._pointOfInterestPrev = this.m_pointOfInterest);
			}
			else if (base.PointOfInterest != this.m_pointOfInterest)
			{
				this._pointOfInterestPrev = (this.m_pointOfInterest = base.PointOfInterest);
			}
		}

		private void InitDevice()
		{
			this._devicePrev = (this.m_device = base.Device);
		}

		private void UpdateDevice()
		{
			if (this._devicePrev != this.m_device)
			{
				this._devicePrev = (this.m_device = base.Device);
			}
			else if (base.Device != this.m_device)
			{
				this._devicePrev = (this.m_device = base.Device);
			}
		}

		private void InitCameraProfile()
		{
			base.CameraProfile = (this._cameraProfilePrev = this.m_cameraProfile);
		}

		private void UpdateCameraProfile()
		{
			if (this._cameraProfilePrev != this.m_cameraProfile)
			{
				base.CameraProfile = (this._cameraProfilePrev = this.m_cameraProfile);
			}
			else if (base.CameraProfile != this.m_cameraProfile)
			{
				this._cameraProfilePrev = (this.m_cameraProfile = base.CameraProfile);
			}
		}

		private void InitObjectOfInterest()
		{
			this._objectOfInterestPrev = (this.m_objectOfInterest = base.ObjectOfInterest());
		}

		private void UpdateObjectOfInterest()
		{
			if (this._objectOfInterestPrev != this.m_objectOfInterest)
			{
				this._objectOfInterestPrev = (this.m_objectOfInterest = base.ObjectOfInterest());
			}
			else if (base.ObjectOfInterest() != this.m_objectOfInterest)
			{
				this._objectOfInterestPrev = (this.m_objectOfInterest = base.ObjectOfInterest());
			}
		}

		private void InitUserProfileValues()
		{
			if (MetaSingleton<MetaUserSettingsManager>.Instance != null)
			{
				if (MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true) != null)
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
