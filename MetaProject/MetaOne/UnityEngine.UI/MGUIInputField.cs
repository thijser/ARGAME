using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/MGUI Input Field", 31)]
	public class MGUIInputField : Selectable, IPointerClickHandler, IDragHandler, IEndDragHandler, IUpdateSelectedHandler, ICanvasElement, IEventSystemHandler, IBeginDragHandler, ISubmitHandler
	{
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
			Custom
		}

		public enum InputType
		{
			Standard,
			AutoCorrect,
			Password
		}

		public enum CharacterValidation
		{
			None,
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			EmailAddress
		}

		public enum LineType
		{
			SingleLine,
			MultiLineSubmit,
			MultiLineNewline
		}

		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		protected enum EditState
		{
			Continue,
			Finish
		}

		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		private const float kHScrollSpeed = 0.05f;

		private const float kVScrollSpeed = 0.1f;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		protected static TouchScreenKeyboard m_Keyboard;

		private static readonly char[] kSeparators = new char[]
		{
			' ',
			'.',
			','
		};

		[FormerlySerializedAs("text"), SerializeField]
		protected Text m_TextComponent;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		private MGUIInputField.ContentType m_ContentType;

		[FormerlySerializedAs("inputType"), SerializeField]
		private MGUIInputField.InputType m_InputType;

		[FormerlySerializedAs("asteriskChar"), SerializeField]
		private char m_AsteriskChar = '*';

		[FormerlySerializedAs("keyboardType"), SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		[SerializeField]
		private MGUIInputField.LineType m_LineType;

		[FormerlySerializedAs("hideMobileInput"), SerializeField]
		private bool m_HideMobileInput;

		[FormerlySerializedAs("validation"), SerializeField]
		private MGUIInputField.CharacterValidation m_CharacterValidation;

		[FormerlySerializedAs("characterLimit"), SerializeField]
		private int m_CharacterLimit;

		[FormerlySerializedAs("m_OnSubmit"), FormerlySerializedAs("onSubmit"), SerializeField]
		private MGUIInputField.SubmitEvent m_EndEdit = new MGUIInputField.SubmitEvent();

		[FormerlySerializedAs("onValueChange"), SerializeField]
		private MGUIInputField.OnChangeEvent m_OnValueChange = new MGUIInputField.OnChangeEvent();

		[FormerlySerializedAs("onValidateInput"), SerializeField]
		private MGUIInputField.OnValidateInput m_OnValidateInput;

		[FormerlySerializedAs("selectionColor"), SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.807843149f, 1f, 0.7529412f);

		[FormerlySerializedAs("mValue"), SerializeField]
		protected string m_Text = string.Empty;

		[Range(0f, 8f), SerializeField]
		private float m_CaretBlinkRate = 1.7f;

		protected int m_CaretPosition;

		protected int m_CaretSelectPosition;

		private RectTransform caretRectTrans;

		protected UIVertex[] m_CursorVerts;

		private TextGenerator m_InputTextCache;

		private CanvasRenderer m_CachedInputRenderer;

		private readonly List<UIVertex> m_Vbo = new List<UIVertex>();

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

		private string m_OriginalText = string.Empty;

		private bool m_WasCanceled;

		private bool m_HasDoneFocusTransition;

		private Event m_ProcessingEvent = new Event();

		protected TextGenerator cachedInputTextGenerator
		{
			get
			{
				if (this.m_InputTextCache == null)
				{
					this.m_InputTextCache = new TextGenerator();
				}
				return this.m_InputTextCache;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.get_platform();
				switch (platform)
				{
				case 8:
				case 11:
					goto IL_2B;
				case 9:
				case 10:
					IL_1E:
					if (platform != 22)
					{
						return true;
					}
					goto IL_2B;
				}
				goto IL_1E;
				IL_2B:
				return this.m_HideMobileInput;
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
				{
					return MGUIInputField.m_Keyboard.get_text();
				}
				return this.m_Text;
			}
			set
			{
				if (this.text == value)
				{
					return;
				}
				this.m_Text = value;
				if (MGUIInputField.m_Keyboard != null)
				{
					MGUIInputField.m_Keyboard.set_text(this.text);
				}
				if (this.m_CaretPosition > this.m_Text.Length)
				{
					this.m_CaretPosition = (this.m_CaretSelectPosition = this.m_Text.Length);
				}
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
				if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
				{
					this.SetCaretActive();
				}
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
				if (SetPropertyUtility.SetStruct<MGUIInputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
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
				if (SetPropertyUtility.SetStruct<MGUIInputField.LineType>(ref this.m_LineType, value))
				{
					this.SetToCustomIfContentTypeIsNot(new MGUIInputField.ContentType[]
					{
						MGUIInputField.ContentType.Standard,
						MGUIInputField.ContentType.Autocorrected
					});
				}
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
				if (SetPropertyUtility.SetStruct<MGUIInputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
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
				if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
				{
					this.SetToCustom();
				}
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
				if (SetPropertyUtility.SetStruct<MGUIInputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
				}
			}
		}

		public bool multiLine
		{
			get
			{
				return this.m_LineType == MGUIInputField.LineType.MultiLineNewline || this.lineType == MGUIInputField.LineType.MultiLineSubmit;
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
				return textEditor.content.get_text();
			}
			set
			{
				TextEditor textEditor = new TextEditor();
				textEditor.content = new GUIContent(value);
				textEditor.OnFocus();
				textEditor.Copy();
			}
		}

		protected MGUIInputField()
		{
		}

		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
			}
			else if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_DrawStart = 0;
			this.m_DrawEnd = this.m_Text.Length;
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.UpdateLabel();
			}
		}

		protected override void OnDisable()
		{
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			base.OnDisable();
		}

		[DebuggerHidden]
		private IEnumerator CaretBlink()
		{
			MGUIInputField.<CaretBlink>c__Iterator8 <CaretBlink>c__Iterator = new MGUIInputField.<CaretBlink>c__Iterator8();
			<CaretBlink>c__Iterator.<>f__this = this;
			return <CaretBlink>c__Iterator;
		}

		private void SetCaretVisible()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_CaretVisible = true;
			this.m_BlinkStartTime = Time.get_unscaledTime();
			this.SetCaretActive();
		}

		private void SetCaretActive()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			if (this.m_CaretBlinkRate > 0f)
			{
				if (this.m_BlickCoroutine == null)
				{
					this.m_BlickCoroutine = base.StartCoroutine(this.CaretBlink());
				}
			}
			else
			{
				this.m_CaretVisible = true;
			}
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
			{
				return;
			}
			this.AssignPositioningIfNeeded();
			if (MGUIInputField.m_Keyboard == null || !MGUIInputField.m_Keyboard.get_active())
			{
				if (MGUIInputField.m_Keyboard != null && MGUIInputField.m_Keyboard.get_wasCanceled())
				{
					this.m_WasCanceled = true;
				}
				this.OnDeselect(null);
				return;
			}
			string text = MGUIInputField.m_Keyboard.get_text();
			if (this.m_Text != text)
			{
				this.m_Text = string.Empty;
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					if (c == '\r' || c == '\u0003')
					{
						c = '\n';
					}
					if (this.onValidateInput != null)
					{
						c = this.onValidateInput(this.m_Text, this.m_Text.Length, c);
					}
					else if (this.characterValidation != MGUIInputField.CharacterValidation.None)
					{
						c = this.Validate(this.m_Text, this.m_Text.Length, c);
					}
					if (this.lineType == MGUIInputField.LineType.MultiLineSubmit && c == '\n')
					{
						MGUIInputField.m_Keyboard.set_text(this.m_Text);
						this.OnDeselect(null);
						return;
					}
					if (c != '\0')
					{
						this.m_Text += c;
					}
				}
				if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
				{
					this.m_Text = this.m_Text.Substring(0, this.characterLimit);
				}
				int length = this.m_Text.Length;
				this.caretSelectPos = length;
				this.caretPosition = length;
				if (this.m_Text != text)
				{
					MGUIInputField.m_Keyboard.set_text(this.m_Text);
				}
				this.SendOnValueChangedAndUpdateLabel();
			}
			if (MGUIInputField.m_Keyboard.get_done())
			{
				if (MGUIInputField.m_Keyboard.get_wasCanceled())
				{
					this.m_WasCanceled = true;
				}
				this.OnDeselect(null);
			}
		}

		public Vector2 ScreenToLocal(Vector2 screen)
		{
			Canvas canvas = this.m_TextComponent.get_canvas();
			if (canvas == null)
			{
				return screen;
			}
			Vector3 vector = Vector3.get_zero();
			if (canvas.get_renderMode() == null)
			{
				vector = this.m_TextComponent.get_transform().InverseTransformPoint(screen);
			}
			else if (canvas.get_worldCamera() != null)
			{
				Ray ray = canvas.get_worldCamera().ScreenPointToRay(screen);
				Plane plane = new Plane(this.m_TextComponent.get_transform().get_forward(), this.m_TextComponent.get_transform().get_position());
				float num;
				plane.Raycast(ray, ref num);
				vector = this.m_TextComponent.get_transform().InverseTransformPoint(ray.GetPoint(num));
			}
			return new Vector2(vector.x, vector.y);
		}

		private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
		{
			if (!this.multiLine)
			{
				return 0;
			}
			float num = this.m_TextComponent.get_rectTransform().get_rect().get_yMax();
			if (pos.y > num)
			{
				return -1;
			}
			for (int i = 0; i < generator.get_lineCount(); i++)
			{
				float num2 = (float)generator.get_lines()[i].height / this.m_TextComponent.get_pixelsPerUnit();
				if (pos.y <= num && pos.y > num - num2)
				{
					return i;
				}
				num -= num2;
			}
			return generator.get_lineCount();
		}

		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			TextGenerator cachedTextGenerator = this.m_TextComponent.get_cachedTextGenerator();
			if (cachedTextGenerator.get_lineCount() == 0)
			{
				return 0;
			}
			int unclampedCharacterLineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
			if (unclampedCharacterLineFromPosition < 0)
			{
				return 0;
			}
			if (unclampedCharacterLineFromPosition >= cachedTextGenerator.get_lineCount())
			{
				return cachedTextGenerator.get_characterCountVisible();
			}
			int startCharIdx = cachedTextGenerator.get_lines()[unclampedCharacterLineFromPosition].startCharIdx;
			int lineEndPosition = MGUIInputField.GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
			for (int i = startCharIdx; i < lineEndPosition; i++)
			{
				if (i >= cachedTextGenerator.get_characterCountVisible())
				{
					break;
				}
				UICharInfo uICharInfo = cachedTextGenerator.get_characters()[i];
				Vector2 vector = uICharInfo.cursorPos / this.m_TextComponent.get_pixelsPerUnit();
				float num = pos.x - vector.x;
				float num2 = vector.x + uICharInfo.charWidth / this.m_TextComponent.get_pixelsPerUnit() - pos.x;
				if (num < num2)
				{
					return i;
				}
			}
			return lineEndPosition;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.get_button() == null && this.m_TextComponent != null && MGUIInputField.m_Keyboard == null;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = true;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.get_rectTransform(), eventData.get_position(), eventData.get_pressEventCamera(), ref pos);
			this.caretSelectPos = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
			this.MarkGeometryAsDirty();
			this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textComponent.get_rectTransform(), eventData.get_position(), eventData.get_pressEventCamera());
			if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
			{
				this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
			}
			eventData.Use();
		}

		[DebuggerHidden]
		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			MGUIInputField.<MouseDragOutsideRect>c__Iterator9 <MouseDragOutsideRect>c__Iterator = new MGUIInputField.<MouseDragOutsideRect>c__Iterator9();
			<MouseDragOutsideRect>c__Iterator.eventData = eventData;
			<MouseDragOutsideRect>c__Iterator.<$>eventData = eventData;
			<MouseDragOutsideRect>c__Iterator.<>f__this = this;
			return <MouseDragOutsideRect>c__Iterator;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = false;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			EventSystem.get_current().SetSelectedGameObject(base.get_gameObject(), eventData);
			bool allowInput = this.m_AllowInput;
			base.OnPointerDown(eventData);
			if (!this.InPlaceEditing() && (MGUIInputField.m_Keyboard == null || !MGUIInputField.m_Keyboard.get_active()))
			{
				this.OnSelect(eventData);
				return;
			}
			if (allowInput)
			{
				Vector2 pos = this.ScreenToLocal(eventData.get_position());
				int num = this.GetCharacterIndexFromPosition(pos) + this.m_DrawStart;
				this.caretPosition = num;
				this.caretSelectPos = num;
			}
			this.UpdateLabel();
			eventData.Use();
		}

		protected MGUIInputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.get_modifiers();
			RuntimePlatform platform = Application.get_platform();
			bool flag = platform == null || platform == 1 || platform == 3;
			bool flag2 = (!flag) ? ((modifiers & 2) != 0) : ((modifiers & 8) != 0);
			bool shift = (modifiers & 1) != 0;
			KeyCode keyCode = evt.get_keyCode();
			switch (keyCode)
			{
			case 271:
				goto IL_187;
			case 272:
			case 277:
				IL_84:
				switch (keyCode)
				{
				case 97:
					if (flag2)
					{
						this.SelectAll();
						return MGUIInputField.EditState.Continue;
					}
					goto IL_1A3;
				case 98:
					IL_9A:
					switch (keyCode)
					{
					case 118:
						if (flag2)
						{
							this.Append(MGUIInputField.clipboard);
							return MGUIInputField.EditState.Continue;
						}
						goto IL_1A3;
					case 119:
						IL_B0:
						if (keyCode == 8)
						{
							this.Backspace();
							return MGUIInputField.EditState.Continue;
						}
						if (keyCode == 13)
						{
							goto IL_187;
						}
						if (keyCode == 27)
						{
							this.m_WasCanceled = true;
							return MGUIInputField.EditState.Finish;
						}
						if (keyCode != 127)
						{
							goto IL_1A3;
						}
						this.ForwardSpace();
						return MGUIInputField.EditState.Continue;
					case 120:
						if (flag2)
						{
							MGUIInputField.clipboard = this.GetSelectedString();
							this.Delete();
							return MGUIInputField.EditState.Continue;
						}
						goto IL_1A3;
					}
					goto IL_B0;
				case 99:
					if (flag2)
					{
						MGUIInputField.clipboard = this.GetSelectedString();
						return MGUIInputField.EditState.Continue;
					}
					goto IL_1A3;
				}
				goto IL_9A;
			case 273:
				this.MoveUp(shift);
				return MGUIInputField.EditState.Continue;
			case 274:
				this.MoveDown(shift);
				return MGUIInputField.EditState.Continue;
			case 275:
				this.MoveRight(shift, flag2);
				return MGUIInputField.EditState.Continue;
			case 276:
				this.MoveLeft(shift, flag2);
				return MGUIInputField.EditState.Continue;
			case 278:
				this.MoveTextStart(shift);
				return MGUIInputField.EditState.Continue;
			case 279:
				this.MoveTextEnd(shift);
				return MGUIInputField.EditState.Continue;
			}
			goto IL_84;
			IL_187:
			if (this.lineType != MGUIInputField.LineType.MultiLineNewline)
			{
				return MGUIInputField.EditState.Finish;
			}
			IL_1A3:
			if (!this.multiLine && evt.get_character() == '\t')
			{
				return MGUIInputField.EditState.Continue;
			}
			char c = evt.get_character();
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (this.IsValidChar(c))
			{
				this.Append(c);
			}
			if (c == '\0' && Input.get_compositionString().Length > 0)
			{
				this.UpdateLabel();
			}
			return MGUIInputField.EditState.Continue;
		}

		private bool IsValidChar(char c)
		{
			return c != '\u007f' && (c == '\t' || c == '\n' || this.m_TextComponent.get_font().HasCharacter(c));
		}

		public void ProcessEvent(Event e)
		{
			this.KeyPressed(e);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (this.m_ShouldActivateNextUpdate)
			{
				this.ActivateInputField();
				this.m_ShouldActivateNextUpdate = false;
				return;
			}
			if (!this.isFocused)
			{
				return;
			}
			bool flag = false;
			while (Event.PopEvent(this.m_ProcessingEvent))
			{
				if (this.m_ProcessingEvent.get_rawType() == 4)
				{
					flag = true;
					MGUIInputField.EditState editState = this.KeyPressed(this.m_ProcessingEvent);
					if (editState == MGUIInputField.EditState.Finish)
					{
						this.DeactivateInputField();
						break;
					}
				}
			}
			if (flag)
			{
				this.UpdateLabel();
			}
			eventData.Use();
		}

		private string GetSelectedString()
		{
			if (!this.hasSelection)
			{
				return string.Empty;
			}
			int num = this.caretPosition;
			int num2 = this.caretSelectPos;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = num; i < num2; i++)
			{
				stringBuilder.Append(this.text[i]);
			}
			return stringBuilder.ToString();
		}

		private int FindtNextWordBegin()
		{
			if (this.caretSelectPos + 1 >= this.text.Length)
			{
				return this.text.Length;
			}
			int num = this.text.IndexOfAny(MGUIInputField.kSeparators, this.caretSelectPos + 1);
			if (num == -1)
			{
				num = this.text.Length;
			}
			else
			{
				num++;
			}
			return num;
		}

		private void MoveRight(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Max(this.caretPosition, this.caretSelectPos);
				this.caretSelectPos = num;
				this.caretPosition = num;
				return;
			}
			int num2;
			if (ctrl)
			{
				num2 = this.FindtNextWordBegin();
			}
			else
			{
				num2 = this.caretSelectPos + 1;
			}
			if (shift)
			{
				this.caretSelectPos = num2;
			}
			else
			{
				int num = num2;
				this.caretPosition = num;
				this.caretSelectPos = num;
			}
		}

		private int FindtPrevWordBegin()
		{
			if (this.caretSelectPos - 2 < 0)
			{
				return 0;
			}
			int num = this.text.LastIndexOfAny(MGUIInputField.kSeparators, this.caretSelectPos - 2);
			if (num == -1)
			{
				num = 0;
			}
			else
			{
				num++;
			}
			return num;
		}

		private void MoveLeft(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				int num = Mathf.Min(this.caretPosition, this.caretSelectPos);
				this.caretSelectPos = num;
				this.caretPosition = num;
				return;
			}
			int num2;
			if (ctrl)
			{
				num2 = this.FindtPrevWordBegin();
			}
			else
			{
				num2 = this.caretSelectPos - 1;
			}
			if (shift)
			{
				this.caretSelectPos = num2;
			}
			else
			{
				int num = num2;
				this.caretPosition = num;
				this.caretSelectPos = num;
			}
		}

		private int DetermineCharacterLine(int charPos, TextGenerator generator)
		{
			if (!this.multiLine)
			{
				return 0;
			}
			for (int i = 0; i < generator.get_lineCount() - 1; i++)
			{
				if (generator.get_lines()[i + 1].startCharIdx > charPos)
				{
					return i;
				}
			}
			return generator.get_lineCount() - 1;
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.cachedInputTextGenerator.get_characterCountVisible())
			{
				return 0;
			}
			UICharInfo uICharInfo = this.cachedInputTextGenerator.get_characters()[originalPos];
			int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
			if (num - 1 < 0)
			{
				return (!goToFirstChar) ? originalPos : 0;
			}
			int num2 = this.cachedInputTextGenerator.get_lines()[num].startCharIdx - 1;
			for (int i = this.cachedInputTextGenerator.get_lines()[num - 1].startCharIdx; i < num2; i++)
			{
				if (this.cachedInputTextGenerator.get_characters()[i].cursorPos.x >= uICharInfo.cursorPos.x)
				{
					return i;
				}
			}
			return num2;
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.cachedInputTextGenerator.get_characterCountVisible())
			{
				return this.text.Length;
			}
			UICharInfo uICharInfo = this.cachedInputTextGenerator.get_characters()[originalPos];
			int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
			if (num + 1 >= this.cachedInputTextGenerator.get_lineCount())
			{
				return (!goToLastChar) ? originalPos : this.text.Length;
			}
			int lineEndPosition = MGUIInputField.GetLineEndPosition(this.cachedInputTextGenerator, num + 1);
			for (int i = this.cachedInputTextGenerator.get_lines()[num + 1].startCharIdx; i < lineEndPosition; i++)
			{
				if (this.cachedInputTextGenerator.get_characters()[i].cursorPos.x >= uICharInfo.cursorPos.x)
				{
					return i;
				}
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
			int num2 = (!this.multiLine) ? this.text.Length : this.LineDownCharacterPosition(this.caretSelectPos, goToLastChar);
			if (shift)
			{
				this.caretSelectPos = num2;
			}
			else
			{
				int num = num2;
				this.caretSelectPos = num;
				this.caretPosition = num;
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
			int num2 = (!this.multiLine) ? 0 : this.LineUpCharacterPosition(this.caretSelectPos, goToFirstChar);
			if (shift)
			{
				this.caretSelectPos = num2;
			}
			else
			{
				int num = num2;
				this.caretPosition = num;
				this.caretSelectPos = num;
			}
		}

		private void Delete()
		{
			if (this.caretPosition == this.caretSelectPos)
			{
				return;
			}
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
			else if (this.caretPosition < this.text.Length)
			{
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
			else if (this.caretPosition > 0)
			{
				this.m_Text = this.text.Remove(this.caretPosition - 1, 1);
				int num = this.caretPosition - 1;
				this.caretPosition = num;
				this.caretSelectPos = num;
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		private void Insert(char c)
		{
			string text = c.ToString();
			this.Delete();
			if (this.characterLimit > 0 && this.text.Length >= this.characterLimit)
			{
				return;
			}
			this.m_Text = this.text.Insert(this.m_CaretPosition, text);
			this.caretSelectPos = (this.caretPosition += text.Length);
			this.SendOnValueChanged();
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			this.SendOnValueChanged();
			this.UpdateLabel();
		}

		private void SendOnValueChanged()
		{
			if (this.onValueChange != null)
			{
				this.onValueChange.Invoke(this.text);
			}
		}

		protected void SendOnSubmit()
		{
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
			}
		}

		protected virtual void Append(string input)
		{
			if (!this.InPlaceEditing())
			{
				return;
			}
			int i = 0;
			int length = input.Length;
			while (i < length)
			{
				char c = input[i];
				if (c >= ' ')
				{
					this.Append(c);
				}
				i++;
			}
		}

		protected virtual void Append(char input)
		{
			if (!this.InPlaceEditing())
			{
				return;
			}
			if (this.onValidateInput != null)
			{
				input = this.onValidateInput(this.text, this.caretPosition, input);
			}
			else if (this.characterValidation != MGUIInputField.CharacterValidation.None)
			{
				input = this.Validate(this.text, this.caretPosition, input);
			}
			if (input == '\0')
			{
				return;
			}
			this.Insert(input);
		}

		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.get_font() != null)
			{
				string text;
				if (Input.get_compositionString().Length > 0)
				{
					text = this.text.Substring(0, this.m_CaretPosition) + Input.get_compositionString() + this.text.Substring(this.m_CaretPosition);
				}
				else
				{
					text = this.text;
				}
				string text2;
				if (this.inputType == MGUIInputField.InputType.Password)
				{
					text2 = new string(this.asteriskChar, text.Length);
				}
				else
				{
					text2 = text;
				}
				bool flag = string.IsNullOrEmpty(text);
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.set_enabled(flag);
				}
				if (!this.m_AllowInput)
				{
					this.m_DrawStart = 0;
					this.m_DrawEnd = this.m_Text.Length;
				}
				if (!flag)
				{
					Vector2 size = this.m_TextComponent.get_rectTransform().get_rect().get_size();
					TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(size);
					generationSettings.generateOutOfBounds = true;
					this.cachedInputTextGenerator.Populate(text2, generationSettings);
					this.SetDrawRangeToContainCaretPosition(this.cachedInputTextGenerator, this.caretSelectPos, ref this.m_DrawStart, ref this.m_DrawEnd);
					text2 = text2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, text2.Length) - this.m_DrawStart);
					this.SetCaretVisible();
				}
				this.m_TextComponent.set_text(text2);
				this.MarkGeometryAsDirty();
			}
		}

		private bool IsSelectionVisible()
		{
			return this.m_DrawStart <= this.caretPosition && this.m_DrawStart <= this.caretSelectPos && this.m_DrawEnd >= this.caretPosition && this.m_DrawEnd >= this.caretSelectPos;
		}

		private static int GetLineStartPosition(TextGenerator gen, int line)
		{
			line = Mathf.Clamp(line, 0, gen.get_lines().Count - 1);
			return gen.get_lines()[line].startCharIdx;
		}

		private static int GetLineEndPosition(TextGenerator gen, int line)
		{
			line = Mathf.Max(line, 0);
			if (line + 1 < gen.get_lines().Count)
			{
				return gen.get_lines()[line + 1].startCharIdx;
			}
			return gen.get_characterCountVisible();
		}

		private void SetDrawRangeToContainCaretPosition(TextGenerator gen, int caretPos, ref int drawStart, ref int drawEnd)
		{
			Vector2 size = gen.get_rectExtents().get_size();
			if (this.multiLine)
			{
				IList<UILineInfo> lines = gen.get_lines();
				int num = this.DetermineCharacterLine(caretPos, gen);
				int num2 = (int)size.y;
				if (drawEnd <= caretPos)
				{
					drawEnd = MGUIInputField.GetLineEndPosition(gen, num);
					for (int i = num; i >= 0; i--)
					{
						num2 -= lines[i].height;
						if (num2 < 0)
						{
							break;
						}
						drawStart = MGUIInputField.GetLineStartPosition(gen, i);
					}
				}
				else
				{
					if (drawStart > caretPos)
					{
						drawStart = MGUIInputField.GetLineStartPosition(gen, num);
					}
					int num3 = this.DetermineCharacterLine(drawStart, gen);
					int num4 = num3;
					drawEnd = MGUIInputField.GetLineEndPosition(gen, num4);
					num2 -= lines[num4].height;
					while (true)
					{
						if (num4 < lines.Count - 1)
						{
							num4++;
							if (num2 < lines[num4].height)
							{
								break;
							}
							drawEnd = MGUIInputField.GetLineEndPosition(gen, num4);
							num2 -= lines[num4].height;
						}
						else
						{
							if (num3 <= 0)
							{
								break;
							}
							num3--;
							if (num2 < lines[num3].height)
							{
								break;
							}
							drawStart = MGUIInputField.GetLineStartPosition(gen, num3);
							num2 -= lines[num3].height;
						}
					}
				}
			}
			else
			{
				float num5 = size.x;
				IList<UICharInfo> characters = gen.get_characters();
				if (drawEnd <= caretPos)
				{
					drawEnd = Mathf.Min(caretPos, gen.get_characterCountVisible());
					drawStart = 0;
					for (int j = drawEnd; j > 0; j--)
					{
						num5 -= characters[j - 1].charWidth;
						if (num5 < 0f)
						{
							drawStart = j;
							break;
						}
					}
				}
				else
				{
					if (drawStart > caretPos)
					{
						drawStart = caretPos;
					}
					drawEnd = gen.get_characterCountVisible();
					for (int k = drawStart; k < gen.get_characterCountVisible(); k++)
					{
						num5 -= characters[k].charWidth;
						if (num5 < 0f)
						{
							drawEnd = k;
							break;
						}
					}
				}
			}
		}

		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == 4)
			{
				this.UpdateGeometry();
			}
		}

		private void UpdateGeometry()
		{
			if (!this.shouldHideMobileInput)
			{
				return;
			}
			if (this.m_CachedInputRenderer == null && this.m_TextComponent != null)
			{
				GameObject gameObject = new GameObject(base.get_transform().get_name() + " Input Caret");
				gameObject.set_hideFlags(4);
				gameObject.get_transform().SetParent(this.m_TextComponent.get_transform().get_parent());
				gameObject.get_transform().SetAsFirstSibling();
				gameObject.set_layer(base.get_gameObject().get_layer());
				this.caretRectTrans = gameObject.AddComponent<RectTransform>();
				this.m_CachedInputRenderer = gameObject.AddComponent<CanvasRenderer>();
				this.m_CachedInputRenderer.SetMaterial(Graphic.get_defaultGraphicMaterial(), null);
				this.AssignPositioningIfNeeded();
			}
			if (this.m_CachedInputRenderer == null)
			{
				return;
			}
			this.OnFillVBO(this.m_Vbo);
			if (this.m_Vbo.Count == 0)
			{
				this.m_CachedInputRenderer.SetVertices(null, 0);
			}
			else
			{
				this.m_CachedInputRenderer.SetVertices(this.m_Vbo.ToArray(), this.m_Vbo.Count);
			}
			this.m_Vbo.Clear();
		}

		private void AssignPositioningIfNeeded()
		{
			if (this.m_TextComponent != null && this.caretRectTrans != null && (this.caretRectTrans.get_localPosition() != this.m_TextComponent.get_rectTransform().get_localPosition() || this.caretRectTrans.get_localRotation() != this.m_TextComponent.get_rectTransform().get_localRotation() || this.caretRectTrans.get_localScale() != this.m_TextComponent.get_rectTransform().get_localScale() || this.caretRectTrans.get_anchorMin() != this.m_TextComponent.get_rectTransform().get_anchorMin() || this.caretRectTrans.get_anchorMax() != this.m_TextComponent.get_rectTransform().get_anchorMax() || this.caretRectTrans.get_anchoredPosition() != this.m_TextComponent.get_rectTransform().get_anchoredPosition() || this.caretRectTrans.get_sizeDelta() != this.m_TextComponent.get_rectTransform().get_sizeDelta() || this.caretRectTrans.get_pivot() != this.m_TextComponent.get_rectTransform().get_pivot()))
			{
				this.caretRectTrans.set_localPosition(this.m_TextComponent.get_rectTransform().get_localPosition());
				this.caretRectTrans.set_localRotation(this.m_TextComponent.get_rectTransform().get_localRotation());
				this.caretRectTrans.set_localScale(this.m_TextComponent.get_rectTransform().get_localScale());
				this.caretRectTrans.set_anchorMin(this.m_TextComponent.get_rectTransform().get_anchorMin());
				this.caretRectTrans.set_anchorMax(this.m_TextComponent.get_rectTransform().get_anchorMax());
				this.caretRectTrans.set_anchoredPosition(this.m_TextComponent.get_rectTransform().get_anchoredPosition());
				this.caretRectTrans.set_sizeDelta(this.m_TextComponent.get_rectTransform().get_sizeDelta());
				this.caretRectTrans.set_pivot(this.m_TextComponent.get_rectTransform().get_pivot());
			}
		}

		private void OnFillVBO(List<UIVertex> vbo)
		{
			if (!this.isFocused)
			{
				return;
			}
			Rect rect = this.m_TextComponent.get_rectTransform().get_rect();
			Vector2 size = rect.get_size();
			Vector2 textAnchorPivot = Text.GetTextAnchorPivot(this.m_TextComponent.get_alignment());
			Vector2 zero = Vector2.get_zero();
			zero.x = Mathf.Lerp(rect.get_xMin(), rect.get_xMax(), textAnchorPivot.x);
			zero.y = Mathf.Lerp(rect.get_yMin(), rect.get_yMax(), textAnchorPivot.y);
			Vector2 vector = this.m_TextComponent.PixelAdjustPoint(zero);
			Vector2 roundingOffset = vector - zero + Vector2.Scale(size, textAnchorPivot);
			roundingOffset.x -= Mathf.Floor(0.5f + roundingOffset.x);
			roundingOffset.y -= Mathf.Floor(0.5f + roundingOffset.y);
			if (!this.hasSelection)
			{
				this.GenerateCursor(vbo, roundingOffset);
			}
			else
			{
				this.GenerateHightlight(vbo, roundingOffset);
			}
		}

		private void GenerateCursor(List<UIVertex> vbo, Vector2 roundingOffset)
		{
			if (!this.m_CaretVisible)
			{
				return;
			}
			if (this.m_CursorVerts == null)
			{
				this.CreateCursorVerts();
			}
			float num = 1f;
			float num2 = (float)this.m_TextComponent.get_fontSize();
			int num3 = Mathf.Max(0, this.caretPosition - this.m_DrawStart);
			TextGenerator cachedTextGenerator = this.m_TextComponent.get_cachedTextGenerator();
			if (cachedTextGenerator == null)
			{
				return;
			}
			if (this.m_TextComponent.get_resizeTextForBestFit())
			{
				num2 = (float)cachedTextGenerator.get_fontSizeUsedForBestFit() / this.m_TextComponent.get_pixelsPerUnit();
			}
			Vector2 zero = Vector2.get_zero();
			if (cachedTextGenerator.get_characterCountVisible() + 1 > num3 || num3 == 0)
			{
				zero.x = cachedTextGenerator.get_characters()[num3].cursorPos.x;
			}
			zero.x /= this.m_TextComponent.get_pixelsPerUnit();
			if (zero.x > this.m_TextComponent.get_rectTransform().get_rect().get_xMax())
			{
				zero.x = this.m_TextComponent.get_rectTransform().get_rect().get_xMax();
			}
			int endLine = this.DetermineCharacterLine(num3, cachedTextGenerator);
			float num4 = this.SumLineHeights(endLine, cachedTextGenerator);
			zero.y = this.m_TextComponent.get_rectTransform().get_rect().get_yMax() - num4 / this.m_TextComponent.get_pixelsPerUnit();
			this.m_CursorVerts[0].position = new Vector3(zero.x, zero.y - num2, 0f);
			this.m_CursorVerts[1].position = new Vector3(zero.x + num, zero.y - num2, 0f);
			this.m_CursorVerts[2].position = new Vector3(zero.x + num, zero.y, 0f);
			this.m_CursorVerts[3].position = new Vector3(zero.x, zero.y, 0f);
			if (roundingOffset != Vector2.get_zero())
			{
				for (int i = 0; i < this.m_CursorVerts.Length; i++)
				{
					UIVertex item = this.m_CursorVerts[i];
					item.position.x = item.position.x + roundingOffset.x;
					item.position.y = item.position.y + roundingOffset.y;
					vbo.Add(item);
				}
			}
			else
			{
				for (int j = 0; j < this.m_CursorVerts.Length; j++)
				{
					vbo.Add(this.m_CursorVerts[j]);
				}
			}
			zero.y = (float)Screen.get_height() - zero.y;
			Input.set_compositionCursorPos(zero);
		}

		private void CreateCursorVerts()
		{
			this.m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i] = UIVertex.simpleVert;
				this.m_CursorVerts[i].color = this.m_TextComponent.get_color();
				this.m_CursorVerts[i].uv0 = Vector2.get_zero();
			}
		}

		private float SumLineHeights(int endLine, TextGenerator generator)
		{
			float num = 0f;
			for (int i = 0; i < endLine; i++)
			{
				num += (float)generator.get_lines()[i].height;
			}
			return num;
		}

		private void GenerateHightlight(List<UIVertex> vbo, Vector2 roundingOffset)
		{
			int num = Mathf.Max(0, this.caretPosition - this.m_DrawStart);
			int num2 = Mathf.Max(0, this.caretSelectPos - this.m_DrawStart);
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			num2--;
			TextGenerator cachedTextGenerator = this.m_TextComponent.get_cachedTextGenerator();
			int num4 = this.DetermineCharacterLine(num, cachedTextGenerator);
			float num5 = (float)this.m_TextComponent.get_fontSize();
			if (this.m_TextComponent.get_resizeTextForBestFit())
			{
				num5 = (float)cachedTextGenerator.get_fontSizeUsedForBestFit() / this.m_TextComponent.get_pixelsPerUnit();
			}
			if (this.cachedInputTextGenerator != null && this.cachedInputTextGenerator.get_lines().Count > 0)
			{
				num5 = (float)this.cachedInputTextGenerator.get_lines()[0].height;
			}
			if (this.m_TextComponent.get_resizeTextForBestFit() && this.cachedInputTextGenerator != null)
			{
				num5 = (float)this.cachedInputTextGenerator.get_fontSizeUsedForBestFit();
			}
			int lineEndPosition = MGUIInputField.GetLineEndPosition(cachedTextGenerator, num4);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.get_zero();
			simpleVert.color = this.selectionColor;
			int num6 = num;
			while (num6 <= num2 && num6 < cachedTextGenerator.get_characterCountVisible())
			{
				if (num6 + 1 == lineEndPosition || num6 == num2)
				{
					UICharInfo uICharInfo = cachedTextGenerator.get_characters()[num];
					UICharInfo uICharInfo2 = cachedTextGenerator.get_characters()[num6];
					float num7 = this.SumLineHeights(num4, cachedTextGenerator);
					Vector2 vector = new Vector2(uICharInfo.cursorPos.x / this.m_TextComponent.get_pixelsPerUnit(), this.m_TextComponent.get_rectTransform().get_rect().get_yMax() - num7 / this.m_TextComponent.get_pixelsPerUnit());
					Vector2 vector2 = new Vector2((uICharInfo2.cursorPos.x + uICharInfo2.charWidth) / this.m_TextComponent.get_pixelsPerUnit(), vector.y - num5 / this.m_TextComponent.get_pixelsPerUnit());
					if (vector2.x > this.m_TextComponent.get_rectTransform().get_rect().get_xMax() || vector2.x < this.m_TextComponent.get_rectTransform().get_rect().get_xMin())
					{
						vector2.x = this.m_TextComponent.get_rectTransform().get_rect().get_xMax();
					}
					simpleVert.position = new Vector3(vector.x, vector2.y, 0f) + roundingOffset;
					vbo.Add(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector2.y, 0f) + roundingOffset;
					vbo.Add(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector.y, 0f) + roundingOffset;
					vbo.Add(simpleVert);
					simpleVert.position = new Vector3(vector.x, vector.y, 0f) + roundingOffset;
					vbo.Add(simpleVert);
					num = num6 + 1;
					num4++;
					lineEndPosition = MGUIInputField.GetLineEndPosition(cachedTextGenerator, num4);
				}
				num6++;
			}
		}

		protected char Validate(string text, int pos, char ch)
		{
			if (this.characterValidation == MGUIInputField.CharacterValidation.None || !base.get_enabled())
			{
				return ch;
			}
			if (this.characterValidation == MGUIInputField.CharacterValidation.Integer || this.characterValidation == MGUIInputField.CharacterValidation.Decimal)
			{
				if (pos != 0 || text.Length <= 0 || text[0] != '-')
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && pos == 0)
					{
						return ch;
					}
					if (ch == '.' && this.characterValidation == MGUIInputField.CharacterValidation.Decimal && !text.Contains("."))
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == MGUIInputField.CharacterValidation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (this.characterValidation == MGUIInputField.CharacterValidation.Name)
			{
				char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
				char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && c == ' ')
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && c != ' ' && c != '\'')
					{
						return char.ToLower(ch);
					}
					return ch;
				}
				else if (ch == '\'')
				{
					if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
				}
				else if (ch == ' ' && c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
				{
					return ch;
				}
			}
			else if (this.characterValidation == MGUIInputField.CharacterValidation.EmailAddress)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '@' && text.IndexOf('@') == -1)
				{
					return ch;
				}
				if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
				{
					return ch;
				}
				if (ch == '.')
				{
					char c3 = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
					char c4 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
					if (c3 != '.' && c4 != '.')
					{
						return ch;
					}
				}
			}
			return '\0';
		}

		public void ActivateInputField()
		{
			if (this.m_AllowInput)
			{
				return;
			}
			if (this.m_TextComponent == null || this.m_TextComponent.get_font() == null || !this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (this.isFocused)
			{
				if (MGUIInputField.m_Keyboard != null && !MGUIInputField.m_Keyboard.get_active())
				{
					MGUIInputField.m_Keyboard.set_active(true);
					MGUIInputField.m_Keyboard.set_text(this.m_Text);
				}
				this.m_ShouldActivateNextUpdate = true;
				return;
			}
			if (TouchScreenKeyboard.get_isSupported())
			{
				if (Input.get_touchSupported())
				{
					TouchScreenKeyboard.set_hideInput(this.shouldHideMobileInput);
				}
				MGUIInputField.m_Keyboard = ((this.inputType != MGUIInputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == MGUIInputField.InputType.AutoCorrect, this.multiLine) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true));
			}
			else
			{
				Input.set_imeCompositionMode(1);
				this.OnFocus();
			}
			this.m_AllowInput = true;
			this.m_OriginalText = this.text;
			this.m_WasCanceled = false;
			this.SetCaretVisible();
			this.UpdateLabel();
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.ActivateInputField();
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.get_button() != null)
			{
				return;
			}
			this.ActivateInputField();
		}

		public void DeactivateInputField()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_AllowInput = false;
			this.m_HasDoneFocusTransition = false;
			if (this.m_TextComponent != null && this.IsActive() && this.IsInteractable())
			{
				if (this.m_WasCanceled)
				{
					this.text = this.m_OriginalText;
				}
				if (MGUIInputField.m_Keyboard != null)
				{
					MGUIInputField.m_Keyboard.set_active(false);
					MGUIInputField.m_Keyboard = null;
				}
				this.m_CaretPosition = (this.m_CaretSelectPosition = 0);
				this.SendOnSubmit();
				Input.set_imeCompositionMode(0);
			}
			this.MarkGeometryAsDirty();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			this.DeactivateInputField();
			base.OnDeselect(eventData);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (!this.isFocused)
			{
				this.m_ShouldActivateNextUpdate = true;
			}
		}

		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case MGUIInputField.ContentType.Standard:
				this.m_InputType = MGUIInputField.InputType.Standard;
				this.m_KeyboardType = 0;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.None;
				return;
			case MGUIInputField.ContentType.Autocorrected:
				this.m_InputType = MGUIInputField.InputType.AutoCorrect;
				this.m_KeyboardType = 0;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.None;
				return;
			case MGUIInputField.ContentType.IntegerNumber:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Standard;
				this.m_KeyboardType = 4;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.Integer;
				return;
			case MGUIInputField.ContentType.DecimalNumber:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Standard;
				this.m_KeyboardType = 2;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.Decimal;
				return;
			case MGUIInputField.ContentType.Alphanumeric:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Standard;
				this.m_KeyboardType = 1;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.Alphanumeric;
				return;
			case MGUIInputField.ContentType.Name:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Standard;
				this.m_KeyboardType = 0;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.Name;
				return;
			case MGUIInputField.ContentType.EmailAddress:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Standard;
				this.m_KeyboardType = 7;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.EmailAddress;
				return;
			case MGUIInputField.ContentType.Password:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Password;
				this.m_KeyboardType = 0;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.None;
				return;
			case MGUIInputField.ContentType.Pin:
				this.m_LineType = MGUIInputField.LineType.SingleLine;
				this.m_InputType = MGUIInputField.InputType.Password;
				this.m_KeyboardType = 4;
				this.m_CharacterValidation = MGUIInputField.CharacterValidation.Integer;
				return;
			default:
				return;
			}
		}

		private void SetToCustomIfContentTypeIsNot(params MGUIInputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType == MGUIInputField.ContentType.Custom)
			{
				return;
			}
			for (int i = 0; i < allowedContentTypes.Length; i++)
			{
				if (this.contentType == allowedContentTypes[i])
				{
					return;
				}
			}
			this.contentType = MGUIInputField.ContentType.Custom;
		}

		private void SetToCustom()
		{
			if (this.contentType == MGUIInputField.ContentType.Custom)
			{
				return;
			}
			this.contentType = MGUIInputField.ContentType.Custom;
		}

		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.m_HasDoneFocusTransition)
			{
				state = 1;
			}
			else if (state == 2)
			{
				this.m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
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
