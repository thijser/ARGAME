

namespace Meta
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

	[AddComponentMenu("Event/Hands Input Module")]
	internal class HandsInputModule : PointerInputModule
	{
		private class HandPointerData
		{
			public bool m_pressed;

			public bool m_held;

			public bool m_released;

			public bool m_invalidPress;

			public bool m_scrolling;

			public float m_pressTimer;

			public PointerInputModule.MouseButtonEventData m_pointerData;

			public GameObject m_fingerGO;

			public GameObject m_pressedObject;

			public HandPointerData(GameObject finger, EventSystem eventSystem, int pointerId)
			{
				this.m_fingerGO = finger;
				this.m_pointerData = new PointerInputModule.MouseButtonEventData();
				this.m_pointerData.buttonData = new PointerEventData(eventSystem);
				this.m_pointerData.buttonData.pointerId = pointerId;
			}
		}

		public enum InputMode
		{
			Mouse,
			Buttons
		}

		private HandsInputModule.InputMode m_CurrentInputMode = HandsInputModule.InputMode.Buttons;

		private static bool _processMouse = true;

		private HandsInputModule.HandPointerData[] _fingerPointerData;

		private HandsInputModule.HandPointerData[] _fingertipsData;

		private static bool[] _handDisabled;

		private float _colliderDepth = 0.04f;

		private bool behind;

		private float behindTimerStart;

		private float behindTimerThreshold = 1f;

		internal static bool processMouse
		{
			get
			{
				return HandsInputModule._processMouse;
			}
			set
			{
				HandsInputModule._processMouse = value;
			}
		}

		public HandsInputModule.InputMode inputMode
		{
			get
			{
				return this.m_CurrentInputMode;
			}
		}

		protected HandsInputModule()
		{
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.lastSelectedGameObject;
			}
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(null, this.GetBaseEventData());
			base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
			this._fingerPointerData = new HandsInputModule.HandPointerData[2];
			this._fingertipsData = new HandsInputModule.HandPointerData[10];
			HandsInputModule._handDisabled = new bool[2];
			for (int i = 0; i < 2; i++)
			{
				if (Hands.GetHands()[i] != null)
				{
					this._fingerPointerData[i] = new HandsInputModule.HandPointerData(Hands.GetHands()[i].pointer.gameObject, base.eventSystem, i);
					for (int j = 0; j < 5; j++)
					{
						this._fingertipsData[5 * i + j] = new HandsInputModule.HandPointerData(Hands.GetHands()[i].fingers[j].gameObject, base.eventSystem, j);
					}
				}
			}
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			base.ClearSelection();
		}

		public override void Process()
		{
			this.SendUpdateEventToSelectedObject();
			if (HandsInputModule._processMouse)
			{
				this.ProcessMouseEvent(null);
			}
			if (Hands.handConfig.fingertips)
			{
				for (int i = 0; i < 10; i++)
				{
					if (this._fingertipsData[i] != null && !HandsInputModule._handDisabled[i / 5])
					{
						this.ProcessMetaEvent(this._fingertipsData[i]);
					}
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					if (this._fingerPointerData[j] != null && !HandsInputModule._handDisabled[j])
					{
						this.ProcessMetaEvent(this._fingerPointerData[j]);
					}
				}
			}
		}

		internal void ProcessMouseEvent(PointerInputModule.MouseButtonEventData mouseButtonEventData = null)
		{
			PointerInputModule.MouseState mouseState = null;
			bool pressed;
			bool released;
			PointerInputModule.MouseButtonEventData mouseButtonEventData2;
			if (mouseButtonEventData == null)
			{
				mouseState = this.GetMousePointerEventData();
				pressed = mouseState.AnyPressesThisFrame();
				released = mouseState.AnyReleasesThisFrame();
				mouseButtonEventData2 = mouseState.GetButtonState(0).eventData;
			}
			else
			{
				pressed = mouseButtonEventData.PressedThisFrame();
				released = mouseButtonEventData.ReleasedThisFrame();
				mouseButtonEventData2 = mouseButtonEventData;
			}
			if (!this.UseMouse(pressed, released, mouseButtonEventData2.buttonData) && mouseButtonEventData == null)
			{
				return;
			}
			this.ProcessMousePress(mouseButtonEventData2);
			this.ProcessMove(mouseButtonEventData2.buttonData);
			this.ProcessDrag(mouseButtonEventData2.buttonData);
			if (mouseState != null)
			{
				this.ProcessMousePress(mouseState.GetButtonState(PointerEventData.InputButton.Right).eventData);
				this.ProcessDrag(mouseState.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
				this.ProcessMousePress(mouseState.GetButtonState(PointerEventData.InputButton.Middle).eventData);
				this.ProcessDrag(mouseState.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			}
			if (!Mathf.Approximately(mouseButtonEventData2.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(mouseButtonEventData2.buttonData.pointerCurrentRaycast.gameObject);
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, mouseButtonEventData2.buttonData, ExecuteEvents.scrollHandler);
			}
		}

		private void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					float num = unscaledTime - buttonData.clickTime;
					if (num < 0.3f)
					{
						PointerEventData expr_B0 = buttonData;
						expr_B0.clickCount = expr_B0.clickCount + 1;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				buttonData.dragging = false;
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					base.HandlePointerExitAndEnter(buttonData, null);
					base.HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		private bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		private bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
		{
			if (this.m_CurrentInputMode == HandsInputModule.InputMode.Mouse)
			{
				return true;
			}
			if (pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling())
			{
				this.m_CurrentInputMode = HandsInputModule.InputMode.Mouse;
				base.eventSystem.SetSelectedGameObject(null);
			}
			return this.m_CurrentInputMode == HandsInputModule.InputMode.Mouse;
		}

		private void ProcessMetaEvent(HandsInputModule.HandPointerData handPointData)
		{
			MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"));
			bool flag = false;
			Vector3 position = UnityEngine.Camera.main.transform.position;
			Vector3 vector = Vector3.Normalize(handPointData.m_fingerGO.transform.position - UnityEngine.Camera.main.transform.position);
			LayerMask layerMask = 1056;
            RaycastHit raycastHit;
			if (Physics.Raycast(position, vector, out raycastHit, float.PositiveInfinity, layerMask))
			{
				if (MetaCamera.IsVisible(raycastHit.collider.bounds))
				{
					handPointData.m_pressTimer += Time.deltaTime;
					if (raycastHit.collider.GetComponent<Scrollbar>() != null || raycastHit.collider.GetComponent<ScrollRect>() != null || raycastHit.collider.GetComponent<Slider>() != null)
					{
						handPointData.m_scrolling = true;
					}
					else
					{
						handPointData.m_scrolling = false;
					}
					float num = Vector3.Dot(raycastHit.collider.transform.right, handPointData.m_fingerGO.transform.position - handPointData.m_pointerData.buttonData.worldPosition);
					float num2 = Vector3.Dot(raycastHit.collider.transform.up, handPointData.m_fingerGO.transform.position - handPointData.m_pointerData.buttonData.worldPosition);
					handPointData.m_pointerData.buttonData.delta = new Vector2(num, num2);
					PointerEventData expr_1B4 = handPointData.m_pointerData.buttonData;
					expr_1B4.position = expr_1B4.position + handPointData.m_pointerData.buttonData.delta;
					handPointData.m_pointerData.buttonData.worldPosition = handPointData.m_fingerGO.transform.position;
					float num3 = Vector3.Distance(UnityEngine.Camera.main.transform.position, handPointData.m_fingerGO.transform.position);
					if ((num3 - raycastHit.distance < this._colliderDepth || handPointData.m_scrolling) && num3 - raycastHit.distance >= 0f)
					{
						this.behind = false;
						if (!handPointData.m_invalidPress && (handPointData.m_pressTimer >= 0.5f || handPointData.m_held))
						{
							flag = true;
							handPointData.m_pressed = !handPointData.m_held;
							handPointData.m_held = true;
							if (handPointData.m_pressed)
							{
								handPointData.m_pointerData.buttonData.position = handPointData.m_fingerGO.transform.position;
								RaycastResult pointerCurrentRaycast = default(RaycastResult);
								pointerCurrentRaycast.gameObject = raycastHit.collider.gameObject;
								handPointData.m_pointerData.buttonData.pointerCurrentRaycast = pointerCurrentRaycast;
							}
						}
					}
					else if (num3 - raycastHit.distance < 0f)
					{
						RaycastResult pointerCurrentRaycast2 = default(RaycastResult);
						pointerCurrentRaycast2.gameObject = raycastHit.collider.gameObject;
						handPointData.m_pointerData.buttonData.pointerCurrentRaycast = pointerCurrentRaycast2;
						handPointData.m_invalidPress = false;
						float distance = Vector3.Distance(handPointData.m_fingerGO.transform.position, raycastHit.point);
						MetaSingleton<InputIndicators>.Instance.UpdateHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"), distance);
						MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"), Color.green);
					}
					else
					{
						RaycastResult pointerCurrentRaycast3 = default(RaycastResult);
						pointerCurrentRaycast3.gameObject = null;
						handPointData.m_pointerData.buttonData.pointerCurrentRaycast = pointerCurrentRaycast3;
						if (!handPointData.m_held)
						{
							handPointData.m_invalidPress = true;
						}
						if (!this.behind)
						{
							MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"), new Color(0f, 0f, 0f, 0f));
							this.behindTimerStart = Time.time;
							this.behind = true;
						}
						else if (Time.time - this.behindTimerStart > this.behindTimerThreshold)
						{
							MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"), Color.red);
						}
						MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"));
						float distance2 = Vector3.Distance(handPointData.m_fingerGO.transform.position, raycastHit.point);
						MetaSingleton<InputIndicators>.Instance.UpdateHoverIndicator(handPointData.m_fingerGO.transform.FindChild("HoverIndicator"), distance2);
					}
				}
				else
				{
					handPointData.m_invalidPress = false;
					RaycastResult pointerCurrentRaycast4 = default(RaycastResult);
					pointerCurrentRaycast4.gameObject = null;
					handPointData.m_pointerData.buttonData.pointerCurrentRaycast = pointerCurrentRaycast4;
				}
			}
			else
			{
				handPointData.m_invalidPress = false;
				RaycastResult pointerCurrentRaycast5 = default(RaycastResult);
				pointerCurrentRaycast5.gameObject = null;
				handPointData.m_pointerData.buttonData.pointerCurrentRaycast = pointerCurrentRaycast5;
			}
			if (handPointData.m_held && !flag)
			{
				handPointData.m_pressed = false;
				handPointData.m_held = false;
				handPointData.m_released = true;
			}
			if (!handPointData.m_invalidPress)
			{
				if (handPointData.m_pressed)
				{
					handPointData.m_pressedObject = handPointData.m_pointerData.buttonData.pointerCurrentRaycast.gameObject;
					handPointData.m_pointerData.buttonState = 0;
				}
				else if (handPointData.m_released)
				{
					RaycastResult pointerCurrentRaycast6 = default(RaycastResult);
					pointerCurrentRaycast6.gameObject = handPointData.m_pressedObject;
					handPointData.m_pointerData.buttonData.pointerCurrentRaycast = pointerCurrentRaycast6;
				}

				this.ProcessMousePress(handPointData.m_pointerData);
				if (handPointData.m_released)
				{
					handPointData.m_pressTimer = 0f;
					handPointData.m_released = false;
				}
			}
			this.ProcessMove(handPointData.m_pointerData.buttonData);
			handPointData.m_pointerData.buttonData.useDragThreshold = false;
			this.ProcessDrag(handPointData.m_pointerData.buttonData);
		}

		public static void ToggleHand(HandType hand, bool disable)
		{
			HandsInputModule._handDisabled[(int)hand] = disable;
			if (disable)
			{
				MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(Hands.GetHands()[(int)hand].pointer.gameObject.transform.FindChild("HoverIndicator"));
				MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(Hands.GetHands()[(int)hand].pointer.gameObject.transform.FindChild("FingertipIndicator"));
			}
			else if (MetaSingleton<InputIndicators>.Instance.fingertipIndicators)
			{
				Hands.GetHands()[(int)hand].pointer.gameObject.transform.FindChild("FingertipIndicator").GetComponent<Renderer>().enabled = true;
			}
		}
	}
}
