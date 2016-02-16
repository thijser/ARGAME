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
			if (Application.isPlaying)
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
					this._hudLockObject = new GameObject().transform;
					this._hudLockObject.position = base.transform.position;
					this._hudLockObject.rotation = base.transform.rotation;
					this._hudLockObject.parent = UnityEngine.Camera.main.transform;
					this._hudLockObject.name = base.gameObject.name + ".HUDLockObject";
				}
			}
			else if (this._hudLockObject != null)
			{
                UnityEngine.Object.Destroy(this._hudLockObject.gameObject);
			}
		}

		public void UpdateLockObject()
		{
			if (this._hudLockObject != null)
			{
				this._hudLockObject.position = base.transform.position;
				this._hudLockObject.rotation = base.transform.rotation;
			}
		}

		private void ToggleHUDLayer()
		{
			if (this._metaBody != null && this._metaBody.hud && (this._metaBody.hudLayer || this._metaBody.useDefaultHUDSettings))
			{
				if (base.gameObject.layer != 9 && base.gameObject.layer != 10)
				{
					this._prevLayer = base.gameObject.layer;
					this._prevLayerSet = true;
					if (this._prevLayer != 5)
					{
						base.gameObject.layer = 9;
					}
					else
					{
						base.gameObject.layer = 10;
					}
				}
			}
			else if (this._prevLayerSet)
			{
				base.gameObject.layer = this._prevLayer;
				this._prevLayerSet = false;
			}
		}

		private void UpdateHUDLock()
		{
			if (this._metaBody != null)
			{
				if (this._metaBody.useDefaultHUDSettings)
				{
					base.transform.position = this._hudLockObject.position;
					base.transform.rotation = this._hudLockObject.rotation;
				}
				else
				{
					if (this._metaBody.hudLockPosition)
					{
						Vector3 position = this._hudLockObject.position;
						if (!this._metaBody.hudLockPositionX)
						{
							position.x = base.transform.position.x;
						}
						if (!this._metaBody.hudLockPositionY)
						{
							position.y = base.transform.position.y;
						}
						if (!this._metaBody.hudLockPositionZ)
						{
							position.z = base.transform.position.z;
						}
						base.transform.position = position;
					}
					if (this._metaBody.hudLockRotation)
					{
						Vector3 eulerAngles = this._hudLockObject.rotation.eulerAngles;
						if (!this._metaBody.hudLockRotationX)
						{
							eulerAngles.x = base.transform.rotation.eulerAngles.x;
						}
						if (!this._metaBody.hudLockRotationY)
						{
							eulerAngles.y = base.transform.rotation.eulerAngles.y;
						}
						if (!this._metaBody.hudLockRotationZ)
						{
							eulerAngles.z = base.transform.rotation.eulerAngles.z;
						}
						base.transform.rotation = Quaternion.Euler(eulerAngles);
					}
				}
			}
		}

		private void UpdateOrbitalLock()
		{
			if (this._prevPos != base.transform.position || this._prevRot != base.transform.rotation || this._prevFramePos != MetaCore.Instance.transform.position)
			{
				if (base.transform.position != MetaCore.Instance.transform.position)
				{
					this._orbitalDirection = Vector3.Normalize(base.transform.position - MetaCore.Instance.transform.position);
				}
				else
				{
					this._orbitalDirection = MetaCore.Instance.transform.forward;
				}
				if (this._metaBody.orbitalLockDistance || this._metaBody.useDefaultOrbitalSettings)
				{
					base.transform.position = MetaCore.Instance.transform.position;
					float num;
					if (this._metaBody.useDefaultOrbitalSettings || this._metaBody.userReachDistance)
					{
						num = MetaSingleton<MetaUserSettingsManager>.Instance.GetReachDistance();
					}
					else
					{
						num = this._metaBody.lockDistance;
					}
					base.transform.Translate(this._orbitalDirection * num, 0);
				}
				if (this._metaBody.orbitalLookAtCamera || this._metaBody.useDefaultOrbitalSettings)
				{
					base.transform.LookAt(UnityEngine.Camera.main.transform);
					if (this._metaBody.useDefaultOrbitalSettings || this._metaBody.orbitalLookAtCameraFlipY)
					{
						base.transform.Rotate(new Vector3(0f, 180f, 0f));
					}
				}
			}
			this._prevPos = base.transform.position;
			this._prevRot = base.transform.rotation;
			this._prevFramePos = MetaCore.Instance.transform.position;
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
					Vector3 position = base.transform.position;
					if (this._metaBody.palmLock)
					{
						position = Hands.GetHands()[(int)handType].palm.position;
					}
					else if (this._metaBody.fingerLock)
					{
						position = Hands.GetHands()[(int)handType].pointer.position;
					}
					base.transform.position = Vector3.Lerp(base.transform.position, position, this._movementSmoothing * Time.deltaTime);
				}
			}
		}
	}
}
