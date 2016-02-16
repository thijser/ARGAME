using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/MGUI Scroll Rect", 33), ExecuteInEditMode, RequireComponent(typeof(RectTransform)), SelectionBase]
	public class MGUIScrollRect : UIBehaviour, IScrollHandler, IDragHandler, IInitializePotentialDragHandler, IEndDragHandler, ICanvasElement, IEventSystemHandler, IBeginDragHandler
	{
		public enum MovementType
		{
			Unrestricted,
			Elastic,
			Clamped
		}

		[Serializable]
		public class ScrollRectMetaEvent : UnityEvent<Vector2>
		{
		}

		[SerializeField]
		private RectTransform m_Content;

		[SerializeField]
		private bool m_Horizontal = true;

		[SerializeField]
		private bool m_Vertical = true;

		[SerializeField]
		private MGUIScrollRect.MovementType m_MovementType = MGUIScrollRect.MovementType.Elastic;

		[SerializeField]
		private float m_Elasticity = 0.1f;

		[SerializeField]
		private bool m_Inertia = true;

		[SerializeField]
		private float m_DecelerationRate = 0.135f;

		[SerializeField]
		private float m_ScrollSensitivity = 1f;

		[SerializeField]
		private Scrollbar m_HorizontalScrollbar;

		[SerializeField]
		private Scrollbar m_VerticalScrollbar;

		[SerializeField]
		private MGUIScrollRect.ScrollRectMetaEvent m_OnValueChanged = new MGUIScrollRect.ScrollRectMetaEvent();

		private Vector2 m_ContentStartPosition = Vector2.get_zero();

		private RectTransform m_ViewRect;

		private Bounds m_ContentBounds;

		private Bounds m_ViewBounds;

		private Vector2 m_Velocity;

		private bool m_Dragging;

		private Vector2 m_PrevPosition = Vector2.get_zero();

		private Bounds m_PrevContentBounds;

		private Bounds m_PrevViewBounds;

		[NonSerialized]
		private bool m_HasRebuiltLayout;

		private Vector2 m_PointerStartOffset;

		private readonly Vector3[] m_Corners = new Vector3[4];

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
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.get_onValueChanged().RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
				this.m_HorizontalScrollbar = value;
				if (this.m_HorizontalScrollbar)
				{
					this.m_HorizontalScrollbar.get_onValueChanged().AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
				}
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
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.get_onValueChanged().RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
				this.m_VerticalScrollbar = value;
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.get_onValueChanged().AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
				}
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
				if (this.m_ViewRect == null)
				{
					this.m_ViewRect = (RectTransform)base.get_transform();
				}
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
				this.SetNormalizedPosition(value.x, 0);
				this.SetNormalizedPosition(value.y, 1);
			}
		}

		public float horizontalNormalizedPosition
		{
			get
			{
				this.UpdateBounds();
				if (this.m_ContentBounds.get_size().x <= this.m_ViewBounds.get_size().x)
				{
					return (float)((this.m_ViewBounds.get_min().x <= this.m_ContentBounds.get_min().x) ? 0 : 1);
				}
				return (this.m_ViewBounds.get_min().x - this.m_ContentBounds.get_min().x) / (this.m_ContentBounds.get_size().x - this.m_ViewBounds.get_size().x);
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
				if (this.m_ContentBounds.get_size().y <= this.m_ViewBounds.get_size().y)
				{
					return (float)((this.m_ViewBounds.get_min().y <= this.m_ContentBounds.get_min().y) ? 0 : 1);
				}
				return (this.m_ViewBounds.get_min().y - this.m_ContentBounds.get_min().y) / (this.m_ContentBounds.get_size().y - this.m_ViewBounds.get_size().y);
			}
			set
			{
				this.SetNormalizedPosition(value, 1);
			}
		}

		protected MGUIScrollRect()
		{
		}

		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing != 2)
			{
				return;
			}
			this.UpdateBounds();
			this.UpdateScrollbars(Vector2.get_zero());
			this.UpdatePrevData();
			this.m_HasRebuiltLayout = true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.get_onValueChanged().AddListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.get_onValueChanged().AddListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		protected override void OnDisable()
		{
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.get_onValueChanged().RemoveListener(new UnityAction<float>(this.SetHorizontalNormalizedPosition));
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.get_onValueChanged().RemoveListener(new UnityAction<float>(this.SetVerticalNormalizedPosition));
			}
			this.m_HasRebuiltLayout = false;
			base.OnDisable();
		}

		public override bool IsActive()
		{
			return base.IsActive() && this.m_Content != null;
		}

		private void EnsureLayoutHasRebuilt()
		{
			if (!this.m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
			{
				Canvas.ForceUpdateCanvases();
			}
		}

		public virtual void StopMovement()
		{
			this.m_Velocity = Vector2.get_zero();
		}

		public virtual void OnScroll(PointerEventData data)
		{
			if (!this.IsActive())
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			Vector2 scrollDelta = data.get_scrollDelta();
			scrollDelta.y *= -1f;
			if (this.vertical && !this.horizontal)
			{
				if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				{
					scrollDelta.y = scrollDelta.x;
				}
				scrollDelta.x = 0f;
			}
			if (this.horizontal && !this.vertical)
			{
				if (Mathf.Abs(scrollDelta.y) > Mathf.Abs(scrollDelta.x))
				{
					scrollDelta.x = scrollDelta.y;
				}
				scrollDelta.y = 0f;
			}
			Vector2 vector = this.m_Content.get_anchoredPosition();
			vector += scrollDelta * this.m_ScrollSensitivity;
			if (this.m_MovementType == MGUIScrollRect.MovementType.Clamped)
			{
				vector += this.CalculateOffset(vector - this.m_Content.get_anchoredPosition());
			}
			this.SetContentAnchoredPosition(vector);
			this.UpdateBounds();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			this.m_Velocity = Vector2.get_zero();
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.get_button() != null)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			this.UpdateBounds();
			this.m_PointerStartOffset = eventData.get_position() * 4000f;
			this.m_ContentStartPosition = this.m_Content.get_anchoredPosition();
			this.m_Dragging = true;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.get_button() != null)
			{
				return;
			}
			this.m_Dragging = false;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.get_button() != null)
			{
				return;
			}
			if (!this.IsActive())
			{
				return;
			}
			Vector2 vector = eventData.get_position() * 4000f - this.m_PointerStartOffset;
			Vector2 vector2 = this.m_ContentStartPosition + vector;
			Vector2 vector3 = this.CalculateOffset(vector2 - this.m_Content.get_anchoredPosition());
			vector2 += vector3;
			if (this.m_MovementType == MGUIScrollRect.MovementType.Elastic)
			{
				if (vector3.x != 0f)
				{
					vector2.x -= MGUIScrollRect.RubberDelta(vector3.x, this.m_ViewBounds.get_size().x);
				}
				if (vector3.y != 0f)
				{
					vector2.y -= MGUIScrollRect.RubberDelta(vector3.y, this.m_ViewBounds.get_size().y);
				}
			}
			this.SetContentAnchoredPosition(vector2);
		}

		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!this.m_Horizontal)
			{
				position.x = this.m_Content.get_anchoredPosition().x;
			}
			if (!this.m_Vertical)
			{
				position.y = this.m_Content.get_anchoredPosition().y;
			}
			if (position != this.m_Content.get_anchoredPosition())
			{
				this.m_Content.set_anchoredPosition(position);
				this.UpdateBounds();
			}
		}

		protected virtual void LateUpdate()
		{
			if (!this.m_Content)
			{
				return;
			}
			this.EnsureLayoutHasRebuilt();
			this.UpdateBounds();
			float unscaledDeltaTime = Time.get_unscaledDeltaTime();
			Vector2 vector = this.CalculateOffset(Vector2.get_zero());
			if (!this.m_Dragging && (vector != Vector2.get_zero() || this.m_Velocity != Vector2.get_zero()))
			{
				Vector2 vector2 = this.m_Content.get_anchoredPosition();
				for (int i = 0; i < 2; i++)
				{
					if (this.m_MovementType == MGUIScrollRect.MovementType.Elastic && vector.get_Item(i) != 0f)
					{
						float num = this.m_Velocity.get_Item(i);
						vector2.set_Item(i, Mathf.SmoothDamp(this.m_Content.get_anchoredPosition().get_Item(i), this.m_Content.get_anchoredPosition().get_Item(i) + vector.get_Item(i), ref num, this.m_Elasticity, float.PositiveInfinity, unscaledDeltaTime));
						this.m_Velocity.set_Item(i, num);
					}
					else if (this.m_Inertia)
					{
						int num2;
						int expr_114 = num2 = i;
						float num3 = this.m_Velocity.get_Item(num2);
						this.m_Velocity.set_Item(expr_114, num3 * Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime));
						if (Mathf.Abs(this.m_Velocity.get_Item(i)) < 1f)
						{
							this.m_Velocity.set_Item(i, 0f);
						}
						int expr_168 = num2 = i;
						num3 = vector2.get_Item(num2);
						vector2.set_Item(expr_168, num3 + this.m_Velocity.get_Item(i) * unscaledDeltaTime);
					}
					else
					{
						this.m_Velocity.set_Item(i, 0f);
					}
				}
				if (this.m_Velocity != Vector2.get_zero())
				{
					if (this.m_MovementType == MGUIScrollRect.MovementType.Clamped)
					{
						vector = this.CalculateOffset(vector2 - this.m_Content.get_anchoredPosition());
						vector2 += vector;
					}
					this.SetContentAnchoredPosition(vector2);
				}
			}
			if (this.m_Dragging && this.m_Inertia)
			{
				Vector3 vector3 = (this.m_Content.get_anchoredPosition() - this.m_PrevPosition) / unscaledDeltaTime;
				this.m_Velocity = Vector3.Lerp(this.m_Velocity, vector3, unscaledDeltaTime * 10f);
			}
			if (this.m_ViewBounds != this.m_PrevViewBounds || this.m_ContentBounds != this.m_PrevContentBounds || this.m_Content.get_anchoredPosition() != this.m_PrevPosition)
			{
				this.UpdateScrollbars(vector);
				this.m_OnValueChanged.Invoke(this.normalizedPosition);
				this.UpdatePrevData();
			}
		}

		private void UpdatePrevData()
		{
			if (this.m_Content == null)
			{
				this.m_PrevPosition = Vector2.get_zero();
			}
			else
			{
				this.m_PrevPosition = this.m_Content.get_anchoredPosition();
			}
			this.m_PrevViewBounds = this.m_ViewBounds;
			this.m_PrevContentBounds = this.m_ContentBounds;
		}

		private void UpdateScrollbars(Vector2 offset)
		{
			if (this.m_HorizontalScrollbar)
			{
				this.m_HorizontalScrollbar.set_size(Mathf.Clamp01((this.m_ViewBounds.get_size().x - Mathf.Abs(offset.x)) / this.m_ContentBounds.get_size().x));
				this.m_HorizontalScrollbar.set_value(this.horizontalNormalizedPosition);
			}
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.set_size(Mathf.Clamp01((this.m_ViewBounds.get_size().y - Mathf.Abs(offset.y)) / this.m_ContentBounds.get_size().y));
				this.m_VerticalScrollbar.set_value(this.verticalNormalizedPosition);
			}
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
			float num = this.m_ContentBounds.get_size().get_Item(axis) - this.m_ViewBounds.get_size().get_Item(axis);
			float num2 = this.m_ViewBounds.get_min().get_Item(axis) - value * num;
			float num3 = this.m_Content.get_localPosition().get_Item(axis) + num2 - this.m_ContentBounds.get_min().get_Item(axis);
			Vector3 localPosition = this.m_Content.get_localPosition();
			if (Mathf.Abs(localPosition.get_Item(axis) - num3) > 0.01f)
			{
				localPosition.set_Item(axis, num3);
				this.m_Content.set_localPosition(localPosition);
				this.m_Velocity.set_Item(axis, 0f);
				this.UpdateBounds();
			}
		}

		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1f - 1f / (Mathf.Abs(overStretching) * 0.55f / viewSize + 1f)) * viewSize * Mathf.Sign(overStretching);
		}

		private void UpdateBounds()
		{
			this.m_ViewBounds = new Bounds(this.viewRect.get_rect().get_center(), this.viewRect.get_rect().get_size());
			this.m_ContentBounds = this.GetBounds();
			if (this.m_Content == null)
			{
				return;
			}
			Vector3 size = this.m_ContentBounds.get_size();
			Vector3 center = this.m_ContentBounds.get_center();
			Vector3 vector = this.m_ViewBounds.get_size() - size;
			if (vector.x > 0f)
			{
				center.x -= vector.x * (this.m_Content.get_pivot().x - 0.5f);
				size.x = this.m_ViewBounds.get_size().x;
			}
			if (vector.y > 0f)
			{
				center.y -= vector.y * (this.m_Content.get_pivot().y - 0.5f);
				size.y = this.m_ViewBounds.get_size().y;
			}
			this.m_ContentBounds.set_size(size);
			this.m_ContentBounds.set_center(center);
		}

		private Bounds GetBounds()
		{
			if (this.m_Content == null)
			{
				return default(Bounds);
			}
			Vector3 vector = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
			Vector3 vector2 = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
			Matrix4x4 worldToLocalMatrix = this.viewRect.get_worldToLocalMatrix();
			this.m_Content.GetWorldCorners(this.m_Corners);
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector3 = worldToLocalMatrix.MultiplyPoint3x4(this.m_Corners[i]);
				vector = Vector3.Min(vector3, vector);
				vector2 = Vector3.Max(vector3, vector2);
			}
			Bounds result = new Bounds(vector, Vector3.get_zero());
			result.Encapsulate(vector2);
			return result;
		}

		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 zero = Vector2.get_zero();
			if (this.m_MovementType == MGUIScrollRect.MovementType.Unrestricted)
			{
				return zero;
			}
			Vector2 vector = this.m_ContentBounds.get_min();
			Vector2 vector2 = this.m_ContentBounds.get_max();
			if (this.m_Horizontal)
			{
				vector.x += delta.x;
				vector2.x += delta.x;
				if (vector.x > this.m_ViewBounds.get_min().x)
				{
					zero.x = this.m_ViewBounds.get_min().x - vector.x;
				}
				else if (vector2.x < this.m_ViewBounds.get_max().x)
				{
					zero.x = this.m_ViewBounds.get_max().x - vector2.x;
				}
			}
			if (this.m_Vertical)
			{
				vector.y += delta.y;
				vector2.y += delta.y;
				if (vector2.y < this.m_ViewBounds.get_max().y)
				{
					zero.y = this.m_ViewBounds.get_max().y - vector2.y;
				}
				else if (vector.y > this.m_ViewBounds.get_min().y)
				{
					zero.y = this.m_ViewBounds.get_min().y - vector.y;
				}
			}
			return zero;
		}

		virtual Transform get_transform()
		{
			return base.get_transform();
		}

		virtual bool IsDestroyed()
		{
			return base.IsDestroyed();
		}
	}
}
