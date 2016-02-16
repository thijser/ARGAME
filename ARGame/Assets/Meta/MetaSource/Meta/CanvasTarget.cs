using System;
using UnityEngine;

namespace Meta
{
	internal class CanvasTarget : MonoBehaviour
	{
		internal bool m_kalmanFiltering = true;

		internal float m_KalmanVelocity = 100f;

		internal bool m_debugMode;

		internal bool m_InheritScale;

		[SerializeField]
		internal bool m_enabledOnPreLockMode;

		[SerializeField]
		internal bool m_enabledOnPostLockMode = true;

		private KalmanFilterClient _kalmanFilter = new KalmanFilterClient();

		private Vector3 _originalScale = new Vector3(1f, 1f, 1f);

		private Vector3 _lastPosition;

		private float _positionDelta;

		private Vector3 _smoothedPosition = new Vector3(0f, 0f, 0f);

		private Quaternion _smoothedRotation = default(Quaternion);

		[SerializeField]
		private bool m_IsStable;

		internal float stabilityVelocityThreshold = 1f;

		internal float stabilityTimeThreshold = 1f;

		internal float stabilityVelocity;

		internal float stabilityTimeElapsed;

		internal float lastUnstableTime;

		private bool _lockedToTracker;

		internal bool enabledOnPostLockMode
		{
			get
			{
				return this.m_enabledOnPostLockMode;
			}
			set
			{
				this.m_enabledOnPostLockMode = value;
			}
		}

		internal bool enabledOnPreLockMode
		{
			get
			{
				return this.m_enabledOnPreLockMode;
			}
			set
			{
				this.m_enabledOnPreLockMode = value;
			}
		}

		internal bool IsStable
		{
			get
			{
				return this.m_IsStable;
			}
		}

		internal bool lockedToTracker
		{
			get
			{
				return this._lockedToTracker;
			}
			set
			{
				this._lockedToTracker = value;
			}
		}

		private void CheckStability()
		{
			this.stabilityVelocity = Time.deltaTime * this.stabilityVelocityThreshold;
			if (this._positionDelta > this.stabilityVelocity || !MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
			{
				this.m_IsStable = false;
				this.lastUnstableTime = Time.time;
			}
			else
			{
				this.stabilityTimeElapsed = Time.time - this.lastUnstableTime;
				if (this.stabilityTimeElapsed > this.stabilityTimeThreshold)
				{
					this.m_IsStable = true;
				}
			}
		}

		private void SetCanvasTransform()
		{
			Quaternion rotation = CanvasTracker.GetRotation();
			Vector3 position = CanvasTracker.GetPosition();
			Vector3 scale = CanvasTracker.GetScale();
			if (MetaSingleton<CanvasTracker>.Instance.IsValid)
			{
				this._positionDelta = Vector3.Distance(this._lastPosition, position);
				this.CheckStability();
				if (MetaSingleton<CanvasTracker>.Instance.RectanglesDetected)
				{
					base.transform.rotation = rotation;
					base.transform.position = position;
				}
				if (this.m_kalmanFiltering)
				{
					this._kalmanFilter.kalmanVelocity = this.m_KalmanVelocity;
					this._kalmanFilter.KalmanFilterSmoothTransform(base.transform, out this._smoothedPosition, out this._smoothedRotation);
					base.transform.position = this._smoothedPosition;
					base.transform.rotation = this._smoothedRotation;
				}
				this._lastPosition = base.transform.position;
				if (this.m_InheritScale)
				{
					base.transform.localScale = Vector3.Scale(scale, this._originalScale);
				}
				else
				{
					base.transform.localScale = this._originalScale;
				}
			}
		}

		private void Start()
		{
			CanvasTracker.AddTarget(this);
			this._originalScale = base.transform.localScale;
		}

		private void LateUpdate()
		{
			if (this.lockedToTracker)
			{
				this.SetCanvasTransform();
			}
		}
	}
}
