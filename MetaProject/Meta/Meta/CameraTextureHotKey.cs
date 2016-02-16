// Decompiled with JetBrains decompiler
// Type: Meta.CameraTextureHotKey
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class CameraTextureHotKey : MonoBehaviour
  {
    private CameraTextureTarget cameraTextureTarget;
    private MeshRenderer webCameraTextureRenderer;

    public CameraTextureHotKey()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      if (Object.op_Equality((Object) this.cameraTextureTarget, (Object) null))
        this.cameraTextureTarget = (CameraTextureTarget) ((Component) this).GetComponent<CameraTextureTarget>();
      if (!Object.op_Equality((Object) this.webCameraTextureRenderer, (Object) null))
        return;
      this.webCameraTextureRenderer = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
    }

    private void Update()
    {
      if (Object.op_Equality((Object) this.cameraTextureTarget, (Object) null))
        this.cameraTextureTarget = (CameraTextureTarget) ((Component) this).GetComponent<CameraTextureTarget>();
      if (Object.op_Equality((Object) this.webCameraTextureRenderer, (Object) null))
        this.webCameraTextureRenderer = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
      if (!Object.op_Inequality((Object) this.cameraTextureTarget, (Object) null) || !(MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed != string.Empty) || !Input.GetKeyDown(MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed))
        return;
      this.cameraTextureTarget.Toggle();
    }
  }
}
