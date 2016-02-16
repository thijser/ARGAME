using System;
using UnityEngine;

namespace Meta
{
	public class Palm : HandGameEntity
	{
		private float _radius;

		private Vector3 _normal;

		private Vector2 _orientations;

		public float radius
		{
			get
			{
				return this._radius;
			}
			internal set
			{
				this._radius = value;
			}
		}

		internal Vector3 normal
		{
			get
			{
				return this._normal;
			}
			set
			{
				this._normal = value;
			}
		}

		internal Vector2 orientations
		{
			get
			{
				return this._orientations;
			}
			set
			{
				this._orientations = value;
			}
		}

		internal void SetOrientation(float armAngle)
		{
			this._orientation = Quaternion.Euler(new Vector3(-1f * this._orientations.x, -1f * (90f + this._orientations.y), -90f + armAngle));
		}

		internal void CopyTo(ref Palm palm)
		{
			palm = (Palm)base.Clone();
		}
	}
}
