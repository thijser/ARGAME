// Decompiled with JetBrains decompiler
// Type: Meta.DeviceSettings
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class DeviceSettings : ScriptableObject
  {
    [SerializeField]
    internal string m_ProfileName;
    [SerializeField]
    internal bool m_DefaultProfile;
    [SerializeField]
    internal DeviceType m_device;
    [Range(50f, 75f)]
    [SerializeField]
    internal float m_screenInteraxialDistance;
    [SerializeField]
    [Range(-20f, 20f)]
    internal float m_horizontalScreenOffset;
    [SerializeField]
    [Range(-20f, 20f)]
    internal float m_verticalScreenOffset;
    [SerializeField]
    internal Vector3 m_sensorScreenOffset;
    [SerializeField]
    [Range(-30f, 30f)]
    internal float m_sensorScreenAngle;
    [SerializeField]
    internal bool m_useCustomAspectRatio;
    [SerializeField]
    internal float m_customAspectRatio;
    [SerializeField]
    [Range(0.01f, 0.03f)]
    internal float m_screenWidth;
    [SerializeField]
    [Range(0.01f, 0.03f)]
    internal float m_screenHeight;
    [SerializeField]
    [Range(50f, 75f)]
    internal float m_eyeInteraxialDistance;
    [SerializeField]
    [Range(0.0f, 0.04f)]
    internal float m_eyeballRadius;
    [SerializeField]
    [Range(0.0f, 0.1f)]
    internal float m_nearPlaneDistance;
    [Range(0.01f, 50f)]
    [SerializeField]
    internal float m_farPlaneDistance;

    public DeviceSettings()
    {
      base.\u002Ector();
    }

    internal void DeepCopyTo(DeviceSettings destination)
    {
      destination.m_ProfileName = this.m_ProfileName;
      destination.m_device = this.m_device;
      destination.m_screenInteraxialDistance = this.m_screenInteraxialDistance;
      destination.m_horizontalScreenOffset = this.m_horizontalScreenOffset;
      destination.m_verticalScreenOffset = this.m_verticalScreenOffset;
      destination.m_sensorScreenOffset = this.m_sensorScreenOffset;
      destination.m_sensorScreenAngle = this.m_sensorScreenAngle;
      destination.m_useCustomAspectRatio = this.m_useCustomAspectRatio;
      destination.m_customAspectRatio = this.m_customAspectRatio;
      destination.m_screenWidth = this.m_screenWidth;
      destination.m_screenHeight = this.m_screenHeight;
      destination.m_eyeInteraxialDistance = this.m_eyeInteraxialDistance;
      destination.m_eyeballRadius = this.m_eyeballRadius;
      destination.m_nearPlaneDistance = this.m_nearPlaneDistance;
      destination.m_farPlaneDistance = this.m_farPlaneDistance;
      destination.m_DefaultProfile = false;
    }
  }
}
