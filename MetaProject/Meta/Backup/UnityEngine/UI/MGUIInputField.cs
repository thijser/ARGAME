// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.MGUIInputField
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
  [AddComponentMenu("UI/MGUI Input Field", 31)]
  public class MGUIInputField : Selectable, IPointerClickHandler, IDragHandler, IEndDragHandler, IUpdateSelectedHandler, ICanvasElement, IEventSystemHandler, IBeginDragHandler, ISubmitHandler
  {
    private static readonly char[] kSeparators = new char[3]
    {
      ' ',
      '.',
      ','
    };
    private const float kHScrollSpeed = 0.05f;
    private const float kVScrollSpeed = 0.1f;
    private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";
    protected static TouchScreenKeyboard m_Keyboard;
    [FormerlySerializedAs("text")]
    [SerializeField]
    protected Text m_TextComponent;
    [SerializeField]
    protected Graphic m_Placeholder;
    [SerializeField]
    private MGUIInputField.ContentType m_ContentType;
    [SerializeField]
    [FormerlySerializedAs("inputType")]
    private MGUIInputField.InputType m_InputType;
    [FormerlySerializedAs("asteriskChar")]
    [SerializeField]
    private char m_AsteriskChar;
    [SerializeField]
    [FormerlySerializedAs("keyboardType")]
    private TouchScreenKeyboardType m_KeyboardType;
    [SerializeField]
    private MGUIInputField.LineType m_LineType;
    [SerializeField]
    [FormerlySerializedAs("hideMobileInput")]
    private bool m_HideMobileInput;
    [SerializeField]
    [FormerlySerializedAs("validation")]
    private MGUIInputField.CharacterValidation m_CharacterValidation;
    [SerializeField]
    [FormerlySerializedAs("characterLimit")]
    private int m_CharacterLimit;
    [FormerlySerializedAs("m_OnSubmit")]
    [SerializeField]
    [FormerlySerializedAs("onSubmit")]
    private MGUIInputField.SubmitEvent m_EndEdit;
    [FormerlySerializedAs("onValueChange")]
    [SerializeField]
    private MGUIInputField.OnChangeEvent m_OnValueChange;
    [FormerlySerializedAs("onValidateInput")]
    [SerializeField]
    private MGUIInputField.OnValidateInput m_OnValidateInput;
    [SerializeField]
    [FormerlySerializedAs("selectionColor")]
    private Color m_SelectionColor;
    [FormerlySerializedAs("mValue")]
    [SerializeField]
    protected string m_Text;
    [Range(0.0f, 8f)]
    [SerializeField]
    private float m_CaretBlinkRate;
    protected int m_CaretPosition;
    protected int m_CaretSelectPosition;
    private RectTransform caretRectTrans;
    protected UIVertex[] m_CursorVerts;
    private TextGenerator m_InputTextCache;
    private CanvasRenderer m_CachedInputRenderer;
    private readonly List<UIVertex> m_Vbo;
    private bool m_AllowInput;
    private bool m_ShouldActivateNextUpdate;
    private bool m_UpdateDrag;
    private bool m_DragPositionOutOfBounds;
    protected bool m_CaretVisible;
    private Coroutine m_BlickCoroutine;
    private float m_BlinkStartTime;
    protected int m_DrawStart;
    protected int m_DrawEnd;
    private Coroutine m_DragCoroutine;
    private string m_OriginalText;
    private bool m_WasCanceled;
    private bool m_HasDoneFocusTransition;
    private Event m_ProcessingEvent;

    protected TextGenerator cachedInputTextGenerator
    {
      get
      {
        if (this.m_InputTextCache == null)
          this.m_InputTextCache = new TextGenerator();
        return this.m_InputTextCache;
      }
    }

    public bool shouldHideMobileInput
    {
      get
      {
        RuntimePlatform platform = Application.get_platform();
        switch (platform - 8)
        {
          case 0:
          case 3:
            return this.m_HideMobileInput;
          default:
            if (platform != 22)
              return true;
            goto case 0;
        }
      }
      set
      {
        SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
      }
    }

    public string text
    {
      get
      {
        if (MGUIInputField.m_Keyboard != null && MGUIInputField.m_Keyboard.get_active() && !this.InPlaceEditing())
          return MGUIInputField.m_Keyboard.get_text();
        return this.m_Text;
      }
      set
      {
        if (this.text == value)
          return;
        this.m_Text = value;
        if (MGUIInputField.m_Keyboard != null)
          MGUIInputField.m_Keyboard.set_text(this.text);
        if (this.m_CaretPosition > this.m_Text.Length)
          this.m_CaretPosition = this.m_CaretSelectPosition = this.m_Text.Length;
        this.SendOnValueChangedAndUpdateLabel();
      }
    }

    public bool isFocused
    {
      get
      {
        return this.m_AllowInput;
      }
    }

    public float caretBlinkRate
    {
      get
      {
        return this.m_CaretBlinkRate;
      }
      set
      {
        if (!SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) || !this.m_AllowInput)
          return;
        this.SetCaretActive();
      }
    }

    public Text textComponent
    {
      get
      {
        return this.m_TextComponent;
      }
      set
      {
        SetPropertyUtility.SetClass<Text>(ref this.m_TextComponent, value);
      }
    }

    public Graphic placeholder
    {
      get
      {
        return this.m_Placeholder;
      }
      set
      {
        SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
      }
    }

    public Color selectionColor
    {
      get
      {
        return this.m_SelectionColor;
      }
      set
      {
        SetPropertyUtility.SetColor(ref this.m_SelectionColor, value);
      }
    }

    public MGUIInputField.SubmitEvent onEndEdit
    {
      get
      {
        return this.m_EndEdit;
      }
      set
      {
        SetPropertyUtility.SetClass<MGUIInputField.SubmitEvent>(ref this.m_EndEdit, value);
      }
    }

    public MGUIInputField.OnChangeEvent onValueChange
    {
      get
      {
        return this.m_OnValueChange;
      }
      set
      {
        SetPropertyUtility.SetClass<MGUIInputField.OnChangeEvent>(ref this.m_OnValueChange, value);
      }
    }

    public MGUIInputField.OnValidateInput onValidateInput
    {
      get
      {
        return this.m_OnValidateInput;
      }
      set
      {
        SetPropertyUtility.SetClass<MGUIInputField.OnValidateInput>(ref this.m_OnValidateInput, value);
      }
    }

    public int characterLimit
    {
      get
      {
        return this.m_CharacterLimit;
      }
      set
      {
        SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, value);
      }
    }

    public MGUIInputField.ContentType contentType
    {
      get
      {
        return this.m_ContentType;
      }
      set
      {
        if (!SetPropertyUtility.SetStruct<MGUIInputField.ContentType>(ref this.m_ContentType, value))
          return;
        this.EnforceContentType();
      }
    }

    public MGUIInputField.LineType lineType
    {
      get
      {
        return this.m_LineType;
      }
      set
      {
        if (!SetPropertyUtility.SetStruct<MGUIInputField.LineType>(ref this.m_LineType, value))
          return;
        this.SetToCustomIfContentTypeIsNot(MGUIInputField.ContentType.Standard, MGUIInputField.ContentType.Autocorrected);
      }
    }

    public MGUIInputField.InputType inputType
    {
      get
      {
        return this.m_InputType;
      }
      set
      {
        if (!SetPropertyUtility.SetStruct<MGUIInputField.InputType>(ref this.m_InputType, value))
          return;
        this.SetToCustom();
      }
    }

    public TouchScreenKeyboardType keyboardType
    {
      get
      {
        return this.m_KeyboardType;
      }
      set
      {
        if (!SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
          return;
        this.SetToCustom();
      }
    }

    public MGUIInputField.CharacterValidation characterValidation
    {
      get
      {
        return this.m_CharacterValidation;
      }
      set
      {
        if (!SetPropertyUtility.SetStruct<MGUIInputField.CharacterValidation>(ref this.m_CharacterValidation, value))
          return;
        this.SetToCustom();
      }
    }

    public bool multiLine
    {
      get
      {
        if (this.m_LineType != MGUIInputField.LineType.MultiLineNewline)
          return this.lineType == MGUIInputField.LineType.MultiLineSubmit;
        return true;
      }
    }

    public char asteriskChar
    {
      get
      {
        return this.m_AsteriskChar;
      }
      set
      {
        SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value);
      }
    }

    public bool wasCanceled
    {
      get
      {
        return this.m_WasCanceled;
      }
    }

    protected int caretPosition
    {
      get
      {
        return this.m_CaretPosition + Input.get_compositionString().Length;
      }
      set
      {
        this.m_CaretPosition = value;
        this.ClampPos(ref this.m_CaretPosition);
      }
    }

    protected int caretSelectPos
    {
      get
      {
        return this.m_CaretSelectPosition + Input.get_compositionString().Length;
      }
      set
      {
        this.m_CaretSelectPosition = value;
        this.ClampPos(ref this.m_CaretSelectPosition);
      }
    }

    private bool hasSelection
    {
      get
      {
        return this.caretPosition != this.caretSelectPos;
      }
    }

    private static string clipboard
    {
      get
      {
        TextEditor textEditor = new TextEditor();
        textEditor.Paste();
        return ((GUIContent) textEditor.content).get_text();
      }
      set
      {
        TextEditor textEditor = new TextEditor();
        textEditor.content = (__Null) new GUIContent(value);
        textEditor.OnFocus();
        textEditor.Copy();
      }
    }

    protected MGUIInputField()
    {
      base.\u002Ector();
    }

    protected void ClampPos(ref int pos)
    {
      if (pos < 0)
      {
        pos = 0;
      }
      else
      {
        if (pos <= this.text.Length)
          return;
        pos = this.text.Length;
      }
    }

    protected virtual void OnEnable()
    {
      base.OnEnable();
      if (this.m_Text == null)
        this.m_Text = string.Empty;
      this.m_DrawStart = 0;
      this.m_DrawEnd = this.m_Text.Length;
      if (!Object.op_Inequality((Object) this.m_TextComponent, (Object) null))
        return;
      // ISSUE: method pointer
      ((Graphic) this.m_TextComponent).RegisterDirtyVerticesCallback(new UnityAction((object) this, __methodptr(MarkGeometryAsDirty)));
      // ISSUE: method pointer
      ((Graphic) this.m_TextComponent).RegisterDirtyVerticesCallback(new UnityAction((object) this, __methodptr(UpdateLabel)));
      this.UpdateLabel();
    }

    protected virtual void OnDisable()
    {
      this.DeactivateInputField();
      if (Object.op_Inequality((Object) this.m_TextComponent, (Object) null))
      {
        // ISSUE: method pointer
        ((Graphic) this.m_TextComponent).UnregisterDirtyVerticesCallback(new UnityAction((object) this, __methodptr(MarkGeometryAsDirty)));
        // ISSUE: method pointer
        ((Graphic) this.m_TextComponent).UnregisterDirtyVerticesCallback(new UnityAction((object) this, __methodptr(UpdateLabel)));
      }
      CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement) this);
      base.OnDisable();
    }

    [DebuggerHidden]
    private IEnumerator CaretBlink()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MGUIInputField.\u003CCaretBlink\u003Ec__Iterator8()
      {
        \u003C\u003Ef__this = this
      };
    }

    private void SetCaretVisible()
    {
      if (!this.m_AllowInput)
        return;
      this.m_CaretVisible = true;
      this.m_BlinkStartTime = Time.get_unscaledTime();
      this.SetCaretActive();
    }

    private void SetCaretActive()
    {
      if (!this.m_AllowInput)
        return;
      if ((double) this.m_CaretBlinkRate > 0.0)
      {
        if (this.m_BlickCoroutine != null)
          return;
        this.m_BlickCoroutine = ((MonoBehaviour) this).StartCoroutine(this.CaretBlink());
      }
      else
        this.m_CaretVisible = true;
    }

    protected void OnFocus()
    {
      this.SelectAll();
    }

    protected void SelectAll()
    {
      this.caretPosition = this.text.Length;
      this.caretSelectPos = 0;
    }

    public void MoveTextEnd(bool shift)
    {
      int length = this.text.Length;
      if (shift)
      {
        this.caretSelectPos = length;
      }
      else
      {
        this.caretPosition = length;
        this.caretSelectPos = this.caretPosition;
      }
      this.UpdateLabel();
    }

    public void MoveTextStart(bool shift)
    {
      int num = 0;
      if (shift)
      {
        this.caretSelectPos = num;
      }
      else
      {
        this.caretPosition = num;
        this.caretSelectPos = this.caretPosition;
      }
      this.UpdateLabel();
    }

    private bool InPlaceEditing()
    {
      return !TouchScreenKeyboard.get_isSupported();
    }

    protected virtual void LateUpdate()
    {
      if (!this.isFocused || this.InPlaceEditing())
        return;
      this.AssignPositioningIfNeeded();
      if (MGUIInputField.m_Keyboard == null || !MGUIInputField.m_Keyboard.get_active())
      {
        if (MGUIInputField.m_Keyboard != null && MGUIInputField.m_Keyboard.get_wasCanceled())
          this.m_WasCanceled = true;
        this.OnDeselect((BaseEventData) null);
      }
      else
      {
        string text = MGUIInputField.m_Keyboard.get_text();
        if (this.m_Text != text)
        {
          this.m_Text = string.Empty;
          for (int index = 0; index < text.Length; ++index)
          {
            char ch = text[index];
            switch (ch)
            {
              case '\r':
              case '\x0003':
                ch = '\n';
                break;
            }
            if (this.onValidateInput != null)
              ch = this.onValidateInput(this.m_Text, this.m_Text.Length, ch);
            else if (this.characterValidation != MGUIInputField.CharacterValidation.None)
              ch = this.Validate(this.m_Text, this.m_Text.Length, ch);
            if (this.lineType == MGUIInputField.LineType.MultiLineSubmit && (int) ch == 10)
            {
              MGUIInputField.m_Keyboard.set_text(this.m_Text);
              this.OnDeselect((BaseEventData) null);
              return;
            }
            if ((int) ch != 0)
              this.m_Text += (string) (object) ch;
          }
          if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
            this.m_Text = this.m_Text.Substring(0, this.characterLimit);
          int length = this.m_Text.Length;
          this.caretSelectPos = length;
          this.caretPosition = length;
          if (this.m_Text != text)
            MGUIInputField.m_Keyboard.set_text(this.m_Text);
          this.SendOnValueChangedAndUpdateLabel();
        }
        if (!MGUIInputField.m_Keyboard.get_done())
          return;
        if (MGUIInputField.m_Keyboard.get_wasCanceled())
          this.m_WasCanceled = true;
        this.OnDeselect((BaseEventData) null);
      }
    }

    public Vector2 ScreenToLocal(Vector2 screen)
    {
      Canvas canvas = ((Graphic) this.m_TextComponent).get_canvas();
      if (Object.op_Equality((Object) canvas, (Object) null))
        return screen;
      Vector3 vector3 = Vector3.get_zero();
      if (canvas.get_renderMode() == null)
        vector3 = ((Component) this.m_TextComponent).get_transform().InverseTransformPoint(Vector2.op_Implicit(screen));
      else if (Object.op_Inequality((Object) canvas.get_worldCamera(), (Object) null))
      {
        Ray ray = canvas.get_worldCamera().ScreenPointToRay(Vector2.op_Implicit(screen));
        Plane plane;
        // ISSUE: explicit reference operation
        ((Plane) @plane).\u002Ector(((Component) this.m_TextComponent).get_transform().get_forward(), ((Component) this.m_TextComponent).get_transform().get_position());
        float num;
        // ISSUE: explicit reference operation
        ((Plane) @plane).Raycast(ray, ref num);
        // ISSUE: explicit reference operation
        vector3 = ((Component) this.m_TextComponent).get_transform().InverseTransformPoint(((Ray) @ray).GetPoint(num));
      }
      return new Vector2((float) vector3.x, (float) vector3.y);
    }

    private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
    {
      if (!this.multiLine)
        return 0;
      Rect rect = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
      // ISSUE: explicit reference operation
      float yMax = ((Rect) @rect).get_yMax();
      if (pos.y > (double) yMax)
        return -1;
      for (int index = 0; index < generator.get_lineCount(); ++index)
      {
        float num = (float) generator.get_lines()[index].height / this.m_TextComponent.get_pixelsPerUnit();
        if (pos.y <= (double) yMax && pos.y > (double) yMax - (double) num)
          return index;
        yMax -= num;
      }
      return generator.get_lineCount();
    }

    protected int GetCharacterIndexFromPosition(Vector2 pos)
    {
      TextGenerator cachedTextGenerator = this.m_TextComponent.get_cachedTextGenerator();
      if (cachedTextGenerator.get_lineCount() == 0)
        return 0;
      int lineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
      if (lineFromPosition < 0)
        return 0;
      if (lineFromPosition >= cachedTextGenerator.get_lineCount())
        return cachedTextGenerator.get_characterCountVisible();
      int num = (int) cachedTextGenerator.get_lines()[lineFromPosition].startCharIdx;
      int lineEndPosition = MGUIInputField.GetLineEndPosition(cachedTextGenerator, lineFromPosition);
      for (int index = num; index < lineEndPosition && index < cachedTextGenerator.get_characterCountVisible(); ++index)
      {
        UICharInfo uiCharInfo = cachedTextGenerator.get_characters()[index];
        Vector2 vector2 = Vector2.op_Division((Vector2) uiCharInfo.cursorPos, this.m_TextComponent.get_pixelsPerUnit());
        if ((double) (float) (pos.x - vector2.x) < vector2.x + uiCharInfo.charWidth / (double) this.m_TextComponent.get_pixelsPerUnit() - pos.x)
          return index;
      }
      return lineEndPosition;
    }

    private bool MayDrag(PointerEventData eventData)
    {
      if (((UIBehaviour) this).IsActive() && this.IsInteractable() && (eventData.get_button() == null && Object.op_Inequality((Object) this.m_TextComponent, (Object) null)))
        return MGUIInputField.m_Keyboard == null;
      return false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
      if (!this.MayDrag(eventData))
        return;
      this.m_UpdateDrag = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
      if (!this.MayDrag(eventData))
        return;
      Vector2 pos;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(((Graphic) this.textComponent).get_rectTransform(), eventData.get_position(), eventData.get_pressEventCamera(), ref pos);
      this.caretSelectPos = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
      this.MarkGeometryAsDirty();
      this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(((Graphic) this.textComponent).get_rectTransform(), eventData.get_position(), eventData.get_pressEventCamera());
      if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
        this.m_DragCoroutine = ((MonoBehaviour) this).StartCoroutine(this.MouseDragOutsideRect(eventData));
      ((BaseEventData) eventData).Use();
    }

    [DebuggerHidden]
    private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MGUIInputField.\u003CMouseDragOutsideRect\u003Ec__Iterator9()
      {
        eventData = eventData,
        \u003C\u0024\u003EeventData = eventData,
        \u003C\u003Ef__this = this
      };
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
      if (!this.MayDrag(eventData))
        return;
      this.m_UpdateDrag = false;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
      if (!this.MayDrag(eventData))
        return;
      EventSystem.get_current().SetSelectedGameObject(((Component) this).get_gameObject(), (BaseEventData) eventData);
      bool flag = this.m_AllowInput;
      base.OnPointerDown(eventData);
      if (!this.InPlaceEditing() && (MGUIInputField.m_Keyboard == null || !MGUIInputField.m_Keyboard.get_active()))
      {
        this.OnSelect((BaseEventData) eventData);
      }
      else
      {
        if (flag)
        {
          int num = this.GetCharacterIndexFromPosition(this.ScreenToLocal(eventData.get_position())) + this.m_DrawStart;
          this.caretPosition = num;
          this.caretSelectPos = num;
        }
        this.UpdateLabel();
        ((BaseEventData) eventData).Use();
      }
    }

    protected MGUIInputField.EditState KeyPressed(Event evt)
    {
      EventModifiers modifiers = evt.get_modifiers();
      RuntimePlatform platform = Application.get_platform();
      bool ctrl = platform != null && platform != 1 && platform != 3 ? (modifiers & 2) != 0 : (modifiers & 8) != 0;
      bool shift = (modifiers & 1) != 0;
      KeyCode keyCode = evt.get_keyCode();
      switch (keyCode - 271)
      {
        case 0:
label_23:
          if (this.lineType != MGUIInputField.LineType.MultiLineNewline)
            return MGUIInputField.EditState.Finish;
          break;
        case 2:
          this.MoveUp(shift);
          return MGUIInputField.EditState.Continue;
        case 3:
          this.MoveDown(shift);
          return MGUIInputField.EditState.Continue;
        case 4:
          this.MoveRight(shift, ctrl);
          return MGUIInputField.EditState.Continue;
        case 5:
          this.MoveLeft(shift, ctrl);
          return MGUIInputField.EditState.Continue;
        case 7:
          this.MoveTextStart(shift);
          return MGUIInputField.EditState.Continue;
        case 8:
          this.MoveTextEnd(shift);
          return MGUIInputField.EditState.Continue;
        default:
          switch (keyCode - 97)
          {
            case 0:
              if (ctrl)
              {
                this.SelectAll();
                return MGUIInputField.EditState.Continue;
              }
              break;
            case 2:
              if (ctrl)
              {
                MGUIInputField.clipboard = this.GetSelectedString();
                return MGUIInputField.EditState.Continue;
              }
              break;
            default:
              switch (keyCode - 118)
              {
                case 0:
                  if (ctrl)
                  {
                    this.Append(MGUIInputField.clipboard);
                    return MGUIInputField.EditState.Continue;
                  }
                  break;
                case 2:
                  if (ctrl)
                  {
                    MGUIInputField.clipboard = this.GetSelectedString();
                    this.Delete();
                    return MGUIInputField.EditState.Continue;
                  }
                  break;
                default:
                  if (keyCode != 8)
                  {
                    if (keyCode != 13)
                    {
                      if (keyCode != 27)
                      {
                        if (keyCode == (int) sbyte.MaxValue)
                        {
                          this.ForwardSpace();
                          return MGUIInputField.EditState.Continue;
                        }
                        break;
                      }
                      this.m_WasCanceled = true;
                      return MGUIInputField.EditState.Finish;
                    }
                    goto label_23;
                  }
                  else
                  {
                    this.Backspace();
                    return MGUIInputField.EditState.Continue;
                  }
              }
          }
      }
      if (!this.multiLine && (int) evt.get_character() == 9)
        return MGUIInputField.EditState.Continue;
      char ch = evt.get_character();
      switch (ch)
      {
        case '\r':
        case '\x0003':
          ch = '\n';
          break;
      }
      if (this.IsValidChar(ch))
        this.Append(ch);
      if ((int) ch == 0 && Input.get_compositionString().Length > 0)
        this.UpdateLabel();
      return MGUIInputField.EditState.Continue;
    }

    private bool IsValidChar(char c)
    {
      if ((int) c == (int) sbyte.MaxValue)
        return false;
      if ((int) c == 9 || (int) c == 10)
        return true;
      return this.m_TextComponent.get_font().HasCharacter(c);
    }

    public void ProcessEvent(Event e)
    {
      int num = (int) this.KeyPressed(e);
    }

    public virtual void OnUpdateSelected(BaseEventData eventData)
    {
      if (this.m_ShouldActivateNextUpdate)
      {
        this.ActivateInputField();
        this.m_ShouldActivateNextUpdate = false;
      }
      else
      {
        if (!this.isFocused)
          return;
        bool flag = false;
        while (Event.PopEvent(this.m_ProcessingEvent))
        {
          if (this.m_ProcessingEvent.get_rawType() == 4)
          {
            flag = true;
            if (this.KeyPressed(this.m_ProcessingEvent) == MGUIInputField.EditState.Finish)
            {
              this.DeactivateInputField();
              break;
            }
          }
        }
        if (flag)
          this.UpdateLabel();
        eventData.Use();
      }
    }

    private string GetSelectedString()
    {
      if (!this.hasSelection)
        return string.Empty;
      int num1 = this.caretPosition;
      int num2 = this.caretSelectPos;
      if (num1 > num2)
      {
        int num3 = num1;
        num1 = num2;
        num2 = num3;
      }
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = num1; index < num2; ++index)
        stringBuilder.Append(this.text[index]);
      return stringBuilder.ToString();
    }

    private int FindtNextWordBegin()
    {
      if (this.caretSelectPos + 1 >= this.text.Length)
        return this.text.Length;
      int num = this.text.IndexOfAny(MGUIInputField.kSeparators, this.caretSelectPos + 1);
      return num != -1 ? num + 1 : this.text.Length;
    }

    private void MoveRight(bool shift, bool ctrl)
    {
      if (this.hasSelection && !shift)
      {
        int num = Mathf.Max(this.caretPosition, this.caretSelectPos);
        this.caretSelectPos = num;
        this.caretPosition = num;
      }
      else
      {
        int num1 = !ctrl ? this.caretSelectPos + 1 : this.FindtNextWordBegin();
        if (shift)
        {
          this.caretSelectPos = num1;
        }
        else
        {
          int num2 = num1;
          this.caretPosition = num2;
          this.caretSelectPos = num2;
        }
      }
    }

    private int FindtPrevWordBegin()
    {
      if (this.caretSelectPos - 2 < 0)
        return 0;
      int num = this.text.LastIndexOfAny(MGUIInputField.kSeparators, this.caretSelectPos - 2);
      return num != -1 ? num + 1 : 0;
    }

    private void MoveLeft(bool shift, bool ctrl)
    {
      if (this.hasSelection && !shift)
      {
        int num = Mathf.Min(this.caretPosition, this.caretSelectPos);
        this.caretSelectPos = num;
        this.caretPosition = num;
      }
      else
      {
        int num1 = !ctrl ? this.caretSelectPos - 1 : this.FindtPrevWordBegin();
        if (shift)
        {
          this.caretSelectPos = num1;
        }
        else
        {
          int num2 = num1;
          this.caretPosition = num2;
          this.caretSelectPos = num2;
        }
      }
    }

    private int DetermineCharacterLine(int charPos, TextGenerator generator)
    {
      if (!this.multiLine)
        return 0;
      for (int index = 0; index < generator.get_lineCount() - 1; ++index)
      {
        if (generator.get_lines()[index + 1].startCharIdx > charPos)
          return index;
      }
      return generator.get_lineCount() - 1;
    }

    private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
    {
      if (originalPos >= this.cachedInputTextGenerator.get_characterCountVisible())
        return 0;
      UICharInfo uiCharInfo = this.cachedInputTextGenerator.get_characters()[originalPos];
      int index1 = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
      if (index1 - 1 < 0)
      {
        if (goToFirstChar)
          return 0;
        return originalPos;
      }
      int num = this.cachedInputTextGenerator.get_lines()[index1].startCharIdx - 1;
      for (int index2 = (int) this.cachedInputTextGenerator.get_lines()[index1 - 1].startCharIdx; index2 < num; ++index2)
      {
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        if ((^(Vector2&) @this.cachedInputTextGenerator.get_characters()[index2].cursorPos).x >= (^(Vector2&) @uiCharInfo.cursorPos).x)
          return index2;
      }
      return num;
    }

    private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
    {
      if (originalPos >= this.cachedInputTextGenerator.get_characterCountVisible())
        return this.text.Length;
      UICharInfo uiCharInfo = this.cachedInputTextGenerator.get_characters()[originalPos];
      int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
      if (num + 1 >= this.cachedInputTextGenerator.get_lineCount())
      {
        if (goToLastChar)
          return this.text.Length;
        return originalPos;
      }
      int lineEndPosition = MGUIInputField.GetLineEndPosition(this.cachedInputTextGenerator, num + 1);
      for (int index = (int) this.cachedInputTextGenerator.get_lines()[num + 1].startCharIdx; index < lineEndPosition; ++index)
      {
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        if ((^(Vector2&) @this.cachedInputTextGenerator.get_characters()[index].cursorPos).x >= (^(Vector2&) @uiCharInfo.cursorPos).x)
          return index;
      }
      return lineEndPosition;
    }

    private void MoveDown(bool shift)
    {
      this.MoveDown(shift, true);
    }

    private void MoveDown(bool shift, bool goToLastChar)
    {
      if (this.hasSelection && !shift)
      {
        int num = Mathf.Max(this.caretPosition, this.caretSelectPos);
        this.caretSelectPos = num;
        this.caretPosition = num;
      }
      int num1 = !this.multiLine ? this.text.Length : this.LineDownCharacterPosition(this.caretSelectPos, goToLastChar);
      if (shift)
      {
        this.caretSelectPos = num1;
      }
      else
      {
        int num2 = num1;
        this.caretSelectPos = num2;
        this.caretPosition = num2;
      }
    }

    private void MoveUp(bool shift)
    {
      this.MoveUp(shift, true);
    }

    private void MoveUp(bool shift, bool goToFirstChar)
    {
      if (this.hasSelection && !shift)
      {
        int num = Mathf.Min(this.caretPosition, this.caretSelectPos);
        this.caretSelectPos = num;
        this.caretPosition = num;
      }
      int num1 = !this.multiLine ? 0 : this.LineUpCharacterPosition(this.caretSelectPos, goToFirstChar);
      if (shift)
      {
        this.caretSelectPos = num1;
      }
      else
      {
        int num2 = num1;
        this.caretPosition = num2;
        this.caretSelectPos = num2;
      }
    }

    private void Delete()
    {
      if (this.caretPosition == this.caretSelectPos)
        return;
      if (this.caretPosition < this.caretSelectPos)
      {
        this.m_Text = this.text.Substring(0, this.caretPosition) + this.text.Substring(this.caretSelectPos, this.text.Length - this.caretSelectPos);
        this.caretSelectPos = this.caretPosition;
      }
      else
      {
        this.m_Text = this.text.Substring(0, this.caretSelectPos) + this.text.Substring(this.caretPosition, this.text.Length - this.caretPosition);
        this.caretPosition = this.caretSelectPos;
      }
      this.SendOnValueChangedAndUpdateLabel();
    }

    private void ForwardSpace()
    {
      if (this.hasSelection)
      {
        this.Delete();
      }
      else
      {
        if (this.caretPosition >= this.text.Length)
          return;
        this.m_Text = this.text.Remove(this.caretPosition, 1);
        this.SendOnValueChangedAndUpdateLabel();
      }
    }

    private void Backspace()
    {
      if (this.hasSelection)
      {
        this.Delete();
      }
      else
      {
        if (this.caretPosition <= 0)
          return;
        this.m_Text = this.text.Remove(this.caretPosition - 1, 1);
        int num = this.caretPosition - 1;
        this.caretPosition = num;
        this.caretSelectPos = num;
        this.SendOnValueChangedAndUpdateLabel();
      }
    }

    private void Insert(char c)
    {
      string str = c.ToString();
      this.Delete();
      if (this.characterLimit > 0 && this.text.Length >= this.characterLimit)
        return;
      this.m_Text = this.text.Insert(this.m_CaretPosition, str);
      this.caretSelectPos = (this.caretPosition += str.Length);
      this.SendOnValueChanged();
    }

    private void SendOnValueChangedAndUpdateLabel()
    {
      this.SendOnValueChanged();
      this.UpdateLabel();
    }

    private void SendOnValueChanged()
    {
      if (this.onValueChange == null)
        return;
      this.onValueChange.Invoke(this.text);
    }

    protected void SendOnSubmit()
    {
      if (this.onEndEdit == null)
        return;
      this.onEndEdit.Invoke(this.m_Text);
    }

    protected virtual void Append(string input)
    {
      if (!this.InPlaceEditing())
        return;
      int index = 0;
      for (int length = input.Length; index < length; ++index)
      {
        char input1 = input[index];
        if ((int) input1 >= 32)
          this.Append(input1);
      }
    }

    protected virtual void Append(char input)
    {
      if (!this.InPlaceEditing())
        return;
      if (this.onValidateInput != null)
        input = this.onValidateInput(this.text, this.caretPosition, input);
      else if (this.characterValidation != MGUIInputField.CharacterValidation.None)
        input = this.Validate(this.text, this.caretPosition, input);
      if ((int) input == 0)
        return;
      this.Insert(input);
    }

    protected void UpdateLabel()
    {
      if (!Object.op_Inequality((Object) this.m_TextComponent, (Object) null) || !Object.op_Inequality((Object) this.m_TextComponent.get_font(), (Object) null))
        return;
      string str1 = Input.get_compositionString().Length <= 0 ? this.text : this.text.Substring(0, this.m_CaretPosition) + Input.get_compositionString() + this.text.Substring(this.m_CaretPosition);
      string str2 = this.inputType != MGUIInputField.InputType.Password ? str1 : new string(this.asteriskChar, str1.Length);
      bool flag = string.IsNullOrEmpty(str1);
      if (Object.op_Inequality((Object) this.m_Placeholder, (Object) null))
        ((Behaviour) this.m_Placeholder).set_enabled(flag);
      if (!this.m_AllowInput)
      {
        this.m_DrawStart = 0;
        this.m_DrawEnd = this.m_Text.Length;
      }
      if (!flag)
      {
        Rect rect = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
        // ISSUE: explicit reference operation
        TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(((Rect) @rect).get_size());
        generationSettings.generateOutOfBounds = (__Null) 1;
        this.cachedInputTextGenerator.Populate(str2, generationSettings);
        this.SetDrawRangeToContainCaretPosition(this.cachedInputTextGenerator, this.caretSelectPos, ref this.m_DrawStart, ref this.m_DrawEnd);
        str2 = str2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, str2.Length) - this.m_DrawStart);
        this.SetCaretVisible();
      }
      this.m_TextComponent.set_text(str2);
      this.MarkGeometryAsDirty();
    }

    private bool IsSelectionVisible()
    {
      return this.m_DrawStart <= this.caretPosition && this.m_DrawStart <= this.caretSelectPos && (this.m_DrawEnd >= this.caretPosition && this.m_DrawEnd >= this.caretSelectPos);
    }

    private static int GetLineStartPosition(TextGenerator gen, int line)
    {
      line = Mathf.Clamp(line, 0, ((ICollection<UILineInfo>) gen.get_lines()).Count - 1);
      return (int) gen.get_lines()[line].startCharIdx;
    }

    private static int GetLineEndPosition(TextGenerator gen, int line)
    {
      line = Mathf.Max(line, 0);
      if (line + 1 < ((ICollection<UILineInfo>) gen.get_lines()).Count)
        return (int) gen.get_lines()[line + 1].startCharIdx;
      return gen.get_characterCountVisible();
    }

    private void SetDrawRangeToContainCaretPosition(TextGenerator gen, int caretPos, ref int drawStart, ref int drawEnd)
    {
      Rect rectExtents = gen.get_rectExtents();
      // ISSUE: explicit reference operation
      Vector2 size = ((Rect) @rectExtents).get_size();
      if (this.multiLine)
      {
        IList<UILineInfo> lines = gen.get_lines();
        int line1 = this.DetermineCharacterLine(caretPos, gen);
        int num1 = (int) size.y;
        if (drawEnd <= caretPos)
        {
          drawEnd = MGUIInputField.GetLineEndPosition(gen, line1);
          for (int line2 = line1; line2 >= 0; --line2)
          {
            num1 -= (int) lines[line2].height;
            if (num1 < 0)
              break;
            drawStart = MGUIInputField.GetLineStartPosition(gen, line2);
          }
        }
        else
        {
          if (drawStart > caretPos)
            drawStart = MGUIInputField.GetLineStartPosition(gen, line1);
          int line2 = this.DetermineCharacterLine(drawStart, gen);
          int line3 = line2;
          drawEnd = MGUIInputField.GetLineEndPosition(gen, line3);
          int num2 = num1 - lines[line3].height;
          while (true)
          {
            while (line3 >= ((ICollection<UILineInfo>) lines).Count - 1)
            {
              if (line2 <= 0)
                return;
              --line2;
              if (num2 < lines[line2].height)
                return;
              drawStart = MGUIInputField.GetLineStartPosition(gen, line2);
              num2 -= (int) lines[line2].height;
            }
            ++line3;
            if (num2 >= lines[line3].height)
            {
              drawEnd = MGUIInputField.GetLineEndPosition(gen, line3);
              num2 -= (int) lines[line3].height;
            }
            else
              break;
          }
        }
      }
      else
      {
        float num = (float) size.x;
        IList<UICharInfo> characters = gen.get_characters();
        if (drawEnd <= caretPos)
        {
          drawEnd = Mathf.Min(caretPos, gen.get_characterCountVisible());
          drawStart = 0;
          for (int index = drawEnd; index > 0; --index)
          {
            num -= (float) characters[index - 1].charWidth;
            if ((double) num < 0.0)
            {
              drawStart = index;
              break;
            }
          }
        }
        else
        {
          if (drawStart > caretPos)
            drawStart = caretPos;
          drawEnd = gen.get_characterCountVisible();
          for (int index = drawStart; index < gen.get_characterCountVisible(); ++index)
          {
            num -= (float) characters[index].charWidth;
            if ((double) num < 0.0)
            {
              drawEnd = index;
              break;
            }
          }
        }
      }
    }

    private void MarkGeometryAsDirty()
    {
      CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild((ICanvasElement) this);
    }

    public virtual void Rebuild(CanvasUpdate update)
    {
      if (update != 4)
        return;
      this.UpdateGeometry();
    }

    private void UpdateGeometry()
    {
      if (!this.shouldHideMobileInput)
        return;
      if (Object.op_Equality((Object) this.m_CachedInputRenderer, (Object) null) && Object.op_Inequality((Object) this.m_TextComponent, (Object) null))
      {
        GameObject gameObject = new GameObject(((Object) ((Component) this).get_transform()).get_name() + " Input Caret");
        ((Object) gameObject).set_hideFlags((HideFlags) 4);
        gameObject.get_transform().SetParent(((Component) this.m_TextComponent).get_transform().get_parent());
        gameObject.get_transform().SetAsFirstSibling();
        gameObject.set_layer(((Component) this).get_gameObject().get_layer());
        this.caretRectTrans = (RectTransform) gameObject.AddComponent<RectTransform>();
        this.m_CachedInputRenderer = (CanvasRenderer) gameObject.AddComponent<CanvasRenderer>();
        this.m_CachedInputRenderer.SetMaterial(Graphic.get_defaultGraphicMaterial(), (Texture) null);
        this.AssignPositioningIfNeeded();
      }
      if (Object.op_Equality((Object) this.m_CachedInputRenderer, (Object) null))
        return;
      this.OnFillVBO(this.m_Vbo);
      if (this.m_Vbo.Count == 0)
        this.m_CachedInputRenderer.SetVertices((UIVertex[]) null, 0);
      else
        this.m_CachedInputRenderer.SetVertices(this.m_Vbo.ToArray(), this.m_Vbo.Count);
      this.m_Vbo.Clear();
    }

    private void AssignPositioningIfNeeded()
    {
      if (!Object.op_Inequality((Object) this.m_TextComponent, (Object) null) || !Object.op_Inequality((Object) this.caretRectTrans, (Object) null) || !Vector3.op_Inequality(((Transform) this.caretRectTrans).get_localPosition(), ((Transform) ((Graphic) this.m_TextComponent).get_rectTransform()).get_localPosition()) && !Quaternion.op_Inequality(((Transform) this.caretRectTrans).get_localRotation(), ((Transform) ((Graphic) this.m_TextComponent).get_rectTransform()).get_localRotation()) && (!Vector3.op_Inequality(((Transform) this.caretRectTrans).get_localScale(), ((Transform) ((Graphic) this.m_TextComponent).get_rectTransform()).get_localScale()) && !Vector2.op_Inequality(this.caretRectTrans.get_anchorMin(), ((Graphic) this.m_TextComponent).get_rectTransform().get_anchorMin())) && (!Vector2.op_Inequality(this.caretRectTrans.get_anchorMax(), ((Graphic) this.m_TextComponent).get_rectTransform().get_anchorMax()) && !Vector2.op_Inequality(this.caretRectTrans.get_anchoredPosition(), ((Graphic) this.m_TextComponent).get_rectTransform().get_anchoredPosition()) && (!Vector2.op_Inequality(this.caretRectTrans.get_sizeDelta(), ((Graphic) this.m_TextComponent).get_rectTransform().get_sizeDelta()) && !Vector2.op_Inequality(this.caretRectTrans.get_pivot(), ((Graphic) this.m_TextComponent).get_rectTransform().get_pivot()))))
        return;
      ((Transform) this.caretRectTrans).set_localPosition(((Transform) ((Graphic) this.m_TextComponent).get_rectTransform()).get_localPosition());
      ((Transform) this.caretRectTrans).set_localRotation(((Transform) ((Graphic) this.m_TextComponent).get_rectTransform()).get_localRotation());
      ((Transform) this.caretRectTrans).set_localScale(((Transform) ((Graphic) this.m_TextComponent).get_rectTransform()).get_localScale());
      this.caretRectTrans.set_anchorMin(((Graphic) this.m_TextComponent).get_rectTransform().get_anchorMin());
      this.caretRectTrans.set_anchorMax(((Graphic) this.m_TextComponent).get_rectTransform().get_anchorMax());
      this.caretRectTrans.set_anchoredPosition(((Graphic) this.m_TextComponent).get_rectTransform().get_anchoredPosition());
      this.caretRectTrans.set_sizeDelta(((Graphic) this.m_TextComponent).get_rectTransform().get_sizeDelta());
      this.caretRectTrans.set_pivot(((Graphic) this.m_TextComponent).get_rectTransform().get_pivot());
    }

    private void OnFillVBO(List<UIVertex> vbo)
    {
      if (!this.isFocused)
        return;
      Rect rect = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
      // ISSUE: explicit reference operation
      Vector2 size = ((Rect) @rect).get_size();
      Vector2 textAnchorPivot = Text.GetTextAnchorPivot(this.m_TextComponent.get_alignment());
      Vector2 zero = Vector2.get_zero();
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      zero.x = (__Null) (double) Mathf.Lerp(((Rect) @rect).get_xMin(), ((Rect) @rect).get_xMax(), (float) textAnchorPivot.x);
      // ISSUE: explicit reference operation
      // ISSUE: explicit reference operation
      zero.y = (__Null) (double) Mathf.Lerp(((Rect) @rect).get_yMin(), ((Rect) @rect).get_yMax(), (float) textAnchorPivot.y);
      Vector2 roundingOffset = Vector2.op_Addition(Vector2.op_Subtraction(((Graphic) this.m_TextComponent).PixelAdjustPoint(zero), zero), Vector2.Scale(size, textAnchorPivot));
      roundingOffset.x = (__Null) (roundingOffset.x - (double) Mathf.Floor((float) (0.5 + roundingOffset.x)));
      roundingOffset.y = (__Null) (roundingOffset.y - (double) Mathf.Floor((float) (0.5 + roundingOffset.y)));
      if (!this.hasSelection)
        this.GenerateCursor(vbo, roundingOffset);
      else
        this.GenerateHightlight(vbo, roundingOffset);
    }

    private void GenerateCursor(List<UIVertex> vbo, Vector2 roundingOffset)
    {
      if (!this.m_CaretVisible)
        return;
      if (this.m_CursorVerts == null)
        this.CreateCursorVerts();
      float num1 = 1f;
      float num2 = (float) this.m_TextComponent.get_fontSize();
      int charPos = Mathf.Max(0, this.caretPosition - this.m_DrawStart);
      TextGenerator cachedTextGenerator = this.m_TextComponent.get_cachedTextGenerator();
      if (cachedTextGenerator == null)
        return;
      if (this.m_TextComponent.get_resizeTextForBestFit())
        num2 = (float) cachedTextGenerator.get_fontSizeUsedForBestFit() / this.m_TextComponent.get_pixelsPerUnit();
      Vector2 zero = Vector2.get_zero();
      if (cachedTextGenerator.get_characterCountVisible() + 1 > charPos || charPos == 0)
      {
        UICharInfo uiCharInfo = cachedTextGenerator.get_characters()[charPos];
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        zero.x = (^(Vector2&) @uiCharInfo.cursorPos).x;
      }
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Vector2& local1 = @zero;
      // ISSUE: explicit reference operation
      double num3 = (^local1).x / (double) this.m_TextComponent.get_pixelsPerUnit();
      // ISSUE: explicit reference operation
      (^local1).x = (__Null) num3;
      // ISSUE: variable of the null type
      __Null local2 = zero.x;
      Rect rect1 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
      // ISSUE: explicit reference operation
      double num4 = (double) ((Rect) @rect1).get_xMax();
      if (local2 > num4)
      {
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        Vector2& local3 = @zero;
        Rect rect2 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
        // ISSUE: explicit reference operation
        double num5 = (double) ((Rect) @rect2).get_xMax();
        // ISSUE: explicit reference operation
        (^local3).x = (__Null) num5;
      }
      float num6 = this.SumLineHeights(this.DetermineCharacterLine(charPos, cachedTextGenerator), cachedTextGenerator);
      // ISSUE: explicit reference operation
      // ISSUE: variable of a reference type
      Vector2& local4 = @zero;
      Rect rect3 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
      // ISSUE: explicit reference operation
      double num7 = (double) ((Rect) @rect3).get_yMax() - (double) num6 / (double) this.m_TextComponent.get_pixelsPerUnit();
      // ISSUE: explicit reference operation
      (^local4).y = (__Null) num7;
      this.m_CursorVerts[0].position = (__Null) new Vector3((float) zero.x, (float) zero.y - num2, 0.0f);
      this.m_CursorVerts[1].position = (__Null) new Vector3((float) zero.x + num1, (float) zero.y - num2, 0.0f);
      this.m_CursorVerts[2].position = (__Null) new Vector3((float) zero.x + num1, (float) zero.y, 0.0f);
      this.m_CursorVerts[3].position = (__Null) new Vector3((float) zero.x, (float) zero.y, 0.0f);
      if (Vector2.op_Inequality(roundingOffset, Vector2.get_zero()))
      {
        for (int index = 0; index < this.m_CursorVerts.Length; ++index)
        {
          UIVertex uiVertex = this.m_CursorVerts[index];
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          __Null& local3 = @uiVertex.position;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: variable of the null type
          __Null local5 = (^(Vector3&) local3).x + roundingOffset.x;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          (^(Vector3&) local3).x = local5;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          __Null& local6 = @uiVertex.position;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: variable of the null type
          __Null local7 = (^(Vector3&) local6).y + roundingOffset.y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          (^(Vector3&) local6).y = local7;
          vbo.Add(uiVertex);
        }
      }
      else
      {
        for (int index = 0; index < this.m_CursorVerts.Length; ++index)
          vbo.Add(this.m_CursorVerts[index]);
      }
      zero.y = (__Null) ((double) Screen.get_height() - zero.y);
      Input.set_compositionCursorPos(zero);
    }

    private void CreateCursorVerts()
    {
      this.m_CursorVerts = new UIVertex[4];
      for (int index = 0; index < this.m_CursorVerts.Length; ++index)
      {
        this.m_CursorVerts[index] = (UIVertex) UIVertex.simpleVert;
        this.m_CursorVerts[index].color = (__Null) Color32.op_Implicit(((Graphic) this.m_TextComponent).get_color());
        this.m_CursorVerts[index].uv0 = (__Null) Vector2.get_zero();
      }
    }

    private float SumLineHeights(int endLine, TextGenerator generator)
    {
      float num = 0.0f;
      for (int index = 0; index < endLine; ++index)
        num += (float) generator.get_lines()[index].height;
      return num;
    }

    private void GenerateHightlight(List<UIVertex> vbo, Vector2 roundingOffset)
    {
      int charPos = Mathf.Max(0, this.caretPosition - this.m_DrawStart);
      int num1 = Mathf.Max(0, this.caretSelectPos - this.m_DrawStart);
      if (charPos > num1)
      {
        int num2 = charPos;
        charPos = num1;
        num1 = num2;
      }
      int num3 = num1 - 1;
      TextGenerator cachedTextGenerator = this.m_TextComponent.get_cachedTextGenerator();
      int num4 = this.DetermineCharacterLine(charPos, cachedTextGenerator);
      float num5 = (float) this.m_TextComponent.get_fontSize();
      if (this.m_TextComponent.get_resizeTextForBestFit())
        num5 = (float) cachedTextGenerator.get_fontSizeUsedForBestFit() / this.m_TextComponent.get_pixelsPerUnit();
      if (this.cachedInputTextGenerator != null && ((ICollection<UILineInfo>) this.cachedInputTextGenerator.get_lines()).Count > 0)
        num5 = (float) this.cachedInputTextGenerator.get_lines()[0].height;
      if (this.m_TextComponent.get_resizeTextForBestFit() && this.cachedInputTextGenerator != null)
        num5 = (float) this.cachedInputTextGenerator.get_fontSizeUsedForBestFit();
      int lineEndPosition = MGUIInputField.GetLineEndPosition(cachedTextGenerator, num4);
      UIVertex uiVertex = (UIVertex) UIVertex.simpleVert;
      uiVertex.uv0 = (__Null) Vector2.get_zero();
      uiVertex.color = (__Null) Color32.op_Implicit(this.selectionColor);
      for (int index = charPos; index <= num3 && index < cachedTextGenerator.get_characterCountVisible(); ++index)
      {
        if (index + 1 == lineEndPosition || index == num3)
        {
          UICharInfo uiCharInfo1 = cachedTextGenerator.get_characters()[charPos];
          UICharInfo uiCharInfo2 = cachedTextGenerator.get_characters()[index];
          float num2 = this.SumLineHeights(num4, cachedTextGenerator);
          Vector2 vector2_1;
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          Vector2& local1 = @vector2_1;
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          double num6 = (^(Vector2&) @uiCharInfo1.cursorPos).x / (double) this.m_TextComponent.get_pixelsPerUnit();
          Rect rect1 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
          // ISSUE: explicit reference operation
          double num7 = (double) ((Rect) @rect1).get_yMax() - (double) num2 / (double) this.m_TextComponent.get_pixelsPerUnit();
          ((Vector2) local1).\u002Ector((float) num6, (float) num7);
          Vector2 vector2_2;
          // ISSUE: explicit reference operation
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ((Vector2) @vector2_2).\u002Ector((float) ((^(Vector2&) @uiCharInfo2.cursorPos).x + uiCharInfo2.charWidth) / this.m_TextComponent.get_pixelsPerUnit(), (float) (vector2_1.y - (double) num5 / (double) this.m_TextComponent.get_pixelsPerUnit()));
          // ISSUE: variable of the null type
          __Null local2 = vector2_2.x;
          Rect rect2 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
          // ISSUE: explicit reference operation
          double num8 = (double) ((Rect) @rect2).get_xMax();
          if (local2 <= num8)
          {
            // ISSUE: variable of the null type
            __Null local3 = vector2_2.x;
            Rect rect3 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
            // ISSUE: explicit reference operation
            double num9 = (double) ((Rect) @rect3).get_xMin();
            if (local3 >= num9)
              goto label_13;
          }
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          Vector2& local4 = @vector2_2;
          Rect rect4 = ((Graphic) this.m_TextComponent).get_rectTransform().get_rect();
          // ISSUE: explicit reference operation
          double num10 = (double) ((Rect) @rect4).get_xMax();
          // ISSUE: explicit reference operation
          (^local4).x = (__Null) num10;
label_13:
          uiVertex.position = (__Null) Vector3.op_Addition(new Vector3((float) vector2_1.x, (float) vector2_2.y, 0.0f), Vector2.op_Implicit(roundingOffset));
          vbo.Add(uiVertex);
          uiVertex.position = (__Null) Vector3.op_Addition(new Vector3((float) vector2_2.x, (float) vector2_2.y, 0.0f), Vector2.op_Implicit(roundingOffset));
          vbo.Add(uiVertex);
          uiVertex.position = (__Null) Vector3.op_Addition(new Vector3((float) vector2_2.x, (float) vector2_1.y, 0.0f), Vector2.op_Implicit(roundingOffset));
          vbo.Add(uiVertex);
          uiVertex.position = (__Null) Vector3.op_Addition(new Vector3((float) vector2_1.x, (float) vector2_1.y, 0.0f), Vector2.op_Implicit(roundingOffset));
          vbo.Add(uiVertex);
          charPos = index + 1;
          ++num4;
          lineEndPosition = MGUIInputField.GetLineEndPosition(cachedTextGenerator, num4);
        }
      }
    }

    protected char Validate(string text, int pos, char ch)
    {
      if (this.characterValidation == MGUIInputField.CharacterValidation.None || !((Behaviour) this).get_enabled())
        return ch;
      if (this.characterValidation == MGUIInputField.CharacterValidation.Integer || this.characterValidation == MGUIInputField.CharacterValidation.Decimal)
      {
        if ((pos != 0 || text.Length <= 0 || (int) text[0] != 45) && ((int) ch >= 48 && (int) ch <= 57 || (int) ch == 45 && pos == 0 || (int) ch == 46 && this.characterValidation == MGUIInputField.CharacterValidation.Decimal && !text.Contains(".")))
          return ch;
      }
      else if (this.characterValidation == MGUIInputField.CharacterValidation.Alphanumeric)
      {
        if ((int) ch >= 65 && (int) ch <= 90 || (int) ch >= 97 && (int) ch <= 122 || (int) ch >= 48 && (int) ch <= 57)
          return ch;
      }
      else if (this.characterValidation == MGUIInputField.CharacterValidation.Name)
      {
        char ch1 = text.Length <= 0 ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
        char ch2 = text.Length <= 0 ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
        if (char.IsLetter(ch))
        {
          if (char.IsLower(ch) && (int) ch1 == 32)
            return char.ToUpper(ch);
          if (char.IsUpper(ch) && (int) ch1 != 32 && (int) ch1 != 39)
            return char.ToLower(ch);
          return ch;
        }
        if ((int) ch == 39)
        {
          if ((int) ch1 != 32 && (int) ch1 != 39 && ((int) ch2 != 39 && !text.Contains("'")))
            return ch;
        }
        else if ((int) ch == 32 && (int) ch1 != 32 && ((int) ch1 != 39 && (int) ch2 != 32) && (int) ch2 != 39)
          return ch;
      }
      else if (this.characterValidation == MGUIInputField.CharacterValidation.EmailAddress && ((int) ch >= 65 && (int) ch <= 90 || (int) ch >= 97 && (int) ch <= 122 || ((int) ch >= 48 && (int) ch <= 57 || (int) ch == 64 && text.IndexOf('@') == -1) || "!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1 || (int) ch == 46 && ((text.Length <= 0 ? (int) ' ' : (int) text[Mathf.Clamp(pos, 0, text.Length - 1)]) != 46 && (text.Length <= 0 ? (int) '\n' : (int) text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]) != 46)))
        return ch;
      return char.MinValue;
    }

    public void ActivateInputField()
    {
      if (this.m_AllowInput || Object.op_Equality((Object) this.m_TextComponent, (Object) null) || (Object.op_Equality((Object) this.m_TextComponent.get_font(), (Object) null) || !((UIBehaviour) this).IsActive()) || !this.IsInteractable())
        return;
      if (this.isFocused)
      {
        if (MGUIInputField.m_Keyboard != null && !MGUIInputField.m_Keyboard.get_active())
        {
          MGUIInputField.m_Keyboard.set_active(true);
          MGUIInputField.m_Keyboard.set_text(this.m_Text);
        }
        this.m_ShouldActivateNextUpdate = true;
      }
      else
      {
        if (TouchScreenKeyboard.get_isSupported())
        {
          if (Input.get_touchSupported())
            TouchScreenKeyboard.set_hideInput(this.shouldHideMobileInput);
          MGUIInputField.m_Keyboard = this.inputType != MGUIInputField.InputType.Password ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == MGUIInputField.InputType.AutoCorrect, this.multiLine) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true);
        }
        else
        {
          Input.set_imeCompositionMode((IMECompositionMode) 1);
          this.OnFocus();
        }
        this.m_AllowInput = true;
        this.m_OriginalText = this.text;
        this.m_WasCanceled = false;
        this.SetCaretVisible();
        this.UpdateLabel();
      }
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
      base.OnSelect(eventData);
      this.ActivateInputField();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
      if (eventData.get_button() != null)
        return;
      this.ActivateInputField();
    }

    public void DeactivateInputField()
    {
      if (!this.m_AllowInput)
        return;
      this.m_AllowInput = false;
      this.m_HasDoneFocusTransition = false;
      if (Object.op_Inequality((Object) this.m_TextComponent, (Object) null) && ((UIBehaviour) this).IsActive() && this.IsInteractable())
      {
        if (this.m_WasCanceled)
          this.text = this.m_OriginalText;
        if (MGUIInputField.m_Keyboard != null)
        {
          MGUIInputField.m_Keyboard.set_active(false);
          MGUIInputField.m_Keyboard = (TouchScreenKeyboard) null;
        }
        this.m_CaretPosition = this.m_CaretSelectPosition = 0;
        this.SendOnSubmit();
        Input.set_imeCompositionMode((IMECompositionMode) 0);
      }
      this.MarkGeometryAsDirty();
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
      this.DeactivateInputField();
      base.OnDeselect(eventData);
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
      if (!((UIBehaviour) this).IsActive() || !this.IsInteractable() || this.isFocused)
        return;
      this.m_ShouldActivateNextUpdate = true;
    }

    private void EnforceContentType()
    {
      switch (this.contentType)
      {
        case MGUIInputField.ContentType.Standard:
          this.m_InputType = MGUIInputField.InputType.Standard;
          this.m_KeyboardType = (TouchScreenKeyboardType) 0;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.None;
          break;
        case MGUIInputField.ContentType.Autocorrected:
          this.m_InputType = MGUIInputField.InputType.AutoCorrect;
          this.m_KeyboardType = (TouchScreenKeyboardType) 0;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.None;
          break;
        case MGUIInputField.ContentType.IntegerNumber:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Standard;
          this.m_KeyboardType = (TouchScreenKeyboardType) 4;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.Integer;
          break;
        case MGUIInputField.ContentType.DecimalNumber:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Standard;
          this.m_KeyboardType = (TouchScreenKeyboardType) 2;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.Decimal;
          break;
        case MGUIInputField.ContentType.Alphanumeric:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Standard;
          this.m_KeyboardType = (TouchScreenKeyboardType) 1;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.Alphanumeric;
          break;
        case MGUIInputField.ContentType.Name:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Standard;
          this.m_KeyboardType = (TouchScreenKeyboardType) 0;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.Name;
          break;
        case MGUIInputField.ContentType.EmailAddress:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Standard;
          this.m_KeyboardType = (TouchScreenKeyboardType) 7;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.EmailAddress;
          break;
        case MGUIInputField.ContentType.Password:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Password;
          this.m_KeyboardType = (TouchScreenKeyboardType) 0;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.None;
          break;
        case MGUIInputField.ContentType.Pin:
          this.m_LineType = MGUIInputField.LineType.SingleLine;
          this.m_InputType = MGUIInputField.InputType.Password;
          this.m_KeyboardType = (TouchScreenKeyboardType) 4;
          this.m_CharacterValidation = MGUIInputField.CharacterValidation.Integer;
          break;
      }
    }

    private void SetToCustomIfContentTypeIsNot(params MGUIInputField.ContentType[] allowedContentTypes)
    {
      if (this.contentType == MGUIInputField.ContentType.Custom)
        return;
      for (int index = 0; index < allowedContentTypes.Length; ++index)
      {
        if (this.contentType == allowedContentTypes[index])
          return;
      }
      this.contentType = MGUIInputField.ContentType.Custom;
    }

    private void SetToCustom()
    {
      if (this.contentType == MGUIInputField.ContentType.Custom)
        return;
      this.contentType = MGUIInputField.ContentType.Custom;
    }

    protected virtual void DoStateTransition(Selectable.SelectionState state, bool instant)
    {
      if (this.m_HasDoneFocusTransition)
        state = (Selectable.SelectionState) 1;
      else if (state == 2)
        this.m_HasDoneFocusTransition = true;
      base.DoStateTransition(state, instant);
    }

    Transform ICanvasElement.get_transform()
    {
      return ((Component) this).get_transform();
    }

    bool ICanvasElement.IsDestroyed()
    {
      return ((UIBehaviour) this).IsDestroyed();
    }

    public enum ContentType
    {
      Standard,
      Autocorrected,
      IntegerNumber,
      DecimalNumber,
      Alphanumeric,
      Name,
      EmailAddress,
      Password,
      Pin,
      Custom,
    }

    public enum InputType
    {
      Standard,
      AutoCorrect,
      Password,
    }

    public enum CharacterValidation
    {
      None,
      Integer,
      Decimal,
      Alphanumeric,
      Name,
      EmailAddress,
    }

    public enum LineType
    {
      SingleLine,
      MultiLineSubmit,
      MultiLineNewline,
    }

    [Serializable]
    public class SubmitEvent : UnityEvent<string>
    {
      public SubmitEvent()
      {
        base.\u002Ector();
      }
    }

    [Serializable]
    public class OnChangeEvent : UnityEvent<string>
    {
      public OnChangeEvent()
      {
        base.\u002Ector();
      }
    }

    protected enum EditState
    {
      Continue,
      Finish,
    }

    public delegate char OnValidateInput(string text, int charIndex, char addedChar);
  }
}
