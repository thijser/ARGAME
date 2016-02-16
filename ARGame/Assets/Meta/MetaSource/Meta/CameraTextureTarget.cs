using System;
using UnityEngine;

namespace Meta
{
	public class CameraTextureTarget : MonoBehaviour
	{
		[SerializeField]
		internal MeshRenderer renderTarget;

		internal Texture2D CameraTexture;

		[SerializeField]
		internal int sourceDevice;

		private int lastSource = -1;

		private void OnValidate()
		{
			this.SwitchDevice();
		}

		internal void Start()
		{
			if (this.renderTarget == null)
			{
				this.renderTarget = base.GetComponent<MeshRenderer>();
			}
		}

		private void SetTargetTexture()
		{
			if (MetaSingleton<DeviceTextureSource>.Instance != null && MetaCore.Instance != null && MetaSingleton<DeviceTextureSource>.Instance.enabled)
			{
				this.CameraTexture = MetaSingleton<DeviceTextureSource>.Instance.GetDeviceTexture(this.sourceDevice);
				if (this.renderTarget != null && this.renderTarget.material != null)
				{
					this.renderTarget.material.mainTexture = this.CameraTexture;
				}
			}
		}

		private void OnEnable()
		{
			if (MetaSingleton<DeviceTextureSource>.Instance != null && MetaCore.Instance != null)
			{
				MetaSingleton<DeviceTextureSource>.Instance.registerTextureDevice(this.sourceDevice);
				this.SetTargetTexture();
			}
		}

		private void Disable()
		{
			if (MetaSingleton<DeviceTextureSource>.Instance != null && MetaCore.Instance != null)
			{
				MetaSingleton<DeviceTextureSource>.Instance.unregisterTextureDevice(this.sourceDevice);
				this.SetTargetTexture();
			}
		}

		public void Toggle()
		{
			if (this.renderTarget == null)
			{
				this.renderTarget = base.GetComponent<MeshRenderer>();
			}
			base.enabled = !base.enabled;
			this.renderTarget.enabled = base.enabled;
		}

		internal void CycleSource()
		{
			this.sourceDevice = ++this.sourceDevice % MetaSingleton<DeviceTextureSource>.Instance.GetNumberOfSources();
			this.SwitchDevice();
		}

		internal void SwitchDevice()
		{
			if (MetaSingleton<DeviceTextureSource>.Instance != null && MetaSingleton<DeviceTextureSource>.Instance.enabled && MetaCore.Instance != null)
			{
				MetaSingleton<DeviceTextureSource>.Instance.unregisterTextureDevice(this.lastSource);
				this.lastSource = this.sourceDevice;
				MetaSingleton<DeviceTextureSource>.Instance.registerTextureDevice(this.sourceDevice);
			}
			this.SetTargetTexture();
		}
	}
}
