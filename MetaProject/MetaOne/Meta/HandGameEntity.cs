using System;
using UnityEngine;

namespace Meta
{
	public class HandGameEntity : HandEntity
	{
		private GameObject _objectOfInterest;

		private GameObject _unityGameObject;

		private float _rayCastSpread = 5f;

		private float _rayCastMinConfidence = 15f;

		public GameObject objectOfInterest
		{
			get
			{
				return this._objectOfInterest;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return this._unityGameObject;
			}
		}

		public new Vector3 position
		{
			get
			{
				return this._unityGameObject.get_transform().get_parent().TransformPoint(this._position);
			}
		}

		public Vector3 localPosition
		{
			get
			{
				return this._position;
			}
			internal set
			{
				this._position = value;
			}
		}

		public new Quaternion orientation
		{
			get
			{
				return this._orientation * Quaternion.Inverse(this._unityGameObject.get_transform().get_parent().get_rotation());
			}
		}

		public Quaternion localOrientation
		{
			get
			{
				return this._orientation;
			}
			internal set
			{
				this._orientation = value;
			}
		}

		public float rayCastSpread
		{
			get
			{
				return this._rayCastSpread;
			}
			internal set
			{
				if (value != this._rayCastSpread)
				{
					if (value > 45f)
					{
						this._rayCastSpread = 45f;
					}
					else if (value < 5f)
					{
						this._rayCastSpread = 5f;
					}
					else
					{
						this._rayCastSpread = value;
					}
				}
			}
		}

		public float rayCastMinConfidence
		{
			get
			{
				return this._rayCastMinConfidence;
			}
			internal set
			{
				if (value != this._rayCastMinConfidence)
				{
					if (value > 90f)
					{
						this._rayCastMinConfidence = 90f;
					}
					else if (value < 15f)
					{
						this._rayCastMinConfidence = 15f;
					}
					else
					{
						this._rayCastMinConfidence = value;
					}
				}
			}
		}

		internal void InstantiateObject(GameObject prefab, string name, Transform parentTransform)
		{
			this._unityGameObject = (Object.Instantiate(prefab, new Vector3(0f, 0f, 0f), Quaternion.get_identity()) as GameObject);
			this._unityGameObject.get_transform().set_parent(parentTransform);
			this._unityGameObject.set_layer(31);
			this._unityGameObject.set_name(name);
		}

		internal void SetTransform(Vector3 _position, Quaternion _orientation, Vector3 _scale)
		{
			this._unityGameObject.get_transform().set_position(_position);
			this._unityGameObject.get_transform().set_localRotation(_orientation);
			this._unityGameObject.get_transform().set_localScale(_scale);
			this._unityGameObject.SetActive(this._isValid);
		}

		internal void MultiRayCast(LayerMask layers)
		{
			Vector3 position = this._unityGameObject.get_transform().get_position();
			Vector3 direction = position - Camera.get_main().get_transform().get_position();
			RaycastHit[] hits = MultiRaycast.MultiRayCast(position, direction, 5, 5, this._rayCastSpread, layers, false);
			this._objectOfInterest = MultiRaycast.MostHit(hits);
		}

		internal void CopyTo(ref HandGameEntity HandEntity)
		{
			HandEntity = (HandGameEntity)base.Clone();
		}

		internal void CorrectCoordinates(Transform parentTransform)
		{
			this._position.Set(-this._position.x, this._position.y, this._position.z);
		}
	}
}