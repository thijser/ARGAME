using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta
{
	[ExecuteInEditMode]
	public class MetaUI : MetaSingleton<MetaUI>
	{
		private GameObject _IMUCalibratingIndicator;

		private GameObject _LoadingIndicator;

		private GameObject _eventSystem;

		[HideInInspector, SerializeField]
		private GameObject _mguiCanvas;

		[SerializeField]
		private bool _enableGrid = true;

		[SerializeField]
		private VectorMeshObject.MetaVectorStyle _defaultVectorStyle = new VectorMeshObject.MetaVectorStyle();

		internal GameObject mguiCanvas
		{
			get
			{
				return this._mguiCanvas;
			}
		}

		public bool enableGrid
		{
			get
			{
				return this._enableGrid;
			}
			set
			{
				if (base.get_transform().FindChild("Grid") != null)
				{
					base.get_transform().FindChild("Grid").get_gameObject().SetActive(value);
				}
				this._enableGrid = value;
			}
		}

		internal VectorMeshObject.MetaVectorStyle defaultVectorStyle
		{
			get
			{
				return this._defaultVectorStyle;
			}
			set
			{
				this._defaultVectorStyle = value;
			}
		}

		private void Awake()
		{
			this._IMUCalibratingIndicator = base.get_transform().FindChild("IMU Calibrating").GetChild(0).get_gameObject();
			this._LoadingIndicator = base.get_transform().FindChild("Loading").GetChild(0).get_gameObject();
			this._eventSystem = Object.FindObjectOfType<EventSystem>().get_gameObject();
		}

		private void Update()
		{
			this.enableGrid = this._enableGrid;
		}

		internal void ToggleIMUCalibratingIndicator(bool show)
		{
			this._IMUCalibratingIndicator.SetActive(show);
			this._eventSystem.SetActive(!show);
			MetaSingleton<Hands>.Instance.get_gameObject().GetComponent<GestureManager>().set_enabled(!show);
		}

		internal void ToggleLoadingIndicator(bool show)
		{
			this._LoadingIndicator.SetActive(show);
		}
	}
}
