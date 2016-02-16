using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	internal class CanvasTracker : MetaSingleton<CanvasTracker>, IMetaEventReceiver
	{
		[HideInInspector]
		internal bool m_debugMode;

		private List<CanvasTarget> m_canvasTargets = new List<CanvasTarget>();

		internal bool m_keyboardControlled = true;

		[SerializeField]
		private bool m_IsValid;

		[SerializeField]
		private bool m_RectanglesDetected;

		internal Matrix4x4 m_transformation_matrix_local;

		internal Matrix4x4 m_transformation_matrix_global;

		[SerializeField]
		private bool m_TargetLocked;

		private static bool m_IsInitialized;

		private int m_numFrames;

		private bool m_mirror;

		private float modelCorrectionThetaDegrees = -270f;

		internal static List<CanvasTarget> CanvasTargets
		{
			get
			{
				return MetaSingleton<CanvasTracker>.Instance.m_canvasTargets;
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.m_IsValid;
			}
		}

		internal bool RectanglesDetected
		{
			get
			{
				return this.m_RectanglesDetected;
			}
		}

		internal bool TargetLocked
		{
			get
			{
				return this.m_TargetLocked;
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "registerCanvasTracker")]
		protected static extern void RegisterDLL();

		[DllImport("MetaVisionDLL", EntryPoint = "InitPlaneTracker")]
		protected static extern void InitDLL();

		[DllImport("MetaVisionDLL", EntryPoint = "ReleasePlaneTracker")]
		protected static extern void ReleaseDLL();

		[DllImport("MetaVisionDLL", EntryPoint = "GetRectangleTransform")]
		protected static extern void GetRectangleTransformDLL(ref mMatrix transformation_matrix);

		[DllImport("MetaVisionDLL", EntryPoint = "SetTargetLock")]
		protected static extern void SetTargetLockDLL(bool value);

		[DllImport("MetaVisionDLL", EntryPoint = "GetTargetLock")]
		protected static extern bool GetTargetLockDLL();

		[DllImport("MetaVisionDLL", EntryPoint = "GetRectanglesDetected")]
		protected static extern bool GetRectanglesDetectedDLL();

		public void MetaInit()
		{
			CanvasTracker.RegisterDLL();
		}

		public void MetaUpdate()
		{
		}

		public void MetaOnDestroy()
		{
		}

		public void MetaLateUpdate()
		{
		}

		private void Start()
		{
			if (MetaCore.Instance.m_depthSensorConnected)
			{
				this.InitializeTracker();
				this.m_IsValid = false;
			}
			else
			{
				base.enabled = false;
			}
		}

		private void Update()
		{
			this.GetData();
			this.KeyboardControl();
		}

		private void OnDestroy()
		{
			if (base.enabled)
			{
				this.Release();
			}
		}

		private void InitializeTracker()
		{
			if (!CanvasTracker.m_IsInitialized)
			{
				CanvasTracker.m_IsInitialized = true;
				CanvasTracker.InitDLL();
			}
		}

		private void KeyboardControl()
		{
			if (this.m_keyboardControlled)
			{
				if (Input.GetKeyDown(KeyCode.L))
				{
					CanvasTracker.SetTargetLockDLL(true);
					if (this.m_debugMode)
					{
						Debug.Log((!this.m_TargetLocked) ? "Target UNLOCKED" : "Target LOCKED");
					}
				}
				else if (Input.GetKeyDown(KeyCode.K))
				{
					CanvasTracker.SetTargetLockDLL(false);
					if (this.m_debugMode)
					{
						Debug.Log((!this.m_TargetLocked) ? "Target UNLOCKED" : "Target LOCKED");
					}
				}
			}
		}

		internal void LockTarget()
		{
			CanvasTracker.SetTargetLockDLL(true);
		}

		internal void UnlockTarget()
		{
			CanvasTracker.SetTargetLockDLL(false);
		}

		private void Release()
		{
			CanvasTracker.ReleaseDLL();
			this.m_IsValid = false;
			CanvasTracker.m_IsInitialized = false;
		}

		private void GetData()
		{
			this.m_RectanglesDetected = CanvasTracker.GetRectanglesDetectedDLL();
			mMatrix matrix_buffer = default(mMatrix);
			CanvasTracker.GetRectangleTransformDLL(ref matrix_buffer);
			this.m_TargetLocked = CanvasTracker.GetTargetLockDLL();
			if (mMatrix.IsNaN(matrix_buffer))
			{
				this.m_IsValid = false;
				if (this.m_debugMode)
				{
					Debug.Log("NAN DETECTED in the fetched matrix");
				}
				return;
			}
			this.m_IsValid = true;
			if (this.m_IsValid)
			{
				this.m_numFrames = ((this.m_numFrames <= 1000) ? (this.m_numFrames + 1) : 100);
			}
			Matrix4x4 matrix4x = default(Matrix4x4); // Copy matrix.
			matrix4x[0] = matrix_buffer.m00;
			matrix4x[4] = matrix_buffer.m10;
			matrix4x[8] = matrix_buffer.m20;
			matrix4x[12] = matrix_buffer.m30;
			matrix4x[1] = matrix_buffer.m01;
			matrix4x[5] = matrix_buffer.m11;
			matrix4x[9] = matrix_buffer.m21;
			matrix4x[13] = matrix_buffer.m31;
			matrix4x[2] = matrix_buffer.m02;
			matrix4x[6] = matrix_buffer.m12;
			matrix4x[10] = matrix_buffer.m22;
			matrix4x[14] = matrix_buffer.m32;
			matrix4x[3] = matrix_buffer.m03;
			matrix4x[7] = matrix_buffer.m13;
			matrix4x[11] = matrix_buffer.m23;
			matrix4x[15] = matrix_buffer.m33;
			matrix4x = matrix4x.transpose;
			float num = 0.0174532924f * this.modelCorrectionThetaDegrees;
			matrix4x *= new Matrix4x4
			{
				m00 = 1f,
				m11 = Mathf.Cos(num),
				m22 = Mathf.Cos(num),
				m33 = 1f,
				m12 = Mathf.Sin(num),
				m21 = -Mathf.Sin(num)
			};
			Matrix4x4 matrix4x2 = default(Matrix4x4);
			matrix4x2.m00 = -1f;
			matrix4x2.m11 = 1f;
			matrix4x2.m22 = 1f;
			matrix4x2.m33 = 1f;
			if (this.m_mirror)
			{
				matrix4x = matrix4x2 * matrix4x;
			}
			this.m_transformation_matrix_local = matrix4x;
			Matrix4x4 matrix4x3 = default(Matrix4x4);
			matrix4x3.m00 = 1f;
			matrix4x3.m11 = 1f;
			matrix4x3.m22 = -1f;
			matrix4x3.m33 = 1f;
			this.m_transformation_matrix_global = base.transform.localToWorldMatrix * this.m_transformation_matrix_local;
		}

		internal static Vector3 GetPosition()
		{
			return MetaSingleton<CanvasTracker>.Instance.m_transformation_matrix_global.PositionFromMatrix();
		}

		internal static Quaternion GetRotation()
		{
			return MetaSingleton<CanvasTracker>.Instance.m_transformation_matrix_global.QuaternionFromMatrix();
		}

		internal static Vector3 GetScale()
		{
			return MetaSingleton<CanvasTracker>.Instance.m_transformation_matrix_global.ScaleFromMatrix();
		}

		internal static void AddTarget(CanvasTarget pTarget)
		{
			if (MetaSingleton<CanvasTracker>.Instance != null)
			{
				MetaSingleton<CanvasTracker>.Instance.m_canvasTargets.Add(pTarget);
			}
			else
			{
				Debug.LogWarning("CanvasTracker.AddTarget can't find an Instance yet.");
			}
		}
	}
}
