// Decompiled with JetBrains decompiler
// Type: Meta.MetaMouse
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta
{
  public class MetaMouse : MetaSingleton<MetaMouse>
  {
    private Vector2 _lastMousePos = Vector2.get_zero();
    private float _currentDepth = 0.39f;
    private float _targetDepth = 0.39f;
    private float _defaultDepth = 0.39f;
    private float _depthSpeed = 5f;
    private float _depthOffset = 1.0 / 1000.0;
    [SerializeField]
    private bool _enableMetaMouse = true;
    [SerializeField]
    private float _sensitivity = 1f;
    [SerializeField]
    private Color _normalColor = new Color(1f, 0.0f, 0.0f, 1f);
    [SerializeField]
    private Color _highlightedColor = new Color(0.945f, 0.353f, 0.141f, 1f);
    [SerializeField]
    private Color _pressedColor = new Color(0.945f, 0.353f, 0.141f, 1f);
    private Camera _leftCam;
    private Camera _rightCam;
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
    [SerializeField]
    private bool _lockToScreen;

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
            this.OnEnable();
        }
        else if (this._enableMetaMouse)
          this.OnDisable();
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
        return Vector2.op_Implicit(this.GetMouseWorldToScreenPoint());
      }
    }

    public Vector3 worldPosition
    {
      get
      {
        return this._virtualPointer.get_transform().get_position();
      }
    }

    public GameObject objectOfInterest
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
        if ((double) value < 0.100000001490116)
          this._sensitivity = 0.1f;
        else if ((double) value > 10.0)
          this._sensitivity = 10f;
        else
          this._sensitivity = value;
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
      this._virtualPointer = ((Component) ((Component) this).get_transform().GetChild(0)).get_gameObject();
      this._hudLockObject = new GameObject().get_transform();
      this._hudLockObject.set_parent(((Component) Camera.get_main()).get_transform());
      this._hudLockObject.set_rotation(this._virtualPointer.get_transform().get_rotation());
      ((Object) this._hudLockObject).set_name(((Object) ((Component) this).get_gameObject()).get_name() + ".HUDLockObject");
      this._leftCam = GameObject.Find("MetaCameraLeft").get_camera();
      this._rightCam = GameObject.Find("MetaCameraRight").get_camera();
      this._eventSystem = (EventSystem) Object.FindObjectOfType<EventSystem>();
      this._pointerEventData = new PointerEventData(this._eventSystem);
      this._pointerEventData.set_pointerId(-1);
      this._mouseButtonEventData = new PointerInputModule.MouseButtonEventData();
      this._mouseButtonEventData.buttonData = (__Null) this._pointerEventData;
    }

    private bool MouseIsOnWindow()
    {
      int num1 = (int) ProMouse.get_Instance().GetLocalMousePosition().x;
      int num2 = (int) ProMouse.get_Instance().GetLocalMousePosition().y;
      return num1 >= 0 && num1 <= Screen.get_width() && (num2 >= 0 && num2 <= Screen.get_height()) && Vector2.op_Inequality(ProMouse.get_Instance().GetLocalMousePosition(), this._lastMousePos);
    }

    private bool VirtualPointerIsOnWindow()
    {
      return MetaCamera.IsVisible(((Component) this._virtualPointer.get_transform().GetChild(0)).get_renderer().get_bounds());
    }

    private Vector3 GetMouseScreenToWorldPoint()
    {
      return MetaCamera.GetCameraMode() != CameraType.Monocular ? (Input.get_mousePosition().x > (double) (Screen.get_width() / 2) ? this._rightCam.ScreenToWorldPoint(new Vector3((float) Input.get_mousePosition().x, (float) Input.get_mousePosition().y, this._defaultDepth)) : this._leftCam.ScreenToWorldPoint(new Vector3((float) Input.get_mousePosition().x, (float) Input.get_mousePosition().y, this._defaultDepth))) : Camera.get_main().ScreenToWorldPoint(new Vector3((float) Input.get_mousePosition().x, (float) Input.get_mousePosition().y, this._defaultDepth));
    }

    private Vector3 GetMouseWorldToScreenPoint()
    {
      return MetaCamera.GetCameraMode() != CameraType.Monocular ? (Camera.get_main().WorldToScreenPoint(this._virtualPointer.get_transform().get_position()).x > (double) (Screen.get_width() / 2) ? this._rightCam.WorldToScreenPoint(this._virtualPointer.get_transform().get_position()) : this._leftCam.WorldToScreenPoint(this._virtualPointer.get_transform().get_position())) : Camera.get_main().WorldToScreenPoint(this._virtualPointer.get_transform().get_position());
    }

    private void LateUpdate()
    {
      if (!this._enableMetaMouse)
        return;
      if (!this._metaMouseActive && this.MouseIsOnWindow())
        this.InitMetaMouse();
      else if (this._metaMouseActive && !this.VirtualPointerIsOnWindow())
        this.ProcessHiddenMouse();
      else if (this._metaMouseActive && this._activeButNotOnScreen)
      {
        this._activeButNotOnScreen = false;
        this.ProcessMouse();
      }
      else
      {
        if (!this._metaMouseActive || this._activeButNotOnScreen)
          return;
        this.ProcessMouse();
      }
    }

    private void InitMetaMouse()
    {
      this._metaMouseActive = true;
      this._virtualPointer.get_transform().set_position(this.GetMouseScreenToWorldPoint());
      if (this._lockToScreen)
        ((Component) this._hudLockObject).get_transform().set_position(this._virtualPointer.get_transform().get_position());
      this._virtualPointer.SetActive(true);
      this._pointerEventData.set_position(Vector2.op_Implicit(this._virtualPointer.get_transform().get_position()));
    }

    private void ProcessHiddenMouse()
    {
      if (!this._activeButNotOnScreen)
      {
        this._mouseLeavePoint = Vector2.op_Implicit(this.GetMouseWorldToScreenPoint());
        this._activeButNotOnScreen = true;
      }
      if ((double) Input.GetAxis("Mouse X") <= 0.0 && (double) Input.GetAxis("Mouse Y") <= 0.0)
        return;
      ScreenCursor.SetMouseCursorLockState(false);
      this._metaMouseActive = false;
      ProMouse.get_Instance().SetCursorPosition((int) this._mouseLeavePoint.x, (int) this._mouseLeavePoint.y);
      this._lastMousePos = ProMouse.get_Instance().GetLocalMousePosition();
      if (this._lockToScreen)
        ((Component) this._hudLockObject).get_transform().set_position(Camera.get_main().ScreenToWorldPoint(new Vector3((float) Input.get_mousePosition().x, (float) Input.get_mousePosition().y, this._defaultDepth)));
      this._virtualPointer.SetActive(false);
      this._activeButNotOnScreen = false;
    }

    private void ProcessMouse()
    {
      ScreenCursor.SetMouseCursorLockState(true);
      Vector3 vector3 = Vector3.op_Addition(this._virtualPointer.get_transform().get_position(), Vector3.op_Addition(Vector3.op_Multiply(Vector3.op_Multiply(Input.GetAxis("Mouse X") / 100f, ((Component) Camera.get_main()).get_transform().get_right()), this._sensitivity), Vector3.op_Multiply(Vector3.op_Multiply(Input.GetAxis("Mouse Y") / 100f, ((Component) Camera.get_main()).get_transform().get_up()), this._sensitivity)));
      if (this.lockToScreen)
      {
        ((Component) this._hudLockObject).get_transform().set_position(vector3);
        this._virtualPointer.get_transform().set_position(this._hudLockObject.get_position());
      }
      else
        this._virtualPointer.get_transform().set_position(vector3);
      this._virtualPointer.get_transform().set_rotation(this._hudLockObject.get_rotation());
      this._mouseButtonEventData.buttonState = !Input.GetMouseButtonDown(0) ? (!Input.GetMouseButtonUp(0) ? (__Null) 3 : (__Null) 1) : (__Null) 0;
      if (Object.op_Inequality((Object) this._pointerEventData.get_pointerDrag(), (Object) null))
      {
        this._pointerEventData.set_delta(new Vector2(Vector3.Dot(this._pointerEventData.get_pointerDrag().get_transform().get_right(), Vector3.op_Subtraction(vector3, this._pointerEventData.get_worldPosition())), Vector3.Dot(this._pointerEventData.get_pointerDrag().get_transform().get_up(), Vector3.op_Subtraction(vector3, this._pointerEventData.get_worldPosition()))));
        PointerEventData pointerEventData = this._pointerEventData;
        Vector2 vector2 = Vector2.op_Addition(pointerEventData.get_position(), this._pointerEventData.get_delta());
        pointerEventData.set_position(vector2);
      }
      this._pointerEventData.set_worldPosition(this._virtualPointer.get_transform().get_position());
      Ray ray = Camera.get_main().ScreenPointToRay(Vector2.op_Implicit(Vector2.op_Implicit(Camera.get_main().WorldToScreenPoint(this._virtualPointer.get_transform().get_position()))));
      LayerMask layerMask = LayerMask.op_Implicit(1056);
      RaycastHit raycastHit;
      if (Physics.Raycast(ray, ref raycastHit, float.PositiveInfinity, LayerMask.op_Implicit(layerMask)))
      {
        // ISSUE: explicit reference operation
        GameObject gameObject = ((Component) ((RaycastHit) @raycastHit).get_transform()).get_gameObject();
        if (Object.op_Equality((Object) gameObject, (Object) this._hoveredObject) || MetaMouse.ObjectImplemenetsPointerInterface(gameObject))
        {
          this._hoveredObject = gameObject;
          if (Input.GetMouseButtonDown(0))
          {
            // ISSUE: explicit reference operation
            this._pointerEventData.set_position(Vector2.op_Implicit(((RaycastHit) @raycastHit).get_point()));
          }
          RaycastResult raycastResult = (RaycastResult) null;
          // ISSUE: explicit reference operation
          ((RaycastResult) @raycastResult).set_gameObject(gameObject);
          this._pointerEventData.set_pointerCurrentRaycast(raycastResult);
          // ISSUE: explicit reference operation
          this._targetDepth = ((RaycastHit) @raycastHit).get_distance() - this._depthOffset;
          if (Input.GetMouseButton(0))
            this.SetCursorColor(this._pressedColor);
          else
            this.SetCursorColor(this._highlightedColor);
        }
        else
          this.SetCursorToNormal();
      }
      else
        this.SetCursorToNormal();
      ((PointerEventData) this._mouseButtonEventData.buttonData).set_useDragThreshold(false);
      if (Object.op_Inequality((Object) ((Component) this._eventSystem).GetComponent<HandsInputModule>(), (Object) null))
        ((HandsInputModule) ((Component) this._eventSystem).GetComponent<HandsInputModule>()).ProcessMouseEvent(this._mouseButtonEventData);
      this.AdjustPointerDepth();
    }

    private void SetCursorToNormal()
    {
      if (Object.op_Equality((Object) this._pointerEventData.get_pointerPress(), (Object) null))
      {
        this._targetDepth = this._defaultDepth;
        this.SetCursorColor(this._normalColor);
      }
      this._pointerEventData.set_pointerCurrentRaycast((RaycastResult) null);
    }

    private void AdjustPointerDepth()
    {
      this._currentDepth = Mathf.Lerp(this._currentDepth, this._targetDepth, Time.get_deltaTime() * this._depthSpeed);
      Vector3 vector3 = Vector3.Normalize(Vector3.op_Subtraction(this._virtualPointer.get_transform().get_position(), ((Component) Camera.get_main()).get_transform().get_position()));
      if (this.lockToScreen)
      {
        ((Component) this._hudLockObject).get_transform().set_position(((Component) Camera.get_main()).get_transform().get_position());
        ((Component) this._hudLockObject).get_transform().Translate(Vector3.op_Multiply(vector3, this._currentDepth), (Space) 0);
        this._virtualPointer.get_transform().set_position(((Component) this._hudLockObject).get_transform().get_position());
      }
      else
      {
        this._virtualPointer.get_transform().set_position(((Component) Camera.get_main()).get_transform().get_position());
        this._virtualPointer.get_transform().Translate(Vector3.op_Multiply(vector3, this._currentDepth), (Space) 0);
      }
    }

    private static bool ObjectImplemenetsPointerInterface(GameObject objectToSearch)
    {
      foreach (MonoBehaviour monoBehaviour in (MonoBehaviour[]) objectToSearch.GetComponents<MonoBehaviour>())
      {
        if (monoBehaviour is IPointerClickHandler || monoBehaviour is IPointerDownHandler || (monoBehaviour is IPointerUpHandler || monoBehaviour is IPointerEnterHandler) || monoBehaviour is IPointerExitHandler)
          return true;
      }
      return false;
    }

    private void SetCursorColor(Color color)
    {
      foreach (Renderer renderer in (Renderer[]) this._virtualPointer.GetComponentsInChildren<Renderer>())
        renderer.get_material().set_color(color);
    }

    private void OnEnable()
    {
      if (!Application.get_isPlaying())
        return;
      HandsInputModule.processMouse = false;
      ScreenCursor.SetMouseCursorVisibility(false);
    }

    private void OnDisable()
    {
      if (!Application.get_isPlaying())
        return;
      HandsInputModule.processMouse = true;
      if (Object.op_Inequality((Object) MetaSingleton<RenderingCameraManagerBase>.Instance, (Object) null) && MetaCamera.GetCameraMode() == CameraType.Monocular)
        ScreenCursor.SetMouseCursorVisibility(true);
      else
        ScreenCursor.SetMouseCursorVisibility(false);
      ScreenCursor.SetMouseCursorLockState(false);
      this._virtualPointer.SetActive(false);
      this._metaMouseActive = false;
    }
  }
}
