using System;
using System.Runtime.InteropServices;

namespace Meta
{
	internal struct CppHandData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] top;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] left;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] right;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] center;

		public bool valid;

		public int angle;

		public CppPalm palm;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public CppFinger[] fingers;

		public CppGestureData gesture;

		public float handOpenness;

		public void Init()
		{
			this.top = new float[3];
			this.left = new float[3];
			this.right = new float[3];
			this.center = new float[3];
			this.valid = false;
			this.angle = 0;
			this.palm = default(CppPalm);
			this.palm.Init();
			this.fingers = new CppFinger[5];
			for (int i = 0; i < 5; i++)
			{
				this.fingers[i].Init();
			}
			this.gesture = default(CppGestureData);
			this.gesture.Init();
			this.handOpenness = 0f;
		}
	}
}
