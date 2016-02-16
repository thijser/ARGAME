using System;
using UnityEngine;

namespace Meta
{
	internal class MouseLocalizer : Localizer
	{
		public float sensitivityX = 5f;

		public float sensitivityY = 5f;

		public float minimumX = -360f;

		public float maximumX = 360f;

		public float minimumY = -90f;

		public float maximumY = 90f;

		public float smoothSpeed = 20f;

		private float rotationX;

		private float smoothRotationX;

		private float rotationY;

		private float smoothRotationY;

		private Vector3 _position;

		private Quaternion _rotation;

		private bool bActive;

		private bool _prevMouseCursorVisibility;

		private bool _prevMouseCursorLockState;

		private bool _stereoMouseEnabled;

		public void Update()
		{
			if (Input.GetMouseButton(1))
			{
				if (!this.bActive)
				{
					this._prevMouseCursorVisibility = ScreenCursor.GetMouseCursorVisibility();
					this._prevMouseCursorLockState = ScreenCursor.GetMouseCursorLockState();
					this._stereoMouseEnabled = MetaSingleton<MetaMouse>.Instance.enableMetaMouse;
					this.bActive = true;
				}
				this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
				this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
				ScreenCursor.SetMouseCursorVisibility(false);
				ScreenCursor.SetMouseCursorLockState(true);
				MetaSingleton<MetaMouse>.Instance.enableMetaMouse = false;
			}
			else if (this.bActive)
			{
				this.bActive = false;
				ScreenCursor.SetMouseCursorVisibility(this._prevMouseCursorVisibility);
				ScreenCursor.SetMouseCursorLockState(this._prevMouseCursorLockState);
				MetaSingleton<MetaMouse>.Instance.enableMetaMouse = this._stereoMouseEnabled;
			}
			this.smoothRotationX += (this.rotationX - this.smoothRotationX) * this.smoothSpeed * Time.smoothDeltaTime;
			this.smoothRotationY += (this.rotationY - this.smoothRotationY) * this.smoothSpeed * Time.smoothDeltaTime;
			base.transform.localEulerAngles = new Vector3(-this.smoothRotationY, this.smoothRotationX, 0f);
			if (Input.GetMouseButton(1))
			{
				Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				Vector3 vector2 = base.transform.rotation * vector;
				Transform expr_1A3 = base.transform;
				expr_1A3.position = expr_1A3.position + vector2 * 25f * Time.smoothDeltaTime;
				Transform expr_1CE = base.transform;
				expr_1CE.position = expr_1CE.position + base.transform.rotation * Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 200f;
			}
			if (this._targetGO == null)
			{
				base.SetDefaultTargetGO();
			}
			this.UpdateTargetGOTransform();
		}

		public void UpdateTargetGOTransform()
		{
			this._targetGO.transform.position = base.transform.position;
			this._targetGO.transform.rotation = base.transform.rotation;
		}
	}
}
