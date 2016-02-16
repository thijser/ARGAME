using System;
using UnityEngine;

namespace Meta
{
	internal class MetaTransform : MonoBehaviour
	{
		private MetaBody _metaBody;

		private Transform _hudLockObject;

		private Vector3 _orbitalDirection;

		private int _prevLayer;

		private bool _prevLayerSet;

		private float _movementSmoothing = 10f;

		private Vector3 _prevPos;

		private Quaternion _prevRot;

		private Vector3 _prevFramePos;

		private void Awake()
		{
			this._metaBody = base.GetComponent<MetaBody>();
		}

		private void Update()
		{
		}

		private void LateUpdate()
		{
			if (this._metaBody != null)
			{
				if (this._metaBody.hud)
				{
					this.UpdateHUDLock();
				}
				else if (this._metaBody.orbital)
				{
					this.UpdateOrbitalLock();
				}
				else if (this._metaBody.palmLock || this._metaBody.fingerLock)
				{
					this.UpdatePalmOrFingerLock();
				}
			}
		}

		public void SetupHUDLock()
		{
			if (Application.get_isPlaying())
			{
				this.CreateDestroyHUDLockObject();
				this.ToggleHUDLayer();
			}
		}

		private void CreateDestroyHUDLockObject()
		{
			if (this._metaBody != null && this._metaBody.hud)
			{
				if (this._hudLockObject == null)
				{
					this._hudLockObject = new GameObject().get_transform();
					this._hudLockObject.set_position(base.get_transform().get_position());
					this._hudLockObject.set_rotation(base.get_transform().get_rotation());
					this._hudLockObject.set_parent(Camera.get_main().get_transform());
					this._hudLockObject.set_name(base.get_gameObject().get_name() + ".HUDLockObject");
				}
			}
			else if (this._hudLockObject != null)
			{
				Object.Destroy(this._hudLockObject.get_gameObject());
			}
		}

		public void UpdateLockObject()
		{
			if (this._hudLockObject != null)
			{
				this._hudLockObject.set_position(base.get_transform().get_position());
				this._hudLockObject.set_rotation(base.get_transform().get_rotation());
			}
		}

		private void ToggleHUDLayer()
		{
			if (this._metaBody != null && this._metaBody.hud && (this._metaBody.hudLayer || this._metaBody.useDefaultHUDSettings))
			{
				if (base.get_gameObject().get_layer() != 9 && base.get_gameObject().get_layer() != 10)
				{
					this._prevLayer = base.get_gameObject().get_layer();
					this._prevLayerSet = true;
					if (this._prevLayer != 5)
					{
						base.get_gameObject().set_layer(9);
					}
					else
					{
						base.get_gameObject().set_layer(10);
					}
				}
			}
			else if (this._prevLayerSet)
			{
				base.get_gameObject().set_layer(this._prevLayer);
				this._prevLayerSet = false;
			}
		}

		private void UpdateHUDLock()
		{
			if (this._metaBody != null)
			{
				if (this._metaBody.useDefaultHUDSettings)
				{
					base.get_transform().set_position(this._hudLockObject.get_position());
					base.get_transform().set_rotation(this._hudLockObject.get_rotation());
				}
				else
				{
					if (this._metaBody.hudLockPosition)
					{
						Vector3 position = this._hudLockObject.get_position();
						if (!this._metaBody.hudLockPositionX)
						{
							position.x = base.get_transform().get_position().x;
						}
						if (!this._metaBody.hudLockPositionY)
						{
							position.y = base.get_transform().get_position().y;
						}
						if (!this._metaBody.hudLockPositionZ)
						{
							position.z = base.get_transform().get_position().z;
						}
						base.get_transform().set_position(position);
					}
					if (this._metaBody.hudLockRotation)
					{
						Vector3 eulerAngles = this._hudLockObject.get_rotation().get_eulerAngles();
						if (!this._metaBody.hudLockRotationX)
						{
							eulerAngles.x = base.get_transform().get_rotation().get_eulerAngles().x;
						}
						if (!this._metaBody.hudLockRotationY)
						{
							eulerAngles.y = base.get_transform().get_rotation().get_eulerAngles().y;
						}
						if (!this._metaBody.hudLockRotationZ)
						{
							eulerAngles.z = base.get_transform().get_rotation().get_eulerAngles().z;
						}
						base.get_transform().set_rotation(Quaternion.Euler(eulerAngles));
					}
				}
			}
		}

		private void UpdateOrbitalLock()
		{
			if (this._prevPos != base.get_transform().get_position() || this._prevRot != base.get_transform().get_rotation() || this._prevFramePos != MetaCore.Instance.get_transform().get_position())
			{
				if (base.get_transform().get_position() != MetaCore.Instance.get_transform().get_position())
				{
					this._orbitalDirection = Vector3.Normalize(base.get_transform().get_position() - MetaCore.Instance.get_transform().get_position());
				}
				else
				{
					this._orbitalDirection = MetaCore.Instance.get_transform().get_forward();
				}
				if (this._metaBody.orbitalLockDistance || this._metaBody.useDefaultOrbitalSettings)
				{
					base.get_transform().set_position(MetaCore.Instance.get_transform().get_position());
					float num;
					if (this._metaBody.useDefaultOrbitalSettings || this._metaBody.userReachDistance)
					{
						num = MetaSingleton<MetaUserSettingsManager>.Instance.GetReachDistance();
					}
					else
					{
						num = this._metaBody.lockDistance;
					}
					base.get_transform().Translate(this._orbitalDirection * num, 0);
				}
				if (this._metaBody.orbitalLookAtCamera || this._metaBody.useDefaultOrbitalSettings)
				{
					base.get_transform().LookAt(Camera.get_main().get_transform());
					if (this._metaBody.useDefaultOrbitalSettings || this._metaBody.orbitalLookAtCameraFlipY)
					{
						base.get_transform().Rotate(new Vector3(0f, 180f, 0f));
					}
				}
			}
			this._prevPos = base.get_transform().get_position();
			this._prevRot = base.get_transform().get_rotation();
			this._prevFramePos = MetaCore.Instance.get_transform().get_position();
		}

		private void UpdatePalmOrFingerLock()
		{
			HandType handType = this._metaBody.lockHandType;
			if (handType != HandType.UNKNOWN)
			{
				if (handType == HandType.EITHER)
				{
					if (Hands.right != null && Hands.right.isValid)
					{
						handType = HandType.RIGHT;
					}
					else
					{
						if (Hands.left == null || !Hands.left.isValid)
						{
							return;
						}
						handType = HandType.LEFT;
					}
				}
				if (Hands.GetHands()[(int)handType] != null && Hands.GetHands()[(int)handType].isValid)
				{
					Vector3 position = base.get_transform().get_position();
					if (this._metaBody.palmLock)
					{
						position = Hands.GetHands()[(int)handType].palm.position;
					}
					else if (this._metaBody.fingerLock)
					{
						position = Hands.GetHands()[(int)handType].pointer.position;
					}
					base.get_transform().set_position(Vector3.Lerp(base.get_transform().get_position(), position, this._movementSmoothing * Time.get_deltaTime()));
				}
			}
		}
	}
}
