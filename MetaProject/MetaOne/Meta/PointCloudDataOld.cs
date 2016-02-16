using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct PointCloudDataOld
	{
		private bool valid;

		public IntPtr vertices;

		public IntPtr color;

		public int size;
	}
}
