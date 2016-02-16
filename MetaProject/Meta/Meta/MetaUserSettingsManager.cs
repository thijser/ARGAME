// Decompiled with JetBrains decompiler
// Type: Meta.MetaUserSettingsManager
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Meta
{
  public class MetaUserSettingsManager : MetaSingleton<MetaUserSettingsManager>
  {
    public bool m_loadUserProfile = true;
    private string _currentUser = string.Empty;
    public bool m_forceLocalProfile;
    private UserSettings _originalUserProfile;
    public UserSettings m_myUserProfile;

    private void Awake()
    {
      if (!this.m_loadUserProfile)
        return;
      if (Object.op_Equality((Object) this.m_myUserProfile, (Object) null))
      {
        this._currentUser = MetaUserSettingsManager.GetCurrentUser(false, this.m_forceLocalProfile);
        if (!MetaUserSettingsManager.JSONLoadUserProfile(this._currentUser, ref this._originalUserProfile, true))
          this._originalUserProfile = (UserSettings) null;
      }
      else
        this._currentUser = ((Object) this.m_myUserProfile).get_name().Replace(".userProfile", string.Empty);
      if (!Object.op_Inequality((Object) this._originalUserProfile, (Object) null))
        return;
      MetaUserSettingsManager.CreateBufferProfile(ref this._originalUserProfile, ref this.m_myUserProfile);
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
      string localUserID = string.Empty;
      if (!localIDOnly)
        localUserID = PlayerPrefs.GetString("stayLoggedInAsUserID");
      if (localUserID == string.Empty)
      {
        if (!loginIDOnly)
        {
          try
          {
            localUserID = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/LocalUserID.txt");
          }
          catch
          {
            Debug.Log((object) "Local user ID not found, loading the MetaUser profile.");
          }
          if (localUserID == string.Empty)
          {
            localUserID = "MetaUser";
            MetaUserSettingsManager.SaveCurrentLocalUser(localUserID);
          }
        }
      }
      return localUserID;
    }

    public static bool SaveCurrentLocalUser(string localUserID)
    {
      try
      {
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/");
        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/LocalUserID.txt", localUserID);
        return true;
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex.ToString());
        return false;
      }
    }

    public static bool JSONLoadUserProfile(string user, ref UserSettings userProfile, bool createIfMissing = true)
    {
      if (!(user != string.Empty))
        return false;
      userProfile = (UserSettings) ScriptableObject.CreateInstance<UserSettings>();
      string jsonString = string.Empty;
      try
      {
        jsonString = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/" + user + ".userProfile");
      }
      catch
      {
        Debug.Log((object) ("JSON user profile not found for user " + user + "."));
      }
      if (jsonString == string.Empty)
      {
        if (!createIfMissing)
          return false;
        UserSettings userProfile1 = (UserSettings) ScriptableObject.CreateInstance<UserSettings>();
        UserSettings.InstantiateNewUserSettings(user, ref userProfile1);
        userProfile1.m_renderingProfile = (RenderingSettings) ScriptableObject.CreateInstance<RenderingSettings>();
        userProfile1.m_renderingProfile.Init(user);
        MetaUserSettingsManager.JSONSaveUserProfile(user, userProfile1);
        return MetaUserSettingsManager.JSONLoadUserProfile(user, ref userProfile, true);
      }
      userProfile.JSONToUserProfile(jsonString);
      ((Object) userProfile).set_name(userProfile.m_ProfileName);
      if (Object.op_Inequality((Object) userProfile.m_truescaleCameraProfile, (Object) null))
        ((Object) userProfile.m_truescaleCameraProfile).set_name(userProfile.m_truescaleCameraProfile.m_ProfileName);
      if (Object.op_Inequality((Object) userProfile.m_widescaleCameraProfile, (Object) null))
        ((Object) userProfile.m_widescaleCameraProfile).set_name(userProfile.m_widescaleCameraProfile.m_ProfileName);
      if (Object.op_Inequality((Object) userProfile.m_renderingProfile, (Object) null))
        ((Object) userProfile.m_renderingProfile).set_name(userProfile.m_renderingProfile.m_ProfileName);
      return true;
    }

    public static bool JSONSaveUserProfile(string user, UserSettings userProfile)
    {
      if (!(user != string.Empty) || !Object.op_Inequality((Object) userProfile, (Object) null))
        return false;
      string contents = userProfile.UserProfileToJSON();
      try
      {
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/");
        File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Personal).Replace("\\", "/") + "/Meta/User Profiles/" + user + ".userProfile", contents);
        return true;
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex.ToString());
        return false;
      }
    }

    public static bool CreateBufferProfile(ref UserSettings originalProfile, ref UserSettings bufferProfile)
    {
      bufferProfile = (UserSettings) ScriptableObject.CreateInstance<UserSettings>();
      if (!Object.op_Inequality((Object) originalProfile, (Object) null))
        return false;
      originalProfile.DeepCopyTo(bufferProfile, false, true);
      ((Object) bufferProfile).set_name(((Object) originalProfile).get_name() + " (buffer)");
      bufferProfile.m_renderingProfile = (RenderingSettings) ScriptableObject.CreateInstance<RenderingSettings>();
      ((Object) bufferProfile.m_renderingProfile).set_name(((Object) bufferProfile.m_renderingProfile).get_name() + " (buffer)");
      if (Object.op_Inequality((Object) originalProfile.m_renderingProfile, (Object) null))
        originalProfile.m_renderingProfile.DeepCopyTo(bufferProfile.m_renderingProfile);
      return true;
    }

    public static bool SaveBufferProfile(ref UserSettings originalProfile, ref UserSettings bufferProfile)
    {
      if (!Object.op_Inequality((Object) originalProfile, (Object) null) || !Object.op_Inequality((Object) bufferProfile, (Object) null))
        return false;
      bufferProfile.DeepCopyTo(originalProfile, true, true);
      if (Object.op_Inequality((Object) bufferProfile.m_renderingProfile, (Object) null))
        bufferProfile.m_renderingProfile.DeepCopyTo(originalProfile.m_renderingProfile);
      return true;
    }

    public bool SaveUserProfile()
    {
      if (MetaUserSettingsManager.SaveBufferProfile(ref this._originalUserProfile, ref this.m_myUserProfile))
        return MetaUserSettingsManager.JSONSaveUserProfile(this._currentUser, this._originalUserProfile);
      return false;
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

    public RenderingSettings GetRenderingProfile()
    {
      if (Object.op_Inequality((Object) this.m_myUserProfile, (Object) null))
        return this.m_myUserProfile.m_renderingProfile;
      return (RenderingSettings) null;
    }

    public DeviceSettings GetCameraProfile(bool trueScale)
    {
      if (!Object.op_Inequality((Object) this.m_myUserProfile, (Object) null))
        return (DeviceSettings) null;
      if (trueScale)
        return this.m_myUserProfile.m_truescaleCameraProfile;
      return this.m_myUserProfile.m_widescaleCameraProfile;
    }

    public Vector3 GetEyeOffset(bool left)
    {
      if (!Object.op_Inequality((Object) this.m_myUserProfile, (Object) null))
        return new Vector3(0.0f, 0.0f, 0.0f);
      if (left)
        return this.m_myUserProfile.m_leftEyeOffset;
      return this.m_myUserProfile.m_rightEyeOffset;
    }

    public float GetReachDistance()
    {
      if (Object.op_Inequality((Object) this.m_myUserProfile, (Object) null))
        return this.m_myUserProfile.m_reachDistance;
      return 0.4f;
    }

    public IMUModel GetIMUModel()
    {
      if (Object.op_Inequality((Object) this.m_myUserProfile, (Object) null))
        return this.m_myUserProfile.m_imuModel;
      return IMUModel.UnknownIMU;
    }

    public bool GetFOVSetting()
    {
      if (Object.op_Inequality((Object) this.m_myUserProfile, (Object) null))
        return this.m_myUserProfile.m_fovExpanded;
      return true;
    }

    public bool SetUserProfileValue(string key, object value)
    {
      try
      {
        string key1 = key;
        if (key1 != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (MetaUserSettingsManager.\u003C\u003Ef__switch\u0024map0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            MetaUserSettingsManager.\u003C\u003Ef__switch\u0024map0 = new Dictionary<string, int>(7)
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
          }
          int num;
          // ISSUE: reference to a compiler-generated field
          if (MetaUserSettingsManager.\u003C\u003Ef__switch\u0024map0.TryGetValue(key1, out num))
          {
            switch (num)
            {
              case 0:
                this.m_myUserProfile.m_imuModel = (IMUModel) value;
                goto label_14;
              case 1:
                this.m_myUserProfile.m_leftEyeOffset = (Vector3) value;
                goto label_14;
              case 2:
                this.m_myUserProfile.m_rightEyeOffset = (Vector3) value;
                goto label_14;
              case 3:
                this.m_myUserProfile.m_reachDistance = (float) value;
                goto label_14;
              case 4:
                this.m_myUserProfile.m_eyeInteraxialDistance = (float) value;
                goto label_14;
              case 5:
                this.m_myUserProfile.m_screenInteraxialDistance = (float) value;
                goto label_14;
              case 6:
                this.m_myUserProfile.m_fovExpanded = (bool) value;
                goto label_14;
            }
          }
        }
        Debug.LogError((object) ("User profile doesn't have editable property: " + key));
        return false;
      }
      catch (InvalidCastException ex)
      {
        Debug.LogError((object) ex.ToString());
        return false;
      }
label_14:
      MetaUserSettingsManager.JSONSaveUserProfile(this._currentUser, this.m_myUserProfile);
      return true;
    }
  }
}
