using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta
{
	public class MetaKeyboard : MetaSingleton<MetaKeyboard>
	{
		private Toggle _shiftKey;

		private Toggle _capsKey;

		private GameObject _keyboardParent;

		[HideInInspector, SerializeField]
		private bool _enableVirtualKeyboard = true;

		private GameObject _keyboardObject;

		private MGUIInputField _keyboardObjectInputField;

		private GameObject lastFocussedInputField;

		private bool _inputTextJustSelected;

		private int _caretSelectPos;

		private int _caretPosition;

		private Dictionary<string, string> _keyboardMap = new Dictionary<string, string>
		{
			{
				"1",
				"!"
			},
			{
				"2",
				"@"
			},
			{
				"3",
				"#"
			},
			{
				"4",
				"$"
			},
			{
				"5",
				"%"
			},
			{
				"6",
				"^"
			},
			{
				"7",
				"&"
			},
			{
				"8",
				"*"
			},
			{
				"9",
				"("
			},
			{
				"0",
				")"
			},
			{
				"`",
				"~"
			},
			{
				"-",
				"_"
			},
			{
				"=",
				"+"
			},
			{
				"[",
				"{"
			},
			{
				"]",
				"}"
			},
			{
				"\\",
				"|"
			},
			{
				";",
				":"
			},
			{
				"'",
				"\""
			},
			{
				",",
				"<"
			},
			{
				".",
				">"
			},
			{
				"/",
				"?"
			}
		};

		private EventSystem eventSystem;

		public bool enableVirtualKeyboard
		{
			get
			{
				return this._enableVirtualKeyboard;
			}
			set
			{
				this._enableVirtualKeyboard = value;
				if (!value && this._keyboardParent.get_activeSelf())
				{
					this._keyboardParent.SetActive(false);
				}
				else if (value && this._keyboardObjectInputField != null)
				{
					this.FocusKeyboard(this.keyboardObject);
				}
			}
		}

		public GameObject keyboardObject
		{
			get
			{
				return this._keyboardObject;
			}
			private set
			{
				if (this.keyboardObject != value)
				{
					this._keyboardObject = value;
					if (this._keyboardObject != null && this._keyboardObject.GetComponent<MGUIInputField>() != null)
					{
						this._keyboardObjectInputField = this._keyboardObject.GetComponent<MGUIInputField>();
						this._inputFieldCaretPosition = 0;
						this._inputFieldCaretSelectPos = this._keyboardObjectInputField.text.Length;
						this._inputTextJustSelected = true;
					}
					else
					{
						this._keyboardObjectInputField = null;
					}
				}
			}
		}

		private int _inputFieldCaretPosition
		{
			get
			{
				return (int)MetaManager.GetPrivateProperty("caretPosition", this._keyboardObjectInputField);
			}
			set
			{
				MetaManager.SetPrivateProperty("caretPosition", value, this._keyboardObjectInputField);
			}
		}

		private int _inputFieldCaretSelectPos
		{
			get
			{
				return (int)MetaManager.GetPrivateProperty("caretSelectPos", this._keyboardObjectInputField);
			}
			set
			{
				MetaManager.SetPrivateProperty("caretSelectPos", value, this._keyboardObjectInputField);
			}
		}

		private void UpdateKeyboardObject()
		{
			if (this.eventSystem != null && this.lastFocussedInputField != this.eventSystem.get_currentSelectedGameObject() && ((this.eventSystem.get_currentSelectedGameObject() != null && this.eventSystem.get_currentSelectedGameObject().get_activeInHierarchy() && this.eventSystem.get_currentSelectedGameObject().GetComponent<MGUIInputField>() != null) || this.eventSystem.get_currentSelectedGameObject() == null))
			{
				this.lastFocussedInputField = this.eventSystem.get_currentSelectedGameObject();
				if (this.lastFocussedInputField != null)
				{
					this.FocusKeyboard(this.lastFocussedInputField);
				}
			}
			else if (this.keyboardObject == this.lastFocussedInputField && ((this.lastFocussedInputField != null && !this.lastFocussedInputField.get_activeInHierarchy()) || this.eventSystem.get_currentSelectedGameObject() == null || !this.IsInputFieldOrKey(this.eventSystem.get_currentSelectedGameObject())))
			{
				this.lastFocussedInputField = null;
				this.CloseKeyboard();
			}
			else if (this._keyboardObjectInputField != null)
			{
				this.AutoSetKeyboardPosition();
			}
		}

		private void FocusKeyboard(GameObject newKeyboardObject)
		{
			this.keyboardObject = newKeyboardObject;
			if (!this._keyboardParent.get_activeSelf() && this._enableVirtualKeyboard)
			{
				this._keyboardParent.SetActive(true);
			}
			if (this._keyboardObject.GetComponent<MGUIInputField>() != null)
			{
				this.AutoSetKeyboardPosition();
			}
		}

		private void AutoSetKeyboardPosition()
		{
			Vector3 position = this._keyboardObject.get_transform().get_position() - this._keyboardObject.get_transform().get_forward() * 0.01f - this._keyboardObject.get_transform().get_up() * 0.02f;
			this._keyboardParent.get_transform().set_position(position);
			this._keyboardParent.get_transform().set_rotation(this._keyboardObject.get_transform().get_rotation());
		}

		private void CloseKeyboard()
		{
			this.keyboardObject = null;
			this.lastFocussedInputField = null;
			this.eventSystem.SetSelectedGameObject(null);
			this._keyboardParent.SetActive(false);
		}

		public void PressKey(string keyPressed)
		{
			if (this.keyboardObject != null)
			{
				if (keyPressed == "cancel" || keyPressed == "esc")
				{
					this.CloseKeyboard();
				}
				else
				{
					Event @event = new Event();
					string text = keyPressed;
					switch (text)
					{
					case "#":
						@event.set_keyCode(35);
						@event.set_character('#');
						goto IL_1B1;
					case "%":
						@event.set_keyCode(53);
						@event.set_character('%');
						goto IL_1B1;
					case "^":
						@event.set_keyCode(94);
						@event.set_character('^');
						goto IL_1B1;
					case "&":
						@event.set_keyCode(38);
						@event.set_character('&');
						goto IL_1B1;
					case "Enter":
						@event.set_keyCode(13);
						@event.set_character('\n');
						goto IL_1B1;
					}
					@event = Event.KeyboardEvent(keyPressed);
					if (this._shiftKey.get_isOn() ^ this._capsKey.get_isOn())
					{
						if (this._keyboardMap.ContainsKey(keyPressed))
						{
							keyPressed = this._keyboardMap[keyPressed];
							@event = Event.KeyboardEvent(keyPressed);
						}
						else
						{
							@event.set_character(char.ToUpper(@event.get_character()));
						}
					}
					else
					{
						@event.set_character(char.ToLower(@event.get_character()));
					}
					IL_1B1:
					if (this.keyboardObject != null)
					{
						if (this._keyboardObjectInputField != null)
						{
							this._keyboardObjectInputField.Select();
							this._inputFieldCaretPosition = this._caretPosition;
							this._inputFieldCaretSelectPos = this._caretSelectPos;
							this._keyboardObjectInputField.ProcessEvent(@event);
							this._keyboardObjectInputField.textComponent.set_text(this._keyboardObjectInputField.text);
							this.Unshift();
							this._inputTextJustSelected = false;
						}
						else
						{
							this.keyboardObject.SendMessage("PressKey", @event);
						}
					}
				}
			}
		}

		private void Unshift()
		{
			if (this._shiftKey.get_isOn())
			{
				this._shiftKey.set_isOn(false);
			}
		}

		public void ENTER(bool realKeyboard = false)
		{
			if (this.keyboardObject != null)
			{
				if (this._keyboardObjectInputField != null && !realKeyboard && this._keyboardObjectInputField.lineType == MGUIInputField.LineType.MultiLineNewline)
				{
					this.PressKey("Enter");
				}
				else if (this._keyboardObjectInputField != null)
				{
					this.keyboardObject.get_gameObject().SendMessage("Submit", this._keyboardObjectInputField.text, 1);
					this.CloseKeyboard();
				}
			}
		}

		public void RequestKeyboard(GameObject keyboardObject)
		{
			this.FocusKeyboard(keyboardObject);
		}

		public void ReleaseKeyboard(GameObject keyboardObject)
		{
			if (this._keyboardObject == keyboardObject)
			{
				this.CloseKeyboard();
			}
		}

		public void SetKeyboardPosition(GameObject keyboardObject, Vector3 pos)
		{
			if (this._keyboardObject == keyboardObject)
			{
				this._keyboardParent.get_transform().set_position(pos);
			}
		}

		public void SetKeyboardRotation(GameObject keyboardObject, Quaternion rot)
		{
			if (this._keyboardObject == keyboardObject)
			{
				this._keyboardParent.get_transform().set_rotation(rot);
			}
		}

		private bool IsInputFieldOrKey(GameObject obj)
		{
			if (obj != null)
			{
				if (obj.GetComponent<MGUIInputField>() != null)
				{
					return true;
				}
				if (obj.get_transform().get_parent() != null && obj.get_transform().get_parent().get_parent() != null && obj.get_transform().get_parent().get_parent().get_parent() != null && obj.get_transform().get_parent().get_parent().get_parent().get_parent() != null && obj.get_transform().get_parent().get_parent().get_parent().get_parent().GetComponent<MetaKeyboard>() != null)
				{
					return true;
				}
			}
			return false;
		}

		private void Awake()
		{
			this._keyboardParent = base.get_transform().GetChild(0).get_gameObject();
			this.eventSystem = Object.FindObjectOfType<EventSystem>();
			this._shiftKey = base.get_transform().GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton SHIFT").GetComponent<Toggle>();
			this._shiftKey = base.get_transform().GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton SHIFT").GetComponent<Toggle>();
			this._capsKey = base.get_transform().GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton CAPS").GetComponent<Toggle>();
		}

		private void Update()
		{
			this.UpdateKeyboardObject();
		}

		private void LateUpdate()
		{
			if (this._keyboardObjectInputField != null && this._keyboardObjectInputField.isFocused)
			{
				this._caretPosition = this._inputFieldCaretPosition;
				this._caretSelectPos = this._inputFieldCaretSelectPos;
				if (MetaCamera.GetCameraMode() == CameraType.Stereo && (this._inputFieldCaretPosition != 0 || this._inputFieldCaretSelectPos != this._keyboardObjectInputField.text.Length || !this._inputTextJustSelected))
				{
					this._inputFieldCaretPosition = this._caretSelectPos;
					this._inputTextJustSelected = false;
				}
			}
			if (Input.GetKeyDown(13))
			{
				this.ENTER(true);
			}
		}
	}
}
