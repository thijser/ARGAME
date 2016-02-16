using System;
using UnityEngine;

namespace Meta
{
	public class RenderingSettings : ScriptableObject
	{
		public string m_ProfileName = "Default profile";

		public float m_hNear = 1073.5f;

		public float m_hFar = 630f;

		public float m_xNearLeft = 250f;

		public float m_xFarLeft = 126.6f;

		public float m_physicalSize = 0.12f;

		public float m_screenWidth = 1280f;

		public float m_screenHeight = 720f;

		public float m_physicalSpaceBetween = 0.3175f;

		public float m_worldNearDepth = 0.381f;

		public float m_desiredNearPoint = 0.02f;

		public float m_farPlaneDistance = 10000f;

		public float m_xNearRightOffset;

		public float m_xFarRightOffset;

		public float m_yNearLeft;

		public float m_yFarLeft;

		public float m_yNearRight;

		public float m_yFarRight;

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
