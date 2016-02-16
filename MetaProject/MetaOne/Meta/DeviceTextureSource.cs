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

		private int _colorIndex;

		private int _depthIndex = 1;

		private int _irIndex = 2;

		private byte[] byteData;

		public Texture2D GetDeviceTexture(int device)
		{
			if (device >= 0 && device < this.textureSources.Length && this.textureSources[device] != null)
			{
				return this.textureSources[device].textureData;
			}
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
			if (this.textureSources[this._colorIndex] == null)
			{
				CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(DeviceTextureSource.getRGBTexture);
				CameraTextureSource cameraTextureSource = new CameraTextureSource(MetaCore.Instance.DeviceInformation.colorWidth, MetaCore.Instance.DeviceInformation.colorHeight, getTextureHandle);
				this.textureSources[this._colorIndex] = cameraTextureSource;
			}
		}

		private void registerDepthData()
		{
			if (this.textureSources[this._depthIndex] == null)
			{
				CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(DeviceTextureSource.getDepthTexture);
				CameraTextureSource cameraTextureSource = new CameraTextureSource(MetaCore.Instance.DeviceInformation.depthWidth, MetaCore.Instance.DeviceInformation.depthHeight, getTextureHandle);
				this.textureSources[this._depthIndex] = cameraTextureSource;
			}
		}

		private void registerIRData()
		{
			if (this.textureSources[this._irIndex] == null)
			{
				CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(DeviceTextureSource.getIRTexture);
				CameraTextureSource cameraTextureSource = new CameraTextureSource(MetaCore.Instance.DeviceInformation.depthWidth, MetaCore.Instance.DeviceInformation.depthHeight, getTextureHandle);
				this.textureSources[this._irIndex] = cameraTextureSource;
			}
		}

		private void registerImageData()
		{
			if (this.textureSources[3] == null && Assembly.GetAssembly(base.GetType()).GetType("MetaInternalEditor", false, true) == null)
			{
				Color32[] array = new Color32[16384];
				byte[] array2 = new byte[65536];
				this.byteData = new byte[65536];
				string path = "Assets\\Meta Internal\\Art Assets\\test_image.jpg";
				if (File.Exists(path))
				{
					array2 = File.ReadAllBytes(path);
					Texture2D texture2D = new Texture2D(128, 128);
					texture2D.LoadImage(array2);
					array = texture2D.GetPixels32();
					try
					{
						IntPtr source = GCHandle.Alloc(array, GCHandleType.Pinned).AddrOfPinnedObject();
						Marshal.Copy(source, this.byteData, 0, array.Length * 4);
					}
					catch (Exception ex)
					{
						Debug.Log(ex);
					}
				}
				CameraTextureSource.GetTextureDataHandle getTextureHandle = new CameraTextureSource.GetTextureDataHandle(this.getFakeImageTexture);
				CameraTextureSource cameraTextureSource = new CameraTextureSource(128, 128, getTextureHandle);
				this.textureSources[3] = cameraTextureSource;
			}
		}

		public void registerTextureDevice(int device)
		{
			if (device < 0)
			{
				return;
			}
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
			if (device >= 0 && device < this.textureSources.Length && this.textureSources[device] != null)
			{
				this.textureSources[device].unregisterClient();
			}
		}

		public void MetaInit()
		{
			if (MetaCore.Instance != null)
			{
				DeviceTextureSource.registerCameraTexture();
			}
		}

		public void MetaUpdate()
		{
			for (int i = 0; i < this.textureSources.Length; i++)
			{
				if (this.textureSources[i] != null)
				{
					this.textureSources[i].Update();
				}
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
