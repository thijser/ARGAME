using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Meta
{
	public class MetaUserSettingsManager : MetaSingleton<MetaUserSettingsManager>
	{
		public bool m_loadUserProfile = true;

		public bool m_forceLocalProfile;

		private UserSettings _originalUserProfile;

		public UserSettings m_myUserProfile;

		private string _currentUser = string.Empty;

        private readonly IDictionary<string, int> map = new Dictionary<string, int>(7)
        {
            {
                "m_imuModel",
                0
            },
            {
                "m_leftEyeOffset",
                1
            },
            {
                "m_rightEyeOffset",
                2
            },
            {
                "m_reachDistance",
                3
            },
            {
                "m_eyeInteraxialDistance",
                4
            },
            {
                "m_screenInteraxialDistance",
                5
            },
            {
                "m_FOVExpanded",
                6
            }
        };

        private void Awake()
		{
			if (this.m_loadUserProfile)
			{
				if (this.m_myUserProfile == null)
				{
					this._currentUser = MetaUserSettingsManager.GetCurrentUser(false, this.m_forceLocalProfile);
					if (!MetaUserSettingsManager.JSONLoadUserProfile(this._currentUser, ref this._originalUserProfile, true))
					{
						this._originalUserProfile = null;
					}
				}
				else
				{
					this._currentUser = this.m_myUserProfile.name.Replace(".userProfile", string.Empty);
				}
				if (this._originalUserProfile != null)
				{
					MetaUserSettingsManager.CreateBufferProfile(ref this._originalUserProfile, ref this.m_myUserProfile);
				}
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void OnDestroy()
		{
		}

		public static string GetCurrentUser(bool loginIDOnly = false, bool localIDOnly = false)
		{
			string text = string.Empty;
			if (!localIDOnly)
			{
				text = PlayerPrefs.GetString("stayLoggedInAsUserID");
			}
			if (text == string.Empty && !loginIDOnly)
			{
				try
				{
					text = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/LocalUserID.txt");
				}
				catch
				{
					Debug.Log("Local user ID not found, loading the MetaUser profile.");
				}
				if (text == string.Empty)
				{
					text = "MetaUser";
					MetaUserSettingsManager.SaveCurrentLocalUser(text);
				}
			}
			return text;
		}

		public static bool SaveCurrentLocalUser(string localUserID)
		{
			bool result;
			try
			{
				Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/");
				File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/LocalUserID.txt", localUserID);
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				result = false;
			}
			return result;
		}

		public static bool JSONLoadUserProfile(string user, ref UserSettings userProfile, bool createIfMissing = true)
		{
			if (!(user != string.Empty))
			{
				return false;
			}
			userProfile = ScriptableObject.CreateInstance<UserSettings>();
			string text = string.Empty;
			try
			{
				text = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/" + user + ".userProfile");
			}
			catch
			{
				Debug.Log("JSON user profile not found for user " + user + ".");
			}
			if (!(text == string.Empty))
			{
				userProfile.JSONToUserProfile(text);
				userProfile.name = userProfile.m_ProfileName;
				if (userProfile.m_truescaleCameraProfile != null)
				{
					userProfile.m_truescaleCameraProfile.name = userProfile.m_truescaleCameraProfile.m_ProfileName;
				}
				if (userProfile.m_widescaleCameraProfile != null)
				{
					userProfile.m_widescaleCameraProfile.name = userProfile.m_widescaleCameraProfile.m_ProfileName;
				}
				if (userProfile.m_renderingProfile != null)
				{
					userProfile.m_renderingProfile.name = userProfile.m_renderingProfile.m_ProfileName;
				}
				return true;
			}
			if (createIfMissing)
			{
				UserSettings userSettings = ScriptableObject.CreateInstance<UserSettings>();
				UserSettings.InstantiateNewUserSettings(user, ref userSettings);
				userSettings.m_renderingProfile = ScriptableObject.CreateInstance<RenderingSettings>();
				userSettings.m_renderingProfile.Init(user);
				MetaUserSettingsManager.JSONSaveUserProfile(user, userSettings);
				return MetaUserSettingsManager.JSONLoadUserProfile(user, ref userProfile, true);
			}
			return false;
		}

		public static bool JSONSaveUserProfile(string user, UserSettings userProfile)
		{
			if (user != string.Empty && userProfile != null)
			{
				string contents = userProfile.UserProfileToJSON();
				try
				{
					Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/");
					File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/" + user + ".userProfile", contents);
					bool result = true;
					return result;
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString());
					bool result = false;
					return result;
				}
				return false;
			}
			return false;
		}

		public static bool CreateBufferProfile(ref UserSettings originalProfile, ref UserSettings bufferProfile)
		{
			bufferProfile = ScriptableObject.CreateInstance<UserSettings>();
			if (originalProfile != null)
			{
				originalProfile.DeepCopyTo(bufferProfile, false, true);
				bufferProfile.name = originalProfile.name + " (buffer)";
				bufferProfile.m_renderingProfile = ScriptableObject.CreateInstance<RenderingSettings>();
				bufferProfile.m_renderingProfile.name = bufferProfile.m_renderingProfile.name + " (buffer)";
				if (originalProfile.m_renderingProfile != null)
				{
					originalProfile.m_renderingProfile.DeepCopyTo(bufferProfile.m_renderingProfile);
				}
				return true;
			}
			return false;
		}

		public static bool SaveBufferProfile(ref UserSettings originalProfile, ref UserSettings bufferProfile)
		{
			if (originalProfile != null && bufferProfile != null)
			{
				bufferProfile.DeepCopyTo(originalProfile, true, true);
				if (bufferProfile.m_renderingProfile != null)
				{
					bufferProfile.m_renderingProfile.DeepCopyTo(originalProfile.m_renderingProfile);
				}
				return true;
			}
			return false;
		}

		public bool SaveUserProfile()
		{
			return MetaUserSettingsManager.SaveBufferProfile(ref this._originalUserProfile, ref this.m_myUserProfile) && MetaUserSettingsManager.JSONSaveUserProfile(this._currentUser, this._originalUserProfile);
		}

		public static bool MakeCameraProfileWithIPD(UserSettings userProfile, DeviceSettings CameraProfile, ref DeviceSettings CameraProfileWithIPD, bool trueScale = true)
		{
			CameraProfileWithIPD = ScriptableObject.CreateInstance<DeviceSettings>();
			if (userProfile != null && CameraProfile != null)
			{
				if (trueScale)
				{
					CameraProfileWithIPD.name = "User Truescale Profile";
				}
				else
				{
					CameraProfileWithIPD.name = "User Widescale Profile";
				}
				CameraProfile.DeepCopyTo(CameraProfileWithIPD);
				CameraProfileWithIPD.m_eyeInteraxialDistance = userProfile.m_eyeInteraxialDistance;
				CameraProfileWithIPD.m_screenInteraxialDistance = userProfile.m_screenInteraxialDistance;
				return true;
			}
			return false;
		}

		public RenderingSettings GetRenderingProfile()
		{
			if (this.m_myUserProfile != null)
			{
				return this.m_myUserProfile.m_renderingProfile;
			}
			return null;
		}

		public DeviceSettings GetCameraProfile(bool trueScale)
		{
			if (!(this.m_myUserProfile != null))
			{
				return null;
			}
			if (trueScale)
			{
				return this.m_myUserProfile.m_truescaleCameraProfile;
			}
			return this.m_myUserProfile.m_widescaleCameraProfile;
		}

		public Vector3 GetEyeOffset(bool left)
		{
			if (!(this.m_myUserProfile != null))
			{
				return new Vector3(0f, 0f, 0f);
			}
			if (left)
			{
				return this.m_myUserProfile.m_leftEyeOffset;
			}
			return this.m_myUserProfile.m_rightEyeOffset;
		}

		public float GetReachDistance()
		{
			if (this.m_myUserProfile != null)
			{
				return this.m_myUserProfile.m_reachDistance;
			}
			return 0.4f;
		}

		public IMUModel GetIMUModel()
		{
			if (this.m_myUserProfile != null)
			{
				return this.m_myUserProfile.m_imuModel;
			}
			return IMUModel.UnknownIMU;
		}

		public bool GetFOVSetting()
		{
			return !(this.m_myUserProfile != null) || this.m_myUserProfile.m_fovExpanded;
		}

		public bool SetUserProfileValue(string key, object value)
		{
			try
			{
				if (key != null)
				{
					int num;
					if (map.TryGetValue(key, out num))
					{
						switch (num)
						{
						case 0:
							this.m_myUserProfile.m_imuModel = (IMUModel)((int)value);
							break;
						case 1:
							this.m_myUserProfile.m_leftEyeOffset = (Vector3)value;
							break;
						case 2:
							this.m_myUserProfile.m_rightEyeOffset = (Vector3)value;
							break;
						case 3:
							this.m_myUserProfile.m_reachDistance = (float)value;
							break;
						case 4:
							this.m_myUserProfile.m_eyeInteraxialDistance = (float)value;
							break;
						case 5:
							this.m_myUserProfile.m_screenInteraxialDistance = (float)value;
							break;
						case 6:
							this.m_myUserProfile.m_fovExpanded = (bool)value;
							break;
						default:
							goto IL_146;
						}
						goto IL_17C;
					}
				}
				IL_146:
				Debug.LogError("User profile doesn't have editable property: " + key);
				bool result = false;
				return result;
			}
			catch (InvalidCastException ex)
			{
				Debug.LogError(ex.ToString());
				bool result = false;
				return result;
			}
			IL_17C:
			MetaUserSettingsManager.JSONSaveUserProfile(this._currentUser, this.m_myUserProfile);
			return true;
		}
	}
}
