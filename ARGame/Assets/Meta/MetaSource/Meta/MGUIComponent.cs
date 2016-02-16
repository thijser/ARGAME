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
			if (!Application.isPlaying)
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
			if (pointerEvent.pointerId == -1 && (base.transform.GetComponent<Scrollbar>() != null || base.transform.GetComponent<ScrollRect>() != null || base.transform.GetComponent<Slider>() != null))
			{
				this.InstantiatePressIndicator(pointerEvent);
				if (this._enablePressSound)
				{
					MetaSingleton<InputIndicators>.Instance.InstantiatePressSound(base.transform.position, this._pressSound);
				}
			}
		}

		public void OnPointerUp(PointerEventData pointerEvent)
		{
			if (pointerEvent.pointerId == -1)
			{
				this.InstantiatePressIndicator(pointerEvent);
				if (this._enablePressSound)
				{
					MetaSingleton<InputIndicators>.Instance.InstantiatePressSound(base.transform.position, this._pressSound);
				}
			}
		}

		private void InstantiatePressIndicator(PointerEventData pointerEvent)
		{
			if (pointerEvent.pointerId == 0 || pointerEvent.pointerId == 1)
			{
				Quaternion rotation = Hands.GetHands()[pointerEvent.pointerId].pointer.gameObject.transform.FindChild("FingertipIndicator").rotation;
				MetaSingleton<InputIndicators>.Instance.InstantiatePressIndicator(pointerEvent.worldPosition, rotation);
			}
		}

		private void ResizeCollider()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			BoxCollider component2 = base.GetComponent<BoxCollider>();
			if (component2 != null && component != null && component2.size != new Vector3(component.rect.width, component.rect.height, 1f))
			{
				component2.size = new Vector3(component.rect.width, component.rect.height, 1f);
				component2.center = Vector3.zero;
			}
		}

		public void SetParent()
		{
			if (base.transform.parent == null)
			{
				Transform transform = this.FindNonMetaUICanvas();
				if (transform == null && MetaSingleton<MetaUI>.Instance != null)
				{
					transform = ((GameObject)UnityEngine.Object.Instantiate(MetaSingleton<MetaUI>.Instance.mguiCanvas, new Vector3(0f, 0f, 0.4f), Quaternion.identity)).transform;
					transform.name = "MGUI.Canvas";
				}
				if (transform != null)
				{
					base.transform.SetParent(transform);
					base.transform.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
					base.transform.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
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
			Canvas[] array = UnityEngine.Object.FindObjectsOfType<Canvas>();
			Canvas[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Canvas canvas = array2[i];
				if (this.RecursiveParentIsNotMetaUI(canvas.transform))
				{
					return canvas.transform;
				}
			}
			return null;
		}

		private bool RecursiveParentIsNotMetaUI(Transform obj)
		{
			return !(obj.GetComponent<MetaUI>() != null) && (!(obj.parent != null) || this.RecursiveParentIsNotMetaUI(obj.parent));
		}
	}
}
