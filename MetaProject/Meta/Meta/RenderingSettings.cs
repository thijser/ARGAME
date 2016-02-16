// Decompiled with JetBrains decompiler
// Type: Meta.RenderingSettings
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class RenderingSettings : ScriptableObject
  {
    public string m_ProfileName;
    public float m_hNear;
    public float m_hFar;
    public float m_xNearLeft;
    public float m_xFarLeft;
    public float m_physicalSize;
    public float m_screenWidth;
    public float m_screenHeight;
    public float m_physicalSpaceBetween;
    public float m_worldNearDepth;
    public float m_desiredNearPoint;
    public float m_farPlaneDistance;
    public float m_xNearRightOffset;
    public float m_xFarRightOffset;
    public float m_yNearLeft;
    public float m_yFarLeft;
    public float m_yNearRight;
    public float m_yFarRight;

    public RenderingSettings()
    {
      base.\u002Ector();
    }

    public void Init(string profileName)
    {
      this.m_ProfileName = profileName;
    }

    public void DeepCopyTo(RenderingSettings destination)
    {
      destination.m_hNear = this.m_hNear;
      destination.m_hFar = this.m_hFar;
      destination.m_xNearLeft = this.m_xNearLeft;
      destination.m_xFarLeft = this.m_xFarLeft;
      destination.m_physicalSize = this.m_physicalSize;
      destination.m_screenWidth = this.m_screenWidth;
      destination.m_screenHeight = this.m_screenHeight;
      destination.m_physicalSpaceBetween = this.m_physicalSpaceBetween;
      destination.m_worldNearDepth = this.m_worldNearDepth;
      destination.m_desiredNearPoint = this.m_desiredNearPoint;
      destination.m_farPlaneDistance = this.m_farPlaneDistance;
      destination.m_xNearRightOffset = this.m_xNearRightOffset;
      destination.m_xFarRightOffset = this.m_xFarRightOffset;
      destination.m_yNearLeft = this.m_yNearLeft;
      destination.m_yFarLeft = this.m_yFarLeft;
      destination.m_yNearRight = this.m_yNearRight;
      destination.m_yFarRight = this.m_yFarRight;
    }
  }
}
