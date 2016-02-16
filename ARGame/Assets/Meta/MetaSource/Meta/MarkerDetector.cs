using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	public class MarkerDetector : MetaSingleton<MarkerDetector>, IMetaEventReceiver
	{
		private struct MarkerData
		{
			public int id;

			public Vector3[] cornerCoords;

			public Vector3 centerCoords;

			public Matrix4x4 transformMatrix;
		}

		private struct CppMarkerData
		{
			public int id;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public float[] corner2DCoords;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
			public float[] corner3DCoords;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public float[] center2DCoords;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public float[] center3DCoords;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
			public float[] rotationMatrix;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public float[] translationVector;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public float[] transformMatrix;
		}

		private struct CppMarkerDataArray
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public MarkerDetector.CppMarkerData[] cppMarkerData;
		}

		private Vector3 markerOffset = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private bool debug;

		private int _numDetectedMarkers;

		internal float markerReleaseRange = 0.2f;

		private Dictionary<int, Matrix4x4> markerTransformDict;

		public List<int> updatedMarkerTransforms;

		[Range(0.001f, 0.5f), SerializeField]
		private double _markerSizeMeters = 0.05;

		[HideInInspector]
		private MarkerDetector.CppMarkerDataArray _cppMarkerDataArray;

		public double markerSizeMeters
		{
			get
			{
				return this._markerSizeMeters;
			}
			set
			{
				this._markerSizeMeters = value;
			}
		}

		[Obsolete]
		public void SetMarkerSize(double markerSizeMeters)
		{
			MarkerDetector.SetMarkerSize_(markerSizeMeters);
		}

		public int GetNumberOfVisibleMarkers()
		{
			return this._numDetectedMarkers;
		}

		public static int GetNumberOfMarkersVisible()
		{
			return MetaSingleton<MarkerDetector>.Instance._numDetectedMarkers;
		}

		private void Start()
		{
			MetaCore.Instance.Log("Initialize MarkerDetector");
			this.markerTransformDict = new Dictionary<int, Matrix4x4>();
			this._numDetectedMarkers = 0;
			if (this.markerSizeMeters < 0.001)
			{
				this.markerSizeMeters = 0.001;
			}
		}

		public void MetaLateUpdate()
		{
		}

		public void MetaInit()
		{
			this._cppMarkerDataArray = default(MarkerDetector.CppMarkerDataArray);
			this._cppMarkerDataArray.cppMarkerData = new MarkerDetector.CppMarkerData[10];
			for (int i = 0; i < 10; i++)
			{
				this._cppMarkerDataArray.cppMarkerData[i].center2DCoords = new float[2];
				this._cppMarkerDataArray.cppMarkerData[i].corner2DCoords = new float[8];
				this._cppMarkerDataArray.cppMarkerData[i].center3DCoords = new float[3];
				this._cppMarkerDataArray.cppMarkerData[i].corner3DCoords = new float[12];
				this._cppMarkerDataArray.cppMarkerData[i].rotationMatrix = new float[9];
				this._cppMarkerDataArray.cppMarkerData[i].translationVector = new float[3];
				this._cppMarkerDataArray.cppMarkerData[i].transformMatrix = new float[16];
			}
			MarkerDetector.RegisterMarkerDetector_();
			MarkerDetector.SetMarkerSize_(this.markerSizeMeters);
			MarkerDetector.EnableDebugDisplay_(this.debug);
			MarkerDetector.EnableMarkerDetector_();
		}

		public void MetaUpdate()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this._numDetectedMarkers = MarkerDetector.GetMarkerData_(ref this._cppMarkerDataArray);
				this.updatedMarkerTransforms = new List<int>();
				this.UpdateMarkerTransforms();
			}
		}

		public void MetaOnDestroy()
		{
		}

		[DllImport("MetaVisionDLL", EntryPoint = "registerMarkerDetector")]
		private static extern void RegisterMarkerDetector_();

		[DllImport("MetaVisionDLL", EntryPoint = "enableMarkerDetector")]
		private static extern void EnableMarkerDetector_();

		[DllImport("MetaVisionDLL", EntryPoint = "disableMarkerDetector")]
		private static extern void DisableMarkerDetector_();

		[DllImport("MetaVisionDLL", EntryPoint = "setMarkerSize")]
		private static extern void SetMarkerSize_(double markerSizeMeters);

		[DllImport("MetaVisionDLL", EntryPoint = "getNumberOfVisibleMarkers")]
		private static extern int GetNumberOfVisibleMarkers_();

		[DllImport("MetaVisionDLL", EntryPoint = "getMarkerData")]
		private static extern int GetMarkerData_(ref MarkerDetector.CppMarkerDataArray cppMarkerData);

		[DllImport("MetaVisionDLL", EntryPoint = "enableDebugDisplay")]
		private static extern void EnableDebugDisplay_(bool enable);

		private void UpdateMarkerTransforms()
		{
			int num = Math.Min(this._numDetectedMarkers, 10);
			for (int i = 0; i < num; i++)
			{
				int id = this._cppMarkerDataArray.cppMarkerData[i].id;
				this.updatedMarkerTransforms.Add(id);
				if (!this.markerTransformDict.ContainsKey(id))
				{
					Matrix4x4 value = default(Matrix4x4);
					this.markerTransformDict.Add(id, value);
				}
				this.markerTransformDict[id] = this.FloatArrToMatrix4_(ref this._cppMarkerDataArray.cppMarkerData[i].transformMatrix);
			}
		}

		private MarkerDetector.MarkerData GetMarkerDataAt(int index)
		{
			return this.CopyCppMarkerDataToMarkerData_(ref this._cppMarkerDataArray.cppMarkerData[index]);
		}

		public void GetMarkerTransform(int markerID, ref Transform newTransform)
		{
			if (this.markerTransformDict != null && this.markerTransformDict.ContainsKey(markerID))
			{
				Matrix4x4 m = base.transform.localToWorldMatrix * this.markerTransformDict[markerID];
				newTransform.position = m.PositionFromMatrix();
				newTransform.rotation = m.QuaternionFromMatrix();
				newTransform.position = newTransform.position + this.markerOffset;
				newTransform.Rotate(90f, 0f, 0f);
			}
		}

		private Vector3 FloatArrToVec3_(ref float[] arr)
		{
			return new Vector3(arr[0], arr[1], arr[2]);
		}

		private Vector3 FloatSubArrToVec3_(ref float[] arr, int startIdx)
		{
			return new Vector3(arr[startIdx], arr[startIdx + 1], arr[startIdx + 2]);
		}

		private Matrix4x4 FloatArrToMatrix4_(ref float[] arr)
		{
			Matrix4x4 result = default(Matrix4x4);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					result[i, j] = arr[i * 4 + j];
				}
			}
			return result;
		}

		private MarkerDetector.MarkerData CopyCppMarkerDataToMarkerData_(ref MarkerDetector.CppMarkerData cppMarkerData)
		{
			MarkerDetector.MarkerData result = default(MarkerDetector.MarkerData);
			result.id = cppMarkerData.id;
			for (int i = 0; i < 4; i++)
			{
				result.cornerCoords[i] = this.FloatSubArrToVec3_(ref cppMarkerData.corner3DCoords, i * 3);
			}
			result.transformMatrix = this.FloatArrToMatrix4_(ref cppMarkerData.transformMatrix);
			return result;
		}
	}
}
