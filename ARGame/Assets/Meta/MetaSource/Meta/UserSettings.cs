using FullSerializer;
using System;
using UnityEngine;

namespace Meta
{
	public class UserSettings : ScriptableObject
	{
		public string m_ProfileName = "Default profile";

		public IMUModel m_imuModel = IMUModel.UnknownIMU;

		public Vector3 m_leftEyeOffset = default(Vector3);

		public Vector3 m_rightEyeOffset = default(Vector3);

		[Range(0.1f, 1.5f)]
		public float m_reachDistance = 0.4f;

		[Range(50f, 75f)]
		public float m_eyeInteraxialDistance = 64f;

		[Range(50f, 75f)]
		public float m_screenInteraxialDistance = 60f;

		public RenderingSettings m_renderingProfile;

		public DeviceSettings m_truescaleCameraProfile;

		public DeviceSettings m_widescaleCameraProfile;

		public bool m_fovExpanded = true;

		public static void ResetUserSettings(ref UserSettings userProfile, UserSettings baseProfile = null, bool revertToDefault = false)
		{
			userProfile = ScriptableObject.CreateInstance<UserSettings>();
			if (baseProfile != null)
			{
				if (revertToDefault)
				{
					userProfile.Init(baseProfile.name.Replace(".userProfile", string.Empty));
				}
				else
				{
					baseProfile.DeepCopyTo(userProfile, false, false);
				}
				userProfile.name = baseProfile.name;
			}
		}

		public static void InstantiateNewUserSettings(string user, ref UserSettings userProfile)
		{
			userProfile = ScriptableObject.CreateInstance<UserSettings>();
			userProfile.name = user + ".userProfile";
			userProfile.Init(user);
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

		public void Init(string profileName)
		{
			this.m_ProfileName = profileName;
			MetaCore component;
			if (GameObject.Find("MetaFrame") != null && (component = GameObject.Find("MetaFrame").GetComponent<MetaCore>()) != null)
			{
				if (component.m_defaultMeta1TruescaleProfile != null)
				{
					this.m_truescaleCameraProfile = component.m_defaultMeta1TruescaleProfile;
				}
				else
				{
					Debug.LogError("No default Truescale profile in MetaCore.");
				}
				if (component.m_defaultMeta1WidescaleProfile != null)
				{
					this.m_widescaleCameraProfile = component.m_defaultMeta1WidescaleProfile;
				}
				else
				{
					Debug.LogError("No default Widescale profile in MetaCore.");
				}
			}
			if (this.m_renderingProfile != null)
			{
				this.m_renderingProfile.Init(profileName);
			}
		}

		public void DeepCopyTo(UserSettings destination, bool ignoreCameraProfiles = false, bool ignoreRenderingProfile = false)
		{
			destination.m_ProfileName = this.m_ProfileName;
			destination.m_imuModel = this.m_imuModel;
			destination.m_leftEyeOffset = this.m_leftEyeOffset;
			destination.m_rightEyeOffset = this.m_rightEyeOffset;
			destination.m_reachDistance = this.m_reachDistance;
			destination.m_eyeInteraxialDistance = this.m_eyeInteraxialDistance;
			destination.m_screenInteraxialDistance = this.m_screenInteraxialDistance;
			destination.m_fovExpanded = this.m_fovExpanded;
			if (!ignoreRenderingProfile)
			{
				destination.m_renderingProfile = this.m_renderingProfile;
			}
			if (!ignoreCameraProfiles)
			{
				destination.m_truescaleCameraProfile = this.m_truescaleCameraProfile;
				destination.m_widescaleCameraProfile = this.m_widescaleCameraProfile;
			}
		}

		public string UserProfileToJSON()
		{
			fsSerializer fsSerializer = new fsSerializer();
			fsData fsData;
			fsFailure fsFailure = fsSerializer.TrySerialize(typeof(UserSettings), this, out fsData);
			if (fsFailure.Failed)
			{
				throw new Exception(fsFailure.FailureReason);
			}
			return fsJsonPrinter.PrettyJson(fsData);
		}

		public bool JSONToUserProfile(string jsonString)
		{
			fsSerializer fsSerializer = new fsSerializer();
			fsData fsData;
			fsFailure fsFailure = fsJsonParser.Parse(jsonString, out fsData);
			if (fsFailure.Failed)
			{
				throw new Exception(fsFailure.FailureReason);
			}
			object obj = null;
			fsFailure = fsSerializer.TryDeserialize(fsData, typeof(UserSettings), ref obj);
			if (fsFailure.Failed)
			{
				throw new Exception(fsFailure.FailureReason);
			}
			UserSettings userSettings = (UserSettings)obj;
			userSettings.DeepCopyTo(this, false, false);
			return true;
		}
	}
}
