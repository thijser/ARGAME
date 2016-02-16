using System;
using UnityEngine;

namespace Meta
{
	public class MetaCalibration
	{
		public static void UseMetaCalibration(bool useCalibration)
		{
			if (!useCalibration)
			{
				Debug.LogWarning("Disabling the Meta Calibration system can cause unpredictable rendering issues with your glasses. We suggest leaving it on.");
			}
			MetaCamera.SetAllowRealTimePoiUpdate(useCalibration);
			MetaSingleton<RenderingCameraManagerBase>.Instance.m_useExperimentalRendering = useCalibration;
		}
	}
}
