namespace Meta
{
    using System.Collections;
    using System.Diagnostics;
    using UnityEngine;

    public class InputIndicators : MetaSingleton<InputIndicators>
	{
		[SerializeField]
		private bool _handCloud;

		[SerializeField]
		private bool _fingertipIndicators = true;

		[SerializeField]
		private bool _hoverIndicators = true;

		[SerializeField]
		private bool _pressIndicators = true;

		[SerializeField]
		private bool _pressSounds = true;

		[SerializeField]
		private bool _dwellIndicators = true;

		[SerializeField]
		private bool _grabCircles = true;

		[SerializeField]
		private bool _grabCirclesAlways;

		private bool _handCloudPrev;

		private bool _fingertipIndicatorsPrev;

		private bool _fingertipsPrev;

		[HideInInspector, SerializeField]
		private GameObject _handCloudObject;

		[HideInInspector, SerializeField]
		private GameObject _fingertipIndicator;

		[HideInInspector, SerializeField]
		private GameObject _hoverIndicator;

		[HideInInspector, SerializeField]
		private GameObject _pressIndicator;

		[HideInInspector, SerializeField]
		private GameObject _pressSound;

		[HideInInspector, SerializeField]
		private GameObject _grabCircleSolid;

		[HideInInspector, SerializeField]
		private GameObject _grabCircleDotted;

		[HideInInspector, SerializeField]
		private GameObject _dwellIndicator;

		[HideInInspector, SerializeField]
		private Texture2D[] _dwellIndicatorTextures;

		private Transform[] _fingerPointerIndicatorTransforms = new Transform[2];

		private Transform[] _fingertipIndicatorTransforms = new Transform[10];

		private Transform[] _fingerPointerHoverTransforms = new Transform[2];

		private Transform[] _fingertipHoverTransforms = new Transform[10];

		private Transform[] _grabCircleSolidTransforms = new Transform[2];

		private Transform[] _grabCircleDottedTransforms = new Transform[2];

		private float _minHandOpenness = -100f;

		private float _maxHandOpenness = 20f;

		private bool[] _dwellSet = new bool[2];

		public bool handCloud
		{
			get
			{
				return this._handCloud;
			}
			set
			{
				this._handCloud = value;
			}
		}

		public bool fingertipIndicators
		{
			get
			{
				return this._fingertipIndicators;
			}
			set
			{
				this._fingertipIndicators = value;
			}
		}

		public bool hoverIndicators
		{
			get
			{
				return this._hoverIndicators;
			}
			set
			{
				this._hoverIndicators = value;
			}
		}

		public bool pressIndicators
		{
			get
			{
				return this._pressIndicators;
			}
			set
			{
				this._pressIndicators = value;
			}
		}

		public bool pressSounds
		{
			get
			{
				return this._pressSounds;
			}
			set
			{
				this._pressSounds = value;
			}
		}

		public bool dwellIndicators
		{
			get
			{
				return this._dwellIndicators;
			}
			set
			{
				this._dwellIndicators = value;
			}
		}

		public bool grabCircles
		{
			get
			{
				return this._grabCircles;
			}
			set
			{
				this._grabCircles = value;
			}
		}

		public bool grabCirclesAlways
		{
			get
			{
				return this._grabCirclesAlways;
			}
			set
			{
				this._grabCirclesAlways = value;
			}
		}

		private void Start()
		{
			this.InitFingertipIndicatorTransforms();
			this.InitGrabCircleTransforms();
			this.SelectFingerIndicators();
			this.SetHandCloud();
		}

		private void Update()
		{
			this.CheckIndicatorSettings();
		}

		private void LateUpdate()
		{
			this._dwellSet = new bool[2];
		}

		private void InitFingertipIndicatorTransforms()
		{
			for (int i = 0; i < 2; i++)
			{
				if (Hands.GetHands()[i] != null)
				{
					this._fingerPointerIndicatorTransforms[i] = this.InstantiatePrefab(this._fingertipIndicator, "FingertipIndicator", Hands.GetHands()[i].pointer.gameObject.transform, new Vector3(0.001f, 0.001f, 0.001f), null);
					this._fingerPointerHoverTransforms[i] = this.InstantiatePrefab(this._hoverIndicator, "HoverIndicator", Hands.GetHands()[i].pointer.gameObject.transform, new Vector3(0.001f, 0.001f, 0.001f), null);
					this._fingerPointerHoverTransforms[i] = this.InstantiatePrefab(this._dwellIndicator, "PointerDwellIndicator", Hands.GetHands()[i].pointer.gameObject.transform, new Vector3(0.002f, 0.002f, 0.002f), null);
					for (int j = 0; j < 5; j++)
					{
						this._fingertipIndicatorTransforms[j + i * 5] = this.InstantiatePrefab(this._fingertipIndicator, "FingertipIndicator", Hands.GetHands()[i].fingers[j].gameObject.transform, new Vector3(0.001f, 0.001f, 0.001f), null);
						this._fingertipHoverTransforms[j + i * 5] = this.InstantiatePrefab(this._hoverIndicator, "HoverIndicator", Hands.GetHands()[i].fingers[j].gameObject.transform, new Vector3(0.001f, 0.001f, 0.001f), null);
					}
				}
			}
		}

		private void InitGrabCircleTransforms()
		{
			for (int i = 0; i < 2; i++)
			{
				if (Hands.GetHands()[i] != null)
				{
					this._grabCircleSolidTransforms[i] = this.InstantiatePrefab(this._grabCircleSolid, "GrabCircleSolid", Hands.GetHands()[i].palm.gameObject.transform, new Vector3(0.003f, 0.003f, 0.003f), new Color?(new Color(1f, 0.647058845f, 0f, 1f)));
					this._grabCircleDottedTransforms[i] = this.InstantiatePrefab(this._grabCircleDotted, "GrabCircleDotted", Hands.GetHands()[i].palm.gameObject.transform, new Vector3(0.003f, 0.003f, 0.003f), new Color?(new Color(1f, 1f, 0.392156869f, 1f)));
				}
			}
		}

		internal void UpdateDwellIndicators(HandType hand, float completion)
		{
			Transform transform = Hands.GetHands()[(int)hand].pointer.gameObject.transform.FindChild("PointerDwellIndicator");
			transform.GetComponent<Renderer>().enabled = true;
			int num = (int)((float)(this._dwellIndicatorTextures.Length - 1) * completion);
			if (num <= 0)
			{
				num = 0;
			}
			if (num >= this._dwellIndicatorTextures.Length - 1)
			{
				num = this._dwellIndicatorTextures.Length - 1;
			}
			transform.GetComponent<Renderer>().material.mainTexture = this._dwellIndicatorTextures[num];
			this._dwellSet[(int)hand] = true;
		}

		internal void HideDwellIndicators(HandType hand)
		{
			if (!this._dwellSet[(int)hand])
			{
				Transform transform = Hands.GetHands()[(int)hand].pointer.gameObject.transform.FindChild("PointerDwellIndicator");
				transform.GetComponent<Renderer>().enabled = false;
			}
		}

		private Transform InstantiatePrefab(GameObject prefab, string name, Transform parent, Vector3 localScale, Color? color = null)
		{
			Transform transform = ((GameObject)Object.Instantiate(prefab)).transform;
			transform.name = name;
			transform.parent = parent;
			transform.localPosition = Vector3.zero;
			transform.localScale = localScale;
			if (color.HasValue)
			{
				transform.GetComponent<Renderer>().material.color = color.Value;
				transform.GetComponent<Renderer>().material.SetColor("_SpecColor", color.Value);
			}
			return transform;
		}

		private void CheckIndicatorSettings()
		{
			if (this._fingertipsPrev != Hands.handConfig.fingertips || this._fingertipIndicators != this._fingertipIndicatorsPrev)
			{
				Hands.handConfig.fingertips = Hands.handConfig.fingertips;
				this.SelectFingerIndicators();
			}
			if (this._handCloudPrev != this._handCloud)
			{
				this.SetHandCloud();
			}
		}

		private void SelectFingerIndicators()
		{
			this._fingertipsPrev = Hands.handConfig.fingertips;
			this._fingertipIndicatorsPrev = this._fingertipIndicators;
			if (Hands.handConfig.fingertips)
			{
				this.SetFingertipRenderers(false, this._fingertipIndicators);
			}
			else
			{
				this.SetFingertipRenderers(this._fingertipIndicators, false);
			}
		}

		private void SetHandCloud()
		{
			Hands.handConfig.SetAllParameters();
			this._handCloudPrev = this._handCloud;
			this._handCloudObject.SetActive(this._handCloud);
		}

		private void SetFingertipRenderers(bool pointers, bool fingertips)
		{
			for (int i = 0; i < 2; i++)
			{
				if (this._fingerPointerIndicatorTransforms[i] != null)
				{
					this._fingerPointerIndicatorTransforms[i].GetComponent<Renderer>().enabled = pointers;
					if (!pointers)
					{
						this._fingerPointerHoverTransforms[i].GetComponent<Renderer>().enabled = false;
					}
				}
			}
			for (int j = 0; j < 10; j++)
			{
				if (this._fingertipIndicatorTransforms[j] != null)
				{
					this._fingertipIndicatorTransforms[j].GetComponent<Renderer>().enabled = fingertips;
				}
				if (!fingertips && this._fingertipHoverTransforms[j] != null)
				{
					this._fingertipHoverTransforms[j].GetComponent<Renderer>().enabled = false;
				}
			}
		}

		internal void DisableHoverIndicator(Transform hoverIndicator)
		{
			hoverIndicator.GetComponent<Renderer>().enabled = false;
		}

		internal void UpdateHoverIndicator(Transform hoverIndicator, float distance)
		{
			if (this._fingertipIndicators && this._hoverIndicators)
			{
				float num = 0.0125f;
				if (distance < 0.2f)
				{
					num += 0.0924999937f * (1f - distance / 0.2f);
				}
				num /= 100f;
				hoverIndicator.localScale = new Vector3(num, num, num);
				hoverIndicator.GetComponent<Renderer>().enabled = true;
			}
		}

		internal void ColorHoverIndicator(Transform hoverIndicator, Color color)
		{
			hoverIndicator.GetComponent<Renderer>().material.color = color;
		}

		public void InstantiatePressIndicator(Vector3 pos, Quaternion rot)
		{
			if (this._pressIndicators && this._pressIndicator != null)
			{
				Object.Instantiate(this._pressIndicator, pos, rot);
			}
		}

		public void InstantiatePressSound(Vector3 pos, AudioClip pressSound)
		{
			if (this._pressSounds && this._pressSound != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this._pressSound, pos, Quaternion.identity) as GameObject;
				if (pressSound != null)
				{
					gameObject.GetComponent<AudioSource>().clip = pressSound;
				}
				gameObject.GetComponent<AudioSource>().loop = false;
				gameObject.GetComponent<AudioSource>().Play();
				base.StartCoroutine("DestroySound", gameObject);
			}
		}

		[DebuggerHidden]
		private IEnumerator DestroySound(GameObject sound)
		{
            yield return sound;
		}

		internal void UpdateGrabCircles(int hand, bool objectWithinRange)
		{
			if (this._grabCircles && (objectWithinRange || this._grabCirclesAlways))
			{
				if (Hands.GetHands()[hand] != null)
				{
					float num = Hands.GetHands()[hand].handOpenness;
					if (Hands.GetHands()[hand].gesture.type == MetaGesture.GRAB)
					{
						float num2 = this._maxHandOpenness - Hands.handConfig.grabThreshold;
						if (num > this._maxHandOpenness)
						{
							num = this._maxHandOpenness;
						}
						float num3 = Mathf.Abs(num - Hands.handConfig.grabThreshold) / num2;
						if (num < Hands.handConfig.grabThreshold)
						{
							num3 = 0f;
						}
						this.SetCircleScale(hand, (1f - num3) * 0.0024f + 0.0006f);
					}
					else
					{
						float num4 = Hands.handConfig.grabThreshold - this._minHandOpenness;
						if (num < this._minHandOpenness)
						{
							num = this._minHandOpenness;
						}
						float num5 = 1f - Mathf.Abs(num - Hands.handConfig.grabThreshold) / num4;
						if (num > Hands.handConfig.grabThreshold)
						{
							num5 = 1f;
						}
						this.SetCircleScale(hand, (1f - num5) * 0.006f + 0.003f);
					}
					this._grabCircleSolidTransforms[hand].LookAt(UnityEngine.Camera.main.transform);
					this._grabCircleSolidTransforms[hand].Rotate(new Vector3(90f, 0f, 0f));
					this._grabCircleDottedTransforms[hand].rotation = this._grabCircleSolidTransforms[hand].rotation;
					this.SetCircleGrabbedColor(hand, objectWithinRange);
					this.ToggleGrabCircle(hand, true);
				}
			}
			else
			{
				this.ToggleGrabCircle(hand, false);
			}
		}

		private void SetCircleGrabbedColor(int hand, bool objectWithinRange)
		{
			if (objectWithinRange)
			{
				this._grabCircleSolidTransforms[hand].GetComponent<Renderer>().material.color = Color.green;
				this._grabCircleSolidTransforms[hand].GetComponent<Renderer>().material.SetColor("_Emission", Color.green);
			}
			else
			{
				this._grabCircleSolidTransforms[hand].GetComponent<Renderer>().material.color = Color.gray;
				this._grabCircleSolidTransforms[hand].GetComponent<Renderer>().material.SetColor("_Emission", Color.gray);
			}
		}

		private void SetCircleScale(int hand, float scale)
		{
			Vector3 vector = new Vector3(scale, scale, scale);
			vector = Vector3.Lerp(this._grabCircleDottedTransforms[hand].localScale, vector, 10f * Time.deltaTime);
			this._grabCircleDottedTransforms[hand].localScale = vector;
		}

		private void ToggleGrabCircle(int hand, bool visibility)
		{
			if (this._grabCircleSolidTransforms[hand] != null)
			{
				this._grabCircleSolidTransforms[hand].GetComponent<Renderer>().enabled = visibility;
			}
			if (this._grabCircleDottedTransforms[hand] != null)
			{
				this._grabCircleDottedTransforms[hand].GetComponent<Renderer>().enabled = visibility;
			}
		}
	}
}
