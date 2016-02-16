using System;
using UnityEngine;

namespace Meta
{
	public class HandEntity
	{
		protected Vector3 _position;

		protected Quaternion _orientation;

		protected Vector3 _scale;

		protected bool _isValid;

		public Vector3 position
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

		public Quaternion orientation
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

		public Vector3 scale
		{
			get
			{
				return this._scale;
			}
			internal set
			{
				this._scale = value;
			}
		}

		public bool isValid
		{
			get
			{
				return this._isValid;
			}
			internal set
			{
				this._isValid = value;
				if (!this._isValid)
				{
					this._position.Set(0f, 0f, 0f);
					this._orientation = Quaternion.get_identity();
					this._scale.Set(1f, 1f, 1f);
				}
			}
		}

		protected object Clone()
		{
			return base.MemberwiseClone();
		}

		internal void CopyTo(ref HandEntity HandEntity)
		{
			HandEntity = (HandEntity)this.Clone();
		}
	}
}
