using System;
using UnityEngine;

namespace Meta
{
	internal class KalmanFilterClient
	{
		private float m_KalmanVelocity = 100f;

		private int m_KalmanID = -1;

		private int m_KalmanIDRot = -1;

		private int m_KalmanIDW = -1;

		private float xrot;

		private float yrot;

		private float zrot;

		private float wrot;

		private float dummy;

		private float dummy2;

		public float kalmanVelocity
		{
			get
			{
				return this.m_KalmanVelocity;
			}
			set
			{
				this.m_KalmanVelocity = value;
			}
		}

		public void KalmanFilterSmoothTransform(Transform transform, out Vector3 position, out Quaternion rotation)
		{
			bool flag = false;
			if (this.m_KalmanID == -1)
			{
				KalmanFilter.GetNewID(ref this.m_KalmanID, true, this.m_KalmanVelocity);
				KalmanFilter.GetNewID(ref this.m_KalmanIDRot, true, this.m_KalmanVelocity);
				KalmanFilter.GetNewID(ref this.m_KalmanIDW, true, this.m_KalmanVelocity);
				flag = true;
			}
			Quaternion rotation2 = transform.rotation;
			if (flag)
			{
				KalmanFilter.InitKalman(this.m_KalmanIDRot, rotation2.x, rotation2.y, rotation2.z);
				KalmanFilter.InitKalman(this.m_KalmanIDW, rotation2.w, 0f, 0f);
			}
			this.xrot = rotation2.x;
			this.yrot = rotation2.y;
			this.zrot = rotation2.z;
			this.wrot = rotation2.w;
			this.dummy = 0f;
			this.dummy2 = 0f;
			KalmanFilter.UpdateKalman(this.m_KalmanIDRot, ref this.xrot, ref this.yrot, ref this.zrot, this.m_KalmanVelocity);
			KalmanFilter.UpdateKalman(this.m_KalmanIDW, ref this.wrot, ref this.dummy, ref this.dummy2, this.m_KalmanVelocity);
			if (!float.IsNaN(this.xrot))
			{
				rotation = new Quaternion(this.xrot, this.yrot, this.zrot, this.wrot);
			}
			else
			{
				Debug.LogError("UpdateTransform: Quaternion.x is NaN.");
				rotation = transform.rotation;
			}
			Vector3 position2 = transform.position;
			if (flag)
			{
				KalmanFilter.InitKalman(this.m_KalmanID, position2.x, position2.y, position2.z);
			}
			float x = position2.x;
			float y = position2.y;
			float z = position2.z;
			KalmanFilter.UpdateKalman(this.m_KalmanID, ref x, ref y, ref z, this.m_KalmanVelocity);
			position = new Vector3(x, y, z);
		}
	}
}
