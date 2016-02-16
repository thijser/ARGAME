using System;
using UnityEngine;

namespace Meta
{
	internal class CameraTextureHotKey : MonoBehaviour
	{
		private CameraTextureTarget cameraTextureTarget;

		private MeshRenderer webCameraTextureRenderer;

		private void Start()
		{
			if (this.cameraTextureTarget == null)
			{
				this.cameraTextureTarget = base.GetComponent<CameraTextureTarget>();
			}
			if (this.webCameraTextureRenderer == null)
			{
				this.webCameraTextureRenderer = base.GetComponent<MeshRenderer>();
			}
		}

		private void Update()
		{
			if (this.cameraTextureTarget == null)
			{
				this.cameraTextureTarget = base.GetComponent<CameraTextureTarget>();
			}
			if (this.webCameraTextureRenderer == null)
			{
				this.webCameraTextureRenderer = base.GetComponent<MeshRenderer>();
			}
			if (this.cameraTextureTarget != null && MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed != string.Empty && Input.GetKeyDown(MetaSingleton<KeyboardShortcuts>.Instance.toggleRGBFeed))
			{
				this.cameraTextureTarget.Toggle();
			}
		}
	}
}
