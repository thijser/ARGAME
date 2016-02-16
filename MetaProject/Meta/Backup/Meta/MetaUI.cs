// Decompiled with JetBrains decompiler
// Type: Meta.MetaUI
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta
{
  [ExecuteInEditMode]
  public class MetaUI : MetaSingleton<MetaUI>
  {
    [SerializeField]
    private bool _enableGrid = true;
    [SerializeField]
    private VectorMeshObject.MetaVectorStyle _defaultVectorStyle = new VectorMeshObject.MetaVectorStyle();
    private GameObject _IMUCalibratingIndicator;
    private GameObject _LoadingIndicator;
    private GameObject _eventSystem;
    [SerializeField]
    [HideInInspector]
    private GameObject _mguiCanvas;

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
        if (Object.op_Inequality((Object) ((Component) this).get_transform().FindChild("Grid"), (Object) null))
          ((Component) ((Component) this).get_transform().FindChild("Grid")).get_gameObject().SetActive(value);
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
      this._IMUCalibratingIndicator = ((Component) ((Component) this).get_transform().FindChild("IMU Calibrating").GetChild(0)).get_gameObject();
      this._LoadingIndicator = ((Component) ((Component) this).get_transform().FindChild("Loading").GetChild(0)).get_gameObject();
      this._eventSystem = ((Component) Object.FindObjectOfType<EventSystem>()).get_gameObject();
    }

    private void Update()
    {
      this.enableGrid = this._enableGrid;
    }

    internal void ToggleIMUCalibratingIndicator(bool show)
    {
      this._IMUCalibratingIndicator.SetActive(show);
      this._eventSystem.SetActive(!show);
      ((Behaviour) ((Component) MetaSingleton<Hands>.Instance).get_gameObject().GetComponent<GestureManager>()).set_enabled(!show);
    }

    internal void ToggleLoadingIndicator(bool show)
    {
      this._LoadingIndicator.SetActive(show);
    }
  }
}
