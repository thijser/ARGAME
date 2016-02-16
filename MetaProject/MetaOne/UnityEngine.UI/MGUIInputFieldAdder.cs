using System;

namespace UnityEngine.UI
{
	public class MGUIInputFieldAdder : MonoBehaviour
	{
		[SerializeField]
		private Text m_TextComponent;

		[SerializeField]
		private string m_Text;

		[SerializeField]
		private MGUIInputField.ContentType m_ContentType;

		[SerializeField]
		private MGUIInputField.LineType m_LineType;

		[SerializeField]
		private MGUIInputField.InputType m_InputType;

		[SerializeField]
		private MGUIInputField.CharacterValidation m_CharacterValidation;

		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		[SerializeField]
		private int m_CharacterLimit;

		[SerializeField]
		private float m_CaretBlinkRate;

		[SerializeField]
		private Color m_SelectionColor;

		[SerializeField]
		private bool m_HideMobileInput;

		[SerializeField]
		private Graphic m_Placeholder;

		[SerializeField]
		private MGUIInputField.OnChangeEvent m_OnValueChange;

		[SerializeField]
		private MGUIInputField.SubmitEvent m_EndEdit;

		private void Start()
		{
			if (base.get_gameObject().GetComponent<MGUIInputField>() == null)
			{
				MGUIInputField mGUIInputField = base.get_gameObject().AddComponent<MGUIInputField>();
				mGUIInputField.set_transition(3);
				mGUIInputField.textComponent = this.m_TextComponent;
				mGUIInputField.text = this.m_Text;
				mGUIInputField.contentType = this.m_ContentType;
				mGUIInputField.lineType = this.m_LineType;
				mGUIInputField.inputType = this.m_InputType;
				mGUIInputField.characterValidation = this.m_CharacterValidation;
				mGUIInputField.keyboardType = this.m_KeyboardType;
				mGUIInputField.characterLimit = this.m_CharacterLimit;
				mGUIInputField.caretBlinkRate = this.m_CaretBlinkRate;
				mGUIInputField.selectionColor = this.m_SelectionColor;
				mGUIInputField.shouldHideMobileInput = this.m_HideMobileInput;
				mGUIInputField.placeholder = this.m_Placeholder;
				mGUIInputField.onValueChange = this.m_OnValueChange;
				mGUIInputField.onEndEdit = this.m_EndEdit;
				base.set_hideFlags(2);
			}
		}
	}
}
