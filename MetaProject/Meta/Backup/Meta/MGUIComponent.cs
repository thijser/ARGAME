// Decompiled with JetBrains decompiler
// Type: Meta.MGUIComponent
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta
{
  [ExecuteInEditMode]
  public class MGUIComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
  {
    [SerializeField]
    private bool _enablePressSound;
    [SerializeField]
    private AudioClip _pressSound;
    [SerializeField]
    private bool _autoResizeCollider;
    private bool _parentSet;

    public MGUIComponent()
    {
      base.\u002Ector();
    }

    private void Start()
    {
    }

    private void Update()
    {
      if (Application.get_isPlaying())
        return;
      if (this._autoResizeCollider)
        this.ResizeCollider();
      if (this._parentSet)
        return;
      this.SetParent();
    }

    public void OnPointerDown(PointerEventData pointerEvent)
    {
      if (pointerEvent.get_pointerId() != -1 || !Object.op_Inequality((Object) ((Component) ((Component) this).get_transform()).GetComponent<Scrollbar>(), (Object) null) && !Object.op_Inequality((Object) ((Component) ((Component) this).get_transform()).GetComponent<MGUIScrollRect>(), (Object) null) && (!Object.op_Inequality((Object) ((Component) ((Component) this).get_transform()).GetComponent<ScrollRect>(), (Object) null) && !Object.op_Inequality((Object) ((Component) ((Component) this).get_transform()).GetComponent<Slider>(), (Object) null)))
        return;
      this.InstantiatePressIndicator(pointerEvent);
      if (!this._enablePressSound)
        return;
      MetaSingleton<InputIndicators>.Instance.InstantiatePressSound(((Component) this).get_transform().get_position(), this._pressSound);
    }

    public void OnPointerUp(PointerEventData pointerEvent)
    {
      if (pointerEvent.get_pointerId() != -1)
        return;
      this.InstantiatePressIndicator(pointerEvent);
      if (!this._enablePressSound)
        return;
      MetaSingleton<InputIndicators>.Instance.InstantiatePressSound(((Component) this).get_transform().get_position(), this._pressSound);
    }

    private void InstantiatePressIndicator(PointerEventData pointerEvent)
    {
      if (pointerEvent.get_pointerId() != 0 && pointerEvent.get_pointerId() != 1)
        return;
      Quaternion rotation = Hands.GetHands()[pointerEvent.get_pointerId()].pointer.gameObject.get_transform().FindChild("FingertipIndicator").get_rotation();
      MetaSingleton<InputIndicators>.Instance.InstantiatePressIndicator(pointerEvent.get_worldPosition(), rotation);
    }

    private void ResizeCollider()
    {
      RectTransform rectTransform = (RectTransform) ((Component) this).GetComponent<RectTransform>();
      BoxCollider boxCollider1 = (BoxCollider) ((Component) this).GetComponent<BoxCollider>();
      if (!Object.op_Inequality((Object) boxCollider1, (Object) null) || !Object.op_Inequality((Object) rectTransform, (Object) null))
        return;
      Vector3 size = boxCollider1.get_size();
      Rect rect1 = rectTransform.get_rect();
      // ISSUE: explicit reference operation
      double num1 = (double) ((Rect) @rect1).get_width();
      Rect rect2 = rectTransform.get_rect();
      // ISSUE: explicit reference operation
      double num2 = (double) ((Rect) @rect2).get_height();
      double num3 = 1.0;
      Vector3 vector3_1 = new Vector3((float) num1, (float) num2, (float) num3);
      if (!Vector3.op_Inequality(size, vector3_1))
        return;
      BoxCollider boxCollider2 = boxCollider1;
      Rect rect3 = rectTransform.get_rect();
      // ISSUE: explicit reference operation
      double num4 = (double) ((Rect) @rect3).get_width();
      Rect rect4 = rectTransform.get_rect();
      // ISSUE: explicit reference operation
      double num5 = (double) ((Rect) @rect4).get_height();
      double num6 = 1.0;
      Vector3 vector3_2 = new Vector3((float) num4, (float) num5, (float) num6);
      boxCollider2.set_size(vector3_2);
      boxCollider1.set_center(Vector3.get_zero());
    }

    public void SetParent()
    {
      if (Object.op_Equality((Object) ((Component) this).get_transform().get_parent(), (Object) null))
      {
        Transform transform = this.FindNonMetaUICanvas();
        if (Object.op_Equality((Object) transform, (Object) null) && Object.op_Inequality((Object) MetaSingleton<MetaUI>.Instance, (Object) null))
        {
          transform = ((GameObject) Object.Instantiate((Object) MetaSingleton<MetaUI>.Instance.mguiCanvas, new Vector3(0.0f, 0.0f, 0.4f), Quaternion.get_identity())).get_transform();
          ((Object) transform).set_name("MGUI.Canvas");
        }
        if (!Object.op_Inequality((Object) transform, (Object) null))
          return;
        ((Component) this).get_transform().SetParent(transform);
        ((Transform) ((Component) ((Component) this).get_transform()).GetComponent<RectTransform>()).set_localScale(new Vector3(1f, 1f, 1f));
        ((Transform) ((Component) ((Component) this).get_transform()).GetComponent<RectTransform>()).set_localPosition(new Vector3(0.0f, 0.0f, 0.0f));
        this._parentSet = true;
      }
      else
        this._parentSet = true;
    }

    private Transform FindNonMetaUICanvas()
    {
      foreach (Canvas canvas in (Canvas[]) Object.FindObjectsOfType<Canvas>())
      {
        if (this.RecursiveParentIsNotMetaUI(((Component) canvas).get_transform()))
          return ((Component) canvas).get_transform();
      }
      return (Transform) null;
    }

    private bool RecursiveParentIsNotMetaUI(Transform obj)
    {
      if (Object.op_Inequality((Object) ((Component) obj).GetComponent<MetaUI>(), (Object) null))
        return false;
      if (Object.op_Inequality((Object) obj.get_parent(), (Object) null))
        return this.RecursiveParentIsNotMetaUI(obj.get_parent());
      return true;
    }
  }
}
