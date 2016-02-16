using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	internal class KeyboardShortcuts : MetaSingleton<KeyboardShortcuts>
	{
		private bool _originalExperimentalRenderingState;

		[SerializeField]
		private string _toggleHelpPanel = "f1";

		[SerializeField]
		private string _toggleMonoStereo = "f2";

		[SerializeField]
		private string _toggleWideRectified = "f3";

		[SerializeField]
		private string _recalibrate = "f4";

		[SerializeField]
		private string _reload = "f5";

		[SerializeField]
		private string _toggleHandPointCloud = "f6";

		[SerializeField]
		private string _toggleFingertipIndicators = "f7";

		[SerializeField]
		private string _toggleRGBFeed = "f8";

		[SerializeField]
		private string _toggleShadeFovLens = "f9";

		private bool _checkDuplicates;

		private Dictionary<string, int> _keys;

		public string toggleHelpPanel
		{
			get
			{
				return this._toggleHelpPanel;
			}
			set
			{
				this.SetShortcut(ref this._toggleHelpPanel, value);
			}
		}

		public string toggleMonoStereo
		{
			get
			{
				return this._toggleMonoStereo;
			}
			set
			{
				this.SetShortcut(ref this._toggleMonoStereo, value);
			}
		}

		public string toggleWideRectified
		{
			get
			{
				return this._toggleWideRectified;
			}
			set
			{
				this.SetShortcut(ref this._toggleWideRectified, value);
			}
		}

		public string recalibrate
		{
			get
			{
				return this._recalibrate;
			}
			set
			{
				this.SetShortcut(ref this._recalibrate, value);
			}
		}

		public string reload
		{
			get
			{
				return this._reload;
			}
			set
			{
				this.SetShortcut(ref this._reload, value);
			}
		}

		public string toggleHandPointCloud
		{
			get
			{
				return this._toggleHandPointCloud;
			}
			set
			{
				this.SetShortcut(ref this._toggleHandPointCloud, value);
			}
		}

		public string toggleFingertipIndicators
		{
			get
			{
				return this._toggleFingertipIndicators;
			}
			set
			{
				this.SetShortcut(ref this._toggleFingertipIndicators, value);
			}
		}

		public string toggleRGBFeed
		{
			get
			{
				return this._toggleRGBFeed;
			}
			set
			{
				this.SetShortcut(ref this._toggleRGBFeed, value);
			}
		}

		public string toggleShadeFovLens
		{
			get
			{
				return this._toggleShadeFovLens;
			}
			set
			{
				this.SetShortcut(ref this._toggleShadeFovLens, value);
			}
		}

		private void SetShortcut(ref string shortcut, string value)
		{
			if (Application.isPlaying)
			{
				this._checkDuplicates = true;
				try
				{
					value = value.ToLower();
					if (value != string.Empty)
					{
						Input.GetKey(value);
					}
					shortcut = value;
				}
				catch (UnityException ex)
				{
					Debug.LogError(ex.Message);
					shortcut = string.Empty;
				}
			}
		}

		private void Start()
		{
			if (MetaSingleton<RenderingCameraManagerBase>.Instance != null)
			{
				this._originalExperimentalRenderingState = MetaSingleton<RenderingCameraManagerBase>.Instance.m_useExperimentalRendering;
			}
			this.InitShortcuts();
		}

		private void InitShortcuts()
		{
			this.toggleMonoStereo = this._toggleMonoStereo;
			this.toggleWideRectified = this._toggleWideRectified;
			this.reload = this._reload;
			this.recalibrate = this._recalibrate;
			this.toggleFingertipIndicators = this._toggleFingertipIndicators;
			this.toggleHandPointCloud = this._toggleHandPointCloud;
			this.toggleRGBFeed = this._toggleRGBFeed;
			this.toggleWideRectified = this._toggleWideRectified;
		}

		private void CheckForDuplicates()
		{
			this._keys = new Dictionary<string, int>();
			this.CheckDuplicate(this._toggleMonoStereo, ref this._toggleMonoStereo);
			this.CheckDuplicate(this._toggleWideRectified, ref this._toggleWideRectified);
			this.CheckDuplicate(this._reload, ref this._reload);
			this.CheckDuplicate(this._recalibrate, ref this._recalibrate);
			this.CheckDuplicate(this._toggleFingertipIndicators, ref this._toggleFingertipIndicators);
			this.CheckDuplicate(this._toggleHandPointCloud, ref this._toggleHandPointCloud);
			this.CheckDuplicate(this._toggleRGBFeed, ref this._toggleRGBFeed);
			this.CheckDuplicate(this._toggleShadeFovLens, ref this._toggleShadeFovLens);
			this._checkDuplicates = false;
		}

		private void CheckDuplicate(string key, ref string keyRef)
		{
			if (!this._keys.ContainsKey(key))
			{
				if (key != string.Empty)
				{
					this._keys.Add(key, 1);
				}
			}
			else
			{
				Debug.LogError("Multiple functions defined for single shortcut key: " + key);
				keyRef = string.Empty;
			}
		}

		private void Update()
		{
			if (this._toggleMonoStereo != string.Empty && Input.GetKeyDown(this._toggleMonoStereo))
			{
				if (MetaCamera.GetCameraMode() == CameraType.Stereo)
				{
					MetaCamera.SetCameraMode(CameraType.Monocular);
				}
				else
				{
					MetaCamera.SetCameraMode(CameraType.Stereo);
				}
			}
			if (this._toggleWideRectified != string.Empty && Input.GetKeyDown(this._toggleWideRectified))
			{
				if (MetaCore.Instance.trueScale)
				{
					MetaCamera.SetAllowRealTimeSettingsUpdate(true);
					if (MetaSingleton<MetaUserSettingsManager>.Instance != null && MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile != null && MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile.m_widescaleCameraProfile != null)
					{
						MetaCamera.SetCameraProfile(MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile.m_widescaleCameraProfile);
					}
					else
					{
						MetaCamera.SetCameraProfile(MetaCore.Instance.m_defaultMeta1WidescaleProfile);
					}
					MetaSingleton<RenderingCameraManagerBase>.Instance.m_useExperimentalRendering = false;
					MetaCore.Instance.trueScale = false;
				}
				else
				{
					MetaCamera.SetAllowRealTimeSettingsUpdate(true);
					if (MetaSingleton<MetaUserSettingsManager>.Instance != null && MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile != null && MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile.m_truescaleCameraProfile != null)
					{
						MetaCamera.SetCameraProfile(MetaSingleton<MetaUserSettingsManager>.Instance.m_myUserProfile.m_truescaleCameraProfile);
					}
					else
					{
						MetaCamera.SetCameraProfile(MetaCore.Instance.m_defaultMeta1TruescaleProfile);
					}
					MetaSingleton<RenderingCameraManagerBase>.Instance.m_useExperimentalRendering = this._originalExperimentalRenderingState;
					MetaCore.Instance.trueScale = true;
				}
			}
			if (this._reload != string.Empty && Input.GetKeyDown(this._reload))
			{
				MetaCore.LoadScene(Application.loadedLevel, true);
			}
			if (this._toggleHandPointCloud != string.Empty && Input.GetKeyDown(this._toggleHandPointCloud))
			{
				MetaSingleton<InputIndicators>.Instance.handCloud = !MetaSingleton<InputIndicators>.Instance.handCloud;
			}
			if (this._toggleFingertipIndicators != string.Empty && Input.GetKeyDown(this._toggleFingertipIndicators))
			{
				MetaSingleton<InputIndicators>.Instance.fingertipIndicators = !MetaSingleton<InputIndicators>.Instance.fingertipIndicators;
				MetaSingleton<InputIndicators>.Instance.hoverIndicators = MetaSingleton<InputIndicators>.Instance.fingertipIndicators;
			}
			if (this._toggleShadeFovLens != string.Empty && Input.GetKeyDown(this._toggleShadeFovLens))
			{
				if (MetaCamera.GetFOVExpanded())
				{
					MetaCamera.SetFOVExpanded(false);
					MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_FOVExpanded", false);
				}
				else
				{
					MetaCamera.SetFOVExpanded(true);
					MetaSingleton<MetaUserSettingsManager>.Instance.SetUserProfileValue("m_FOVExpanded", true);
				}
			}
			if (Input.GetKey((KeyCode)27) && Input.GetKey((KeyCode)306))
			{
				Application.Quit();
			}
		}

		private void LateUpdate()
		{
			if (this._checkDuplicates)
			{
				this.CheckForDuplicates();
			}
		}
	}
}
