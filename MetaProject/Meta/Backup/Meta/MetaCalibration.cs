// Decompiled with JetBrains decompiler
// Type: Meta.MetaCalibration
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class MetaCalibration
  {
    public static void UseMetaCalibration(bool useCalibration)
    {
      if (!useCalibration)
        Debug.LogWarning((object) "Disabling the Meta Calibration system can cause unpredictable rendering issues with your glasses. We suggest leaving it on.");
      MetaCamera.SetAllowRealTimePoiUpdate(useCalibration);
      MetaSingleton<RenderingCameraManagerBase>.Instance.m_useExperimentalRendering = useCalibration;
    }
  }
}
