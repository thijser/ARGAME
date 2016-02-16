// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.MGUIInputFieldAdder
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

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

    public MGUIInputFieldAdder()
    {
      base.\u002Ector();
    }

    private void Start()
    {
      if (!Object.op_Equality((Object) ((Component) this).get_gameObject().GetComponent<MGUIInputField>(), (Object) null))
        return;
      MGUIInputField mguiInputField = (MGUIInputField) ((Component) this).get_gameObject().AddComponent<MGUIInputField>();
      mguiInputField.set_transition((Selectable.Transition) 3);
      mguiInputField.textComponent = this.m_TextComponent;
      mguiInputField.text = this.m_Text;
      mguiInputField.contentType = this.m_ContentType;
      mguiInputField.lineType = this.m_LineType;
      mguiInputField.inputType = this.m_InputType;
      mguiInputField.characterValidation = this.m_CharacterValidation;
      mguiInputField.keyboardType = this.m_KeyboardType;
      mguiInputField.characterLimit = this.m_CharacterLimit;
      mguiInputField.caretBlinkRate = this.m_CaretBlinkRate;
      mguiInputField.selectionColor = this.m_SelectionColor;
      mguiInputField.shouldHideMobileInput = this.m_HideMobileInput;
      mguiInputField.placeholder = this.m_Placeholder;
      mguiInputField.onValueChange = this.m_OnValueChange;
      mguiInputField.onEndEdit = this.m_EndEdit;
      ((Object) this).set_hideFlags((HideFlags) 2);
    }
  }
}
