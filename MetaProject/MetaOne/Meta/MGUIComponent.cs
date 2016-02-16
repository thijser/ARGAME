using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta
{
	[ExecuteInEditMode]
	public class MGUIComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		[SerializeField]
		private bool _enablePressSound = true;

		[SerializeField]
		private AudioClip _pressSound;

		[SerializeField]
		private bool _autoResizeCollider = true;

		private bool _parentSet;

		private void Start()
		{
		}

		private void Update()
		{
			if (!Application.get_isPlaying())
			{
				if (this._autoResizeCollider)
				{
					this.ResizeCollider();
				}
				if (!this._parentSet)
				{
					this.SetParent();
				}
			}
		}

		public void OnPointerDown(PointerEventData pointerEvent)
		{
			if (pointerEvent.get_pointerId() == -1 && (base.get_transform().GetComponent<Scrollbar>() != null || base.get_transform().GetComponent<MGUIScrollRect>() != null || base.get_transform().GetComponent<ScrollRect>() != null || base.get_transform().GetComponent<Slider>() != null))
			{
				this.InstantiatePressIndicator(pointerEvent);
				if (this._enablePressSound)
				{
					MetaSingleton<InputIndicators>.Instance.InstantiatePressSound(base.get_transform().get_position(), this._pressSound);
				}
			}
		}

		public void OnPointerUp(PointerEventData pointerEvent)
		{
			if (pointerEvent.get_pointerId() == -1)
			{
				this.InstantiatePressIndicator(pointerEvent);
				if (this._enablePressSound)
				{
					MetaSingleton<InputIndicators>.Instance.InstantiatePressSound(base.get_transform().get_position(), this._pressSound);
				}
			}
		}

		private void InstantiatePressIndicator(PointerEventData pointerEvent)
		{
			if (pointerEvent.get_pointerId() == 0 || pointerEvent.get_pointerId() == 1)
			{
				Quaternion rotation = Hands.GetHands()[pointerEvent.get_pointerId()].pointer.gameObject.get_transform().FindChild("FingertipIndicator").get_rotation();
				MetaSingleton<InputIndicators>.Instance.InstantiatePressIndicator(pointerEvent.get_worldPosition(), rotation);
			}
		}

		private void ResizeCollider()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			BoxCollider component2 = base.GetComponent<BoxCollider>();
			if (component2 != null && component != null && component2.get_size() != new Vector3(component.get_rect().get_width(), component.get_rect().get_height(), 1f))
			{
				component2.set_size(new Vector3(component.get_rect().get_width(), component.get_rect().get_height(), 1f));
				component2.set_center(Vector3.get_zero());
			}
		}

		public void SetParent()
		{
			if (base.get_transform().get_parent() == null)
			{
				Transform transform = this.FindNonMetaUICanvas();
				if (transform == null && MetaSingleton<MetaUI>.Instance != null)
				{
					transform = ((GameObject)Object.Instantiate(MetaSingleton<MetaUI>.Instance.mguiCanvas, new Vector3(0f, 0f, 0.4f), Quaternion.get_identity())).get_transform();
					transform.set_name("MGUI.Canvas");
				}
				if (transform != null)
				{
					base.get_transform().SetParent(transform);
					base.get_transform().GetComponent<RectTransform>().set_localScale(new Vector3(1f, 1f, 1f));
					base.get_transform().GetComponent<RectTransform>().set_localPosition(new Vector3(0f, 0f, 0f));
					this._parentSet = true;
				}
			}
			else
			{
				this._parentSet = true;
			}
		}

		private Transform FindNonMetaUICanvas()
		{
			Canvas[] array = Object.FindObjectsOfType<Canvas>();
			Canvas[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Canvas canvas = array2[i];
				if (this.RecursiveParentIsNotMetaUI(canvas.get_transform()))
				{
					return canvas.get_transform();
				}
			}
			return null;
		}

		private bool RecursiveParentIsNotMetaUI(Transform obj)
		{
			return !(obj.GetComponent<MetaUI>() != null) && (!(obj.get_parent() != null) || this.RecursiveParentIsNotMetaUI(obj.get_parent()));
		}
	}
}
