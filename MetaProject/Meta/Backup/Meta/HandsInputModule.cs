// Decompiled with JetBrains decompiler
// Type: Meta.HandsInputModule
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta
{
  [AddComponentMenu("Event/Hands Input Module")]
  internal class HandsInputModule : PointerInputModule
  {
    private static bool _processMouse = true;
    private HandsInputModule.InputMode m_CurrentInputMode;
    private HandsInputModule.HandPointerData[] _fingerPointerData;
    private HandsInputModule.HandPointerData[] _fingertipsData;
    private static bool[] _handDisabled;
    private float _colliderDepth;
    private bool behind;
    private float behindTimerStart;
    private float behindTimerThreshold;

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
      base.\u002Ector();
    }

    public virtual void ActivateModule()
    {
      ((BaseInputModule) this).ActivateModule();
      GameObject selectedGameObject = ((BaseInputModule) this).get_eventSystem().get_currentSelectedGameObject();
      if (Object.op_Equality((Object) selectedGameObject, (Object) null))
        selectedGameObject = ((BaseInputModule) this).get_eventSystem().get_lastSelectedGameObject();
      if (Object.op_Equality((Object) selectedGameObject, (Object) null))
        selectedGameObject = ((BaseInputModule) this).get_eventSystem().get_firstSelectedGameObject();
      ((BaseInputModule) this).get_eventSystem().SetSelectedGameObject((GameObject) null, ((BaseInputModule) this).GetBaseEventData());
      ((BaseInputModule) this).get_eventSystem().SetSelectedGameObject(selectedGameObject, ((BaseInputModule) this).GetBaseEventData());
      this._fingerPointerData = new HandsInputModule.HandPointerData[2];
      this._fingertipsData = new HandsInputModule.HandPointerData[10];
      HandsInputModule._handDisabled = new bool[2];
      for (int pointerId1 = 0; pointerId1 < 2; ++pointerId1)
      {
        if (Hands.GetHands()[pointerId1] != null)
        {
          this._fingerPointerData[pointerId1] = new HandsInputModule.HandPointerData(Hands.GetHands()[pointerId1].pointer.gameObject, ((BaseInputModule) this).get_eventSystem(), pointerId1);
          for (int pointerId2 = 0; pointerId2 < 5; ++pointerId2)
            this._fingertipsData[5 * pointerId1 + pointerId2] = new HandsInputModule.HandPointerData(Hands.GetHands()[pointerId1].fingers[pointerId2].gameObject, ((BaseInputModule) this).get_eventSystem(), pointerId2);
        }
      }
    }

    public virtual void DeactivateModule()
    {
      ((BaseInputModule) this).DeactivateModule();
      this.ClearSelection();
    }

    public virtual void Process()
    {
      this.SendUpdateEventToSelectedObject();
      if (HandsInputModule._processMouse)
        this.ProcessMouseEvent((PointerInputModule.MouseButtonEventData) null);
      if (Hands.handConfig.fingertips)
      {
        for (int index = 0; index < 10; ++index)
        {
          if (this._fingertipsData[index] != null && !HandsInputModule._handDisabled[index / 5])
            this.ProcessMetaEvent(this._fingertipsData[index]);
        }
      }
      else
      {
        for (int index = 0; index < 2; ++index)
        {
          if (this._fingerPointerData[index] != null && !HandsInputModule._handDisabled[index])
            this.ProcessMetaEvent(this._fingerPointerData[index]);
        }
      }
    }

    internal void ProcessMouseEvent(PointerInputModule.MouseButtonEventData mouseButtonEventData = null)
    {
      PointerInputModule.MouseState mouseState = (PointerInputModule.MouseState) null;
      bool pressed;
      bool released;
      PointerInputModule.MouseButtonEventData data;
      if (mouseButtonEventData == null)
      {
        mouseState = this.GetMousePointerEventData();
        pressed = mouseState.AnyPressesThisFrame();
        released = mouseState.AnyReleasesThisFrame();
        data = mouseState.GetButtonState((PointerEventData.InputButton) 0).get_eventData();
      }
      else
      {
        pressed = mouseButtonEventData.PressedThisFrame();
        released = mouseButtonEventData.ReleasedThisFrame();
        data = mouseButtonEventData;
      }
      if (!this.UseMouse(pressed, released, (PointerEventData) data.buttonData) && mouseButtonEventData == null)
        return;
      this.ProcessMousePress(data);
      this.ProcessMove((PointerEventData) data.buttonData);
      this.ProcessDrag((PointerEventData) data.buttonData);
      if (mouseState != null)
      {
        this.ProcessMousePress(mouseState.GetButtonState((PointerEventData.InputButton) 1).get_eventData());
        this.ProcessDrag((PointerEventData) mouseState.GetButtonState((PointerEventData.InputButton) 1).get_eventData().buttonData);
        this.ProcessMousePress(mouseState.GetButtonState((PointerEventData.InputButton) 2).get_eventData());
        this.ProcessDrag((PointerEventData) mouseState.GetButtonState((PointerEventData.InputButton) 2).get_eventData().buttonData);
      }
      Vector2 scrollDelta = ((PointerEventData) data.buttonData).get_scrollDelta();
      // ISSUE: explicit reference operation
      if (Mathf.Approximately(((Vector2) @scrollDelta).get_sqrMagnitude(), 0.0f))
        return;
      RaycastResult pointerCurrentRaycast = ((PointerEventData) data.buttonData).get_pointerCurrentRaycast();
      // ISSUE: explicit reference operation
      ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(((RaycastResult) @pointerCurrentRaycast).get_gameObject()), (BaseEventData) data.buttonData, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_scrollHandler());
    }

    private void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
    {
      PointerEventData pointerEventData1 = (PointerEventData) data.buttonData;
      RaycastResult pointerCurrentRaycast = pointerEventData1.get_pointerCurrentRaycast();
      // ISSUE: explicit reference operation
      GameObject gameObject1 = ((RaycastResult) @pointerCurrentRaycast).get_gameObject();
      if (data.PressedThisFrame())
      {
        pointerEventData1.set_eligibleForClick(true);
        pointerEventData1.set_delta(Vector2.get_zero());
        pointerEventData1.set_dragging(false);
        pointerEventData1.set_useDragThreshold(true);
        pointerEventData1.set_pressPosition(pointerEventData1.get_position());
        pointerEventData1.set_pointerPressRaycast(pointerEventData1.get_pointerCurrentRaycast());
        this.DeselectIfSelectionChanged(gameObject1, (BaseEventData) pointerEventData1);
        GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) pointerEventData1, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_pointerDownHandler());
        if (Object.op_Equality((Object) gameObject2, (Object) null))
          gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
        float unscaledTime = Time.get_unscaledTime();
        if (Object.op_Equality((Object) gameObject2, (Object) pointerEventData1.get_lastPress()))
        {
          if ((double) (unscaledTime - pointerEventData1.get_clickTime()) < 0.300000011920929)
          {
            PointerEventData pointerEventData2 = pointerEventData1;
            int num = pointerEventData2.get_clickCount() + 1;
            pointerEventData2.set_clickCount(num);
          }
          else
            pointerEventData1.set_clickCount(1);
          pointerEventData1.set_clickTime(unscaledTime);
        }
        else
          pointerEventData1.set_clickCount(1);
        pointerEventData1.set_pointerPress(gameObject2);
        pointerEventData1.set_rawPointerPress(gameObject1);
        pointerEventData1.set_clickTime(unscaledTime);
        pointerEventData1.set_pointerDrag(ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1));
        if (Object.op_Inequality((Object) pointerEventData1.get_pointerDrag(), (Object) null))
          ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEventData1.get_pointerDrag(), (BaseEventData) pointerEventData1, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_initializePotentialDrag());
      }
      if (!data.ReleasedThisFrame())
        return;
      ExecuteEvents.Execute<IPointerUpHandler>(pointerEventData1.get_pointerPress(), (BaseEventData) pointerEventData1, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_pointerUpHandler());
      GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
      if (Object.op_Equality((Object) pointerEventData1.get_pointerPress(), (Object) eventHandler) && pointerEventData1.get_eligibleForClick())
        ExecuteEvents.Execute<IPointerClickHandler>(pointerEventData1.get_pointerPress(), (BaseEventData) pointerEventData1, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_pointerClickHandler());
      else if (Object.op_Inequality((Object) pointerEventData1.get_pointerDrag(), (Object) null))
        ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) pointerEventData1, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_dropHandler());
      pointerEventData1.set_eligibleForClick(false);
      pointerEventData1.set_pointerPress((GameObject) null);
      pointerEventData1.set_rawPointerPress((GameObject) null);
      pointerEventData1.set_dragging(false);
      if (Object.op_Inequality((Object) pointerEventData1.get_pointerDrag(), (Object) null))
        ExecuteEvents.Execute<IEndDragHandler>(pointerEventData1.get_pointerDrag(), (BaseEventData) pointerEventData1, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_endDragHandler());
      pointerEventData1.set_pointerDrag((GameObject) null);
      if (!Object.op_Inequality((Object) gameObject1, (Object) pointerEventData1.get_pointerEnter()))
        return;
      ((BaseInputModule) this).HandlePointerExitAndEnter(pointerEventData1, (GameObject) null);
      ((BaseInputModule) this).HandlePointerExitAndEnter(pointerEventData1, gameObject1);
    }

    private bool SendUpdateEventToSelectedObject()
    {
      if (Object.op_Equality((Object) ((BaseInputModule) this).get_eventSystem().get_currentSelectedGameObject(), (Object) null))
        return false;
      BaseEventData baseEventData = ((BaseInputModule) this).GetBaseEventData();
      ExecuteEvents.Execute<IUpdateSelectedHandler>(((BaseInputModule) this).get_eventSystem().get_currentSelectedGameObject(), baseEventData, (ExecuteEvents.EventFunction<M0>) ExecuteEvents.get_updateSelectedHandler());
      return baseEventData.get_used();
    }

    private bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
    {
      if (this.m_CurrentInputMode == HandsInputModule.InputMode.Mouse)
        return true;
      if (pressed || released || (pointerData.IsPointerMoving() || pointerData.IsScrolling()))
      {
        this.m_CurrentInputMode = HandsInputModule.InputMode.Mouse;
        ((BaseInputModule) this).get_eventSystem().SetSelectedGameObject((GameObject) null);
      }
      return this.m_CurrentInputMode == HandsInputModule.InputMode.Mouse;
    }

    private void ProcessMetaEvent(HandsInputModule.HandPointerData handPointData)
    {
      MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"));
      bool flag = false;
      Vector3 position = ((Component) Camera.get_main()).get_transform().get_position();
      Vector3 vector3 = Vector3.Normalize(Vector3.op_Subtraction(handPointData.m_fingerGO.get_transform().get_position(), ((Component) Camera.get_main()).get_transform().get_position()));
      LayerMask layerMask = LayerMask.op_Implicit(1056);
      RaycastHit raycastHit;
      if (Physics.Raycast(position, vector3, ref raycastHit, float.PositiveInfinity, LayerMask.op_Implicit(layerMask)))
      {
        // ISSUE: explicit reference operation
        if (MetaCamera.IsVisible(((RaycastHit) @raycastHit).get_collider().get_bounds()))
        {
          handPointData.m_pressTimer += Time.get_deltaTime();
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          handPointData.m_scrolling = Object.op_Inequality((Object) ((Component) ((RaycastHit) @raycastHit).get_collider()).GetComponent<Scrollbar>(), (Object) null) || Object.op_Inequality((Object) ((Component) ((RaycastHit) @raycastHit).get_collider()).GetComponent<MGUIScrollRect>(), (Object) null) || (Object.op_Inequality((Object) ((Component) ((RaycastHit) @raycastHit).get_collider()).GetComponent<ScrollRect>(), (Object) null) || Object.op_Inequality((Object) ((Component) ((RaycastHit) @raycastHit).get_collider()).GetComponent<Slider>(), (Object) null));
          // ISSUE: explicit reference operation
          float num1 = Vector3.Dot(((Component) ((RaycastHit) @raycastHit).get_collider()).get_transform().get_right(), Vector3.op_Subtraction(handPointData.m_fingerGO.get_transform().get_position(), ((PointerEventData) handPointData.m_pointerData.buttonData).get_worldPosition()));
          // ISSUE: explicit reference operation
          float num2 = Vector3.Dot(((Component) ((RaycastHit) @raycastHit).get_collider()).get_transform().get_up(), Vector3.op_Subtraction(handPointData.m_fingerGO.get_transform().get_position(), ((PointerEventData) handPointData.m_pointerData.buttonData).get_worldPosition()));
          ((PointerEventData) handPointData.m_pointerData.buttonData).set_delta(new Vector2(num1, num2));
          // ISSUE: variable of the null type
          __Null local = handPointData.m_pointerData.buttonData;
          Vector2 vector2 = Vector2.op_Addition(((PointerEventData) local).get_position(), ((PointerEventData) handPointData.m_pointerData.buttonData).get_delta());
          ((PointerEventData) local).set_position(vector2);
          ((PointerEventData) handPointData.m_pointerData.buttonData).set_worldPosition(handPointData.m_fingerGO.get_transform().get_position());
          float num3 = Vector3.Distance(((Component) Camera.get_main()).get_transform().get_position(), handPointData.m_fingerGO.get_transform().get_position());
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          if (((double) num3 - (double) ((RaycastHit) @raycastHit).get_distance() < (double) this._colliderDepth || handPointData.m_scrolling) && (double) num3 - (double) ((RaycastHit) @raycastHit).get_distance() >= 0.0)
          {
            this.behind = false;
            if (!handPointData.m_invalidPress && ((double) handPointData.m_pressTimer >= 0.5 || handPointData.m_held))
            {
              flag = true;
              handPointData.m_pressed = !handPointData.m_held;
              handPointData.m_held = true;
              if (handPointData.m_pressed)
              {
                ((PointerEventData) handPointData.m_pointerData.buttonData).set_position(Vector2.op_Implicit(handPointData.m_fingerGO.get_transform().get_position()));
                RaycastResult raycastResult = (RaycastResult) null;
                // ISSUE: explicit reference operation
                // ISSUE: explicit reference operation
                ((RaycastResult) @raycastResult).set_gameObject(((Component) ((RaycastHit) @raycastHit).get_collider()).get_gameObject());
                ((PointerEventData) handPointData.m_pointerData.buttonData).set_pointerCurrentRaycast(raycastResult);
              }
            }
          }
          else
          {
            // ISSUE: explicit reference operation
            if ((double) num3 - (double) ((RaycastHit) @raycastHit).get_distance() < 0.0)
            {
              RaycastResult raycastResult = (RaycastResult) null;
              // ISSUE: explicit reference operation
              // ISSUE: explicit reference operation
              ((RaycastResult) @raycastResult).set_gameObject(((Component) ((RaycastHit) @raycastHit).get_collider()).get_gameObject());
              ((PointerEventData) handPointData.m_pointerData.buttonData).set_pointerCurrentRaycast(raycastResult);
              handPointData.m_invalidPress = false;
              // ISSUE: explicit reference operation
              float distance = Vector3.Distance(handPointData.m_fingerGO.get_transform().get_position(), ((RaycastHit) @raycastHit).get_point());
              MetaSingleton<InputIndicators>.Instance.UpdateHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), distance);
              MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), Color.get_green());
            }
            else
            {
              RaycastResult raycastResult = (RaycastResult) null;
              // ISSUE: explicit reference operation
              ((RaycastResult) @raycastResult).set_gameObject((GameObject) null);
              ((PointerEventData) handPointData.m_pointerData.buttonData).set_pointerCurrentRaycast(raycastResult);
              if (!handPointData.m_held)
                handPointData.m_invalidPress = true;
              if (!this.behind)
              {
                MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), new Color(0.0f, 0.0f, 0.0f, 0.0f));
                this.behindTimerStart = Time.get_time();
                this.behind = true;
              }
              else if ((double) Time.get_time() - (double) this.behindTimerStart > (double) this.behindTimerThreshold)
                MetaSingleton<InputIndicators>.Instance.ColorHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), Color.get_red());
              MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"));
              // ISSUE: explicit reference operation
              float distance = Vector3.Distance(handPointData.m_fingerGO.get_transform().get_position(), ((RaycastHit) @raycastHit).get_point());
              MetaSingleton<InputIndicators>.Instance.UpdateHoverIndicator(handPointData.m_fingerGO.get_transform().FindChild("HoverIndicator"), distance);
            }
          }
        }
        else
        {
          handPointData.m_invalidPress = false;
          RaycastResult raycastResult = (RaycastResult) null;
          // ISSUE: explicit reference operation
          ((RaycastResult) @raycastResult).set_gameObject((GameObject) null);
          ((PointerEventData) handPointData.m_pointerData.buttonData).set_pointerCurrentRaycast(raycastResult);
        }
      }
      else
      {
        handPointData.m_invalidPress = false;
        RaycastResult raycastResult = (RaycastResult) null;
        // ISSUE: explicit reference operation
        ((RaycastResult) @raycastResult).set_gameObject((GameObject) null);
        ((PointerEventData) handPointData.m_pointerData.buttonData).set_pointerCurrentRaycast(raycastResult);
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
          HandsInputModule.HandPointerData handPointerData = handPointData;
          RaycastResult pointerCurrentRaycast = ((PointerEventData) handPointData.m_pointerData.buttonData).get_pointerCurrentRaycast();
          // ISSUE: explicit reference operation
          GameObject gameObject = ((RaycastResult) @pointerCurrentRaycast).get_gameObject();
          handPointerData.m_pressedObject = gameObject;
          handPointData.m_pointerData.buttonState = (__Null) 0;
        }
        else if (handPointData.m_released)
        {
          RaycastResult raycastResult = (RaycastResult) null;
          // ISSUE: explicit reference operation
          ((RaycastResult) @raycastResult).set_gameObject(handPointData.m_pressedObject);
          ((PointerEventData) handPointData.m_pointerData.buttonData).set_pointerCurrentRaycast(raycastResult);
          handPointData.m_pointerData.buttonState = (__Null) 1;
        }
        else
          handPointData.m_pointerData.buttonState = (__Null) 3;
        this.ProcessMousePress(handPointData.m_pointerData);
        if (handPointData.m_released)
        {
          handPointData.m_pressTimer = 0.0f;
          handPointData.m_released = false;
        }
      }
      this.ProcessMove((PointerEventData) handPointData.m_pointerData.buttonData);
      ((PointerEventData) handPointData.m_pointerData.buttonData).set_useDragThreshold(false);
      this.ProcessDrag((PointerEventData) handPointData.m_pointerData.buttonData);
    }

    public static void ToggleHand(HandType hand, bool disable)
    {
      HandsInputModule._handDisabled[(int) hand] = disable;
      if (disable)
      {
        MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(Hands.GetHands()[(int) hand].pointer.gameObject.get_transform().FindChild("HoverIndicator"));
        MetaSingleton<InputIndicators>.Instance.DisableHoverIndicator(Hands.GetHands()[(int) hand].pointer.gameObject.get_transform().FindChild("FingertipIndicator"));
      }
      else
      {
        if (!MetaSingleton<InputIndicators>.Instance.fingertipIndicators)
          return;
        ((Renderer) ((Component) Hands.GetHands()[(int) hand].pointer.gameObject.get_transform().FindChild("FingertipIndicator")).GetComponent<Renderer>()).set_enabled(true);
      }
    }

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
        this.m_pointerData.buttonData = (__Null) new PointerEventData(eventSystem);
        ((PointerEventData) this.m_pointerData.buttonData).set_pointerId(pointerId);
      }
    }

    public enum InputMode
    {
      Mouse,
      Buttons,
    }
  }
}
