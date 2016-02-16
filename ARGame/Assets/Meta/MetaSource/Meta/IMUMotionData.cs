using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	internal class IMUMotionData
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct MotionSensorData
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public double[] gyroscopeValues;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public double[] accelerometerValues;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public double[] magnetometerValues;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public double[] fusedAngle;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public double[] orientation;

			private double timestamp;
		}

		private IMUMotionData.MotionSensorData motionSensorData;

		private Quaternion _orientation;

		private Quaternion _correctionOrientation;

		private Vector3 _correctionVector;

		private Vector3 _fusedAngle;

		private Vector3 _setAngle;

		private Vector3 _accelerometerValues;

		private Vector3 _magnetometerValues;

		private Vector3 _gyroscopeValues;

		private Vector3 _smoothedGravity = default(Vector3);

		private float _lambdaGravity = 0.5f;

		private bool _firstGravity = true;

		public Quaternion Orientation
		{
			get
			{
				return this._orientation;
			}
		}

		public Quaternion CorrectionOrientation
		{
			get
			{
				return this._correctionOrientation;
			}
		}

		public Vector3 CorrectionVector
		{
			get
			{
				return this._correctionVector;
			}
			set
			{
				this._correctionVector = value;
			}
		}

		public Vector3 FusedAngle
		{
			get
			{
				return this._fusedAngle;
			}
		}

		public Vector3 SetAngle
		{
			get
			{
				return this._setAngle;
			}
		}

		public Vector3 AccelerometerValues
		{
			get
			{
				return this._accelerometerValues;
			}
		}

		public Vector3 MagnetometerValues
		{
			get
			{
				return this._magnetometerValues;
			}
		}

		public Vector3 GyroscopeValues
		{
			get
			{
				return this._gyroscopeValues;
			}
		}

		public Vector3 SmoothedGravity
		{
			get
			{
				return this._smoothedGravity;
			}
		}

		public float LambdaGravity
		{
			get
			{
				return this._lambdaGravity;
			}
			set
			{
				this._lambdaGravity = Math.Min(1f, Math.Max(value, 0.001f));
			}
		}

		public Vector3 Gravity
		{
			get
			{
				Vector3 result = default(Vector3);
				double num = this.motionSensorData.accelerometerValues[0];
				double num2 = this.motionSensorData.accelerometerValues[1];
				double num3 = this.motionSensorData.accelerometerValues[2];
				double num4 = Math.Sqrt(num2 * num2 + num3 * num3);
				double num5 = (num3 >= 0.0) ? 1.0 : -1.0;
				double num6 = (Math.Abs(num3) >= 1E-06) ? num3 : (num5 * 1E-06);
				double num7 = Math.Atan(num2 / num6);
				double num8 = Math.Atan(num / (num4 + 1E-06));
				double num9 = Math.Cos(num8);
				double num10 = Math.Sin(num8);
				double num11 = Math.Cos(num7);
				double num12 = Math.Sin(num7);
				result[0] = (float)num10;
				result[1] = -(float)(num11 * num9);
				result[2] = (float)(num9 * num12);
				result.Normalize();
				return result;
			}
		}

		public IMUMotionData()
		{
			this.motionSensorData.accelerometerValues = new double[3];
			this.motionSensorData.gyroscopeValues = new double[3];
			this.motionSensorData.magnetometerValues = new double[3];
			this.motionSensorData.orientation = new double[3];
			this.motionSensorData.fusedAngle = new double[3];
			this.InitDataStructs();
		}

		[DllImport("MetaVisionDLL", EntryPoint = "getMotionSensorData")]
		internal static extern void GetMotionSensorData(ref IMUMotionData.MotionSensorData motionSensorData);

		private void InitDataStructs()
		{
			this._orientation = (this._correctionOrientation = Quaternion.identity);
			this._correctionVector = (this._fusedAngle = (this._setAngle = (this._accelerometerValues = (this._magnetometerValues = (this._gyroscopeValues = Vector3.zero)))));
			for (int i = 0; i < 3; i++)
			{
				this.motionSensorData.accelerometerValues[i] = (this.motionSensorData.gyroscopeValues[i] = (this.motionSensorData.magnetometerValues[i] = (this.motionSensorData.orientation[i] = (this.motionSensorData.fusedAngle[i] = 0.0))));
			}
			this._firstGravity = true;
		}

		public void Update()
		{
			IMUMotionData.GetMotionSensorData(ref this.motionSensorData);
			this._orientation.w = (float)this.motionSensorData.orientation[0];
			this._orientation.x = (float)this.motionSensorData.orientation[1];
			this._orientation.y = -(float)this.motionSensorData.orientation[3];
			this._orientation.z = (float)this.motionSensorData.orientation[2];
			if (MetaCore.Instance.imuModel == IMUModel.MPU9150HID)
			{
				this._fusedAngle.x = (float)(this.motionSensorData.fusedAngle[0] * 180.0 / 3.1415926535897931);
				this._fusedAngle.y = (float)(this.motionSensorData.fusedAngle[2] * 180.0 / 3.1415926535897931);
				this._fusedAngle.z = -(float)(this.motionSensorData.fusedAngle[1] * 180.0 / 3.1415926535897931);
			}
			else
			{
				this._fusedAngle.x = (float)(this.motionSensorData.fusedAngle[0] * 180.0 / 3.1415926535897931);
				this._fusedAngle.y = -(float)(this.motionSensorData.fusedAngle[2] * 180.0 / 3.1415926535897931);
				this._fusedAngle.z = (float)(this.motionSensorData.fusedAngle[1] * 180.0 / 3.1415926535897931);
			}
			this._accelerometerValues.x = (float)this.motionSensorData.accelerometerValues[0];
			this._accelerometerValues.y = -(float)this.motionSensorData.accelerometerValues[2];
			this._accelerometerValues.z = (float)this.motionSensorData.accelerometerValues[1];
			this._gyroscopeValues.x = (float)this.motionSensorData.gyroscopeValues[0];
			this._gyroscopeValues.y = -(float)this.motionSensorData.gyroscopeValues[2];
			this._gyroscopeValues.z = (float)this.motionSensorData.gyroscopeValues[1];
			this._magnetometerValues.x = (float)this.motionSensorData.magnetometerValues[0];
			this._magnetometerValues.y = -(float)this.motionSensorData.magnetometerValues[2];
			this._magnetometerValues.z = (float)this.motionSensorData.magnetometerValues[1];
			Quaternion q = this.Compute();
			if (IMUMotionData.isNormalizedQuaternionValid(q))
			{
				if (this._firstGravity)
				{
					this._firstGravity = false;
					this._smoothedGravity = this.Gravity;
				}
				else
				{
					this._smoothedGravity = (this._lambdaGravity * this._smoothedGravity + this.Gravity) / (1f + this._lambdaGravity);
				}
			}
		}

		internal Quaternion Compute()
		{
			this._setAngle = this._fusedAngle - this._correctionVector;
			return Quaternion.Euler(this._setAngle);
		}

		public void Reset()
		{
			if (IMUMotionData.isNormalizedQuaternionValid(this._orientation))
			{
				this._correctionOrientation = this._orientation;
			}
			else
			{
				this._correctionOrientation = Quaternion.identity;
			}
			this._correctionVector = this._fusedAngle;
			this._firstGravity = true;
		}

		internal static bool isNormalizedQuaternionValid(Quaternion Q)
		{
			float num = (float)Math.Sqrt((double)(Q.x * Q.x + Q.y * Q.y + Q.z * Q.z + Q.w * Q.w));
			return Mathf.Abs(num - 1f) < 0.1f;
		}

		internal static bool isQuaternionValid(Quaternion Q)
		{
			float num = (float)Math.Sqrt((double)(Q.x * Q.x + Q.y * Q.y + Q.z * Q.z + Q.w * Q.w));
			return Mathf.Abs(num) > 1E-06f;
		}

		public bool LatchIMU(ref Quaternion oVirt2Gravity)
		{
			Quaternion q = this.Compute();
			if (!IMUMotionData.isNormalizedQuaternionValid(q))
			{
				return false;
			}
			Vector3 smoothedGravity = this.SmoothedGravity;
			smoothedGravity.Normalize();
			Vector3 vector = new Vector3(0f, -1f, 0f);
			Vector3 vector2 = Vector3.Cross(vector, smoothedGravity);
			if ((double)vector2.sqrMagnitude < 1E-07)
			{
				return false;
			}
			Vector3.Cross(smoothedGravity, vector2).Normalize();
			float num = Vector3.Dot(vector, smoothedGravity);
			double num2 = Math.Acos((double)num);
			num2 *= 57.295779513082323;
			vector2.Normalize();
			oVirt2Gravity = Quaternion.AngleAxis((float)num2, vector2);
			return true;
		}
	}
}
