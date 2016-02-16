using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct CppFinger
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] location;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] direction;

		public bool found;

		public void Init()
		{
			this.location = new float[3];
			this.direction = new float[3];
			this.found = false;
		}
	}
}
