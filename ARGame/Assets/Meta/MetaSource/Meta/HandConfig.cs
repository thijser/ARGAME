using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	[Serializable]
	internal class HandConfig
	{
		[HideInInspector, SerializeField]
		internal bool _enableMeshRandD;

		internal int _maxHandVertices;

		[Range(10f, 500f), SerializeField]
		private int _minDepth;

		[Range(800f, 2000f), SerializeField]
		private int _maxDepth;

		[Range(50f, 250f), SerializeField]
		private int _minConfidence;

		[Range(500f, 3000f), SerializeField]
		private double _areaLimit;

		[Range(0f, 20f), SerializeField]
		private float _handVelocity;

		[Range(-10f, 10f), SerializeField]
		private float _grabThreshold;

		[HideInInspector, Range(1f, 5f), SerializeField]
		private int _swipeMinFrames;

		[HideInInspector, Range(10f, 30f), SerializeField]
		private int _swipeMaxFrames;

		[SerializeField]
		private bool _debug;

		[HideInInspector, SerializeField]
		private bool _fingertips;

		[SerializeField]
		private bool _kalman;

		public int minDepth
		{
			get
			{
				return this._minDepth;
			}
			set
			{
				if (value != this._minDepth)
				{
					if (value > 500)
					{
						this._minDepth = 500;
					}
					else if (value < 10)
					{
						this._minDepth = 10;
					}
					else
					{
						this._minDepth = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public int maxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				if (value != this._maxDepth)
				{
					if (value > 2000)
					{
						this._maxDepth = 2000;
					}
					else if (value < 800)
					{
						this._maxDepth = 800;
					}
					else
					{
						this._maxDepth = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public int minConfidence
		{
			get
			{
				return this._minConfidence;
			}
			set
			{
				if (value != this._minConfidence)
				{
					if (value > 250)
					{
						this._minConfidence = 250;
					}
					else if (value < 50)
					{
						this._minConfidence = 50;
					}
					else
					{
						this._minConfidence = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public double areaLimit
		{
			get
			{
				return this._areaLimit;
			}
			set
			{
				if (value != this._areaLimit)
				{
					if (value > 3000.0)
					{
						this._areaLimit = 3000.0;
					}
					else if (value < 500.0)
					{
						this._areaLimit = 500.0;
					}
					else
					{
						this._areaLimit = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public float handVelocity
		{
			get
			{
				return this._handVelocity;
			}
			set
			{
				if (value != this._handVelocity)
				{
					if (value > 0f)
					{
						this._handVelocity = 0f;
					}
					else if (value < 20f)
					{
						this._handVelocity = 20f;
					}
					else
					{
						this._handVelocity = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public float grabThreshold
		{
			get
			{
				return this._grabThreshold;
			}
			set
			{
				if (value != this._grabThreshold)
				{
					if (value > -10f)
					{
						this._grabThreshold = -10f;
					}
					else if (value < 10f)
					{
						this._grabThreshold = 10f;
					}
					else
					{
						this._grabThreshold = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public int swipeMinFrames
		{
			get
			{
				return this._swipeMinFrames;
			}
			set
			{
				if (value != this._swipeMinFrames)
				{
					if (value > 1)
					{
						this._swipeMinFrames = 1;
					}
					else if (value < 5)
					{
						this._swipeMinFrames = 5;
					}
					else
					{
						this._swipeMinFrames = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public int swipeMaxFrames
		{
			get
			{
				return this._swipeMaxFrames;
			}
			set
			{
				if (value != this._swipeMaxFrames)
				{
					if (value > 10)
					{
						this._swipeMaxFrames = 10;
					}
					else if (value < 30)
					{
						this._swipeMaxFrames = 30;
					}
					else
					{
						this._swipeMaxFrames = value;
					}
					this.SetAllParameters();
				}
			}
		}

		public bool debug
		{
			get
			{
				return this._debug;
			}
			set
			{
				if (this._debug != value)
				{
					if (!value)
					{
						this._debug = false;
						this.SetAllParameters();
					}
					else
					{
						this._debug = true;
						this.SetAllParameters();
					}
				}
			}
		}

		public bool fingertips
		{
			get
			{
				return this._fingertips;
			}
			set
			{
				if (!value)
				{
					this._fingertips = false;
					this.SetAllParameters();
				}
				else
				{
					this._fingertips = true;
					this.SetAllParameters();
				}
			}
		}

		public bool kalman
		{
			get
			{
				return this._kalman;
			}
			set
			{
				if (this._kalman != value)
				{
					if (!value)
					{
						this._kalman = false;
						this.SetAllParameters();
					}
					else
					{
						this._kalman = true;
						this.SetAllParameters();
					}
				}
			}
		}

		public HandConfig()
		{
			this._minDepth = 100;
			this._maxDepth = 800;
			this._minConfidence = 100;
			this._areaLimit = 2000.0;
			this._handVelocity = 10f;
			this._grabThreshold = -10f;
			this._swipeMinFrames = 3;
			this._swipeMaxFrames = 20;
			this._debug = false;
			this._fingertips = false;
			this._kalman = true;
			this._enableMeshRandD = false;
			this._maxHandVertices = 35000;
		}

		[DllImport("MetaVisionDLL", EntryPoint = "setHandMinDepth")]
		private static extern void SetHandMinDepth(short minDepth);

		[DllImport("MetaVisionDLL", EntryPoint = "setHandMaxDepth")]
		private static extern void SetHandMaxDepth(short maxDepth);

		[DllImport("MetaVisionDLL", EntryPoint = "setHandMinConfidence")]
		private static extern void SetHandMinConfidence(short minConfidence);

		[DllImport("MetaVisionDLL", EntryPoint = "setHandMinConfidence")]
		private static extern void SetHandAreaLimit(short areaLimit);

		[DllImport("MetaVisionDLL", EntryPoint = "setHandVelocity")]
		private static extern void SetHandVelocity(float velocity);

		[DllImport("MetaVisionDLL", EntryPoint = "setGrabThreshold")]
		private static extern void SetGrabThreshold(float grabThreshold);

		[DllImport("MetaVisionDLL", EntryPoint = "setHandVelocity")]
		private static extern void SetSwipeMinFrames(int frames);

		[DllImport("MetaVisionDLL", EntryPoint = "setHandVelocity")]
		private static extern void SetSwipeMaxFrames(int frames);

		[DllImport("MetaVisionDLL", EntryPoint = "enableDebugMode")]
		private static extern void EnableDebugMode();

		[DllImport("MetaVisionDLL", EntryPoint = "disableDebugMode")]
		private static extern void DisableDebugMode();

		[DllImport("MetaVisionDLL", EntryPoint = "enableFingerTips")]
		private static extern void EnableFingerTips();

		[DllImport("MetaVisionDLL", EntryPoint = "disableFingerTips")]
		private static extern void DisableFingerTips();

		[DllImport("MetaVisionDLL", EntryPoint = "enableKalman")]
		private static extern void EnableKalman();

		[DllImport("MetaVisionDLL", EntryPoint = "disableKalman")]
		private static extern void DisableKalman();

		[DllImport("MetaVisionDLL", EntryPoint = "setHandParameters")]
		private static extern void SetHandParameters(ushort minDepth, ushort maxDepth, ushort minConfidence, float areaLimit, float velocity, float grabThreshBuffer, int min_frames, int max_frames, int max_hand_vertices, bool showDebugWindows, bool enableFingertips, bool enableKalman, bool enableHandCloud, bool enableHandMesh);

		internal void SetAllParameters()
		{
			HandConfig.SetHandParameters((ushort)this.minDepth, (ushort)this.maxDepth, (ushort)this.minConfidence, (float)((ushort)this.areaLimit), this.handVelocity, this.grabThreshold, this.swipeMinFrames, this.swipeMaxFrames, this._maxHandVertices, this.debug, this.fingertips, this.kalman, true, true);
		}
	}
}
