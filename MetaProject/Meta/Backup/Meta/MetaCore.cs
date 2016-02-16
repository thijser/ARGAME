// Decompiled with JetBrains decompiler
// Type: Meta.MetaCore
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

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
    [SerializeField]
    [HideInInspector]
    internal bool _enableVGAMode;
    [HideInInspector]
    [SerializeField]
    private bool _enableCameraLight;
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
    [HideInInspector]
    [SerializeField]
    private bool _trueScale;
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
        if (Object.op_Inequality((Object) ((Component) MetaSingleton<RenderingCameraManagerBase>.Instance).get_transform().FindChild("Light"), (Object) null))
          ((Behaviour) ((Component) ((Component) MetaSingleton<RenderingCameraManagerBase>.Instance).get_transform().FindChild("Light")).GetComponent<Light>()).set_enabled(value);
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

    public MetaCore()
    {
      base.\u002Ector();
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
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MetaCore.\u003CLoadSceneIE\u003Ec__Iterator0()
      {
        async = async,
        \u003C\u0024\u003Easync = async
      };
    }

    [DebuggerHidden]
    private static IEnumerator LoadSceneAdditiveIE(AsyncOperation async, bool displayLoading)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MetaCore.\u003CLoadSceneAdditiveIE\u003Ec__Iterator1()
      {
        async = async,
        \u003C\u0024\u003Easync = async
      };
    }

    private static Transform GetTopParent(Transform obj)
    {
      if (Object.op_Equality((Object) obj.get_parent(), (Object) null))
        return obj;
      return MetaCore.GetTopParent(obj.get_parent());
    }

    private void CheckLocalizationType()
    {
      if ((!(MetaSingleton<MetaLocalization>.Instance.currentLocalization == "IMULocalizer") || this.imuModel != IMUModel.MPU9150Serial) && !(MetaSingleton<MetaLocalization>.Instance.currentLocalization == "SLAMLocalizer"))
        return;
      this._IMUCalibrating = true;
      MetaSingleton<MetaUI>.Instance.ToggleIMUCalibratingIndicator(true);
    }

    private void CheckIMUFinishedCalibrating()
    {
      if ((MetaSingleton<MetaLocalization>.Instance.currentLocalization == "IMULocalizer" || MetaSingleton<MetaLocalization>.Instance.currentLocalization == "SLAMLocalizer") && !Quaternion.op_Inequality(((Component) MetaCore.Instance).get_transform().get_localRotation(), Quaternion.get_identity()))
        return;
      this._IMUCalibrating = false;
      MetaSingleton<MetaUI>.Instance.ToggleIMUCalibratingIndicator(false);
    }

    internal void Log(string debugMessage)
    {
      if (!this.m_debug)
        return;
      Debug.Log((object) debugMessage);
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
      return ((Component) this).get_gameObject();
    }

    private void CheckForComponents()
    {
      if (Object.op_Equality((Object) this._HDMarkerTracker, (Object) null))
        this._HDMarkerTracker = (MarkerDetector) Object.FindObjectOfType(typeof (MarkerDetector));
      if (Object.op_Equality((Object) this._DeviceTextureSource, (Object) null))
        this._DeviceTextureSource = (DeviceTextureSource) Object.FindObjectOfType(typeof (DeviceTextureSource));
      if (Object.op_Equality((Object) this._Hands, (Object) null))
        this._Hands = (Hands) Object.FindObjectOfType(typeof (Hands));
      if (!Object.op_Equality((Object) this._CanvasTracker, (Object) null))
        return;
      this._CanvasTracker = (CanvasTracker) Object.FindObjectOfType(typeof (CanvasTracker));
    }

    private void AddEventReceiverIfNotNull(MonoBehaviour receiver)
    {
      if (!Object.op_Inequality((Object) receiver, (Object) null))
        return;
      this._EventReceivers.Add(receiver);
    }

    internal void Awake()
    {
      if (Object.op_Inequality((Object) MetaCore.Instance, (Object) null))
        Object.Destroy((Object) this);
      MetaCore.Instance = this;
      this._EventReceivers = new List<MonoBehaviour>();
      if (Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance, (Object) null))
      {
        if (MetaSingleton<MetaUserSettingsManager>.Instance.GetIMUModel() != IMUModel.UnknownIMU)
          this._imuModel = MetaSingleton<MetaUserSettingsManager>.Instance.GetIMUModel();
        if (Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true), (Object) null))
          this.m_defaultMeta1TruescaleProfile = MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(true);
        if (Object.op_Inequality((Object) MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(false), (Object) null))
          this.m_defaultMeta1WidescaleProfile = MetaSingleton<MetaUserSettingsManager>.Instance.GetCameraProfile(false);
      }
      this._deviceInfo.imuModel = IMUModel.MPU9150Serial;
      this._deviceInfo.cameraModel = CameraModel.DS325;
      this._deviceInfo.depthFps = 60f;
      this._deviceInfo.depthHeight = 240;
      this._deviceInfo.depthWidth = 320;
      this._deviceInfo.colorFps = 30f;
      this._deviceInfo.colorHeight = 720;
      this._deviceInfo.colorWidth = 1280;
      this.Log("IMU Model = " + (object) this._deviceInfo.imuModel);
      int num;
      if (this._enableVGAMode)
      {
        this._deviceInfo.colorHeight = 480;
        this._deviceInfo.colorWidth = 720;
        num = MetaCore.InitMetaVisionCameraVGA(ref this._deviceInfo, this._imuModel);
      }
      else
        num = MetaCore.InitMetaVisionCamera(ref this._deviceInfo, this._imuModel);
      if (num <= 0)
      {
        this.m_depthSensorConnected = false;
        this.m_mouseLookGestureSimulator = true;
        Debug.LogWarning((object) "Camera not properly connected. Mouselook backup controls enabled: Right-click-hold to look around, WASD to move.");
      }
      else
      {
        this.m_depthSensorConnected = true;
        this.m_mouseLookGestureSimulator = false;
      }
      this.CheckForComponents();
      this.AddEventReceiverIfNotNull((MonoBehaviour) this._HDMarkerTracker);
      this.AddEventReceiverIfNotNull((MonoBehaviour) this._Hands);
      this.AddEventReceiverIfNotNull((MonoBehaviour) this._DeviceTextureSource);
      this.AddEventReceiverIfNotNull((MonoBehaviour) this._CanvasTracker);
      for (int index = 0; index < this._EventReceivers.Count; ++index)
      {
        if (Object.op_Inequality((Object) this._EventReceivers[index], (Object) null) && ((Behaviour) this._EventReceivers[index]).get_enabled())
          ((IMetaEventReceiver) this._EventReceivers[index]).MetaInit();
      }
    }

    private void Start()
    {
      this.StartCoroutine("FixCameraMode");
      this.CheckLocalizationType();
    }

    [DebuggerHidden]
    private IEnumerator FixCameraMode()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MetaCore.\u003CFixCameraMode\u003Ec__Iterator2()
      {
        \u003C\u003Ef__this = this
      };
    }

    private void Update()
    {
      if (!this._imuConnectedChecked)
      {
        if (!IMULocalizer.IsMotionSensorConnected())
          MetaSingleton<MetaLocalization>.Instance.UseMouseLocalizer();
        this._imuConnectedChecked = true;
      }
      if (this._IMUCalibrating)
        this.CheckIMUFinishedCalibrating();
      for (int index = 0; index < this._EventReceivers.Count; ++index)
      {
        if (Object.op_Inequality((Object) this._EventReceivers[index], (Object) null) && ((Behaviour) this._EventReceivers[index]).get_enabled())
          ((IMetaEventReceiver) this._EventReceivers[index]).MetaUpdate();
      }
    }

    private void LateUpdate()
    {
      for (int index = 0; index < this._EventReceivers.Count; ++index)
      {
        if (Object.op_Inequality((Object) this._EventReceivers[index], (Object) null) && ((Behaviour) this._EventReceivers[index]).get_enabled())
          ((IMetaEventReceiver) this._EventReceivers[index]).MetaLateUpdate();
      }
    }

    private void OnDestroy()
    {
      MetaCore.DeinitMeta();
      this.Log("Destroyed");
      for (int index = 0; index < this._EventReceivers.Count; ++index)
      {
        this.Log("Destroyed");
        if (Object.op_Inequality((Object) this._EventReceivers[index], (Object) null) && ((Behaviour) this._EventReceivers[index]).get_enabled())
          ((IMetaEventReceiver) this._EventReceivers[index]).MetaOnDestroy();
      }
    }
  }
}
