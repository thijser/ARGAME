// Decompiled with JetBrains decompiler
// Type: Meta.CameraTextureSource
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  public class CameraTextureSource
  {
    private CameraTextureSource.ImageTexture _texture = new CameraTextureSource.ImageTexture();
    private object thisLock = new object();
    private GCHandle _textureHandle;
    private byte[] _imageData;
    private CameraTextureSource.GetTextureDataHandle _getTextureData;
    private int _imageHeight;
    private int _imageWidth;
    private Texture2D _texture2D;
    private int _numberOfClients;

    public Texture2D textureData
    {
      get
      {
        lock (this.thisLock)
          return this._texture2D;
      }
      set
      {
      }
    }

    public CameraTextureSource(int width, int height, CameraTextureSource.GetTextureDataHandle getTextureHandle)
    {
      this._imageData = new byte[width * height * 4];
      this._imageHeight = this._texture.height = height;
      this._imageWidth = this._texture.width = width;
      this._textureHandle = GCHandle.Alloc((object) this._imageData, GCHandleType.Pinned);
      this._texture.data = this._textureHandle.AddrOfPinnedObject();
      this._textureHandle.Free();
      this._getTextureData = getTextureHandle;
      this._texture2D = new Texture2D(this._imageWidth, this._imageHeight, (TextureFormat) 4, false);
      ((Texture) this._texture2D).set_filterMode((FilterMode) 0);
      ((Texture) this._texture2D).set_mipMapBias(0.0f);
    }

    public void registerClient()
    {
      ++this._numberOfClients;
    }

    public void unregisterClient()
    {
      if (this._numberOfClients <= 0)
        return;
      --this._numberOfClients;
    }

    public void Update()
    {
      if (this._numberOfClients <= 0)
        return;
      lock (this.thisLock)
      {
        this._getTextureData(ref this._texture);
        Marshal.Copy(this._texture.data, this._imageData, 0, this._texture.height * this._texture.width * 4);
        this._texture2D.LoadRawTextureData(this._imageData);
        this._texture2D.Apply();
      }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImageTexture
    {
      public IntPtr data;
      public int height;
      public int width;
    }

    public delegate void GetTextureDataHandle(ref CameraTextureSource.ImageTexture imageTexture);
  }
}
