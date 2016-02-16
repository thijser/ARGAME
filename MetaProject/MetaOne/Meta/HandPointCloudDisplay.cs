using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	internal class HandPointCloudDisplay : MonoBehaviour, IHandVisualiser<PointCloudData>
	{
		private ParticleSystem.Particle[] m_cloud;

		private PointCloudData[] _handPointCloud = new PointCloudData[2];

		private float[][] _handPointCloudVertices = new float[2][];

		[SerializeField]
		private Color _particleColor = Color.get_red();

		[Range(0.001f, 1f), SerializeField]
		private float _particleSize = 0.1f;

		private bool _enabled = true;

		public bool currentlyActive
		{
			get
			{
				return this._enabled;
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "getPointCloudDisplayData")]
		private static extern void GetPointCloudDisplayData(ref PointCloudData leftHandDisplay, ref PointCloudData rightHandDisplay);

		[DllImport("MetaVisionDLL", EntryPoint = "enableHandPointCloud")]
		private static extern void EnableHandPointCloud();

		[DllImport("MetaVisionDLL", EntryPoint = "disableHandPointCloud")]
		private static extern void DisableHandPointCloud();

		private void Start()
		{
			this.m_cloud = new ParticleSystem.Particle[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices];
			for (int i = 0; i < 2; i++)
			{
				this._handPointCloud[i] = default(PointCloudData);
				this._handPointCloud[i].Init();
				this._handPointCloudVertices[i] = new float[MetaSingleton<Hands>.Instance._handConfig._maxHandVertices * 3];
			}
			if (!Hands.useFaker && this._enabled)
			{
				this.Enable();
			}
		}

		private void Update()
		{
			if (!this.currentlyActive)
			{
				return;
			}
			this.GetDisplayData(ref this._handPointCloud[0], ref this._handPointCloud[1]);
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				if (this._handPointCloud[i].vertices != IntPtr.Zero)
				{
					Marshal.Copy(this._handPointCloud[i].vertices, this._handPointCloudVertices[i], 0, this._handPointCloud[i].size * 3);
				}
				this.SetParticlePoints(this._handPointCloudVertices[i], this._handPointCloud[i].size, num);
				num += this._handPointCloud[i].size;
			}
			base.GetComponent<ParticleSystem>().SetParticles(this.m_cloud, num);
		}

		private void OnDestroy()
		{
			if (!Hands.useFaker)
			{
				this.Disable();
			}
		}

		public void GetDisplayData(ref PointCloudData leftHandDisplay, ref PointCloudData rightHandDisplay)
		{
			if (Hands.useFaker)
			{
				MetaOldDLLMetaInputFaker.GetPointCloudDisplayData(ref leftHandDisplay, ref rightHandDisplay);
			}
			else
			{
				HandPointCloudDisplay.GetPointCloudDisplayData(ref leftHandDisplay, ref rightHandDisplay);
			}
		}

		public void Enable()
		{
			HandPointCloudDisplay.EnableHandPointCloud();
			this._enabled = true;
		}

		public void Disable()
		{
			HandPointCloudDisplay.DisableHandPointCloud();
			this._enabled = false;
		}

		private void SetParticlePoints(float[] points, int size, int offset)
		{
			for (int i = 0; i < size; i++)
			{
				this.m_cloud[i + offset].set_position(new Vector3(points[3 * i], points[3 * i + 1], points[3 * i + 2] - base.get_transform().get_localPosition().z));
				this.m_cloud[i + offset].set_color(this._particleColor);
				this.m_cloud[i + offset].set_size(this._particleSize);
			}
		}
	}
}
