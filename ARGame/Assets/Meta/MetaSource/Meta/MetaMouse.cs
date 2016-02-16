using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta
{
	public class MetaMouse : MetaSingleton<MetaMouse>
	{
		private UnityEngine.Camera _leftCam;

		private UnityEngine.Camera _rightCam;

		private EventSystem _eventSystem;

		private PointerEventData _pointerEventData;

		private PointerInputModule.MouseButtonEventData _mouseButtonEventData;

		private GameObject _virtualPointer;

		private Transform _hudLockObject;

		private bool _metaMouseActive;

		private GameObject _hoveredObject;

		private GameObject _pressedObject;

		private bool _activeButNotOnScreen;

		private Vector2 _mouseLeavePoint;

		private Vector2 _lastMousePos = Vector2.zero;

		private float _currentDepth = 0.39f;

		private float _targetDepth = 0.39f;

		private float _defaultDepth = 0.39f;

		private float _depthSpeed = 5f;

		private float _depthOffset = 0.001f;

		[SerializeField]
		private bool _enableMetaMouse = true;

		[SerializeField]
		private bool _lockToScreen;

		[SerializeField]
		private float _sensitivity = 1f;

		[SerializeField]
		private Color _normalColor = new Color(1f, 0f, 0f, 1f);

		[SerializeField]
		private Color _highlightedColor = new Color(0.945f, 0.353f, 0.141f, 1f);

		[SerializeField]
		private Color _pressedColor = new Color(0.945f, 0.353f, 0.141f, 1f);

		public bool enableMetaMouse
		{
			get
			{
				return this._enableMetaMouse;
			}
			set
			{
				if (value)
				{
					if (!this._enableMetaMouse)
					{
						this.OnEnable();
					}
				}
				else if (this._enableMetaMouse)
				{
					this.OnDisable();
				}
				this._enableMetaMouse = value;
			}
		}

		public bool lockToScreen
		{
			get
			{
				return this._lockToScreen;
			}
			set
			{
				this._lockToScreen = value;
			}
		}

		public Vector2 position
		{
			get
			{
				return this.GetMouseWorldToScreenPoint();
			}
		}

		public Vector3 worldPosition
		{
			get
			{
				return this._virtualPointer.transform.position;
			}
		}

		public GameObject ObjectOfInterest
		{
			get
			{
				return this._hoveredObject;
			}
		}

		public float sensitivity
		{
			get
			{
				return this._sensitivity;
			}
			set
			{
				if (value < 0.1f)
				{
					this._sensitivity = 0.1f;
				}
				else if (value > 10f)
				{
					this._sensitivity = 10f;
				}
				else
				{
					this._sensitivity = value;
				}
			}
		}

		public Color normalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				this._normalColor = value;
			}
		}

		public Color highlightedColor
		{
			get
			{
				return this._highlightedColor;
			}
			set
			{
				this._highlightedColor = value;
			}
		}

		public Color pressedColor
		{
			get
			{
				return this._pressedColor;
			}
			set
			{
				this._pressedColor = value;
			}
		}

		private void Awake()
		{
			this._virtualPointer = base.transform.GetChild(0).gameObject;
			this._hudLockObject = new GameObject().transform;
			this._hudLockObject.parent = UnityEngine.Camera.main.transform;
			this._hudLockObject.rotation = this._virtualPointer.transform.rotation;
			this._hudLockObject.name = base.gameObject.name + ".HUDLockObject";
			this._leftCam = GameObject.Find("MetaCameraLeft").GetComponent<UnityEngine.Camera>();
			this._rightCam = GameObject.Find("MetaCameraRight").GetComponent<UnityEngine.Camera>();
			this._eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			this._pointerEventData = new PointerEventData(this._eventSystem);
			this._pointerEventData.pointerId = -1;
			this._mouseButtonEventData = new PointerInputModule.MouseButtonEventData();
			this._mouseButtonEventData.buttonData = this._pointerEventData;
		}

		private bool MouseIsOnWindow()
		{
			int num = (int)ProMouse.Instance.GetLocalMousePosition().x;
			int num2 = (int)ProMouse.Instance.GetLocalMousePosition().y;
			return num >= 0 && num <= Screen.width && num2 >= 0 && num2 <= Screen.height && ProMouse.Instance.GetLocalMousePosition() != this._lastMousePos;
		}

		private bool VirtualPointerIsOnWindow()
		{
			return MetaCamera.IsVisible(this._virtualPointer.transform.GetChild(0).GetComponent<Renderer>().bounds);
		}

		private Vector3 GetMouseScreenToWorldPoint()
		{
			Vector3 result;
			if (MetaCamera.GetCameraMode() == CameraType.Monocular)
			{
				result = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this._defaultDepth));
			}
			else if (Input.mousePosition.x <= (float)(Screen.width / 2))
			{
				result = this._leftCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this._defaultDepth));
			}
			else
			{
				result = this._rightCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this._defaultDepth));
			}
			return result;
		}

		private Vector3 GetMouseWorldToScreenPoint()
		{
			Vector3 result;
			if (MetaCamera.GetCameraMode() == CameraType.Monocular)
			{
				result = UnityEngine.Camera.main.WorldToScreenPoint(this._virtualPointer.transform.position);
			}
			else if (UnityEngine.Camera.main.WorldToScreenPoint(this._virtualPointer.transform.position).x <= (float)(Screen.width / 2))
			{
				result = this._leftCam.WorldToScreenPoint(this._virtualPointer.transform.position);
			}
			else
			{
				result = this._rightCam.WorldToScreenPoint(this._virtualPointer.transform.position);
			}
			return result;
		}

		private void LateUpdate()
		{
			if (this._enableMetaMouse)
			{
				if (!this._metaMouseActive && this.MouseIsOnWindow())
				{
					this.InitMetaMouse();
				}
				else if (this._metaMouseActive && !this.VirtualPointerIsOnWindow())
				{
					this.ProcessHiddenMouse();
				}
				else if (this._metaMouseActive && this._activeButNotOnScreen)
				{
					this._activeButNotOnScreen = false;
					this.ProcessMouse();
				}
				else if (this._metaMouseActive && !this._activeButNotOnScreen)
				{
					this.ProcessMouse();
				}
			}
		}

		private void InitMetaMouse()
		{
			this._metaMouseActive = true;
			this._virtualPointer.transform.position = this.GetMouseScreenToWorldPoint();
			if (this._lockToScreen)
			{
				this._hudLockObject.transform.position = this._virtualPointer.transform.position;
			}
			this._virtualPointer.SetActive(true);
			this._pointerEventData.position = this._virtualPointer.transform.position;
		}

		private void ProcessHiddenMouse()
		{
			if (!this._activeButNotOnScreen)
			{
				this._mouseLeavePoint = this.GetMouseWorldToScreenPoint();
				this._activeButNotOnScreen = true;
			}
			if (Input.GetAxis("Mouse X") > 0f || Input.GetAxis("Mouse Y") > 0f)
			{
				ScreenCursor.SetMouseCursorLockState(false);
				this._metaMouseActive = false;
				ProMouse.Instance.SetCursorPosition((int)this._mouseLeavePoint.x, (int)this._mouseLeavePoint.y);
				this._lastMousePos = ProMouse.Instance.GetLocalMousePosition();
				if (this._lockToScreen)
				{
					this._hudLockObject.transform.position = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this._defaultDepth));
				}
				this._virtualPointer.SetActive(false);
				this._activeButNotOnScreen = false;
			}
		}

		private void ProcessMouse()
		{
			ScreenCursor.SetMouseCursorLockState(true);
			float num = Input.GetAxis("Mouse X") / 100f;
			Vector3 vector = num * UnityEngine.Camera.main.transform.right;
			float num2 = Input.GetAxis("Mouse Y") / 100f;
			Vector3 vector2 = num2 * UnityEngine.Camera.main.transform.up;
			Vector3 vector3 = this._virtualPointer.transform.position + (vector * this._sensitivity + vector2 * this._sensitivity);
			if (this.lockToScreen)
			{
				this._hudLockObject.transform.position = vector3;
				this._virtualPointer.transform.position = this._hudLockObject.position;
			}
			else
			{
				this._virtualPointer.transform.position = vector3;
			}
			this._virtualPointer.transform.rotation = this._hudLockObject.rotation;
			if (Input.GetMouseButtonDown(0))
			{
				this._mouseButtonEventData.buttonState = 0;
			}
			else if (Input.GetMouseButtonUp(0))
			{
			}
			else
			{
			}
			if (this._pointerEventData.pointerDrag != null)
			{
				float num3 = Vector3.Dot(this._pointerEventData.pointerDrag.transform.right, vector3 - this._pointerEventData.worldPosition);
				float num4 = Vector3.Dot(this._pointerEventData.pointerDrag.transform.up, vector3 - this._pointerEventData.worldPosition);
				this._pointerEventData.delta = new Vector2(num3, num4);
				PointerEventData expr_1C2 = this._pointerEventData;
				expr_1C2.position = expr_1C2.position + this._pointerEventData.delta;
			}
			this._pointerEventData.worldPosition = this._virtualPointer.transform.position;
			Vector2 vector4 = UnityEngine.Camera.main.WorldToScreenPoint(this._virtualPointer.transform.position);
			Ray ray = UnityEngine.Camera.main.ScreenPointToRay(vector4);
			LayerMask layerMask = 1056;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, layerMask))
			{
				GameObject gameObject = raycastHit.transform.gameObject;
				if (gameObject == this._hoveredObject || MetaMouse.ObjectImplemenetsPointerInterface(gameObject))
				{
					this._hoveredObject = gameObject;
					if (Input.GetMouseButtonDown(0))
					{
						this._pointerEventData.position = raycastHit.point;
					}
					RaycastResult pointerCurrentRaycast = default(RaycastResult);
					pointerCurrentRaycast.gameObject = gameObject;
					this._pointerEventData.pointerCurrentRaycast = pointerCurrentRaycast;
					this._targetDepth = raycastHit.distance - this._depthOffset;
					if (Input.GetMouseButton(0))
					{
						this.SetCursorColor(this._pressedColor);
					}
					else
					{
						this.SetCursorColor(this._highlightedColor);
					}
				}
				else
				{
					this.SetCursorToNormal();
				}
			}
			else
			{
				this.SetCursorToNormal();
			}
			this._mouseButtonEventData.buttonData.useDragThreshold = false;
			if (this._eventSystem.GetComponent<HandsInputModule>() != null)
			{
				this._eventSystem.GetComponent<HandsInputModule>().ProcessMouseEvent(this._mouseButtonEventData);
			}
			this.AdjustPointerDepth();
		}

		private void SetCursorToNormal()
		{
			if (this._pointerEventData.pointerPress == null)
			{
				this._targetDepth = this._defaultDepth;
				this.SetCursorColor(this._normalColor);
			}
			this._pointerEventData.pointerCurrentRaycast = default(RaycastResult);
		}

		private void AdjustPointerDepth()
		{
			this._currentDepth = Mathf.Lerp(this._currentDepth, this._targetDepth, Time.deltaTime * this._depthSpeed);
			Vector3 vector = Vector3.Normalize(this._virtualPointer.transform.position - UnityEngine.Camera.main.transform.position);
			if (this.lockToScreen)
			{
				this._hudLockObject.transform.position = UnityEngine.Camera.main.transform.position;
				this._hudLockObject.transform.Translate(vector * this._currentDepth, 0);
				this._virtualPointer.transform.position = this._hudLockObject.transform.position;
			}
			else
			{
				this._virtualPointer.transform.position = UnityEngine.Camera.main.transform.position;
				this._virtualPointer.transform.Translate(vector * this._currentDepth, 0);
			}
		}

		private static bool ObjectImplemenetsPointerInterface(GameObject objectToSearch)
		{
			MonoBehaviour[] components = objectToSearch.GetComponents<MonoBehaviour>();
			MonoBehaviour[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				MonoBehaviour monoBehaviour = array[i];
				if (monoBehaviour is IPointerClickHandler || monoBehaviour is IPointerDownHandler || monoBehaviour is IPointerUpHandler || monoBehaviour is IPointerEnterHandler || monoBehaviour is IPointerExitHandler)
				{
					return true;
				}
			}
			return false;
		}

		private void SetCursorColor(Color color)
		{
			Renderer[] componentsInChildren = this._virtualPointer.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Renderer renderer = array[i];
				renderer.material.color = color;
			}
		}

		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				HandsInputModule.processMouse = false;
				ScreenCursor.SetMouseCursorVisibility(false);
			}
		}

		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				HandsInputModule.processMouse = true;
				if (MetaSingleton<RenderingCameraManagerBase>.Instance != null && MetaCamera.GetCameraMode() == CameraType.Monocular)
				{
					ScreenCursor.SetMouseCursorVisibility(true);
				}
				else
				{
					ScreenCursor.SetMouseCursorVisibility(false);
				}
				ScreenCursor.SetMouseCursorLockState(false);
				this._virtualPointer.SetActive(false);
				this._metaMouseActive = false;
			}
		}
	}
}
