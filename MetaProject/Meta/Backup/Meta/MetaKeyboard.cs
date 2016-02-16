// Decompiled with JetBrains decompiler
// Type: Meta.MetaKeyboard
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Meta
{
  public class MetaKeyboard : MetaSingleton<MetaKeyboard>
  {
    [SerializeField]
    [HideInInspector]
    private bool _enableVirtualKeyboard = true;
    private Dictionary<string, string> _keyboardMap = new Dictionary<string, string>()
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
    private Toggle _shiftKey;
    private Toggle _capsKey;
    private GameObject _keyboardParent;
    private GameObject _keyboardObject;
    private MGUIInputField _keyboardObjectInputField;
    private GameObject lastFocussedInputField;
    private bool _inputTextJustSelected;
    private int _caretSelectPos;
    private int _caretPosition;
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
        else
        {
          if (!value || !Object.op_Inequality((Object) this._keyboardObjectInputField, (Object) null))
            return;
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
        if (!Object.op_Inequality((Object) this.keyboardObject, (Object) value))
          return;
        this._keyboardObject = value;
        if (Object.op_Inequality((Object) this._keyboardObject, (Object) null) && Object.op_Inequality((Object) this._keyboardObject.GetComponent<MGUIInputField>(), (Object) null))
        {
          this._keyboardObjectInputField = (MGUIInputField) this._keyboardObject.GetComponent<MGUIInputField>();
          this._inputFieldCaretPosition = 0;
          this._inputFieldCaretSelectPos = this._keyboardObjectInputField.text.Length;
          this._inputTextJustSelected = true;
        }
        else
          this._keyboardObjectInputField = (MGUIInputField) null;
      }
    }

    private int _inputFieldCaretPosition
    {
      get
      {
        return (int) MetaManager.GetPrivateProperty("caretPosition", (object) this._keyboardObjectInputField);
      }
      set
      {
        MetaManager.SetPrivateProperty("caretPosition", (object) value, (object) this._keyboardObjectInputField);
      }
    }

    private int _inputFieldCaretSelectPos
    {
      get
      {
        return (int) MetaManager.GetPrivateProperty("caretSelectPos", (object) this._keyboardObjectInputField);
      }
      set
      {
        MetaManager.SetPrivateProperty("caretSelectPos", (object) value, (object) this._keyboardObjectInputField);
      }
    }

    private void UpdateKeyboardObject()
    {
      if (Object.op_Inequality((Object) this.eventSystem, (Object) null) && Object.op_Inequality((Object) this.lastFocussedInputField, (Object) this.eventSystem.get_currentSelectedGameObject()) && (Object.op_Inequality((Object) this.eventSystem.get_currentSelectedGameObject(), (Object) null) && this.eventSystem.get_currentSelectedGameObject().get_activeInHierarchy() && Object.op_Inequality((Object) this.eventSystem.get_currentSelectedGameObject().GetComponent<MGUIInputField>(), (Object) null) || Object.op_Equality((Object) this.eventSystem.get_currentSelectedGameObject(), (Object) null)))
      {
        this.lastFocussedInputField = this.eventSystem.get_currentSelectedGameObject();
        if (!Object.op_Inequality((Object) this.lastFocussedInputField, (Object) null))
          return;
        this.FocusKeyboard(this.lastFocussedInputField);
      }
      else if (Object.op_Equality((Object) this.keyboardObject, (Object) this.lastFocussedInputField) && (Object.op_Inequality((Object) this.lastFocussedInputField, (Object) null) && !this.lastFocussedInputField.get_activeInHierarchy() || (Object.op_Equality((Object) this.eventSystem.get_currentSelectedGameObject(), (Object) null) || !this.IsInputFieldOrKey(this.eventSystem.get_currentSelectedGameObject()))))
      {
        this.lastFocussedInputField = (GameObject) null;
        this.CloseKeyboard();
      }
      else
      {
        if (!Object.op_Inequality((Object) this._keyboardObjectInputField, (Object) null))
          return;
        this.AutoSetKeyboardPosition();
      }
    }

    private void FocusKeyboard(GameObject newKeyboardObject)
    {
      this.keyboardObject = newKeyboardObject;
      if (!this._keyboardParent.get_activeSelf() && this._enableVirtualKeyboard)
        this._keyboardParent.SetActive(true);
      if (!Object.op_Inequality((Object) this._keyboardObject.GetComponent<MGUIInputField>(), (Object) null))
        return;
      this.AutoSetKeyboardPosition();
    }

    private void AutoSetKeyboardPosition()
    {
      this._keyboardParent.get_transform().set_position(Vector3.op_Subtraction(Vector3.op_Subtraction(this._keyboardObject.get_transform().get_position(), Vector3.op_Multiply(this._keyboardObject.get_transform().get_forward(), 0.01f)), Vector3.op_Multiply(this._keyboardObject.get_transform().get_up(), 0.02f)));
      this._keyboardParent.get_transform().set_rotation(this._keyboardObject.get_transform().get_rotation());
    }

    private void CloseKeyboard()
    {
      this.keyboardObject = (GameObject) null;
      this.lastFocussedInputField = (GameObject) null;
      this.eventSystem.SetSelectedGameObject((GameObject) null);
      this._keyboardParent.SetActive(false);
    }

    public void PressKey(string keyPressed)
    {
      if (!Object.op_Inequality((Object) this.keyboardObject, (Object) null))
        return;
      if (keyPressed == "cancel" || keyPressed == "esc")
      {
        this.CloseKeyboard();
      }
      else
      {
        Event e = new Event();
        string key = keyPressed;
        if (key != null)
        {
          // ISSUE: reference to a compiler-generated field
          if (MetaKeyboard.\u003C\u003Ef__switch\u0024map1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            MetaKeyboard.\u003C\u003Ef__switch\u0024map1 = new Dictionary<string, int>(5)
            {
              {
                "#",
                0
              },
              {
                "%",
                1
              },
              {
                "^",
                2
              },
              {
                "&",
                3
              },
              {
                "Enter",
                4
              }
            };
          }
          int num;
          // ISSUE: reference to a compiler-generated field
          if (MetaKeyboard.\u003C\u003Ef__switch\u0024map1.TryGetValue(key, out num))
          {
            switch (num)
            {
              case 0:
                e.set_keyCode((KeyCode) 35);
                e.set_character('#');
                goto label_20;
              case 1:
                e.set_keyCode((KeyCode) 53);
                e.set_character('%');
                goto label_20;
              case 2:
                e.set_keyCode((KeyCode) 94);
                e.set_character('^');
                goto label_20;
              case 3:
                e.set_keyCode((KeyCode) 38);
                e.set_character('&');
                goto label_20;
              case 4:
                e.set_keyCode((KeyCode) 13);
                e.set_character('\n');
                goto label_20;
            }
          }
        }
        e = Event.KeyboardEvent(keyPressed);
        if (this._shiftKey.get_isOn() ^ this._capsKey.get_isOn())
        {
          if (this._keyboardMap.ContainsKey(keyPressed))
          {
            keyPressed = this._keyboardMap[keyPressed];
            e = Event.KeyboardEvent(keyPressed);
          }
          else
            e.set_character(char.ToUpper(e.get_character()));
        }
        else
          e.set_character(char.ToLower(e.get_character()));
label_20:
        if (!Object.op_Inequality((Object) this.keyboardObject, (Object) null))
          return;
        if (Object.op_Inequality((Object) this._keyboardObjectInputField, (Object) null))
        {
          this._keyboardObjectInputField.Select();
          this._inputFieldCaretPosition = this._caretPosition;
          this._inputFieldCaretSelectPos = this._caretSelectPos;
          this._keyboardObjectInputField.ProcessEvent(e);
          this._keyboardObjectInputField.textComponent.set_text(this._keyboardObjectInputField.text);
          this.Unshift();
          this._inputTextJustSelected = false;
        }
        else
          this.keyboardObject.SendMessage("PressKey", (object) e);
      }
    }

    private void Unshift()
    {
      if (!this._shiftKey.get_isOn())
        return;
      this._shiftKey.set_isOn(false);
    }

    public void ENTER(bool realKeyboard = false)
    {
      if (!Object.op_Inequality((Object) this.keyboardObject, (Object) null))
        return;
      if (Object.op_Inequality((Object) this._keyboardObjectInputField, (Object) null) && !realKeyboard && this._keyboardObjectInputField.lineType == MGUIInputField.LineType.MultiLineNewline)
      {
        this.PressKey("Enter");
      }
      else
      {
        if (!Object.op_Inequality((Object) this._keyboardObjectInputField, (Object) null))
          return;
        this.keyboardObject.get_gameObject().SendMessage("Submit", (object) this._keyboardObjectInputField.text, (SendMessageOptions) 1);
        this.CloseKeyboard();
      }
    }

    public void RequestKeyboard(GameObject keyboardObject)
    {
      this.FocusKeyboard(keyboardObject);
    }

    public void ReleaseKeyboard(GameObject keyboardObject)
    {
      if (!Object.op_Equality((Object) this._keyboardObject, (Object) keyboardObject))
        return;
      this.CloseKeyboard();
    }

    public void SetKeyboardPosition(GameObject keyboardObject, Vector3 pos)
    {
      if (!Object.op_Equality((Object) this._keyboardObject, (Object) keyboardObject))
        return;
      this._keyboardParent.get_transform().set_position(pos);
    }

    public void SetKeyboardRotation(GameObject keyboardObject, Quaternion rot)
    {
      if (!Object.op_Equality((Object) this._keyboardObject, (Object) keyboardObject))
        return;
      this._keyboardParent.get_transform().set_rotation(rot);
    }

    private bool IsInputFieldOrKey(GameObject obj)
    {
      return Object.op_Inequality((Object) obj, (Object) null) && (Object.op_Inequality((Object) obj.GetComponent<MGUIInputField>(), (Object) null) || Object.op_Inequality((Object) obj.get_transform().get_parent(), (Object) null) && Object.op_Inequality((Object) obj.get_transform().get_parent().get_parent(), (Object) null) && (Object.op_Inequality((Object) obj.get_transform().get_parent().get_parent().get_parent(), (Object) null) && Object.op_Inequality((Object) obj.get_transform().get_parent().get_parent().get_parent().get_parent(), (Object) null)) && Object.op_Inequality((Object) ((Component) obj.get_transform().get_parent().get_parent().get_parent().get_parent()).GetComponent<MetaKeyboard>(), (Object) null));
    }

    private void Awake()
    {
      this._keyboardParent = ((Component) ((Component) this).get_transform().GetChild(0)).get_gameObject();
      this.eventSystem = (EventSystem) Object.FindObjectOfType<EventSystem>();
      this._shiftKey = (Toggle) ((Component) ((Component) this).get_transform().GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton SHIFT")).GetComponent<Toggle>();
      this._shiftKey = (Toggle) ((Component) ((Component) this).get_transform().GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton SHIFT")).GetComponent<Toggle>();
      this._capsKey = (Toggle) ((Component) ((Component) this).get_transform().GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton CAPS")).GetComponent<Toggle>();
    }

    private void Update()
    {
      this.UpdateKeyboardObject();
    }

    private void LateUpdate()
    {
      if (Object.op_Inequality((Object) this._keyboardObjectInputField, (Object) null) && this._keyboardObjectInputField.isFocused)
      {
        this._caretPosition = this._inputFieldCaretPosition;
        this._caretSelectPos = this._inputFieldCaretSelectPos;
        if (MetaCamera.GetCameraMode() == CameraType.Stereo && (this._inputFieldCaretPosition != 0 || this._inputFieldCaretSelectPos != this._keyboardObjectInputField.text.Length || !this._inputTextJustSelected))
        {
          this._inputFieldCaretPosition = this._caretSelectPos;
          this._inputTextJustSelected = false;
        }
      }
      if (!Input.GetKeyDown((KeyCode) 13))
        return;
      this.ENTER(true);
    }
  }
}
