// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.MGUIScrollRectAdder
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace UnityEngine.UI
{
  public class MGUIScrollRectAdder : MonoBehaviour
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

    public MGUIScrollRectAdder()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      if (!Object.op_Equality((Object) ((Component) this).get_gameObject().GetComponent<MGUIScrollRect>(), (Object) null))
        return;
      MGUIScrollRect mguiScrollRect = (MGUIScrollRect) ((Component) this).get_gameObject().AddComponent<MGUIScrollRect>();
      mguiScrollRect.content = this.m_Content;
      mguiScrollRect.horizontal = this.m_Horizontal;
      mguiScrollRect.vertical = this.m_Vertical;
      mguiScrollRect.movementType = this.m_MovementType;
      mguiScrollRect.elasticity = this.m_Elasticity;
      mguiScrollRect.inertia = this.m_Inertia;
      mguiScrollRect.decelerationRate = this.m_DecelerationRate;
      mguiScrollRect.scrollSensitivity = this.m_ScrollSensitivity;
      mguiScrollRect.horizontalScrollbar = this.m_HorizontalScrollbar;
      mguiScrollRect.verticalScrollbar = this.m_VerticalScrollbar;
      mguiScrollRect.onValueChanged = this.m_OnValueChanged;
      ((Object) this).set_hideFlags((HideFlags) 2);
    }
  }
}
