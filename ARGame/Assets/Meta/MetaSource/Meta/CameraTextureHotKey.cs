using System;
using UnityEngine;

namespace Meta
{
	internal class CameraTextureHotKey : MonoBehaviour
	{
		private CameraTextureTarget CameraTextureTarget;

		private MeshRenderer webCameraTextureRenderer;

		private void Start()
		{
			if (this.CameraTextureTarget == null)
			{
				this.CameraTextureTarget = base.GetComponent<CameraTextureTarget>();
			}
			if (this.webCameraTextureRenderer == null)
			{
				this.webCameraTextureRenderer = base.GetComponent<MeshRenderer>();
			}
		}

		private void Update()
		{
			if (this.CameraTextureTarget == null)
			{
				this.CameraTextureTarget = base.GetComponent<CameraTextureTarget>();
			}
			if (this.webCameraTextureRenderer == null)
			{
				this.webCameraTextureRenderer = base.GetComponent<MeshRenderer>();
			}
			if (this.CameraTextureTarget != null && MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed != string.Empty && Input.GetKeyDown(MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed))
			{
				this.CameraTextureTarget.Toggle();
			}
		}
	}
}
