using System;
using UnityEngine;

namespace Meta
{
	public class DeviceSettings : ScriptableObject
	{
		[SerializeField]
		internal string m_ProfileName = "Default profile";

		[SerializeField]
		internal bool m_DefaultProfile = true;

		[SerializeField]
		internal DeviceType m_device;

		[Range(50f, 75f), SerializeField]
		internal float m_screenInteraxialDistance = 60f;

		[Range(-20f, 20f), SerializeField]
		internal float m_horizontalScreenOffset;

		[Range(-20f, 20f), SerializeField]
		internal float m_verticalScreenOffset;

		[SerializeField]
		internal Vector3 m_sensorScreenOffset = new Vector3(0f, 0f, 0f);

		[Range(-30f, 30f), SerializeField]
		internal float m_sensorScreenAngle;

		[SerializeField]
		internal bool m_useCustomAspectRatio;

		[SerializeField]
		internal float m_customAspectRatio = 1f;

		[Range(0.01f, 0.03f), SerializeField]
		internal float m_screenWidth = 0.0175f;

		[Range(0.01f, 0.03f), SerializeField]
		internal float m_screenHeight = 0.015f;

		[Range(50f, 75f), SerializeField]
		internal float m_eyeInteraxialDistance = 64f;

		[Range(0f, 0.04f), SerializeField]
		internal float m_eyeballRadius = 0.024f;

		[Range(0f, 0.1f), SerializeField]
		internal float m_nearPlaneDistance = 0.02f;

		[Range(0.01f, 50f), SerializeField]
		internal float m_farPlaneDistance = 30f;

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
