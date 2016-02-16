using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	public class MetaCore : MonoBehaviour
	{
		public static MetaCore Instance;

		[HideInInspector]
		internal bool m_debug;

		[HideInInspector]
		internal bool m_dllLogging;

		[HideInInspector]
		private bool _imuConnectedChecked;

		[HideInInspector, SerializeField]
		internal bool _enableVGAMode;

		[HideInInspector, SerializeField]
		private bool _enableCameraLight = true;

		private IMUModel _imuModel;

		private DeviceInfo _deviceInfo;

		private bool _IMUCalibrating;

		private Hands _Hands;

		private DeviceTextureSource _DeviceTextureSource;

		private CanvasTracker _CanvasTracker;

		private MarkerDetector _HDMarkerTracker;

		private List<MonoBehaviour> _EventReceivers;

		internal bool m_rgbSensorConnected;

		internal bool m_depthSensorConnected;

		internal bool m_mouseLookGestureSimulator;

		[HideInInspector, SerializeField]
		private bool _trueScale = true;

		[SerializeField]
		internal DeviceSettings m_defaultMeta1TruescaleProfile;

		[SerializeField]
		internal DeviceSettings m_defaultMeta1WidescaleProfile;

		public bool enableVGAMode
		{
			get
			{
				return this._enableVGAMode;
			}
			set
			{
				this._enableVGAMode = value;
			}
		}

		public bool enableCameraLight
		{
			get
			{
				return this._enableCameraLight;
			}
			set
			{
				if (MetaSingleton<RenderingCameraManagerBase>.Instance.get_transform().FindChild("Light") != null)
				{
					MetaSingleton<RenderingCameraManagerBase>.Instance.get_transform().FindChild("Light").GetComponent<Light>().set_enabled(value);
				}
				this._enableCameraLight = value;
			}
		}

		public IMUModel imuModel
		{
			get
			{
				return this._imuModel;
			}
			set
			{
				this._imuModel = value;
			}
		}

		internal DeviceInfo DeviceInformation
		{
			get
			{
				return this._deviceInfo;
			}
			set
			{
				this._deviceInfo = value;
			}
		}

		public bool depthSensorConnected
		{
			get
			{
				return this.m_depthSensorConnected;
			}
		}

		public bool trueScale
		{
			get
			{
				return this._trueScale;
			}
			set
			{
				this._trueScale = value;
			}
		}

		public static void LoadScene(string sceneName, bool displayLoading = true)
		{
			AsyncOperation async = Application.LoadLevelAdditiveAsync(sceneName);
			MetaCore.Instance.StartCoroutine(MetaCore.LoadSceneIE(async, displayLoading));
		}

		public static void LoadScene(int sceneNum, bool displayLoading = true)
		{
			AsyncOperation async = Application.LoadLevelAdditiveAsync(sceneNum);
			MetaCore.Instance.StartCoroutine(MetaCore.LoadSceneIE(async, displayLoading));
		}

		public static void LoadSceneAdditive(string sceneName, bool displayLoading = true)
		{
			AsyncOperation async = Application.LoadLevelAdditiveAsync(sceneName);
			MetaCore.Instance.StartCoroutine(MetaCore.LoadSceneAdditiveIE(async, displayLoading));
		}

		public static void LoadSceneAdditive(int sceneNum, bool displayLoading = true)
		{
			AsyncOperation async = Application.LoadLevelAdditiveAsync(sceneNum);
			MetaCore.Instance.StartCoroutine(MetaCore.LoadSceneAdditiveIE(async, displayLoading));
		}

		[DebuggerHidden]
		private static IEnumerator LoadSceneIE(AsyncOperation async, bool displayLoading)
		{
			MetaCore.<LoadSceneIE>c__Iterator0 <LoadSceneIE>c__Iterator = new MetaCore.<LoadSceneIE>c__Iterator0();
			<LoadSceneIE>c__Iterator.async = async;
			<LoadSceneIE>c__Iterator.<$>async = async;
			return <LoadSceneIE>c__Iterator;
		}

		[DebuggerHidden]
		private static IEnumerator LoadSceneAdditiveIE(AsyncOperation async, bool displayLoading)
		{
			MetaCore.<LoadSceneAdditiveIE>c__Iterator1 <LoadSceneAdditiveIE>c__Iterator = new MetaCore.<LoadSceneAdditiveIE>c__Iterator1();
			<LoadSceneAdditiveIE>c__Iterator.async = async;
			<LoadSceneAdditiveIE>c__Iterator.<$>async = async;
			return <LoadSceneAdditiveIE>c__Iterator;
		}

		private static Transform GetTopParent(Transform obj)
		{
			if (obj.get_parent() == null)
			{
				return obj;
			}
			return MetaCore.GetTopParent(obj.get_parent());
		}

		private void CheckLocalizationType()
		{
			if ((MetaSingleton<MetaLocalization>.Instance.currentLocalization == "IMULocalizer" && this.imuModel == IMUModel.MPU9150Serial) || MetaSingleton<MetaLocalization>.Instance.currentLocalization == "SLAMLocalizer")
			{
				this._IMUCalibrating = true;
				MetaSingleton<MetaUI>.Instance.ToggleIMUCalibratingIndicator(true);
			}
		}

		private void CheckIMUFinishedCalibrating()
		{
			if ((!(MetaSingleton<MetaLocalization>.Instance.currentLocalization == "IMULocalizer") && !(MetaSingleton<MetaLocalization>.Instance.currentLocalization == "SLAMLocalizer")) || MetaCore.Instance.get_transform().get_localRotation() != Quaternion.get_identity())
			{
				this._IMUCalibrating = false;
				MetaSingleton<MetaUI>.Instance.ToggleIMUCalibratingIndicator(false);
			}
		}

		internal void Log(string debugMessage)
		{
			if (this.m_debug)
			{
				Debug.Log(debugMessage);
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "initMetaVisionCamera")]
		internal static extern int InitMetaVisionCamera(ref DeviceInfo cameraInfo, IMUModel imuModel);

		[DllImport("MetaVisionDLL", EntryPoint = "initMetaVisionCameraVGA")]
		internal static extern int InitMetaVisionCameraVGA(ref DeviceInfo cameraInfo, IMUModel imuModel);

		[DllImport("MetaVisionDLL", EntryPoint = "initMetaVisionSource")]
		internal static extern int InitMetaVisionSource(ref DeviceInfo cameraInfo, string sourcePath);

		[DllImport("MetaVisionDLL", EntryPoint = "enableDLLLogging")]
		protected static extern void EnableDLLLogging();

		[DllImport("MetaVisionDLL", EntryPoint = "enableRecording")]
		protected static extern void EnableRecording(string folderPath, string folderName);

		[DllImport("MetaVisionDLL", EntryPoint = "deinitMeta")]
		protected static extern void DeinitMeta();

		internal GameObject getMetaFrame()
		{
			return base.get_gameObject();
		}

		private void CheckForComponents()
		{
			if (this._HDMarkerTracker == null)
			{
				this._HDMarkerTracker = (MarkerDetector)Object.FindObjectOfType(typeof(MarkerDetector));
			}
			if (this._DeviceTextureSource == null)
			{
				this._DeviceTextureSource = (DeviceTextureSource)Object.FindObjectOfType(typeof(DeviceTextureSource));
			}
			if (this._Hands == null)
			{
				this._Hands = (Hands)Object.FindObjectOfType(typeof(Hands));
			}
			if (this._CanvasTracker == null)
			{
				this._CanvasTracker = (CanvasTracker)Object.FindObjectOfType(typeof(CanvasTracker));
			}
		}

		private void AddEventReceiverIfNotNull(MonoBehaviour receiver)
		{
			if (receiver != null)
			{
				this._EventReceivers.Add(receiver);
				return;
			}
		}

		internal void Awake()
		{
			if (MetaCore.Instance != null)
			{
				Object.Destroy(this);
			}
			MetaCore.Instance = this;
			this._EventReceivers = new List<MonoBehaviour>();
			if (MetaSingleton<MetaUserSettingsManager>.Instance != null)
			{
				if (MetaSingleton<MetaUserSettingsManager>.Instance.GetIMUModel() != IMUModel.UnknownIMU)
				{
					this._imuModel = MetaSingleton<MetaUserSettingsManager>.Instance.GetIMUModel();
				}
				if (MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true) != null)
				{
					this.m_defaultMeta1TruescaleProfile = MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true);
				}
				if (MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(false) != null)
				{
					this.m_defaultMeta1WidescaleProfile = MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(false);
				}
			}
			this._deviceInfo.imuModel = IMUModel.MPU9150Serial;
			this._deviceInfo.cameraModel = CameraModel.DS325;
			this._deviceInfo.depthFps = 60f;
			this._deviceInfo.depthHeight = 240;
			this._deviceInfo.depthWidth = 320;
			this._deviceInfo.colorFps = 30f;
			this._deviceInfo.colorHeight = 720;
			this._deviceInfo.colorWidth = 1280;
			this.Log("IMU Model = " + this._deviceInfo.imuModel);
			int num;
			if (this._enableVGAMode)
			{
				this._deviceInfo.colorHeight = 480;
				this._deviceInfo.colorWidth = 720;
				num = MetaCore.InitMetaVisionCameraVGA(ref this._deviceInfo, this._imuModel);
			}
			else
			{
				num = MetaCore.InitMetaVisionCamera(ref this._deviceInfo, this._imuModel);
			}
			if (num <= 0)
			{
				this.m_depthSensorConnected = false;
				this.m_mouseLookGestureSimulator = true;
				Debug.LogWarning("Camera not properly connected. Mouselook backup controls enabled: Right-click-hold to look around, WASD to move.");
			}
			else
			{
				this.m_depthSensorConnected = true;
				this.m_mouseLookGestureSimulator = false;
			}
			this.CheckForComponents();
			this.AddEventReceiverIfNotNull(this._HDMarkerTracker);
			this.AddEventReceiverIfNotNull(this._Hands);
			this.AddEventReceiverIfNotNull(this._DeviceTextureSource);
			this.AddEventReceiverIfNotNull(this._CanvasTracker);
			for (int i = 0; i < this._EventReceivers.Count; i++)
			{
				if (this._EventReceivers[i] != null && this._EventReceivers[i].get_enabled())
				{
					IMetaEventReceiver metaEventReceiver = (IMetaEventReceiver)this._EventReceivers[i];
					metaEventReceiver.MetaInit();
				}
			}
		}

		private void Start()
		{
			base.StartCoroutine("FixCameraMode");
			this.CheckLocalizationType();
		}

		[DebuggerHidden]
		private IEnumerator FixCameraMode()
		{
			MetaCore.<FixCameraMode>c__Iterator2 <FixCameraMode>c__Iterator = new MetaCore.<FixCameraMode>c__Iterator2();
			<FixCameraMode>c__Iterator.<>f__this = this;
			return <FixCameraMode>c__Iterator;
		}

		private void Update()
		{
			if (!this._imuConnectedChecked)
			{
				if (!IMULocalizer.IsMotionSensorConnected())
				{
					MetaSingleton<MetaLocalization>.Instance.UseMouseLocalizer();
				}
				this._imuConnectedChecked = true;
			}
			if (this._IMUCalibrating)
			{
				this.CheckIMUFinishedCalibrating();
			}
			for (int i = 0; i < this._EventReceivers.Count; i++)
			{
				if (this._EventReceivers[i] != null && this._EventReceivers[i].get_enabled())
				{
					IMetaEventReceiver metaEventReceiver = (IMetaEventReceiver)this._EventReceivers[i];
					metaEventReceiver.MetaUpdate();
				}
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < this._EventReceivers.Count; i++)
			{
				if (this._EventReceivers[i] != null && this._EventReceivers[i].get_enabled())
				{
					IMetaEventReceiver metaEventReceiver = (IMetaEventReceiver)this._EventReceivers[i];
					metaEventReceiver.MetaLateUpdate();
				}
			}
		}

		private void OnDestroy()
		{
			MetaCore.DeinitMeta();
			this.Log("Destroyed");
			for (int i = 0; i < this._EventReceivers.Count; i++)
			{
				this.Log("Destroyed");
				if (this._EventReceivers[i] != null && this._EventReceivers[i].get_enabled())
				{
					IMetaEventReceiver metaEventReceiver = (IMetaEventReceiver)this._EventReceivers[i];
					metaEventReceiver.MetaOnDestroy();
				}
			}
		}
	}
}
