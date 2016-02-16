using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct PointCloudData
	{
		public IntPtr vertices;

		public int size;

		public void Init()
		{
			GCHandle gCHandle = GCHandle.Alloc(new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3], GCHandleType.Pinned);
			this.vertices = gCHandle.AddrOfPinnedObject();
			gCHandle.Free();
		}
	}
}
