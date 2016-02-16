using System;
using UnityEngine;

namespace Meta
{
	public abstract class Localizer : MonoBehaviour
	{
		protected GameObject _targetGO;

		public GameObject targetGO
		{
			get
			{
				return this._targetGO;
			}
			set
			{
				this._targetGO = value;
			}
		}

		private void OnEnable()
		{
			if (this._targetGO == null)
			{
				this.SetDefaultTargetGO();
			}
		}

		protected void SetDefaultTargetGO()
		{
			if (MetaCore.Instance != null)
			{
				this._targetGO = MetaCore.Instance.getMetaFrame();
			}
		}

		public virtual void ResetLocalizer()
		{
		}

		public Quaternion GetRotation()
		{
			return this._targetGO.get_transform().get_rotation();
		}

		public Vector3 GetPosition()
		{
			return this._targetGO.get_transform().get_position();
		}
	}
}
