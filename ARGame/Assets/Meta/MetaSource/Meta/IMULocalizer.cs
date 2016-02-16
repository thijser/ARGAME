using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	public class IMULocalizer : Localizer
	{
		private bool _resetAtStart = true;

		private IMUMotionData _imuData;

		private Quaternion _imu2Gravity;

		private bool _imu2GravityValid;

		public GameObject gravity_arrow;

		public bool resetAtStart
		{
			get
			{
				return this._resetAtStart;
			}
			set
			{
				this._resetAtStart = value;
			}
		}

		public Vector3 imuOrientation
		{
			get
			{
				if (base.gameObject.activeSelf && base.enabled)
				{
					return this._imuData.FusedAngle;
				}
				return new Vector3(0f, 0f, 0f);
			}
		}

		public Vector3 localizerOrientation
		{
			get
			{
				if (base.gameObject.activeSelf && base.enabled)
				{
					return this._imuData.SetAngle;
				}
				return new Vector3(0f, 0f, 0f);
			}
		}

		public Vector3 accelerometerValues
		{
			get
			{
				if (base.gameObject.activeSelf && base.enabled)
				{
					return this._imuData.AccelerometerValues;
				}
				return new Vector3(0f, 0f, 0f);
			}
		}

		public Vector3 gyroscopeValues
		{
			get
			{
				if (base.gameObject.activeSelf && base.enabled)
				{
					return this._imuData.GyroscopeValues;
				}
				return new Vector3(0f, 0f, 0f);
			}
		}

		public Vector3 magnetometerValues
		{
			get
			{
				if (base.gameObject.activeSelf && base.enabled)
				{
					return this._imuData.MagnetometerValues;
				}
				return new Vector3(0f, 0f, 0f);
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "isMotionSensorConnected")]
		internal static extern bool IsMotionSensorConnected();

		public bool IsIMUConnected()
		{
			return IMULocalizer.IsMotionSensorConnected();
		}

		private void Start()
		{
			this._imuData = new IMUMotionData();
			this.InitDataStructs();
			if (this._resetAtStart)
			{
				base.Invoke("CalibrateAfterDelay", 1f);
			}
		}

		private void InitDataStructs()
		{
			if (this._targetGO == null)
			{
				base.SetDefaultTargetGO();
			}
		}

		private void CalibrateAfterDelay()
		{
			this.ResetLocalizer();
		}

		private void Update()
		{
			this._imuData.Update();
			if (this._targetGO == null)
			{
				base.SetDefaultTargetGO();
			}
			this.UpdateTargetGOTransform();
		}

		private void UpdateTargetGOTransform()
		{
			if (!this._imu2GravityValid)
			{
				this.LatchIMU();
			}
			Quaternion rotation;
			if (this._imu2GravityValid)
			{
				rotation = this._imu2Gravity * this._imuData.Compute();
			}
			else
			{
				rotation = this._imuData.Compute();
			}
			this._targetGO.transform.rotation = rotation;
			Vector3 smoothedGravity = this._imuData.SmoothedGravity;
			smoothedGravity.Normalize();
			Debug.DrawLine(new Vector3(0f, 0f, 0f), 10f * smoothedGravity, Color.green);
		}

		public bool LatchIMU()
		{
			Quaternion identity = Quaternion.identity;
			if (!this._imuData.LatchIMU(ref identity))
			{
				return false;
			}
			this._imu2Gravity = Quaternion.Inverse(identity);
			this._imu2GravityValid = true;
			if (this.gravity_arrow != null)
			{
				this.gravity_arrow.transform.rotation = identity;
			}
			return true;
		}

		internal static bool isQuaternionValid(Quaternion Q)
		{
			float num = (float)Math.Sqrt((double)(Q.x * Q.x + Q.y * Q.y + Q.z * Q.z + Q.w * Q.w));
			return Mathf.Abs(num - 1f) > 0.1f;
		}

		public override void ResetLocalizer()
		{
			this._imuData.Reset();
			this._imu2GravityValid = false;
		}
	}
}
