using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct DeviceInfo
	{
		public int colorHeight;

		public int colorWidth;

		public int depthHeight;

		public int depthWidth;

		public bool streamingColor;

		public bool streamingDepth;

		public float depthFps;

		public float colorFps;

		public CameraModel CameraModel;

		public IMUModel imuModel;
	}
}
