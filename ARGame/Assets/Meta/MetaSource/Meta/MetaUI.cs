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
				if (base.transform.FindChild("Grid") != null)
				{
					base.transform.FindChild("Grid").gameObject.SetActive(value);
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
			this._IMUCalibratingIndicator = base.transform.FindChild("IMU Calibrating").GetChild(0).gameObject;
			this._LoadingIndicator = base.transform.FindChild("Loading").GetChild(0).gameObject;
			this._eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>().gameObject;
		}

		private void Update()
		{
			this.enableGrid = this._enableGrid;
		}

		internal void ToggleIMUCalibratingIndicator(bool show)
		{
			this._IMUCalibratingIndicator.SetActive(show);
			this._eventSystem.SetActive(!show);
			MetaSingleton<Hands>.Instance.gameObject.GetComponent<GestureManager>().enabled = !show;
		}

		internal void ToggleLoadingIndicator(bool show)
		{
			this._LoadingIndicator.SetActive(show);
		}
	}
}
