using System;
using System.Linq;
using UnityEngine;

namespace Meta
{
	[ExecuteInEditMode]
	public class MetaLocalization : MetaSingleton<MetaLocalization>
	{
		protected GameObject _targetGO;

		[SerializeField]
		private Localizer[] _localizers = new Localizer[0];

		[SerializeField]
		private int _currentLocalization;

		public GameObject targetGO
		{
			get
			{
				return this._targetGO;
			}
			private set
			{
				this._targetGO = value;
			}
		}

		public string[] localizers
		{
			get
			{
				return (from x in this._localizers
				select x.get_name()).ToArray<string>();
			}
		}

		public string currentLocalization
		{
			get
			{
				return this._localizers[this._currentLocalization].get_name();
			}
			set
			{
				this.GetLocalizers();
				this._currentLocalization = 0;
				bool flag = false;
				for (int i = 0; i < this._localizers.Length; i++)
				{
					if (value == this._localizers[i].get_name())
					{
						this._currentLocalization = i;
						flag = true;
						break;
					}
				}
				this.SwitchLocalizer();
				if (!flag)
				{
					Debug.LogError("Localizer \"" + value + "\" not found.");
				}
			}
		}

		public IMULocalizer ImuLocalizer
		{
			get
			{
				return this.GetLocalizer("IMULocalizer") as IMULocalizer;
			}
			set
			{
			}
		}

		private void OnValidate()
		{
			this.GetLocalizers();
			this.SwitchLocalizer();
		}

		private void Start()
		{
			this.GetLocalizers();
			this.SwitchLocalizer();
		}

		private void SwitchLocalizer()
		{
			for (int i = 0; i < this._localizers.Length; i++)
			{
				if (this._localizers[i] != null)
				{
					if (i != this._currentLocalization)
					{
						this._localizers[i].set_enabled(false);
					}
					else
					{
						this._localizers[i].set_enabled(true);
						if (this._targetGO != null)
						{
							this._localizers[i].targetGO = this._targetGO;
						}
					}
				}
			}
		}

		public Localizer GetLocalizer(string localizerName)
		{
			Localizer[] array = (from x in this._localizers
			where x.get_name() == localizerName
			select x).ToArray<Localizer>();
			if (array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		public void GetLocalizers()
		{
			this._localizers = base.get_transform().GetComponentsInChildren<Localizer>();
		}

		public void ResetLocalizer()
		{
			if (this._localizers[this._currentLocalization] != null)
			{
				this._localizers[this._currentLocalization].ResetLocalizer();
			}
		}

		public Quaternion GetRotation()
		{
			if (this._localizers[this._currentLocalization] != null)
			{
				return this._localizers[this._currentLocalization].GetRotation();
			}
			return Quaternion.get_identity();
		}

		public Vector3 GetPosition()
		{
			if (this._localizers[this._currentLocalization] != null)
			{
				return this._localizers[this._currentLocalization].GetPosition();
			}
			return new Vector3(0f, 0f, 0f);
		}

		public void UseMouseLocalizer()
		{
			this.currentLocalization = "MouseLocalizer";
		}
	}
}
