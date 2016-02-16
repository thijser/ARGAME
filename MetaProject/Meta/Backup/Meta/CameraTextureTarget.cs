// Decompiled with JetBrains decompiler
// Type: Meta.CameraTextureTarget
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class CameraTextureTarget : MonoBehaviour
  {
    [SerializeField]
    internal MeshRenderer renderTarget;
    internal Texture2D cameraTexture;
    [SerializeField]
    internal int sourceDevice;
    private int lastSource;

    public CameraTextureTarget()
    {
      base.\u002Ector();
    }

    private void OnValidate()
    {
      this.SwitchDevice();
    }

    internal void Start()
    {
      if (!Object.op_Equality((Object) this.renderTarget, (Object) null))
        return;
      this.renderTarget = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
    }

    private void SetTargetTexture()
    {
      if (!Object.op_Inequality((Object) MetaSingleton<DeviceTextureSource>.Instance, (Object) null) || !Object.op_Inequality((Object) MetaCore.Instance, (Object) null) || !((Behaviour) MetaSingleton<DeviceTextureSource>.Instance).get_enabled())
        return;
      this.cameraTexture = MetaSingleton<DeviceTextureSource>.Instance.GetDeviceTexture(this.sourceDevice);
      if (!Object.op_Inequality((Object) this.renderTarget, (Object) null) || !Object.op_Inequality((Object) ((Renderer) this.renderTarget).get_material(), (Object) null))
        return;
      ((Renderer) this.renderTarget).get_material().set_mainTexture((Texture) this.cameraTexture);
    }

    private void OnEnable()
    {
      if (!Object.op_Inequality((Object) MetaSingleton<DeviceTextureSource>.Instance, (Object) null) || !Object.op_Inequality((Object) MetaCore.Instance, (Object) null))
        return;
      MetaSingleton<DeviceTextureSource>.Instance.registerTextureDevice(this.sourceDevice);
      this.SetTargetTexture();
    }

    private void Disable()
    {
      if (!Object.op_Inequality((Object) MetaSingleton<DeviceTextureSource>.Instance, (Object) null) || !Object.op_Inequality((Object) MetaCore.Instance, (Object) null))
        return;
      MetaSingleton<DeviceTextureSource>.Instance.unregisterTextureDevice(this.sourceDevice);
      this.SetTargetTexture();
    }

    public void Toggle()
    {
      if (Object.op_Equality((Object) this.renderTarget, (Object) null))
        this.renderTarget = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
      ((Behaviour) this).set_enabled(!((Behaviour) this).get_enabled());
      ((Renderer) this.renderTarget).set_enabled(((Behaviour) this).get_enabled());
    }

    internal void CycleSource()
    {
      this.sourceDevice = ++this.sourceDevice % MetaSingleton<DeviceTextureSource>.Instance.GetNumberOfSources();
      this.SwitchDevice();
    }

    internal void SwitchDevice()
    {
      if (Object.op_Inequality((Object) MetaSingleton<DeviceTextureSource>.Instance, (Object) null) && ((Behaviour) MetaSingleton<DeviceTextureSource>.Instance).get_enabled() && Object.op_Inequality((Object) MetaCore.Instance, (Object) null))
      {
        MetaSingleton<DeviceTextureSource>.Instance.unregisterTextureDevice(this.lastSource);
        this.lastSource = this.sourceDevice;
        MetaSingleton<DeviceTextureSource>.Instance.registerTextureDevice(this.sourceDevice);
      }
      this.SetTargetTexture();
    }
  }
}
