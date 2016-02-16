
namespace Meta
{
	using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
internal class GestureManager : MetaSingleton<GestureManager>
	{
		private class HandGestureData
		{
			public HandType m_handType;

			public Hand m_hand;

			public Vector3 m_prevPalmPos;

			public MetaGesture m_currGesture;

			public MetaGesture m_currMode;

			public Transform m_closestObj;

			public Transform m_controlledObj;

			public Vector3 m_initialObjOffset;

			public Vector3 m_initialScale;

			public float m_initialDistance;

			public Quaternion m_prevObjRot;

			public Vector3 m_prevHandVector;

			public GameObject m_pointedAtObject;

			public float m_pointedAtTime;

			public List<GameObject> m_touchedObjects;

			public Dictionary<GameObject, float> m_touchDwellTimers;

			public Vector3 variance;

			public float threshold = 5E-05f;

			public bool stable;

			public Vector3 average;

			public int stallFrames = 30;

			public Vector3[] recentLocations;

			public int index;

			public HandGestureData(HandType handType)
			{
				this.m_handType = handType;
				this.m_currMode = MetaGesture.NONE;
				this.m_hand = Hands.GetHands()[(int)this.m_handType];
				this.m_touchedObjects = new List<GameObject>();
				this.m_touchDwellTimers = new Dictionary<GameObject, float>();
				this.variance = default(Vector3);
				this.recentLocations = new Vector3[this.stallFrames];
			}
		}

		private GestureManager.HandGestureData[] _handGestureData;

		[SerializeField]
		private float _touchDwellTime = 1f;

		[SerializeField]
		private float _pointDwellTime = 1f;

		[SerializeField]
		private bool _enableGrab = true;

		[SerializeField]
		private HandType _defaultGrabbingHandType = HandType.EITHER;

		[SerializeField]
		private float _defaultGrabbableDistance = 0.1f;

		[SerializeField]
		private bool _enableGestureSounds = true;

		[SerializeField]
		private bool _enablePinch = true;

		[SerializeField]
		private HandType _defaultPinchingHandType = HandType.EITHER;

		[SerializeField]
		private float _defaultPinchableDistance = 0.1f;

		private MetaPhysics _physics = new MetaPhysics();

		private bool _markerGrabDistIncrease = true;

		private float _markerGrabDistance = 0.2f;

		private float _grabSwitchingBias = 1.25f;

		[SerializeField]
		private AudioClip _grabOn;

		[SerializeField]
		private AudioClip _grabOff;

		[SerializeField]
		private AudioClip _pinchOn;

		[SerializeField]
		private AudioClip _pinchOff;

		[SerializeField]
		private AudioClip _outOfBounds;

		private AudioSource _gestureSound;

		private float _minBound = 0.2f;

		private bool _twoHandedGesturing;

		private Vector3 _prevHandsVector;

		private Quaternion _prevObjRotation;

		private Transform _controlledObj;

		private Vector3 _initialObjOffset;

		private float _initialHandDistance;

		private Vector3 _initialScale;

		private void Start()
		{
			this._handGestureData = new GestureManager.HandGestureData[2];
			this._handGestureData[0] = new GestureManager.HandGestureData(HandType.LEFT);
			this._handGestureData[1] = new GestureManager.HandGestureData(HandType.RIGHT);
			this._gestureSound = base.GetComponent<AudioSource>();
		}

		private void Update()
		{
			this.UpdateGestures();
			this.ProcessStability();
			this.ProcessTwoHandedGestures();
			this.ProcessHands();
			this.UpdateGrabCircles();
		}

		private void UpdateGestures()
		{
			for (int i = 0; i < 2; i++)
			{
				if (this._handGestureData[i].m_hand != null)
				{
					this._handGestureData[i].m_currGesture = this._handGestureData[i].m_hand.gesture.type;
				}
			}
		}

		private void ProcessHands()
		{
			if (!this._twoHandedGesturing)
			{
				for (int i = 0; i < 2; i++)
				{
					if (this._handGestureData[i].m_hand != null)
					{
						if (this._handGestureData[i].m_currMode != MetaGesture.NONE)
						{
							MetaBody component = this._handGestureData[i].m_controlledObj.GetComponent<MetaBody>();
							if (this._handGestureData[i].m_currMode == MetaGesture.GRAB)
							{
								if (component.moveObjectOnGrab)
								{
									this._handGestureData[i].m_controlledObj.SendMessage("OnGrabMove", this._handGestureData[i].m_handType, SendMessageOptions.DontRequireReceiver);
									this._physics.MoveObj(this._handGestureData[i].m_controlledObj, this._handGestureData[i].m_hand.palm.position, this._handGestureData[i].m_initialObjOffset);
									this.CheckBounds(this._handGestureData[i]);
									this._handGestureData[i].m_prevPalmPos = this._handGestureData[i].m_hand.palm.position;
								}
								if (component.rotateObjectOnGrab)
								{
									this._handGestureData[i].m_controlledObj.SendMessage("OnGrabRotate", this._handGestureData[i].m_handType, SendMessageOptions.DontRequireReceiver);
									Vector3 handsVector = this._handGestureData[i].m_hand.palm.position - this._handGestureData[i].m_controlledObj.position;
									this._physics.RotateObj(this._handGestureData[i].m_controlledObj, ref this._handGestureData[i].m_prevObjRot, ref this._handGestureData[i].m_prevHandVector, handsVector);
								}
								if (component.scaleObjectOnGrab)
								{
									this._handGestureData[i].m_controlledObj.SendMessage("OnGrabScale", this._handGestureData[i].m_handType, SendMessageOptions.DontRequireReceiver);
									float handDist = Vector3.Distance(this._handGestureData[i].m_hand.palm.position, UnityEngine.Camera.main.transform.position);
									this._physics.ScaleObj(this._handGestureData[i].m_controlledObj, this._handGestureData[i].m_initialDistance, handDist, this._handGestureData[i].m_initialScale, false);
								}
								MarkerTargetIndicator.UpdateClosestMarkerID(this._handGestureData[i].m_controlledObj, true);
							}
							else if (this._handGestureData[i].m_currMode == MetaGesture.PINCH)
							{
								this._handGestureData[i].m_controlledObj.SendMessage("OnPinchHold", this._handGestureData[i].m_handType, SendMessageOptions.DontRequireReceiver);
								if (component.moveObjectOnPinch)
								{
									this._handGestureData[i].m_controlledObj.SendMessage("OnPinchMove", this._handGestureData[i].m_handType, SendMessageOptions.DontRequireReceiver);
									this._physics.MoveObj(this._handGestureData[i].m_controlledObj, this._handGestureData[i].m_hand.palm.position, this._handGestureData[i].m_initialObjOffset);
								}
								if (component.scaleObjectOnPinch)
								{
									this._handGestureData[i].m_controlledObj.SendMessage("OnPinchScale", this._handGestureData[i].m_handType, SendMessageOptions.DontRequireReceiver);
									float handDist2 = Vector3.Distance(this._handGestureData[i].m_hand.palm.position, UnityEngine.Camera.main.transform.position);
									this._physics.ScaleObj(this._handGestureData[i].m_controlledObj, this._handGestureData[i].m_initialDistance, handDist2, this._handGestureData[i].m_initialScale, false);
								}
							}
							this.CheckRelease(this._handGestureData[i]);
						}
						else
						{
							this.CheckGesture(this._handGestureData[i]);
							this._handGestureData[i].m_prevPalmPos = UnityEngine.Camera.main.transform.position;
						}
						this.CheckTouchGesture(this._handGestureData[i]);
						this.CheckPointGesture(this._handGestureData[i]);
					}
				}
			}
		}

		private void ProcessStability()
		{
			for (int i = 0; i < 2; i++)
			{
				if (Hands.GetHands()[i] != null)
				{
					this._handGestureData[i].recentLocations[this._handGestureData[i].index++ % this._handGestureData[i].stallFrames] = this._handGestureData[i].m_hand.pointer.position;
					if (this._handGestureData[i].index >= this._handGestureData[i].stallFrames)
					{
						this._handGestureData[i].average.x = (this._handGestureData[i].average.y = (this._handGestureData[i].average.z = 0f));
						Vector3[] recentLocations = this._handGestureData[i].recentLocations;
						for (int j = 0; j < recentLocations.Length; j++)
						{
							Vector3 vector = recentLocations[j];
							this._handGestureData[i].average += vector;
						}
						this._handGestureData[i].average /= (float)this._handGestureData[i].stallFrames;
						Vector3 vector2 = default(Vector3);
						Vector3[] recentLocations2 = this._handGestureData[i].recentLocations;
						for (int k = 0; k < recentLocations2.Length; k++)
						{
							Vector3 vector3 = recentLocations2[k];
							vector2.x += vector3.x * vector3.x;
							vector2.y += vector3.y * vector3.y;
							vector2.z += vector3.z * vector3.z;
						}
						vector2 /= (float)this._handGestureData[i].stallFrames;
						this._handGestureData[i].variance.x = vector2.x - this._handGestureData[i].average.x * this._handGestureData[i].average.x;
						this._handGestureData[i].variance.y = vector2.y - this._handGestureData[i].average.y * this._handGestureData[i].average.y;
						this._handGestureData[i].variance.z = vector2.z - this._handGestureData[i].average.z * this._handGestureData[i].average.z;
						this._handGestureData[i].stable = (this._handGestureData[i].variance.x < this._handGestureData[i].threshold && this._handGestureData[i].variance.y < this._handGestureData[i].threshold && this._handGestureData[i].variance.z < this._handGestureData[i].threshold);
					}
				}
			}
		}

		private void ProcessTwoHandedGestures()
		{
			if (this._handGestureData[0].m_currGesture == this._handGestureData[1].m_currGesture)
			{
				if (this._handGestureData[0].m_currGesture == MetaGesture.GRAB)
				{
					if (this._twoHandedGesturing)
					{
						MetaBody component = this._controlledObj.GetComponent<MetaBody>();
						if (component.moveObjectOnTwoHandedGrab)
						{
							Vector3 newPosition = (Hands.right.palm.position + Hands.left.palm.position) / 2f;
							this._physics.MoveObj(this._controlledObj, newPosition, this._initialObjOffset);
						}
						else if (component.rotateObjectOnTwoHandedGrab)
						{
							Vector3 handsVector = Hands.right.palm.position - Hands.left.palm.position;
							this._physics.RotateObj(this._controlledObj, ref this._prevObjRotation, ref this._prevHandsVector, handsVector);
						}
						else if (component.scaleObjectOnTwoHandedGrab)
						{
							float handDist = Vector3.Distance(Hands.left.palm.position, Hands.right.palm.position);
							this._physics.ScaleObj(this._controlledObj, this._initialHandDistance, handDist, this._initialScale, true);
						}
					}
					else
					{
						Transform transform = this.FindClosestObj(null, MetaGesture.GRAB);
						if (transform != null)
						{
							this.StartTwoHandedGesture(transform);
						}
					}
				}
				else
				{
					this.EndTwoHandedGesture();
				}
			}
			else
			{
				this.EndTwoHandedGesture();
			}
		}

		private void StartTwoHandedGesture(Transform closestObj)
		{
			this._controlledObj = closestObj;
			MetaBody component = this._controlledObj.GetComponent<MetaBody>();
			if (this._handGestureData[0].m_currMode == MetaGesture.NONE && this._handGestureData[1].m_currMode == MetaGesture.NONE && this._enableGestureSounds)
			{
				this._gestureSound.clip = this._grabOn;
				this._gestureSound.Play();
			}
			component.gesture = MetaGesture.GRAB;
			component.grabbed = true;
			this._handGestureData[0].m_closestObj = this._controlledObj;
			this._handGestureData[1].m_closestObj = this._controlledObj;
			HandsInputModule.ToggleHand(HandType.LEFT, true);
			HandsInputModule.ToggleHand(HandType.RIGHT, true);
			this._initialObjOffset = (Hands.right.palm.position + Hands.left.palm.position) / 2f - this._controlledObj.position;
			this._prevHandsVector = Hands.right.palm.position - Hands.left.palm.position;
			this._prevObjRotation = this._controlledObj.rotation;
			this._initialHandDistance = Vector3.Distance(Hands.left.palm.position, Hands.right.palm.position);
			this._initialScale = this._controlledObj.localScale;
			this._twoHandedGesturing = true;
		}

		private void EndTwoHandedGesture()
		{
			if (this._twoHandedGesturing)
			{
				this._twoHandedGesturing = false;
				this.EnableHands();
				this._handGestureData[0].m_currMode = MetaGesture.NONE;
				this._handGestureData[1].m_currMode = MetaGesture.NONE;
				this._controlledObj.GetComponent<MetaBody>().gesture = MetaGesture.NONE;
				this._controlledObj.GetComponent<MetaBody>().grabbed = false;
				this._controlledObj = null;
			}
		}

		private void CheckBounds(GestureManager.HandGestureData handGestureData)
		{
			float num = Vector3.Distance(handGestureData.m_prevPalmPos, UnityEngine.Camera.main.transform.position);
			float num2 = Vector3.Distance(handGestureData.m_hand.palm.position, UnityEngine.Camera.main.transform.position);
			if (num2 < this._minBound && num2 < num && num != 0f)
			{
				this.ReleaseObj(handGestureData, true);
				Vector3 forward = UnityEngine.Camera.main.transform.forward;
				Vector3 vector = handGestureData.m_hand.palm.position + 0.1f * forward;
				if (Vector3.Distance(vector, UnityEngine.Camera.main.transform.position) < this._minBound)
				{
					vector = handGestureData.m_prevPalmPos + 0.1f * forward;
				}
				LeanTween.move(handGestureData.m_controlledObj.gameObject, vector, 1f).setEase((LeanTweenType)18);
				this._enableGrab = false;
				this._enablePinch = false;
				base.Invoke("EnableGrabPinch", 1f);
			}
		}

		private void EnableGrabPinch()
		{
			this._enableGrab = true;
			this._enablePinch = true;
		}

		private void CheckGesture(GestureManager.HandGestureData handGestureData)
		{
			this.FindNextGrabbaleObj(handGestureData);
			Transform transform = null;
			if (handGestureData.m_currGesture == MetaGesture.GRAB)
			{
				transform = handGestureData.m_closestObj;
			}
			else if (handGestureData.m_currGesture == MetaGesture.PINCH)
			{
				transform = this.FindClosestObj(handGestureData, MetaGesture.PINCH);
			}
			if (transform != null)
			{
				this.SetControlledObj(handGestureData, transform);
			}
		}

		private void CheckTouchGesture(GestureManager.HandGestureData handGestureData)
		{
			List<GameObject> list = new List<GameObject>();
			Collider[] array = Physics.OverlapSphere(handGestureData.m_hand.pointer.gameObject.transform.position, ((SphereCollider)handGestureData.m_hand.pointer.gameObject.GetComponent<Collider>()).radius);
			Collider[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Collider collider = array2[i];
				if (MetaSingleton<MetaManager>.Instance.touchables.Contains(collider.transform) && (collider.GetComponent<MetaBody>().useDefaultTouchSettings || collider.GetComponent<MetaBody>().touchingHandType == HandType.EITHER || collider.GetComponent<MetaBody>().touchingHandType == handGestureData.m_handType))
				{
					list.Add(collider.gameObject);
					if (handGestureData.m_touchedObjects.Contains(collider.gameObject))
					{
						collider.gameObject.SendMessage("OnTouchHold", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
						Dictionary<GameObject, float> touchDwellTimers;
						Dictionary<GameObject, float> expr_E9 = touchDwellTimers = handGestureData.m_touchDwellTimers;
						GameObject gameObject;
						GameObject expr_F2 = gameObject = collider.gameObject;
						float num = touchDwellTimers[gameObject];
						expr_E9[expr_F2] = num + Time.deltaTime;
						float touchDwellTime;
						if (collider.GetComponent<MetaBody>().useDefaultTouchSettings)
						{
							touchDwellTime = this._touchDwellTime;
						}
						else
						{
							touchDwellTime = collider.GetComponent<MetaBody>().touchDwellTime;
						}
						float completion = handGestureData.m_touchDwellTimers[collider.gameObject] / touchDwellTime;
						if ((collider.GetComponent<MetaBody>().touchableDwellable || collider.GetComponent<MetaBody>().useDefaultTouchSettings) && (handGestureData.stable || (!collider.GetComponent<MetaBody>().useDefaultTouchSettings && !collider.GetComponent<MetaBody>().touchDwellMustBeStable)))
						{
							Dictionary<GameObject, float> touchDwellTimers2;
							Dictionary<GameObject, float> expr_19E = touchDwellTimers2 = handGestureData.m_touchDwellTimers;
							GameObject expr_1A7 = gameObject = collider.gameObject;
							num = touchDwellTimers2[gameObject];
							expr_19E[expr_1A7] = num + Time.deltaTime;
							if (MetaSingleton<InputIndicators>.Instance.dwellIndicators)
							{
								MetaSingleton<InputIndicators>.Instance.UpdateDwellIndicators(handGestureData.m_handType, completion);
							}
							else
							{
								MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
							}
						}
						else
						{
							MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
							handGestureData.m_touchDwellTimers[collider.gameObject] = 0f;
						}
						if (collider.GetComponent<MetaBody>().touchableDwellable && handGestureData.m_touchDwellTimers[collider.gameObject] >= this._touchDwellTime)
						{
							collider.gameObject.SendMessage("OnTouchDwell", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
						}
					}
					else
					{
						handGestureData.m_touchedObjects.Add(collider.gameObject);
						collider.gameObject.SendMessage("OnTouchEnter", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
						handGestureData.m_touchDwellTimers.Add(collider.gameObject, 0f);
					}
				}
			}
			foreach (GameObject current in handGestureData.m_touchedObjects)
			{
				if (!list.Contains(current))
				{
					current.SendMessage("OnTouchExit", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					if (handGestureData.m_touchDwellTimers[current] >= this._touchDwellTime && current.GetComponent<MetaBody>().touchableDwellable)
					{
						current.SendMessage("OnTouchDwellExit", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					}
					MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
					handGestureData.m_touchDwellTimers.Remove(current);
				}
			}
			handGestureData.m_touchedObjects = list;
		}

		private void CheckPointGesture(GestureManager.HandGestureData handGestureData)
		{
			GameObject objectOfInterest = handGestureData.m_hand.pointer.ObjectOfInterest;
			if (handGestureData.m_hand.isValid && objectOfInterest != null && MetaSingleton<MetaManager>.Instance.pointables.Contains(objectOfInterest.transform) && (objectOfInterest.GetComponent<MetaBody>().useDefaultPointSettings ||objectOfInterest.GetComponent<MetaBody>().pointingHandType == HandType.EITHER ||objectOfInterest.GetComponent<MetaBody>().pointingHandType == handGestureData.m_handType))
			{
				if (handGestureData.m_pointedAtObject == objectOfInterest)
				{
					objectOfInterest.SendMessage("OnPointHold", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					handGestureData.m_pointedAtTime += Time.deltaTime;
					if ((objectOfInterest.GetComponent<MetaBody>().useDefaultPointSettings || objectOfInterest.GetComponent<MetaBody>().pointableDwellable) && (handGestureData.stable || (!objectOfInterest.GetComponent<MetaBody>().useDefaultPointSettings && !objectOfInterest.GetComponent<MetaBody>().pointDwellMustBeStable)))
					{
						float pointDwellTime;
						if (objectOfInterest.GetComponent<MetaBody>().useDefaultPointSettings)
						{
							pointDwellTime = this._pointDwellTime;
						}
						else
						{
							pointDwellTime = objectOfInterest.GetComponent<MetaBody>().pointDwellTime;
						}
						if (handGestureData.m_pointedAtTime >= pointDwellTime)
						{
							objectOfInterest.gameObject.SendMessage("OnPointDwell", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
						}
						float completion = handGestureData.m_pointedAtTime / pointDwellTime;
						if (MetaSingleton<InputIndicators>.Instance.dwellIndicators)
						{
							MetaSingleton<InputIndicators>.Instance.UpdateDwellIndicators(handGestureData.m_handType, completion);
						}
						else
						{
							MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
						}
					}
				}
				else
				{
					objectOfInterest.SendMessage("OnPointEnter", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					handGestureData.m_pointedAtTime = 0f;
					if (handGestureData.m_pointedAtObject != null)
					{
						handGestureData.m_pointedAtObject.SendMessage("OnPointExit", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					}
					handGestureData.m_pointedAtObject = objectOfInterest;
				}
			}
			else
			{
				handGestureData.m_pointedAtTime = 0f;
				if (handGestureData.m_pointedAtObject != null)
				{
					handGestureData.m_pointedAtObject.SendMessage("OnPointExit", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
				}
				MetaSingleton<InputIndicators>.Instance.HideDwellIndicators(handGestureData.m_handType);
			}
		}

		private Transform FindClosestObj(GestureManager.HandGestureData handGestureData, MetaGesture gesture)
		{
			Transform result = null;
			float num = 0f;
			Vector3 vector;
			List<Transform> list;
			if (gesture == MetaGesture.GRAB)
			{
				if (handGestureData != null)
				{
					vector = handGestureData.m_hand.palm.position;
				}
				else
				{
					vector = (Hands.left.palm.position + Hands.right.palm.position) / 2f;
				}
				list = MetaSingleton<MetaManager>.Instance.grabbables;
			}
			else
			{
				if (handGestureData != null)
				{
					vector = handGestureData.m_hand.gesture.position;
				}
				else
				{
					vector = (Hands.left.gesture.position + Hands.right.gesture.position) / 2f;
				}
				list = MetaSingleton<MetaManager>.Instance.pinchables;
			}
			foreach (Transform current in list)
			{
				if (current != null)
				{
					MetaBody component = current.GetComponent<MetaBody>();
					if (component != null && (handGestureData == null || component.gesture == MetaGesture.NONE) && current.gameObject.activeInHierarchy && (handGestureData != null || component.twoHandedGrabbable))
					{
						float num2 = Vector3.Distance(vector, current.position);
						if ((num2 < this.GesturableDistance(component, gesture) || (current.GetComponent<Collider>() != null && current.GetComponent<Collider>().bounds.Contains(vector))) && (this.GestureHandType(component, gesture) == HandType.EITHER || handGestureData == null || this.GestureHandType(component, gesture) == handGestureData.m_handType) && (num == 0f || num2 < num))
						{
							result = current;
							num = num2;
						}
					}
				}
			}
			return result;
		}

		private float GesturableDistance(MetaBody metaBody, MetaGesture gesture)
		{
			if (gesture == MetaGesture.GRAB)
			{
				float num;
				if (metaBody.useDefaultGrabSettings)
				{
					num = this._defaultGrabbableDistance;
				}
				else
				{
					num = metaBody.grabbableDistance;
				}
				if (metaBody.markerTarget && metaBody.markerTargetID != -1 && !metaBody.grabbed && this._markerGrabDistIncrease && this._markerGrabDistance > num)
				{
					num = this._markerGrabDistance;
				}
				return num;
			}
			if (gesture != MetaGesture.PINCH)
			{
				return 0f;
			}
			if (metaBody.useDefaultPinchSettings)
			{
				return this._defaultPinchableDistance;
			}
			return metaBody.pinchableDistance;
		}

		private HandType GestureHandType(MetaBody metaBody, MetaGesture gesture)
		{
			if (gesture == MetaGesture.GRAB)
			{
				if (metaBody.useDefaultGrabSettings)
				{
					return this._defaultGrabbingHandType;
				}
				return metaBody.grabbingHandType;
			}
			else
			{
				if (gesture != MetaGesture.PINCH)
				{
					return HandType.UNKNOWN;
				}
				if (metaBody.useDefaultPinchSettings)
				{
					return this._defaultPinchingHandType;
				}
				return metaBody.pinchingHandType;
			}
		}

		private void FindNextGrabbaleObj(GestureManager.HandGestureData handGestureData)
		{
			Transform transform = this.FindClosestObj(handGestureData, MetaGesture.GRAB);
			if (transform != null && handGestureData.m_closestObj != null)
			{
				Vector3 position = handGestureData.m_hand.palm.position;
				float num = Vector3.Distance(position, transform.position);
				float num2 = Vector3.Distance(position, handGestureData.m_closestObj.position);
				num *= this._grabSwitchingBias;
				if (num < num2)
				{
					handGestureData.m_closestObj = transform;
				}
			}
			else
			{
				handGestureData.m_closestObj = transform;
			}
		}

		private void SetControlledObj(GestureManager.HandGestureData handGestureData, Transform controlledObj)
		{
			MetaBody component = controlledObj.GetComponent<MetaBody>();
			if ((handGestureData.m_currGesture == MetaGesture.GRAB && this._enableGrab) || (handGestureData.m_currGesture == MetaGesture.PINCH && this._enablePinch))
			{
				handGestureData.m_controlledObj = controlledObj;
				handGestureData.m_initialObjOffset = handGestureData.m_hand.palm.position - controlledObj.position;
				handGestureData.m_prevObjRot = controlledObj.rotation;
				handGestureData.m_prevHandVector = handGestureData.m_hand.palm.position - handGestureData.m_controlledObj.position;
				handGestureData.m_initialDistance = Vector3.Distance(handGestureData.m_hand.palm.position, UnityEngine.Camera.main.transform.position);
				handGestureData.m_initialScale = controlledObj.localScale;
				HandsInputModule.ToggleHand(handGestureData.m_handType, true);
				if (handGestureData.m_currGesture == MetaGesture.GRAB && this._enableGrab)
				{
					handGestureData.m_currMode = MetaGesture.GRAB;
					handGestureData.m_controlledObj = controlledObj;
					component.gesture = MetaGesture.GRAB;
					component.grabbed = true;
					controlledObj.SendMessage("OnGrab", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					this._gestureSound.clip = this._grabOn;
				}
				else if (handGestureData.m_currGesture == MetaGesture.PINCH && this._enablePinch)
				{
					handGestureData.m_currMode = MetaGesture.PINCH;
					handGestureData.m_controlledObj = controlledObj;
					component.gesture = MetaGesture.PINCH;
					component.pinched = true;
					controlledObj.SendMessage("OnPinch", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
					this._gestureSound.clip = this._pinchOn;
				}
				if (this._enableGestureSounds)
				{
					this._gestureSound.Play();
				}
			}
		}

		private void CheckRelease(GestureManager.HandGestureData handGestureData)
		{
			if ((handGestureData.m_currMode == MetaGesture.GRAB && handGestureData.m_currGesture == MetaGesture.OPEN) || (handGestureData.m_currMode != MetaGesture.GRAB && handGestureData.m_currGesture != handGestureData.m_currMode) || !handGestureData.m_hand.isValid)
			{
				this.ReleaseObj(handGestureData, false);
			}
		}

		private void ReleaseObj(GestureManager.HandGestureData handGestureData, bool forced = false)
		{
			MetaBody component = handGestureData.m_controlledObj.GetComponent<MetaBody>();
			component.gesture = MetaGesture.NONE;
			if (handGestureData.m_currMode == MetaGesture.GRAB)
			{
				component.grabbed = false;
				handGestureData.m_controlledObj.SendMessage("OnRelease", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
				handGestureData.m_controlledObj.SendMessage("OnGrabRelease", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
				MarkerTargetIndicator.UpdateClosestMarkerID(handGestureData.m_controlledObj, false);
				if (this._enableGestureSounds && !forced)
				{
					this._gestureSound.clip = this._grabOff;
					this._gestureSound.Play();
				}
			}
			else if (handGestureData.m_currMode == MetaGesture.PINCH)
			{
				component.pinched = false;
				handGestureData.m_controlledObj.SendMessage("OnPinchRelease", handGestureData.m_handType, SendMessageOptions.DontRequireReceiver);
				if (this._enableGestureSounds && !forced)
				{
					this._gestureSound.clip = this._pinchOff;
					this._gestureSound.Play();
				}
			}
			if (forced)
			{
				this._gestureSound.clip = this._outOfBounds;
				this._gestureSound.Play();
			}
			handGestureData.m_currMode = MetaGesture.NONE;
			base.StartCoroutine("EnableHand", handGestureData.m_handType);
		}

		private void EnableHands()
		{
			base.StartCoroutine("EnableHand", HandType.LEFT);
			base.StartCoroutine("EnableHand", HandType.RIGHT);
		}

		[DebuggerHidden]
		private IEnumerator EnableHand(HandType hand)
		{
            yield return hand;
		}

		private void UpdateGrabCircles()
		{
			for (int i = 0; i < 2; i++)
			{
				MetaSingleton<InputIndicators>.Instance.UpdateGrabCircles(i, this._handGestureData[i].m_closestObj != null);
			}
		}
	}
}
