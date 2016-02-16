using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Meta
{
	internal class SLAMPPLocalizer : Localizer
	{
		public enum SLAMLocalizerState
		{
			atStart,
			waitIMU,
			inError,
			inTracking,
			inRelocalization
		}

		private double[] _trans = new double[3];

		private double[] _quat = new double[4];

		public GameObject gravity_arrow;

		private Quaternion _quaternion;

		private IMUMotionData _imu;

		private Quaternion _imuOrientation;

		private Quaternion _initSlam2World;

		private Quaternion _slam2Gravity;

		[SerializeField]
		private Image slamTimerImage;

		[SerializeField]
		private GameObject slamArrowGO;

		private SLAMPPLocalizer.SLAMLocalizerState _state;

		private DateTime _timeNotTracking = DateTime.Now;

		public int m_maxNotTracking = 3;

		public bool m_useIMU;

		public bool m_autoRelocalize = true;

		public SLAMPPLocalizer.SLAMLocalizerState State
		{
			get
			{
				return this._state;
			}
		}

		public bool AreTracking
		{
			get
			{
				return this._state == SLAMPPLocalizer.SLAMLocalizerState.inTracking;
			}
		}

		public int TimeNotTracking
		{
			get
			{
				switch (this._state)
				{
				case SLAMPPLocalizer.SLAMLocalizerState.atStart:
				case SLAMPPLocalizer.SLAMLocalizerState.inTracking:
					return 0;
				}
				return (DateTime.Now - this._timeNotTracking).Seconds;
			}
		}

		public float TimeNotTrackingFloat
		{
			get
			{
				switch (this._state)
				{
				case SLAMPPLocalizer.SLAMLocalizerState.atStart:
				case SLAMPPLocalizer.SLAMLocalizerState.inTracking:
					return 0f;
				}
				return (float)(DateTime.Now - this._timeNotTracking).TotalSeconds;
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "registerSLAM")]
		protected static extern void RegisterSLAM([MarshalAs(UnmanagedType.BStr)] string iSlamName, float iRelThresh);

		[DllImport("MetaVisionDLL", EntryPoint = "enableSLAM")]
		protected static extern void EnableSLAM();

		[DllImport("MetaVisionDLL", EntryPoint = "relocalizeSLAM")]
		protected static extern void RelocalizeSLAM();

		[DllImport("MetaVisionDLL", EntryPoint = "resetSLAM")]
		protected static extern void ResetSLAM();

		[DllImport("MetaVisionDLL", EntryPoint = "CameraSLAMMetaWorldToCamera")]
		protected static extern int CameraSlamMetaWorldToCamera([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

		[DllImport("MetaVisionDLL", EntryPoint = "CameraSLAMToMetaWorld")]
		protected static extern int CameraSlamToMetaWorld([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

		[DllImport("MetaVisionDLL", EntryPoint = "deltaSLAMMetaWorldCamera")]
		protected static extern int DeltaSlamMetaWorldCamera([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

		[DllImport("MetaVisionDLL", EntryPoint = "deltaSLAMMetaWorld")]
		protected static extern int DeltaSlamMetaWorld([MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] double[] oTrans, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] double[] oQuaternion);

		public void UpdateSLAMTimer()
		{
			if (this._state == SLAMPPLocalizer.SLAMLocalizerState.inRelocalization)
			{
				this.slamTimerImage.enabled = true;
				this.slamTimerImage.fillAmount = this.TimeNotTrackingFloat / (float)(this.m_maxNotTracking + 1);
				if (!this.slamArrowGO.activeSelf)
				{
					this.slamArrowGO.SetActive(true);
				}
			}
			else
			{
				this.slamTimerImage.enabled = false;
				if (this.slamArrowGO.activeSelf)
				{
					this.slamArrowGO.SetActive(false);
				}
			}
		}

		private void Start()
		{
			Debug.Log("Starting SLAM");
			SLAMPPLocalizer.RegisterSLAM("slampp", 0.1f);
			this._imuOrientation = Quaternion.identity;
			if (this.m_useIMU)
			{
				this._imu = new IMUMotionData();
			}
		}

		public void Update()
		{
			if (this._targetGO == null)
			{
				base.SetDefaultTargetGO();
			}
			this.UpdateTargetGOTransform();
			this.UpdateSLAMTimer();
		}

		public static bool StateInRelocalization(SLAMPPLocalizer.SLAMLocalizerState iState)
		{
			return iState == SLAMPPLocalizer.SLAMLocalizerState.inRelocalization || iState == SLAMPPLocalizer.SLAMLocalizerState.inError;
		}

		public static bool StateAtStart(SLAMPPLocalizer.SLAMLocalizerState iState)
		{
			return iState == SLAMPPLocalizer.SLAMLocalizerState.atStart || iState == SLAMPPLocalizer.SLAMLocalizerState.waitIMU;
		}

		private void StateTransition(SLAMPPLocalizer.SLAMLocalizerState iNewState)
		{
			bool flag = SLAMPPLocalizer.StateInRelocalization(iNewState);
			if (this._state != iNewState && SLAMPPLocalizer.StateInRelocalization(this._state) != flag && flag)
			{
				if (SLAMPPLocalizer.StateAtStart(this._state))
				{
					return;
				}
				Debug.Log("Relocalizing SLAM");
				this._timeNotTracking = DateTime.Now;
			}
			if (this.m_autoRelocalize && flag && this.TimeNotTracking > this.m_maxNotTracking)
			{
				this.ResetLocalizer();
				return;
			}
			SLAMPPLocalizer.SLAMLocalizerState sLAMLocalizerState;
			if (SLAMPPLocalizer.StateAtStart(this._state) && !flag)
			{
				if (this._state == SLAMPPLocalizer.SLAMLocalizerState.waitIMU)
				{
					sLAMLocalizerState = this.LatchIMU(iNewState);
				}
				else
				{
					sLAMLocalizerState = this.LatchCurrent(iNewState);
					if (SLAMPPLocalizer.StateInRelocalization(sLAMLocalizerState))
					{
						this._timeNotTracking = DateTime.Now;
					}
				}
			}
			else
			{
				sLAMLocalizerState = iNewState;
			}
			this._state = sLAMLocalizerState;
		}

		internal SLAMPPLocalizer.SLAMLocalizerState LatchCurrent(SLAMPPLocalizer.SLAMLocalizerState iReqState)
		{
			if (!IMUMotionData.isQuaternionValid(this._quaternion))
			{
				return SLAMPPLocalizer.SLAMLocalizerState.inError;
			}
			SLAMPPLocalizer.SLAMLocalizerState result = iReqState;
			if (this._imu != null)
			{
				result = this.LatchIMU(iReqState);
			}
			return result;
		}

		public SLAMPPLocalizer.SLAMLocalizerState LatchIMU(SLAMPPLocalizer.SLAMLocalizerState iReqState)
		{
			Quaternion identity = Quaternion.identity;
			if (!this._imu.LatchIMU(ref identity))
			{
				return SLAMPPLocalizer.SLAMLocalizerState.waitIMU;
			}
			this._imuOrientation = identity;
			this._slam2Gravity = Quaternion.Inverse(this._imuOrientation);
			if (this.gravity_arrow != null)
			{
				this.gravity_arrow.transform.rotation = this._imuOrientation;
			}
			return iReqState;
		}

		public void UpdateTargetGOTransform()
		{
			if (this._imu != null)
			{
				this._imu.Update();
			}
			int num = SLAMPPLocalizer.CameraSlamToMetaWorld(this._trans, this._quat);
			if (num == 0)
			{
				this._quaternion = this.From();
				if (this._state != SLAMPPLocalizer.SLAMLocalizerState.waitIMU)
				{
					this._targetGO.transform.position = new Vector3((float)this._trans[0], (float)this._trans[1], (float)this._trans[2]);
				}
				this.slamArrowGO.transform.position = -this._targetGO.transform.position;
				this.StateTransition(SLAMPPLocalizer.SLAMLocalizerState.inTracking);
				this._targetGO.transform.rotation = this._slam2Gravity * this._quaternion;
				Vector3 smoothedGravity = this._imu.SmoothedGravity;
				smoothedGravity.Normalize();
				Debug.DrawLine(this._targetGO.transform.position, 10f * smoothedGravity, Color.green);
			}
			else if (num == -1)
			{
				this._targetGO.transform.rotation = this._imu.Compute();
				this.StateTransition(SLAMPPLocalizer.SLAMLocalizerState.inRelocalization);
			}
			else
			{
				this._targetGO.transform.rotation = this._imu.Compute();
				this.StateTransition(SLAMPPLocalizer.SLAMLocalizerState.inError);
			}
		}

		public override void ResetLocalizer()
		{
			Debug.Log("Reseting SLAM");
			SLAMPPLocalizer.ResetSLAM();
			this._imu.Reset();
			this._state = SLAMPPLocalizer.SLAMLocalizerState.atStart;
		}

		internal Quaternion From()
		{
			return SLAMPPLocalizer.From(this._quat);
		}

		internal static Quaternion From(double[] iQuat)
		{
			Quaternion result;
			result.x = (float)iQuat[0];
			result.y = (float)iQuat[1];
			result.z = (float)iQuat[2];
			result.w = (float)iQuat[3];
			return result;
		}
	}
}
