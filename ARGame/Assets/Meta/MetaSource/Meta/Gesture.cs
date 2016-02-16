using System;
using UnityEngine;

namespace Meta
{
	public class Gesture
	{
		protected bool _isValid;

		protected MetaGesture _type;

		protected Vector3 _position;

		public bool isValid
		{
			get
			{
				return this._isValid;
			}
		}

		public MetaGesture type
		{
			get
			{
				return this._type;
			}
		}

		public Vector3 position
		{
			get
			{
				return this._position;
			}
		}

		public Gesture()
		{
			this._type = MetaGesture.NONE;
			this._position = Vector3.zero;
			this._isValid = false;
		}

		internal Gesture(CppGestureData cppGesture)
		{
			this._isValid = cppGesture.valid;
			this._type = cppGesture.manipulationGesture;
			if (cppGesture.gesturePoint != null)
			{
				this._position = MetaUtils.FloatToVector3(cppGesture.gesturePoint);
			}
			else
			{
				this._position = Vector3.zero;
			}
		}

		internal void CopyTo(ref Gesture gesture)
		{
			gesture = (Gesture)base.MemberwiseClone();
		}

		internal void LocalToWorldCoordinate(Transform parentTransform)
		{
			this._position.Set(-this._position.x, this._position.y, this._position.z);
			this._position = parentTransform.TransformPoint(this._position);
		}

		internal void Update(CppGestureData cppGesture)
		{
			this._type = cppGesture.manipulationGesture;
			this._position = MetaUtils.FloatToVector3(cppGesture.gesturePoint);
			this._isValid = cppGesture.valid;
		}
	}
}
