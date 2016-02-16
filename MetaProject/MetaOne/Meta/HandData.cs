using System;
using System.Runtime.InteropServices;

namespace Meta
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct HandData
	{
		internal enum Type
		{
			NONE = -1,
			LEFT = 1,
			RIGHT
		}

		internal struct StateData
		{
			internal enum State
			{
				NONE = -1,
				OPEN,
				CLOSED,
				POINT,
				PINCH
			}

			internal HandData.StateData.State state;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal float[] pinch_pt;

			internal float grab_value;
		}

		internal struct Palm
		{
			internal int radius;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			internal float[] orientation_angles;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal float[] norm_vec;
		}

		internal struct Finger
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal float[] loc;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			internal float[] dir;

			internal bool found;
		}

		internal enum FingerTypes
		{
			THUMB,
			INDEX,
			MIDDLE,
			RING,
			PINKY
		}

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		internal float[] top;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		internal float[] left;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		internal float[] right;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		internal float[] center;

		internal HandData.Type type;

		internal HandData.StateData state_data;

		internal bool valid;

		internal int angle;

		internal HandData.Palm palm;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		internal HandData.Finger[] fingers;

		internal MeshData hand_mesh;

		internal PointCloudDataOld hand_point_cloud;
	}
}
