// Decompiled with JetBrains decompiler
// Type: Meta.DeviceTextureSource
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  public class DeviceTextureSource : MetaSingleton<DeviceTextureSource>, IMetaEventReceiver
  {
    private CameraTextureSource[] textureSources = new CameraTextureSource[4];
    private int _depthIndex = 1;
    private int _irIndex = 2;
    private int _colorIndex;
    private byte[] byteData;

    public Texture2D GetDeviceTexture(int device)
    {
      if (device >= 0 && device < this.textureSources.Length && this.textureSources[device] != null)
        return this.textureSources[device].textureData;
      return Texture2D.get_whiteTexture();
    }

    public bool IsDeviceTextureRegistered(int device)
    {
      return this.textureSources[device] != null;
    }

    public int GetNumberOfSources()
    {
      return this.textureSources.Length;
    }

    [DllImport("MetaVisionDLL")]
    internal static extern void registerCameraTexture();

    [DllImport("MetaVisionDLL")]
    internal static extern void getIRTexture(ref CameraTextureSource.ImageTexture irTexture);

    [DllImport("MetaVisionDLL")]
    internal static extern void getDepthTexture(ref CameraTextureSource.ImageTexture depthTexture);

    [DllImport("MetaVisionDLL")]
    internal static extern void getRGBTexture(ref CameraTextureSource.ImageTexture irTexture);

    internal void getFakeImageTexture(ref CameraTextureSource.ImageTexture fakeImage)
    {
      Marshal.Copy(this.byteData, 0, fakeImage.data, this.byteData.Length);
    }

    private void registerColorData()
    {
      if (this.textureSources[this._colorIndex] != null)
        return;
      CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(DeviceTextureSource.getRGBTexture);
      this.textureSources[this._colorIndex] = new CameraTextureSource(MetaCore.Instance.DeviceInformation.colorWidth, MetaCore.Instance.DeviceInformation.colorHeight, getTextureHandle);
    }

    private void registerDepthData()
    {
      if (this.textureSources[this._depthIndex] != null)
        return;
      CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(DeviceTextureSource.getDepthTexture);
      this.textureSources[this._depthIndex] = new CameraTextureSource(MetaCore.Instance.DeviceInformation.depthWidth, MetaCore.Instance.DeviceInformation.depthHeight, getTextureHandle);
    }

    private void registerIRData()
    {
      if (this.textureSources[this._irIndex] != null)
        return;
      CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(DeviceTextureSource.getIRTexture);
      this.textureSources[this._irIndex] = new CameraTextureSource(MetaCore.Instance.DeviceInformation.depthWidth, MetaCore.Instance.DeviceInformation.depthHeight, getTextureHandle);
    }

    private void registerImageData()
    {
      if (this.textureSources[3] != null || Assembly.GetAssembly(((object) this).GetType()).GetType("MetaInternalEditor", false, true) != null)
        return;
      Color32[] color32Array = new Color32[16384];
      byte[] numArray1 = new byte[65536];
      this.byteData = new byte[65536];
      string path = "Assets\\Meta Internal\\Art Assets\\test_image.jpg";
      if (File.Exists(path))
      {
        byte[] numArray2 = File.ReadAllBytes(path);
        Texture2D texture2D = new Texture2D(128, 128);
        texture2D.LoadImage(numArray2);
        Color32[] pixels32 = texture2D.GetPixels32();
        try
        {
          Marshal.Copy(GCHandle.Alloc((object) pixels32, GCHandleType.Pinned).AddrOfPinnedObject(), this.byteData, 0, pixels32.Length * 4);
        }
        catch (Exception ex)
        {
          Debug.Log((object) ex);
        }
      }
      this.textureSources[3] = new CameraTextureSource(128, 128, new CameraTextureSource.GetTextureDataHandle(this.getFakeImageTexture));
    }

    public void registerTextureDevice(int device)
    {
      if (device < 0)
        return;
      switch (device)
      {
        case 0:
          this.registerColorData();
          break;
        case 1:
          this.registerDepthData();
          break;
        case 2:
          this.registerIRData();
          break;
        case 3:
          this.registerImageData();
          break;
        default:
          return;
      }
      this.textureSources[device].registerClient();
    }

    public void unregisterTextureDevice(int device)
    {
      if (device < 0 || device >= this.textureSources.Length || this.textureSources[device] == null)
        return;
      this.textureSources[device].unregisterClient();
    }

    public void MetaInit()
    {
      if (!Object.op_Inequality((Object) MetaCore.Instance, (Object) null))
        return;
      DeviceTextureSource.registerCameraTexture();
    }

    public void MetaUpdate()
    {
      for (int index = 0; index < this.textureSources.Length; ++index)
      {
        if (this.textureSources[index] != null)
          this.textureSources[index].Update();
      }
    }

    public void MetaOnDestroy()
    {
    }

    public void MetaLateUpdate()
    {
    }
  }
}
