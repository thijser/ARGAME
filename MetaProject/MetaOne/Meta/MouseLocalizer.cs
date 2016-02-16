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
			this.smoothRotationX += (this.rotationX - this.smoothRotationX) * this.smoothSpeed * Time.get_smoothDeltaTime();
			this.smoothRotationY += (this.rotationY - this.smoothRotationY) * this.smoothSpeed * Time.get_smoothDeltaTime();
			base.get_transform().set_localEulerAngles(new Vector3(-this.smoothRotationY, this.smoothRotationX, 0f));
			if (Input.GetMouseButton(1))
			{
				Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				Vector3 vector2 = base.get_transform().get_rotation() * vector;
				Transform expr_1A3 = base.get_transform();
				expr_1A3.set_position(expr_1A3.get_position() + vector2 * 25f * Time.get_smoothDeltaTime());
				Transform expr_1CE = base.get_transform();
				expr_1CE.set_position(expr_1CE.get_position() + base.get_transform().get_rotation() * Vector3.get_forward() * Input.GetAxis("Mouse ScrollWheel") * 200f);
			}
			if (this._targetGO == null)
			{
				base.SetDefaultTargetGO();
			}
			this.UpdateTargetGOTransform();
		}

		public void UpdateTargetGOTransform()
		{
			this._targetGO.get_transform().set_position(base.get_transform().get_position());
			this._targetGO.get_transform().set_rotation(base.get_transform().get_rotation());
		}
	}
}
