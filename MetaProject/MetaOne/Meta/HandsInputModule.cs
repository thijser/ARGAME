using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta
{
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
				this.m_pointerData.buttonData.set_pointerId(pointerId);
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
			GameObject gameObject = base.get_eventSystem().get_currentSelectedGameObject();
			if (gameObject == null)
			{
				gameObject = base.get_eventSystem().get_lastSelectedGameObject();
			}
			if (gameObject == null)
			{
				gameObject = base.get_eventSystem().get_firstSelectedGameObject();
			}
			base.get_eventSystem().SetSelectedGameObject(null, this.GetBaseEventData());
			base.get_eventSystem().SetSelectedGameObject(gameObject, this.GetBaseEventData());
			this._fingerPointerData = new HandsInputModule.HandPointerData[2];
			this._fingertipsData = new HandsInputModule.HandPointerData[10];
			HandsInputModule._handDisabled = new bool[2];
			for (int i = 0; i < 2; i++)
			{
				if (Hands.GetHands()[i] != null)
				{
					this._fingerPointerData[i] = new HandsInputModule.HandPointerData(Hands.GetHands()[i].pointer.gameObject, base.get_eventSystem(), i);
					for (int j = 0; j < 5; j++)
					{
						this._fingertipsData[5 * i + j] = new HandsInputModule.HandPointerData(Hands.GetHands()[i].fingers[j].gameObject, base.get_eventSystem(), j);
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
				mouseButtonEventData2 = mouseState.GetButtonState(0).get_eventData();
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
				this.ProcessMousePress(mouseState.GetButtonState(1).get_eventData());
				this.ProcessDrag(mouseState.GetButtonState(1).get_eventData().buttonData);
				this.ProcessMousePress(mouseState.GetButtonState(2).get_eventData());
				this.ProcessDrag(mouseState.GetButtonState(2).get_eventData().buttonData);
			}
			if (!Mathf.Approximately(mouseButtonEventData2.buttonData.get_scrollDelta().get_sqrMagnitude(), 0f))
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(mouseButtonEventData2.buttonData.get_pointerCurrentRaycast().get_gameObject());
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, mouseButtonEventData2.buttonData, ExecuteEvents.get_scrollHandler());
			}
		}

		private void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.get_pointerCurrentRaycast().get_gameObject();
			if (data.PressedThisFrame())
			{
				buttonData.set_eligibleForClick(true);
				buttonData.set_delta(Vector2.get_zero());
				buttonData.set_dragging(false);
				buttonData.set_useDragThreshold(true);
				buttonData.set_pressPosition(buttonData.get_position());
				buttonData.set_pointerPressRaycast(buttonData.get_pointerCurrentRaycast());
				base.DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.get_pointerDownHandler());
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.get_unscaledTime();
				if (gameObject2 == buttonData.get_lastPress())
				{
					float num = unscaledTime - buttonData.get_clickTime();
					if (num < 0.3f)
					{
						PointerEventData expr_B0 = buttonData;
						expr_B0.set_clickCount(expr_B0.get_clickCount() + 1);
					}
					else
					{
						buttonData.set_clickCount(1);
					}
					buttonData.set_clickTime(unscaledTime);
				}
				else
				{
					buttonData.set_clickCount(1);
				}
				buttonData.set_pointerPress(gameObject2);
				buttonData.set_rawPointerPress(gameObject);
				buttonData.set_clickTime(unscaledTime);
				buttonData.set_pointerDrag(ExecuteEvents.GetEventHandler<IDragHandler>(gameObject));
				if (buttonData.get_pointerDrag() != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.get_pointerDrag(), buttonData, ExecuteEvents.get_initializePotentialDrag());
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>(buttonData.get_pointerPress(), buttonData, ExecuteEvents.get_pointerUpHandler());
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.get_pointerPress() == eventHandler && buttonData.get_eligibleForClick())
				{
					ExecuteEvents.Execute<IPointerClickHandler>(buttonData.get_pointerPress(), buttonData, ExecuteEvents.get_pointerClickHandler());
				}
				else if (buttonData.get_pointerDrag() != null)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.get_dropHandler());
				}
				buttonData.set_eligibleForClick(false);
				buttonData.set_pointerPress(null);
				buttonData.set_rawPointerPress(null);
				buttonData.set_dragging(false);
				if (buttonData.get_pointerDrag() != null)
				{
					ExecuteEvents.Execute<IEndDragHandler>(buttonData.get_pointerDrag(), buttonData, ExecuteEvents.get_endDragHandler());
				}
				buttonData.set_pointerDrag(null);
				if (gameObject != buttonData.get_pointerEnter())
				{
					base.HandlePointerExitAndEnter(buttonData, null);
					base.HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		private bool SendUpdateEventToSelectedObject()
		{
			if (base.get_eventSystem().get_currentSelectedGameObject() == null)
			{
				return false;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>(base.get_eventSystem().get_currentSelectedGameObject(), baseEventData, ExecuteEvents.get_updateSelectedHandler());
			return baseEventData.get_used();
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
				base.get_eventSystem().SetSelectedGameObject(null);
			}
			return this.m_CurrentInputMode == HandsInputModule.InputMode.Mouse;
		}

		private void ProcessMetaEvent(HandsInputModule.HandPointerData handPointData)
		{
			MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"));
			bool flag = false;
			Vector3 position = Camera.get_main().get_transform().get_position();
			Vector3 vector = Vector3.Normalize(handPointData.m_fingerGO.get_transform().get_position() - Camera.get_main().get_transform().get_position());
			LayerMask layerMask = 1056;
			RaycastHit raycastHit;
			if (Physics.Raycast(position, vector, ref raycastHit, float.PositiveInfinity, layerMask))
			{
				if (MetaCamera.IsVisible(raycastHit.get_collider().get_bounds()))
				{
					handPointData.m_pressTimer += Time.get_deltaTime();
					if (raycastHit.get_collider().GetComponent<Scrollbar>() != null || raycastHit.get_collider().GetComponent<MGUIScrollRect>() != null || raycastHit.get_collider().GetComponent<ScrollRect>() != null || raycastHit.get_collider().GetComponent<Slider>() != null)
					{
						handPointData.m_scrolling = true;
					}
					else
					{
						handPointData.m_scrolling = false;
					}
					float num = Vector3.Dot(raycastHit.get_collider().get_transform().get_right(), handPointData.m_fingerGO.get_transform().get_position() - handPointData.m_pointerData.buttonData.get_worldPosition());
					float num2 = Vector3.Dot(raycastHit.get_collider().get_transform().get_up(), handPointData.m_fingerGO.get_transform().get_position() - handPointData.m_pointerData.buttonData.get_worldPosition());
					handPointData.m_pointerData.buttonData.set_delta(new Vector2(num, num2));
					PointerEventData expr_1B4 = handPointData.m_pointerData.buttonData;
					expr_1B4.set_position(expr_1B4.get_position() + handPointData.m_pointerData.buttonData.get_delta());
					handPointData.m_pointerData.buttonData.set_worldPosition(handPointData.m_fingerGO.get_transform().get_position());
					float num3 = Vector3.Distance(Camera.get_main().get_transform().get_position(), handPointData.m_fingerGO.get_transform().get_position());
					if ((num3 - raycastHit.get_distance() < this._colliderDepth || handPointData.m_scrolling) && num3 - raycastHit.get_distance() >= 0f)
					{
						this.behind = false;
						if (!handPointData.m_invalidPress && (handPointData.m_pressTimer >= 0.5f || handPointData.m_held))
						{
							flag = true;
							handPointData.m_pressed = !handPointData.m_held;
							handPointData.m_held = true;
							if (handPointData.m_pressed)
							{
								handPointData.m_pointerData.buttonData.set_position(handPointData.m_fingerGO.get_transform().get_position());
								RaycastResult pointerCurrentRaycast = default(RaycastResult);
								pointerCurrentRaycast.set_gameObject(raycastHit.get_collider().get_gameObject());
								handPointData.m_pointerData.buttonData.set_pointerCurrentRaycast(pointerCurrentRaycast);
							}
						}
					}
					else if (num3 - raycastHit.get_distance() < 0f)
					{
						RaycastResult pointerCurrentRaycast2 = default(RaycastResult);
						pointerCurrentRaycast2.set_gameObject(raycastHit.get_collider().get_gameObject());
						handPointData.m_pointerData.buttonData.set_pointerCurrentRaycast(pointerCurrentRaycast2);
						handPointData.m_invalidPress = false;
						float distance = Vector3.Distance(handPointData.m_fingerGO.get_transform().get_position(), raycastHit.get_point());
						MetaSingleton<InputIndicators>.Instance.UpdateHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), distance);
						MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), Color.get_green());
					}
					else
					{
						RaycastResult pointerCurrentRaycast3 = default(RaycastResult);
						pointerCurrentRaycast3.set_gameObject(null);
						handPointData.m_pointerData.buttonData.set_pointerCurrentRaycast(pointerCurrentRaycast3);
						if (!handPointData.m_held)
						{
							handPointData.m_invalidPress = true;
						}
						if (!this.behind)
						{
							MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), new Color(0f, 0f, 0f, 0f));
							this.behindTimerStart = Time.get_time();
							this.behind = true;
						}
						else if (Time.get_time() - this.behindTimerStart > this.behindTimerThreshold)
						{
							MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), Color.get_red());
						}
						MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"));
						float distance2 = Vector3.Distance(handPointData.m_fingerGO.get_transform().get_position(), raycastHit.get_point());
						MetaSingleton<InputIndicators>.Instance.UpdateHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), distance2);
					}
				}
				else
				{
					handPointData.m_invalidPress = false;
					RaycastResult pointerCurrentRaycast4 = default(RaycastResult);
					pointerCurrentRaycast4.set_gameObject(null);
					handPointData.m_pointerData.buttonData.set_pointerCurrentRaycast(pointerCurrentRaycast4);
				}
			}
			else
			{
				handPointData.m_invalidPress = false;
				RaycastResult pointerCurrentRaycast5 = default(RaycastResult);
				pointerCurrentRaycast5.set_gameObject(null);
				handPointData.m_pointerData.buttonData.set_pointerCurrentRaycast(pointerCurrentRaycast5);
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
					handPointData.m_pressedObject = handPointData.m_pointerData.buttonData.get_pointerCurrentRaycast().get_gameObject();
					handPointData.m_pointerData.buttonState = 0;
				}
				else if (handPointData.m_released)
				{
					RaycastResult pointerCurrentRaycast6 = default(RaycastResult);
					pointerCurrentRaycast6.set_gameObject(handPointData.m_pressedObject);
					handPointData.m_pointerData.buttonData.set_pointerCurrentRaycast(pointerCurrentRaycast6);
					handPointData.m_pointerData.buttonState = 1;
				}
				else
				{
					handPointData.m_pointerData.buttonState = 3;
				}
				this.ProcessMousePress(handPointData.m_pointerData);
				if (handPointData.m_released)
				{
					handPointData.m_pressTimer = 0f;
					handPointData.m_released = false;
				}
			}
			this.ProcessMove(handPointData.m_pointerData.buttonData);
			handPointData.m_pointerData.buttonData.set_useDragThreshold(false);
			this.ProcessDrag(handPointData.m_pointerData.buttonData);
		}

		public static void ToggleHand(HandType hand, bool disable)
		{
			HandsInputModule._handDisabled[(int)hand] = disable;
			if (disable)
			{
				MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(Hands.GetHands()[(int)hand].pointer.gameObject.get_transform().FindChild("HoverIndicator"));
				MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(Hands.GetHands()[(int)hand].pointer.gameObject.get_transform().FindChild("FingertipIndicator"));
			}
			else if (MetaSingleton<InputIndicators>.Instance.fingertipIndicators)
			{
				Hands.GetHands()[(int)hand].pointer.gameObject.get_transform().FindChild("FingertipIndicator").GetComponent<Renderer>().set_enabled(true);
			}
		}
	}
}
