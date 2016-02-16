using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct CppGestureData
	{
		public bool valid;

		public MetaGesture manipulationGesture;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] gesturePoint;

		public void Init()
		{
			this.gesturePoint = new float[3];
			this.manipulationGesture = MetaGesture.OPEN;
			this.valid = false;
		}
	}
}
