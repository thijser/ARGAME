// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.MGUIScrollRect
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
  [AddComponentMenu("UI/MGUI Scroll Rect", 33)]
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  [SelectionBase]
  public class MGUIScrollRect : UIBehaviour, IScrollHandler, IDragHandler, IInitializePotentialDragHandler, IEndDragHandler, ICanvasElement, IEventSystemHandler, IBeginDragHandler
  {
    [SerializeField]
    private RectTransform m_Content;
    [SerializeField]
    private bool m_Horizontal;
    [SerializeField]
    private bool m_Vertical;
    [SerializeField]
    private MGUIScrollRect.MovementType m_MovementType;
    [SerializeField]
    private float m_Elasticity;
    [SerializeField]
    private bool m_Inertia;
    [SerializeField]
    private float m_DecelerationRate;
    [SerializeField]
    private float m_ScrollSensitivity;
    [SerializeField]
    private Scrollbar m_HorizontalScrollbar;
    [SerializeField]
    private Scrollbar m_VerticalScrollbar;
    [SerializeField]
    private MGUIScrollRect.ScrollRectMetaEvent m_OnValueChanged;
    private Vector2 m_ContentStartPosition;
    private RectTransform m_ViewRect;
    private Bounds m_ContentBounds;
    private Bounds m_ViewBounds;
    private Vector2 m_Velocity;
    private bool m_Dragging;
    private Vector2 m_PrevPosition;
    private Bounds m_PrevContentBounds;
    private Bounds m_PrevViewBounds;
    [NonSerialized]
    private bool m_HasRebuiltLayout;
    private Vector2 m_PointerStartOffset;
    private readonly Vector3[] m_Corners;

    public RectTransform content
    {
      get
      {
        return this.m_Content;
      }
      set
      {
        this.m_Content = value;
      }
    }

    public bool horizontal
    {
      get
      {
        return this.m_Horizontal;
      }
      set
      {
        this.m_Horizontal = value;
      }
    }

    public bool vertical
    {
      get
      {
        return this.m_Vertical;
      }
      set
      {
        this.m_Vertical = value;
      }
    }

    public MGUIScrollRect.MovementType movementType
    {
      get
      {
        return this.m_MovementType;
      }
      set
      {
        this.m_MovementType = value;
      }
    }

    public float elasticity
    {
      get
      {
        return this.m_Elasticity;
      }
      set
      {
        this.m_Elasticity = value;
      }
    }

    public bool inertia
    {
      get
      {
        return this.m_Inertia;
      }
      set
      {
        this.m_Inertia = value;
      }
    }

    public float decelerationRate
    {
      get
      {
        return this.m_DecelerationRate;
      }
      set
      {
        this.m_DecelerationRate = value;
      }
    }

    public float scrollSensitivity
    {
      get
      {
        return this.m_ScrollSensitivity;
      }
      set
      {
        this.m_ScrollSensitivity = value;
      }
    }

    public Scrollbar horizontalScrollbar
    {
      get
      {
        return this.m_HorizontalScrollbar;
      }
      set
      {
        if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
          ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
        this.m_HorizontalScrollbar = value;
        if (!Object.op_Implicit((Object) this.m_HorizontalScrollbar))
          return;
        ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
      }
    }

    public Scrollbar verticalScrollbar
    {
      get
      {
        return this.m_VerticalScrollbar;
      }
      set
      {
        if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
          ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
        this.m_VerticalScrollbar = value;
        if (!Object.op_Implicit((Object) this.m_VerticalScrollbar))
          return;
        ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
      }
    }

    public MGUIScrollRect.ScrollRectMetaEvent onValueChanged
    {
      get
      {
        return this.m_OnValueChanged;
      }
      set
      {
        this.m_OnValueChanged = value;
      }
    }

    protected RectTransform viewRect
    {
      get
      {
        if (Object.op_Equality((Object) this.m_ViewRect, (Object) null))
          this.m_ViewRect = (RectTransform) ((Component) this).get_transform();
        return this.m_ViewRect;
      }
    }

    public Vector2 velocity
    {
      get
      {
        return this.m_Velocity;
      }
      set
      {
        this.m_Velocity = value;
      }
    }

    public Vector2 normalizedPosition
    {
      get
      {
        return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
      }
      set
      {
        this.SetNormalizedPosition((float) value.x, 0);
        this.SetNormalizedPosition((float) value.y, 1);
      }
    }

    public float horizontalNormalizedPosition
    {
      get
      {
        this.UpdateBounds();
        if (((Bounds) @this.m_ContentBounds).get_size().x <= ((Bounds) @this.m_ViewBounds).get_size().x)
          return ((Bounds) @this.m_ViewBounds).get_min().x <= ((Bounds) @this.m_ContentBounds).get_min().x ? 0.0f : 1f;
        return (float) ((((Bounds) @this.m_ViewBounds).get_min().x - ((Bounds) @this.m_ContentBounds).get_min().x) / (((Bounds) @this.m_ContentBounds).get_size().x - ((Bounds) @this.m_ViewBounds).get_size().x));
      }
      set
      {
        this.SetNormalizedPosition(value, 0);
      }
    }

    public float verticalNormalizedPosition
    {
      get
      {
        this.UpdateBounds();
        if (((Bounds) @this.m_ContentBounds).get_size().y <= ((Bounds) @this.m_ViewBounds).get_size().y)
          return ((Bounds) @this.m_ViewBounds).get_min().y <= ((Bounds) @this.m_ContentBounds).get_min().y ? 0.0f : 1f;
        return (float) ((((Bounds) @this.m_ViewBounds).get_min().y - ((Bounds) @this.m_ContentBounds).get_min().y) / (((Bounds) @this.m_ContentBounds).get_size().y - ((Bounds) @this.m_ViewBounds).get_size().y));
      }
      set
      {
        this.SetNormalizedPosition(value, 1);
      }
    }

    protected MGUIScrollRect()
    {
      base.\u002Ector();
    }

    public virtual void Rebuild(CanvasUpdate executing)
    {
      if (executing != 2)
        return;
      this.UpdateBounds();
      this.UpdateScrollbars(Vector2.get_zero());
      this.UpdatePrevData();
      this.m_HasRebuiltLayout = true;
    }

    protected virtual void OnEnable()
    {
      base.OnEnable();
      if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
      }
      if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
      }
      CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild((ICanvasElement) this);
    }

    protected virtual void OnDisable()
    {
      if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
      }
      if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
      }
      this.m_HasRebuiltLayout = false;
      base.OnDisable();
    }

    public virtual bool IsActive()
    {
      if (base.IsActive())
        return Object.op_Inequality((Object) this.m_Content, (Object) null);
      return false;
    }

    private void EnsureLayoutHasRebuilt()
    {
      if (this.m_HasRebuiltLayout || CanvasUpdateRegistry.IsRebuildingLayout())
        return;
      Canvas.ForceUpdateCanvases();
    }

    public virtual void StopMovement()
    {
      this.m_Velocity = Vector2.get_zero();
    }

    public virtual void OnScroll(PointerEventData data)
    {
      if (!this.IsActive())
        return;
      this.EnsureLayoutHasRebuilt();
      this.UpdateBounds();
      Vector2 scrollDelta = data.get_scrollDelta();
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Vector2& local = @scrollDelta;
      // ISSUE: explicit reference operation
      double num = (^local).y * -1.0;
      // ISSUE: explicit reference operation
      (^local).y = (__Null) num;
      if (this.vertical && !this.horizontal)
      {
        if ((double) Mathf.Abs((float) scrollDelta.x) > (double) Mathf.Abs((float) scrollDelta.y))
          scrollDelta.y = scrollDelta.x;
        scrollDelta.x = (__Null) 0.0;
      }
      if (this.horizontal && !this.vertical)
      {
        if ((double) Mathf.Abs((float) scrollDelta.y) > (double) Mathf.Abs((float) scrollDelta.x))
          scrollDelta.x = scrollDelta.y;
        scrollDelta.y = (__Null) 0.0;
      }
      Vector2 position = Vector2.op_Addition(this.m_Content.get_anchoredPosition(), Vector2.op_Multiply(scrollDelta, this.m_ScrollSensitivity));
      if (this.m_MovementType == MGUIScrollRect.MovementType.Clamped)
        position = Vector2.op_Addition(position, this.CalculateOffset(Vector2.op_Subtraction(position, this.m_Content.get_anchoredPosition())));
      this.SetContentAnchoredPosition(position);
      this.UpdateBounds();
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
      this.m_Velocity = Vector2.get_zero();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
      if (eventData.get_button() != null || !this.IsActive())
        return;
      this.UpdateBounds();
      this.m_PointerStartOffset = Vector2.op_Multiply(eventData.get_position(), 4000f);
      this.m_ContentStartPosition = this.m_Content.get_anchoredPosition();
      this.m_Dragging = true;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
      if (eventData.get_button() != null)
        return;
      this.m_Dragging = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
      if (eventData.get_button() != null || !this.IsActive())
        return;
      Vector2 vector2_1 = Vector2.op_Addition(this.m_ContentStartPosition, Vector2.op_Subtraction(Vector2.op_Multiply(eventData.get_position(), 4000f), this.m_PointerStartOffset));
      Vector2 vector2_2 = this.CalculateOffset(Vector2.op_Subtraction(vector2_1, this.m_Content.get_anchoredPosition()));
      Vector2 position = Vector2.op_Addition(vector2_1, vector2_2);
      if (this.m_MovementType == MGUIScrollRect.MovementType.Elastic)
      {
        if (vector2_2.x != 0.0)
        {
          // ISSUE: explicit reference operation
          position.x = (__Null) (position.x - (double) MGUIScrollRect.RubberDelta((float) vector2_2.x, (float) ((Bounds) @this.m_ViewBounds).get_size().x));
        }
        if (vector2_2.y != 0.0)
        {
          // ISSUE: explicit reference operation
          position.y = (__Null) (position.y - (double) MGUIScrollRect.RubberDelta((float) vector2_2.y, (float) ((Bounds) @this.m_ViewBounds).get_size().y));
        }
      }
      this.SetContentAnchoredPosition(position);
    }

    protected virtual void SetContentAnchoredPosition(Vector2 position)
    {
      if (!this.m_Horizontal)
        position.x = this.m_Content.get_anchoredPosition().x;
      if (!this.m_Vertical)
        position.y = this.m_Content.get_anchoredPosition().y;
      if (!Vector2.op_Inequality(position, this.m_Content.get_anchoredPosition()))
        return;
      this.m_Content.set_anchoredPosition(position);
      this.UpdateBounds();
    }

    protected virtual void LateUpdate()
    {
      if (!Object.op_Implicit((Object) this.m_Content))
        return;
      this.EnsureLayoutHasRebuilt();
      this.UpdateBounds();
      float unscaledDeltaTime = Time.get_unscaledDeltaTime();
      Vector2 offset = this.CalculateOffset(Vector2.get_zero());
      if (!this.m_Dragging && (Vector2.op_Inequality(offset, Vector2.get_zero()) || Vector2.op_Inequality(this.m_Velocity, Vector2.get_zero())))
      {
        Vector2 position = this.m_Content.get_anchoredPosition();
        for (int index = 0; index < 2; ++index)
        {
          // ISSUE: explicit reference operation
          if (this.m_MovementType == MGUIScrollRect.MovementType.Elastic && (double) ((Vector2) @offset).get_Item(index) != 0.0)
          {
            // ISSUE: explicit reference operation
            float num1 = ((Vector2) @this.m_Velocity).get_Item(index);
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            Vector2& local1 = @position;
            int num2 = index;
            Vector2 anchoredPosition1 = this.m_Content.get_anchoredPosition();
            // ISSUE: explicit reference operation
            double num3 = (double) ((Vector2) @anchoredPosition1).get_Item(index);
            Vector2 anchoredPosition2 = this.m_Content.get_anchoredPosition();
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            double num4 = (double) ((Vector2) @anchoredPosition2).get_Item(index) + (double) ((Vector2) @offset).get_Item(index);
            // ISSUE: explicit reference operation
            // ISSUE: variable of a reference type
            float& local2 = @num1;
            double num5 = (double) this.m_Elasticity;
            double num6 = double.PositiveInfinity;
            double num7 = (double) unscaledDeltaTime;
            double num8 = (double) Mathf.SmoothDamp((float) num3, (float) num4, local2, (float) num5, (float) num6, (float) num7);
            ((Vector2) local1).set_Item(num2, (float) num8);
            // ISSUE: explicit reference operation
            ((Vector2) @this.m_Velocity).set_Item(index, num1);
          }
          else if (this.m_Inertia)
          {
            // ISSUE: variable of a reference type
            Vector2& local1;
            int num1;
            // ISSUE: explicit reference operation
            ((Vector2) (local1 = @this.m_Velocity)).set_Item(num1 = index, ((Vector2) local1).get_Item(num1) * Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime));
            // ISSUE: explicit reference operation
            if ((double) Mathf.Abs(((Vector2) @this.m_Velocity).get_Item(index)) < 1.0)
            {
              // ISSUE: explicit reference operation
              ((Vector2) @this.m_Velocity).set_Item(index, 0.0f);
            }
            // ISSUE: variable of a reference type
            Vector2& local2;
            int num2;
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            ((Vector2) (local2 = @position)).set_Item(num2 = index, ((Vector2) local2).get_Item(num2) + ((Vector2) @this.m_Velocity).get_Item(index) * unscaledDeltaTime);
          }
          else
          {
            // ISSUE: explicit reference operation
            ((Vector2) @this.m_Velocity).set_Item(index, 0.0f);
          }
        }
        if (Vector2.op_Inequality(this.m_Velocity, Vector2.get_zero()))
        {
          if (this.m_MovementType == MGUIScrollRect.MovementType.Clamped)
          {
            offset = this.CalculateOffset(Vector2.op_Subtraction(position, this.m_Content.get_anchoredPosition()));
            position = Vector2.op_Addition(position, offset);
          }
          this.SetContentAnchoredPosition(position);
        }
      }
      if (this.m_Dragging && this.m_Inertia)
        this.m_Velocity = Vector2.op_Implicit(Vector3.Lerp(Vector2.op_Implicit(this.m_Velocity), Vector2.op_Implicit(Vector2.op_Division(Vector2.op_Subtraction(this.m_Content.get_anchoredPosition(), this.m_PrevPosition), unscaledDeltaTime)), unscaledDeltaTime * 10f));
      if (!Bounds.op_Inequality(this.m_ViewBounds, this.m_PrevViewBounds) && !Bounds.op_Inequality(this.m_ContentBounds, this.m_PrevContentBounds) && !Vector2.op_Inequality(this.m_Content.get_anchoredPosition(), this.m_PrevPosition))
        return;
      this.UpdateScrollbars(offset);
      this.m_OnValueChanged.Invoke(this.normalizedPosition);
      this.UpdatePrevData();
    }

    private void UpdatePrevData()
    {
      this.m_PrevPosition = !Object.op_Equality((Object) this.m_Content, (Object) null) ? this.m_Content.get_anchoredPosition() : Vector2.get_zero();
      this.m_PrevViewBounds = this.m_ViewBounds;
      this.m_PrevContentBounds = this.m_ContentBounds;
    }

    private void UpdateScrollbars(Vector2 offset)
    {
      if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        this.m_HorizontalScrollbar.set_size(Mathf.Clamp01((float) ((((Bounds) @this.m_ViewBounds).get_size().x - (double) Mathf.Abs((float) offset.x)) / ((Bounds) @this.m_ContentBounds).get_size().x)));
        this.m_HorizontalScrollbar.set_value(this.horizontalNormalizedPosition);
      }
      if (!Object.op_Implicit((Object) this.m_VerticalScrollbar))
        return;
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      this.m_VerticalScrollbar.set_size(Mathf.Clamp01((float) ((((Bounds) @this.m_ViewBounds).get_size().y - (double) Mathf.Abs((float) offset.y)) / ((Bounds) @this.m_ContentBounds).get_size().y)));
      this.m_VerticalScrollbar.set_value(this.verticalNormalizedPosition);
    }

    private void SetHorizontalNormalizedPosition(float value)
    {
      this.SetNormalizedPosition(value, 0);
    }

    private void SetVerticalNormalizedPosition(float value)
    {
      this.SetNormalizedPosition(value, 1);
    }

    private void SetNormalizedPosition(float value, int axis)
    {
      this.EnsureLayoutHasRebuilt();
      this.UpdateBounds();
      // ISSUE: explicit reference operation
      Vector3 size1 = ((Bounds) @this.m_ContentBounds).get_size();
      // ISSUE: explicit reference operation
      double num1 = (double) ((Vector3) @size1).get_Item(axis);
      // ISSUE: explicit reference operation
      Vector3 size2 = ((Bounds) @this.m_ViewBounds).get_size();
      // ISSUE: explicit reference operation
      double num2 = (double) ((Vector3) @size2).get_Item(axis);
      float num3 = (float) (num1 - num2);
      // ISSUE: explicit reference operation
      Vector3 min1 = ((Bounds) @this.m_ViewBounds).get_min();
      // ISSUE: explicit reference operation
      float num4 = ((Vector3) @min1).get_Item(axis) - value * num3;
      Vector3 localPosition1 = ((Transform) this.m_Content).get_localPosition();
      // ISSUE: explicit reference operation
      double num5 = (double) ((Vector3) @localPosition1).get_Item(axis) + (double) num4;
      // ISSUE: explicit reference operation
      Vector3 min2 = ((Bounds) @this.m_ContentBounds).get_min();
      // ISSUE: explicit reference operation
      double num6 = (double) ((Vector3) @min2).get_Item(axis);
      float num7 = (float) (num5 - num6);
      Vector3 localPosition2 = ((Transform) this.m_Content).get_localPosition();
      // ISSUE: explicit reference operation
      if ((double) Mathf.Abs(((Vector3) @localPosition2).get_Item(axis) - num7) <= 0.00999999977648258)
        return;
      // ISSUE: explicit reference operation
      ((Vector3) @localPosition2).set_Item(axis, num7);
      ((Transform) this.m_Content).set_localPosition(localPosition2);
      // ISSUE: explicit reference operation
      ((Vector2) @this.m_Velocity).set_Item(axis, 0.0f);
      this.UpdateBounds();
    }

    private static float RubberDelta(float overStretching, float viewSize)
    {
      return (float) (1.0 - 1.0 / ((double) Mathf.Abs(overStretching) * 0.550000011920929 / (double) viewSize + 1.0)) * viewSize * Mathf.Sign(overStretching);
    }

    private void UpdateBounds()
    {
      Rect rect1 = this.viewRect.get_rect();
      // ISSUE: explicit reference operation
      Vector3 vector3_1 = Vector2.op_Implicit(((Rect) @rect1).get_center());
      Rect rect2 = this.viewRect.get_rect();
      // ISSUE: explicit reference operation
      Vector3 vector3_2 = Vector2.op_Implicit(((Rect) @rect2).get_size());
      this.m_ViewBounds = new Bounds(vector3_1, vector3_2);
      this.m_ContentBounds = this.GetBounds();
      if (Object.op_Equality((Object) this.m_Content, (Object) null))
        return;
      // ISSUE: explicit reference operation
      Vector3 size = ((Bounds) @this.m_ContentBounds).get_size();
      // ISSUE: explicit reference operation
      Vector3 center = ((Bounds) @this.m_ContentBounds).get_center();
      // ISSUE: explicit reference operation
      Vector3 vector3_3 = Vector3.op_Subtraction(((Bounds) @this.m_ViewBounds).get_size(), size);
      if (vector3_3.x > 0.0)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector3& local = @center;
        // ISSUE: explicit reference operation
        double num = (^local).x - vector3_3.x * (this.m_Content.get_pivot().x - 0.5);
        // ISSUE: explicit reference operation
        (^local).x = (__Null) num;
        // ISSUE: explicit reference operation
        size.x = ((Bounds) @this.m_ViewBounds).get_size().x;
      }
      if (vector3_3.y > 0.0)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector3& local = @center;
        // ISSUE: explicit reference operation
        double num = (^local).y - vector3_3.y * (this.m_Content.get_pivot().y - 0.5);
        // ISSUE: explicit reference operation
        (^local).y = (__Null) num;
        // ISSUE: explicit reference operation
        size.y = ((Bounds) @this.m_ViewBounds).get_size().y;
      }
      // ISSUE: explicit reference operation
      ((Bounds) @this.m_ContentBounds).set_size(size);
      // ISSUE: explicit reference operation
      ((Bounds) @this.m_ContentBounds).set_center(center);
    }

    private Bounds GetBounds()
    {
      if (Object.op_Equality((Object) this.m_Content, (Object) null))
        return (Bounds) null;
      Vector3 vector3_1;
      // ISSUE: explicit reference operation
      ((Vector3) @vector3_1).\u002Ector(float.MaxValue, float.MaxValue, float.MaxValue);
      Vector3 vector3_2;
      // ISSUE: explicit reference operation
      ((Vector3) @vector3_2).\u002Ector(float.MinValue, float.MinValue, float.MinValue);
      Matrix4x4 worldToLocalMatrix = ((Transform) this.viewRect).get_worldToLocalMatrix();
      this.m_Content.GetWorldCorners(this.m_Corners);
      for (int index = 0; index < 4; ++index)
      {
        // ISSUE: explicit reference operation
        Vector3 vector3_3 = ((Matrix4x4) @worldToLocalMatrix).MultiplyPoint3x4(this.m_Corners[index]);
        vector3_1 = Vector3.Min(vector3_3, vector3_1);
        vector3_2 = Vector3.Max(vector3_3, vector3_2);
      }
      Bounds bounds;
      // ISSUE: explicit reference operation
      ((Bounds) @bounds).\u002Ector(vector3_1, Vector3.get_zero());
      // ISSUE: explicit reference operation
      ((Bounds) @bounds).Encapsulate(vector3_2);
      return bounds;
    }

    private Vector2 CalculateOffset(Vector2 delta)
    {
      Vector2 zero = Vector2.get_zero();
      if (this.m_MovementType == MGUIScrollRect.MovementType.Unrestricted)
        return zero;
      // ISSUE: explicit reference operation
      Vector2 vector2_1 = Vector2.op_Implicit(((Bounds) @this.m_ContentBounds).get_min());
      // ISSUE: explicit reference operation
      Vector2 vector2_2 = Vector2.op_Implicit(((Bounds) @this.m_ContentBounds).get_max());
      if (this.m_Horizontal)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector2& local1 = @vector2_1;
        // ISSUE: explicit reference operation
        // ISSUE: variable of the null type
        __Null local2 = (^local1).x + delta.x;
        // ISSUE: explicit reference operation
        (^local1).x = local2;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector2& local3 = @vector2_2;
        // ISSUE: explicit reference operation
        // ISSUE: variable of the null type
        __Null local4 = (^local3).x + delta.x;
        // ISSUE: explicit reference operation
        (^local3).x = local4;
        // ISSUE: explicit reference operation
        if (vector2_1.x > ((Bounds) @this.m_ViewBounds).get_min().x)
        {
          // ISSUE: explicit reference operation
          zero.x = ((Bounds) @this.m_ViewBounds).get_min().x - vector2_1.x;
        }
        else
        {
          // ISSUE: explicit reference operation
          if (vector2_2.x < ((Bounds) @this.m_ViewBounds).get_max().x)
          {
            // ISSUE: explicit reference operation
            zero.x = ((Bounds) @this.m_ViewBounds).get_max().x - vector2_2.x;
          }
        }
      }
      if (this.m_Vertical)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector2& local1 = @vector2_1;
        // ISSUE: explicit reference operation
        // ISSUE: variable of the null type
        __Null local2 = (^local1).y + delta.y;
        // ISSUE: explicit reference operation
        (^local1).y = local2;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector2& local3 = @vector2_2;
        // ISSUE: explicit reference operation
        // ISSUE: variable of the null type
        __Null local4 = (^local3).y + delta.y;
        // ISSUE: explicit reference operation
        (^local3).y = local4;
        // ISSUE: explicit reference operation
        if (vector2_2.y < ((Bounds) @this.m_ViewBounds).get_max().y)
        {
          // ISSUE: explicit reference operation
          zero.y = ((Bounds) @this.m_ViewBounds).get_max().y - vector2_2.y;
        }
        else
        {
          // ISSUE: explicit reference operation
          if (vector2_1.y > ((Bounds) @this.m_ViewBounds).get_min().y)
          {
            // ISSUE: explicit reference operation
            zero.y = ((Bounds) @this.m_ViewBounds).get_min().y - vector2_1.y;
          }
        }
      }
      return zero;
    }

    Transform ICanvasElement.get_transform()
    {
      return ((Component) this).get_transform();
    }

    bool ICanvasElement.IsDestroyed()
    {
      return this.IsDestroyed();
    }

    public enum MovementType
    {
      Unrestricted,
      Elastic,
      Clamped,
    }

    [Serializable]
    public class ScrollRectMetaEvent : UnityEvent<Vector2>
    {
      public ScrollRectMetaEvent()
      {
        base.\u002Ector();
      }
    }
  }
}
