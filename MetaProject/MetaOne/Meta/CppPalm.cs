using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct CppPalm
	{
		public int radius;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public float[] orientationAngles;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] normalVector;

		public void Init()
		{
			this.radius = 0;
			this.orientationAngles = new float[3];
			this.normalVector = new float[3];
		}
	}
}
