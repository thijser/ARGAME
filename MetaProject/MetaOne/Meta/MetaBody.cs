using System;
using UnityEngine;

namespace Meta
{
	[AddComponentMenu("Meta/MetaBody")]
	public class MetaBody : MonoBehaviour
	{
		[SerializeField]
		private bool _grabbable;

		[SerializeField]
		private bool _moveObjectOnGrab = true;

		[SerializeField]
		private bool _rotateObjectOnGrab;

		[SerializeField]
		private bool _scaleObjectOnGrab;

		[SerializeField]
		private bool _moveObjectOnTwoHandedGrab;

		[SerializeField]
		private bool _scaleObjectOnTwoHandedGrab;

		[SerializeField]
		private bool _rotateObjectOnTwoHandedGrab;

		[SerializeField]
		private bool _useDefaultGrabSettings = true;

		[SerializeField]
		private float _grabbableDistance = 0.1f;

		[SerializeField]
		private HandType _grabbingHandType = HandType.EITHER;

		private bool _grabbed;

		[SerializeField]
		private bool _pinchable;

		[SerializeField]
		private bool _moveObjectOnPinch;

		[SerializeField]
		private bool _scaleObjectOnPinch = true;

		[SerializeField]
		private bool _useDefaultPinchSettings = true;

		[SerializeField]
		private float _pinchableDistance = 0.1f;

		[SerializeField]
		private HandType _pinchingHandType = HandType.EITHER;

		private bool _minMaxScaleSet;

		[SerializeField]
		private bool _useMinScale = true;

		[SerializeField]
		private bool _useMaxScale = true;

		[SerializeField]
		private float _minScaleRatio = 0.5f;

		[SerializeField]
		private float _maxScaleRatio = 2f;

		private Vector3 _minScale;

		private Vector3 _maxScale;

		private bool _pinched;

		[SerializeField]
		private bool _touchable;

		[SerializeField]
		private bool _useDefaultTouchSettings = true;

		[SerializeField]
		private bool _touchableDwellable = true;

		[SerializeField]
		private bool _touchDwellMustBeStable = true;

		[SerializeField]
		private HandType _touchingHandType = HandType.EITHER;

		[SerializeField]
		private float _touchDwellTime = 1f;

		[SerializeField]
		private bool _pointable;

		[SerializeField]
		private bool _pointableDwellable = true;

		[SerializeField]
		private bool _useDefaultPointSettings = true;

		[SerializeField]
		private bool _pointDwellMustBeStable = true;

		[SerializeField]
		private HandType _pointingHandType = HandType.EITHER;

		[SerializeField]
		private float _pointDwellTime = 1f;

		[SerializeField]
		private bool _gazeable;

		[SerializeField]
		private bool _gazeableDwellable = true;

		private MetaGesture _gesture = MetaGesture.NONE;

		[SerializeField]
		private bool _orbital;

		[SerializeField]
		private bool _useDefaultOrbitalSettings = true;

		[SerializeField]
		private bool _orbitalLockDistance = true;

		[SerializeField]
		private bool _userReachDistance = true;

		[SerializeField]
		private float _lockDistance = 0.4f;

		[SerializeField]
		private bool _orbitalLookAtCamera = true;

		[SerializeField]
		private bool _orbitalLookAtCameraFlipY = true;

		[SerializeField]
		private bool _hud;

		[SerializeField]
		private bool _useDefaultHUDsettings = true;

		[SerializeField]
		private bool _hudLockPosition = true;

		[SerializeField]
		private bool _hudLockPositionX = true;

		[SerializeField]
		private bool _hudLockPositionY = true;

		[SerializeField]
		private bool _hudLockPositionZ = true;

		[SerializeField]
		private bool _hudLockRotation = true;

		[SerializeField]
		private bool _hudLockRotationX = true;

		[SerializeField]
		private bool _hudLockRotationY = true;

		[SerializeField]
		private bool _hudLockRotationZ = true;

		[SerializeField]
		private bool _hudLayer = true;

		[SerializeField]
		private bool _palmLock;

		[SerializeField]
		private HandType _lockHandType = HandType.EITHER;

		[SerializeField]
		private bool _fingerLock;

		[SerializeField]
		private bool _jaysify;

		[SerializeField]
		private bool _arrow;

		[SerializeField]
		private bool _canvasTarget;

		[SerializeField]
		private int _canvasTargetID = 1;

		[SerializeField]
		private bool _markerTarget;

		[SerializeField]
		private int _markerTargetID = -1;

		[SerializeField]
		private bool _markerTargetPlaceable = true;

		[SerializeField]
		private bool _markerTargetPlaceableHighlight = true;

		[SerializeField]
		private bool _markerTargetPersistent;

		private bool _prevSet;

		private bool _markerTargetPrev;

		private bool _canvasTargetPrev;

		public bool grabbable
		{
			get
			{
				return this._grabbable;
			}
			set
			{
				this._grabbable = value;
				if (MetaSingleton<MetaManager>.Instance != null)
				{
					MetaSingleton<MetaManager>.Instance.AddRemoveInteractable(InteractableType.Grabbable, base.get_transform(), value);
				}
			}
		}

		public bool twoHandedGrabbable
		{
			get
			{
				return this.moveObjectOnTwoHandedGrab || this.rotateObjectOnTwoHandedGrab || this.scaleObjectOnTwoHandedGrab;
			}
		}

		public bool moveObjectOnGrab
		{
			get
			{
				return this._moveObjectOnGrab;
			}
			set
			{
				this._moveObjectOnGrab = value;
				if (value)
				{
					this._scaleObjectOnGrab = false;
					this._rotateObjectOnGrab = false;
				}
			}
		}

		public bool rotateObjectOnGrab
		{
			get
			{
				return this._rotateObjectOnGrab;
			}
			set
			{
				this._rotateObjectOnGrab = value;
				if (value)
				{
					this._scaleObjectOnGrab = false;
					this._moveObjectOnGrab = false;
				}
			}
		}

		public bool scaleObjectOnGrab
		{
			get
			{
				return this._scaleObjectOnGrab;
			}
			set
			{
				this._scaleObjectOnGrab = value;
				if (value)
				{
					this._rotateObjectOnGrab = false;
					this._moveObjectOnGrab = false;
					if (Application.get_isPlaying() && !this._minMaxScaleSet)
					{
						this.minScale = base.get_transform().get_localScale() * this.minScaleRatio;
						this.maxScale = base.get_transform().get_localScale() * this.maxScaleRatio;
						this._minMaxScaleSet = true;
					}
				}
			}
		}

		public bool moveObjectOnTwoHandedGrab
		{
			get
			{
				return this._moveObjectOnTwoHandedGrab;
			}
			set
			{
				this._moveObjectOnTwoHandedGrab = value;
				if (value)
				{
					this._rotateObjectOnTwoHandedGrab = false;
					this._scaleObjectOnTwoHandedGrab = false;
				}
			}
		}

		public bool scaleObjectOnTwoHandedGrab
		{
			get
			{
				return this._scaleObjectOnTwoHandedGrab;
			}
			set
			{
				this._scaleObjectOnTwoHandedGrab = value;
				if (value)
				{
					this._moveObjectOnTwoHandedGrab = false;
					this._rotateObjectOnTwoHandedGrab = false;
					if (Application.get_isPlaying() && !this._minMaxScaleSet)
					{
						this.minScale = base.get_transform().get_localScale() * this.minScaleRatio;
						this.maxScale = base.get_transform().get_localScale() * this.maxScaleRatio;
						this._minMaxScaleSet = true;
					}
				}
			}
		}

		public bool rotateObjectOnTwoHandedGrab
		{
			get
			{
				return this._rotateObjectOnTwoHandedGrab;
			}
			set
			{
				this._rotateObjectOnTwoHandedGrab = value;
				if (value)
				{
					this._moveObjectOnTwoHandedGrab = false;
					this._scaleObjectOnTwoHandedGrab = false;
				}
			}
		}

		public bool useDefaultGrabSettings
		{
			get
			{
				return this._useDefaultGrabSettings;
			}
			set
			{
				this._useDefaultGrabSettings = value;
			}
		}

		public float grabbableDistance
		{
			get
			{
				return this._grabbableDistance;
			}
			set
			{
				this._grabbableDistance = value;
			}
		}

		public HandType grabbingHandType
		{
			get
			{
				return this._grabbingHandType;
			}
			set
			{
				this._grabbingHandType = value;
			}
		}

		public bool grabbed
		{
			get
			{
				return this._grabbed;
			}
			set
			{
				this._grabbed = value;
			}
		}

		public bool pinchable
		{
			get
			{
				return this._pinchable;
			}
			set
			{
				this._pinchable = value;
				if (MetaSingleton<MetaManager>.Instance != null)
				{
					MetaSingleton<MetaManager>.Instance.AddRemoveInteractable(InteractableType.Pinchable, base.get_transform(), value);
				}
			}
		}

		public bool moveObjectOnPinch
		{
			get
			{
				return this._moveObjectOnPinch;
			}
			set
			{
				this._moveObjectOnPinch = value;
				if (value)
				{
					this._scaleObjectOnPinch = false;
				}
			}
		}

		public bool scaleObjectOnPinch
		{
			get
			{
				return this._scaleObjectOnPinch;
			}
			set
			{
				this._scaleObjectOnPinch = value;
				if (value)
				{
					this._moveObjectOnPinch = false;
					if (Application.get_isPlaying() && !this._minMaxScaleSet)
					{
						this.minScale = base.get_transform().get_localScale() * this.minScaleRatio;
						this.maxScale = base.get_transform().get_localScale() * this.maxScaleRatio;
						this._minMaxScaleSet = true;
					}
				}
			}
		}

		public bool useDefaultPinchSettings
		{
			get
			{
				return this._useDefaultPinchSettings;
			}
			set
			{
				this._useDefaultPinchSettings = value;
			}
		}

		public float pinchableDistance
		{
			get
			{
				return this._pinchableDistance;
			}
			set
			{
				this._pinchableDistance = value;
			}
		}

		public HandType pinchingHandType
		{
			get
			{
				return this._pinchingHandType;
			}
			set
			{
				this._pinchingHandType = value;
			}
		}

		public bool useMinScale
		{
			get
			{
				return this._useMinScale;
			}
			set
			{
				this._useMinScale = value;
			}
		}

		public bool useMaxScale
		{
			get
			{
				return this._useMaxScale;
			}
			set
			{
				this._useMaxScale = value;
			}
		}

		public float minScaleRatio
		{
			get
			{
				return this._minScaleRatio;
			}
			set
			{
				if (this._minScaleRatio != value)
				{
					if (Application.get_isPlaying())
					{
						this.minScale = this.minScale / this._minScaleRatio * value;
					}
					this._minScaleRatio = value;
				}
			}
		}

		public float maxScaleRatio
		{
			get
			{
				return this._maxScaleRatio;
			}
			set
			{
				if (this._maxScaleRatio != value)
				{
					if (Application.get_isPlaying())
					{
						this.maxScale = this.maxScale / this._maxScaleRatio * value;
					}
					this._maxScaleRatio = value;
				}
			}
		}

		public Vector3 minScale
		{
			get
			{
				return this._minScale;
			}
			set
			{
				this._minScale = value;
			}
		}

		public Vector3 maxScale
		{
			get
			{
				return this._maxScale;
			}
			set
			{
				this._maxScale = value;
			}
		}

		public bool pinched
		{
			get
			{
				return this._pinched;
			}
			set
			{
				this._pinched = value;
			}
		}

		public bool touchable
		{
			get
			{
				return this._touchable;
			}
			set
			{
				this._touchable = value;
				if (MetaSingleton<MetaManager>.Instance != null)
				{
					MetaSingleton<MetaManager>.Instance.AddRemoveInteractable(InteractableType.Touchable, base.get_transform(), value);
				}
			}
		}

		public bool useDefaultTouchSettings
		{
			get
			{
				return this._useDefaultTouchSettings;
			}
			set
			{
				this._useDefaultTouchSettings = value;
			}
		}

		public bool touchableDwellable
		{
			get
			{
				return this._touchableDwellable;
			}
			set
			{
				this._touchableDwellable = value;
			}
		}

		public bool touchDwellMustBeStable
		{
			get
			{
				return this._touchDwellMustBeStable;
			}
			set
			{
				this._touchDwellMustBeStable = value;
			}
		}

		public float touchDwellTime
		{
			get
			{
				return this._touchDwellTime;
			}
			set
			{
				this._touchDwellTime = value;
			}
		}

		public HandType touchingHandType
		{
			get
			{
				return this._touchingHandType;
			}
			set
			{
				this._touchingHandType = value;
			}
		}

		public bool pointable
		{
			get
			{
				return this._pointable;
			}
			set
			{
				this._pointable = value;
				if (MetaSingleton<MetaManager>.Instance != null)
				{
					MetaSingleton<MetaManager>.Instance.AddRemoveInteractable(InteractableType.Pointable, base.get_transform(), value);
				}
			}
		}

		public bool useDefaultPointSettings
		{
			get
			{
				return this._useDefaultPointSettings;
			}
			set
			{
				this._useDefaultPointSettings = value;
			}
		}

		public bool pointDwellMustBeStable
		{
			get
			{
				return this._pointDwellMustBeStable;
			}
			set
			{
				this._pointDwellMustBeStable = value;
			}
		}

		public bool pointableDwellable
		{
			get
			{
				return this._pointableDwellable;
			}
			set
			{
				this._pointableDwellable = value;
			}
		}

		public float pointDwellTime
		{
			get
			{
				return this._pointDwellTime;
			}
			set
			{
				this._pointDwellTime = value;
			}
		}

		public HandType pointingHandType
		{
			get
			{
				return this._pointingHandType;
			}
			set
			{
				this._pointingHandType = value;
			}
		}

		public bool gazeable
		{
			get
			{
				return this._gazeable;
			}
			set
			{
				this._gazeable = value;
			}
		}

		public bool gazeableDwellable
		{
			get
			{
				return this._gazeableDwellable;
			}
			set
			{
				this._gazeableDwellable = value;
			}
		}

		public MetaGesture gesture
		{
			get
			{
				return this._gesture;
			}
			set
			{
				this._gesture = value;
			}
		}

		public bool orbital
		{
			get
			{
				return this._orbital;
			}
			set
			{
				this._orbital = value;
				this.ManageMetaTransform();
				if (value)
				{
					this.hud = false;
					this.palmLock = false;
					this.fingerLock = false;
				}
			}
		}

		public bool useDefaultOrbitalSettings
		{
			get
			{
				return this._useDefaultOrbitalSettings;
			}
			set
			{
				this._useDefaultOrbitalSettings = value;
			}
		}

		public bool orbitalLockDistance
		{
			get
			{
				return this._orbitalLockDistance;
			}
			set
			{
				this._orbitalLockDistance = value;
			}
		}

		public bool userReachDistance
		{
			get
			{
				return this._userReachDistance;
			}
			set
			{
				this._userReachDistance = value;
			}
		}

		public float lockDistance
		{
			get
			{
				return this._lockDistance;
			}
			set
			{
				this._lockDistance = value;
			}
		}

		public bool orbitalLookAtCamera
		{
			get
			{
				return this._orbitalLookAtCamera;
			}
			set
			{
				this._orbitalLookAtCamera = value;
			}
		}

		public bool orbitalLookAtCameraFlipY
		{
			get
			{
				return this._orbitalLookAtCameraFlipY;
			}
			set
			{
				this._orbitalLookAtCameraFlipY = value;
			}
		}

		public bool hud
		{
			get
			{
				return this._hud;
			}
			set
			{
				this._hud = value;
				if (value)
				{
					this.ManageMetaTransform();
					if (base.GetComponent<MetaTransform>() != null)
					{
						base.GetComponent<MetaTransform>().SetupHUDLock();
					}
					this.orbital = false;
					this.palmLock = false;
					this.fingerLock = false;
				}
				else
				{
					if (base.GetComponent<MetaTransform>() != null)
					{
						base.GetComponent<MetaTransform>().SetupHUDLock();
					}
					this.ManageMetaTransform();
				}
			}
		}

		public bool useDefaultHUDSettings
		{
			get
			{
				return this._useDefaultHUDsettings;
			}
			set
			{
				this._useDefaultHUDsettings = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().SetupHUDLock();
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockPosition
		{
			get
			{
				return this._hudLockPosition;
			}
			set
			{
				this._hudLockPosition = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockPositionX
		{
			get
			{
				return this._hudLockPositionX;
			}
			set
			{
				this._hudLockPositionX = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockPositionY
		{
			get
			{
				return this._hudLockPositionY;
			}
			set
			{
				this._hudLockPositionY = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockPositionZ
		{
			get
			{
				return this._hudLockPositionZ;
			}
			set
			{
				this._hudLockPositionZ = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockRotation
		{
			get
			{
				return this._hudLockRotation;
			}
			set
			{
				this._hudLockRotation = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockRotationX
		{
			get
			{
				return this._hudLockRotationX;
			}
			set
			{
				this._hudLockRotationX = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockRotationY
		{
			get
			{
				return this._hudLockRotationY;
			}
			set
			{
				this._hudLockRotationY = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLockRotationZ
		{
			get
			{
				return this._hudLockRotationZ;
			}
			set
			{
				this._hudLockRotationZ = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().UpdateLockObject();
				}
			}
		}

		public bool hudLayer
		{
			get
			{
				return this._hudLayer;
			}
			set
			{
				this._hudLayer = value;
				if (base.GetComponent<MetaTransform>() != null)
				{
					base.GetComponent<MetaTransform>().SetupHUDLock();
				}
			}
		}

		public bool palmLock
		{
			get
			{
				return this._palmLock;
			}
			set
			{
				this._palmLock = value;
				if (value)
				{
					this.hud = false;
					this.orbital = false;
					this.fingerLock = false;
				}
				this.ManageMetaTransform();
			}
		}

		public HandType lockHandType
		{
			get
			{
				return this._lockHandType;
			}
			set
			{
				this._lockHandType = value;
			}
		}

		public bool fingerLock
		{
			get
			{
				return this._fingerLock;
			}
			set
			{
				this._fingerLock = value;
				if (value)
				{
					this.hud = false;
					this.orbital = false;
					this.palmLock = false;
				}
				this.ManageMetaTransform();
			}
		}

		public bool jaysify
		{
			get
			{
				return this._jaysify;
			}
			set
			{
				this._jaysify = value;
				if (this._jaysify)
				{
					if (base.GetComponent<VectorMeshObject>() == null)
					{
						base.get_gameObject().AddComponent<VectorMeshObject>();
						VectorMeshObject component = base.GetComponent<VectorMeshObject>();
						if (base.get_gameObject().get_activeInHierarchy())
						{
							component.BuildWhenReady();
						}
					}
					else
					{
						base.GetComponent<VectorMeshObject>().set_enabled(true);
					}
				}
				else if (base.GetComponent<VectorMeshObject>() != null)
				{
					base.GetComponent<VectorMeshObject>().set_enabled(false);
				}
			}
		}

		public bool arrow
		{
			get
			{
				return this._arrow;
			}
			set
			{
				this._arrow = value;
				if (this._arrow && MetaSingleton<MetaArrow>.Instance != null)
				{
					MetaSingleton<MetaArrow>.Instance.targetTransform = base.get_transform();
				}
			}
		}

		public bool canvasTarget
		{
			get
			{
				return this._canvasTarget;
			}
			set
			{
				this._canvasTarget = value;
				if (MetaSingleton<MetaManager>.Instance != null)
				{
					MetaSingleton<MetaManager>.Instance.AddRemoveInteractable(InteractableType.CanvasTarget, base.get_transform(), this._canvasTarget);
				}
				if (this._canvasTarget)
				{
					if (base.GetComponent<CanvasTarget>() == null)
					{
						base.get_gameObject().AddComponent<CanvasTarget>();
					}
					else
					{
						base.GetComponent<CanvasTarget>().set_enabled(true);
					}
				}
				else if (base.GetComponent<CanvasTarget>() != null)
				{
					base.GetComponent<CanvasTarget>().set_enabled(false);
				}
			}
		}

		public int canvasTargetID
		{
			get
			{
				return this._canvasTargetID;
			}
			set
			{
				this._canvasTargetID = value;
			}
		}

		public bool markerTargetPlaceable
		{
			get
			{
				return this._markerTargetPlaceable;
			}
			set
			{
				this._markerTargetPlaceable = value;
			}
		}

		public bool markerTargetPlaceableHighlight
		{
			get
			{
				return this._markerTargetPlaceableHighlight;
			}
			set
			{
				this._markerTargetPlaceableHighlight = value;
			}
		}

		public bool markerTargetPersistent
		{
			get
			{
				return this._markerTargetPersistent;
			}
			set
			{
				this._markerTargetPersistent = value;
			}
		}

		public bool markerTarget
		{
			get
			{
				return this._markerTarget;
			}
			set
			{
				this._markerTarget = value;
				if (MetaSingleton<MetaManager>.Instance != null)
				{
					MetaSingleton<MetaManager>.Instance.AddRemoveInteractable(InteractableType.MarkerTarget, base.get_transform(), this._markerTarget);
				}
				if (this._markerTarget)
				{
					if (base.GetComponent<MarkerTarget>() == null)
					{
						base.get_gameObject().AddComponent<MarkerTarget>();
					}
					else
					{
						base.GetComponent<MarkerTarget>().set_enabled(true);
					}
					this.markerTargetID = this._markerTargetID;
				}
				else if (base.GetComponent<MarkerTarget>() != null)
				{
					base.GetComponent<MarkerTarget>().set_enabled(false);
				}
			}
		}

		public int markerTargetID
		{
			get
			{
				return this._markerTargetID;
			}
			set
			{
				this._markerTargetID = value;
				if (base.GetComponent<MarkerTarget>() != null)
				{
					base.GetComponent<MarkerTarget>().id = this._markerTargetID;
				}
			}
		}

		private void ManageMetaTransform()
		{
			if (this.hud || this.orbital || this.palmLock || this.fingerLock)
			{
				if (base.GetComponent<MetaTransform>() == null)
				{
					base.get_gameObject().AddComponent<MetaTransform>();
				}
				else
				{
					base.GetComponent<MetaTransform>().set_enabled(true);
				}
			}
			else if (base.GetComponent<MetaTransform>() != null)
			{
				base.GetComponent<MetaTransform>().set_enabled(false);
			}
		}

		public int GetPersistentMarkerTargetID()
		{
			return PlayerPrefs.GetInt(Application.get_loadedLevelName() + "." + base.get_gameObject().get_name());
		}

		public void SetPersistentMarkerTargetID(int idToSet)
		{
			PlayerPrefs.SetInt(Application.get_loadedLevelName() + "." + base.get_gameObject().get_name(), idToSet);
		}

		private void InitState()
		{
			this.grabbable = this._grabbable;
			this.pinchable = this._pinchable;
			this.touchable = this._touchable;
			this.gazeable = this._gazeable;
			this.orbital = this._orbital;
			this.hud = this._hud;
			this.palmLock = this._palmLock;
			this.fingerLock = this._fingerLock;
			this.jaysify = this._jaysify;
			this.arrow = this._arrow;
			this.markerTarget = this._markerTarget;
			this.canvasTarget = this._canvasTarget;
			if (((this.grabbable && this.scaleObjectOnGrab) || this.scaleObjectOnTwoHandedGrab || (this.pinchable && this.scaleObjectOnPinch)) && !this._minMaxScaleSet)
			{
				this.minScale = base.get_transform().get_localScale() * this.minScaleRatio;
				this.maxScale = base.get_transform().get_localScale() * this.maxScaleRatio;
				this._minMaxScaleSet = true;
			}
		}

		private void Start()
		{
			this.InitState();
		}

		private void Update()
		{
		}

		private void OnEnable()
		{
			if (this._prevSet)
			{
				this.markerTarget = this._markerTargetPrev;
				this.canvasTarget = this._canvasTargetPrev;
			}
		}

		private void OnDisable()
		{
			this._prevSet = true;
			this._markerTargetPrev = this.markerTarget;
			this._canvasTargetPrev = this.canvasTarget;
			if (MetaSingleton<MetaManager>.Instance != null)
			{
				this.markerTarget = false;
				this.canvasTarget = false;
			}
		}
	}
}
