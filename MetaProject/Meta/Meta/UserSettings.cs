// Decompiled with JetBrains decompiler
// Type: Meta.UserSettings
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using FullSerializer;
using System;
using UnityEngine;

namespace Meta
{
  public class UserSettings : ScriptableObject
  {
    public string m_ProfileName;
    public IMUModel m_imuModel;
    public Vector3 m_leftEyeOffset;
    public Vector3 m_rightEyeOffset;
    [Range(0.1f, 1.5f)]
    public float m_reachDistance;
    [Range(50f, 75f)]
    public float m_eyeInteraxialDistance;
    [Range(50f, 75f)]
    public float m_screenInteraxialDistance;
    public RenderingSettings m_renderingProfile;
    public DeviceSettings m_truescaleCameraProfile;
    public DeviceSettings m_widescaleCameraProfile;
    public bool m_fovExpanded;

    public UserSettings()
    {
      base.\u002Ector();
    }

    public static void ResetUserSettings(ref UserSettings userProfile, UserSettings baseProfile = null, bool revertToDefault = false)
    {
      userProfile = (UserSettings) ScriptableObject.CreateInstance<UserSettings>();
      if (!Object.op_Inequality((Object) baseProfile, (Object) null))
        return;
      if (revertToDefault)
        userProfile.Init(((Object) baseProfile).get_name().Replace(".userProfile", string.Empty));
      else
        baseProfile.DeepCopyTo(userProfile, false, false);
      ((Object) userProfile).set_name(((Object) baseProfile).get_name());
    }

    public static void InstantiateNewUserSettings(string user, ref UserSettings userProfile)
    {
      userProfile = (UserSettings) ScriptableObject.CreateInstance<UserSettings>();
      ((Object) userProfile).set_name(user + ".userProfile");
      userProfile.Init(user);
    }

    public static bool MakeCameraProfileWithIPD(UserSettings userProfile, DeviceSettings cameraProfile, ref DeviceSettings cameraProfileWithIPD, bool trueScale = true)
    {
      cameraProfileWithIPD = (DeviceSettings) ScriptableObject.CreateInstance<DeviceSettings>();
      if (!Object.op_Inequality((Object) userProfile, (Object) null) || !Object.op_Inequality((Object) cameraProfile, (Object) null))
        return false;
      if (trueScale)
        ((Object) cameraProfileWithIPD).set_name("User Truescale Profile");
      else
        ((Object) cameraProfileWithIPD).set_name("User Widescale Profile");
      cameraProfile.DeepCopyTo(cameraProfileWithIPD);
      cameraProfileWithIPD.m_eyeInteraxialDistance = userProfile.m_eyeInteraxialDistance;
      cameraProfileWithIPD.m_screenInteraxialDistance = userProfile.m_screenInteraxialDistance;
      return true;
    }

    public void Init(string profileName)
    {
      this.m_ProfileName = profileName;
      MetaCore metaCore;
      if (Object.op_Inequality((Object) GameObject.Find("MetaFrame"), (Object) null) && Object.op_Inequality((Object) (metaCore = (MetaCore) GameObject.Find("MetaFrame").GetComponent<MetaCore>()), (Object) null))
      {
        if (Object.op_Inequality((Object) metaCore.m_defaultMeta1TruescaleProfile, (Object) null))
          this.m_truescaleCameraProfile = metaCore.m_defaultMeta1TruescaleProfile;
        else
          Debug.LogError((object) "No default Truescale profile in MetaCore.");
        if (Object.op_Inequality((Object) metaCore.m_defaultMeta1WidescaleProfile, (Object) null))
          this.m_widescaleCameraProfile = metaCore.m_defaultMeta1WidescaleProfile;
        else
          Debug.LogError((object) "No default Widescale profile in MetaCore.");
      }
      if (!Object.op_Inequality((Object) this.m_renderingProfile, (Object) null))
        return;
      this.m_renderingProfile.Init(profileName);
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
        destination.m_renderingProfile = this.m_renderingProfile;
      if (ignoreCameraProfiles)
        return;
      destination.m_truescaleCameraProfile = this.m_truescaleCameraProfile;
      destination.m_widescaleCameraProfile = this.m_widescaleCameraProfile;
    }

    public string UserProfileToJSON()
    {
      fsData fsData;
      fsFailure fsFailure = new fsSerializer().TrySerialize(typeof (UserSettings), (object) this, ref fsData);
      if (fsFailure.get_Failed())
        throw new Exception(fsFailure.get_FailureReason());
      return fsJsonPrinter.PrettyJson(fsData);
    }

    public bool JSONToUserProfile(string jsonString)
    {
      fsSerializer fsSerializer = new fsSerializer();
      fsData fsData;
      fsFailure fsFailure1 = fsJsonParser.Parse(jsonString, ref fsData);
      if (fsFailure1.get_Failed())
        throw new Exception(fsFailure1.get_FailureReason());
      object obj = (object) null;
      fsFailure fsFailure2 = fsSerializer.TryDeserialize(fsData, typeof (UserSettings), ref obj);
      if (fsFailure2.get_Failed())
        throw new Exception(fsFailure2.get_FailureReason());
      ((UserSettings) obj).DeepCopyTo(this, false, false);
      return true;
    }
  }
}
