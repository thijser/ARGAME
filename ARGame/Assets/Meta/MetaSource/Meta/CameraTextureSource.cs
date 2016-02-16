using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	public class CameraTextureSource
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct ImageTexture
		{
			public IntPtr data;

			public int height;

			public int width;
		}

		public delegate void GetTextureDataHandle(ref CameraTextureSource.ImageTexture imageTexture);

		private GCHandle _textureHandle;

		private CameraTextureSource.ImageTexture _texture = default(CameraTextureSource.ImageTexture);

		private byte[] _imageData;

		private CameraTextureSource.GetTextureDataHandle _getTextureData;

		private int _imageHeight;

		private int _imageWidth;

		private Texture2D _texture2D;

		private UnityEngine.Object thisLock = new UnityEngine.Object();

		private int _numberOfClients;

		public Texture2D textureData
		{
			get
			{
				object obj = this.thisLock;
				Texture2D texture2D;
				lock (obj)
				{
					texture2D = this._texture2D;
				}
				return texture2D;
			}
			set
			{
			}
		}

		public CameraTextureSource(int width, int height, CameraTextureSource.GetTextureDataHandle getTextureHandle)
		{
			this._imageData = new byte[width * height * 4];
			this._texture.height = height;
			this._imageHeight = height;
			this._texture.width = width;
			this._imageWidth = width;
			this._textureHandle = GCHandle.Alloc(this._imageData, GCHandleType.Pinned);
			this._texture.data = this._textureHandle.AddrOfPinnedObject();
			this._textureHandle.Free();
			this._getTextureData = getTextureHandle;
			this._texture2D.filterMode = 0;
			this._texture2D.mipMapBias = 0f;
		}

		public void registerClient()
		{
			this._numberOfClients++;
		}

		public void unregisterClient()
		{
			if (this._numberOfClients > 0)
			{
				this._numberOfClients--;
			}
		}

		public void Update()
		{
			if (this._numberOfClients > 0)
			{
				object obj = this.thisLock;
				lock (obj)
				{
					this._getTextureData(ref this._texture);
					Marshal.Copy(this._texture.data, this._imageData, 0, this._texture.height * this._texture.width * 4);
					this._texture2D.LoadRawTextureData(this._imageData);
					this._texture2D.Apply();
				}
			}
		}
	}
}
