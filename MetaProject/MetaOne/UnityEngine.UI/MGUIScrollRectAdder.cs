using System;

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

		private void Start()
		{
			if (base.get_gameObject().GetComponent<MGUIScrollRect>() == null)
			{
				MGUIScrollRect mGUIScrollRect = base.get_gameObject().AddComponent<MGUIScrollRect>();
				mGUIScrollRect.content = this.m_Content;
				mGUIScrollRect.horizontal = this.m_Horizontal;
				mGUIScrollRect.vertical = this.m_Vertical;
				mGUIScrollRect.movementType = this.m_MovementType;
				mGUIScrollRect.elasticity = this.m_Elasticity;
				mGUIScrollRect.inertia = this.m_Inertia;
				mGUIScrollRect.decelerationRate = this.m_DecelerationRate;
				mGUIScrollRect.scrollSensitivity = this.m_ScrollSensitivity;
				mGUIScrollRect.horizontalScrollbar = this.m_HorizontalScrollbar;
				mGUIScrollRect.verticalScrollbar = this.m_VerticalScrollbar;
				mGUIScrollRect.onValueChanged = this.m_OnValueChanged;
				base.set_hideFlags(2);
			}
		}
	}
}
