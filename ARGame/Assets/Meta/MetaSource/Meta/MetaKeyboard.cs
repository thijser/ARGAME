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
				if (!value && this._keyboardParent.activeSelf)
				{
					this._keyboardParent.SetActive(false);
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
			}
		}
        
		private void UpdateKeyboardObject()
		{
			if (this.eventSystem != null && this.lastFocussedInputField != this.eventSystem.currentSelectedGameObject && ((this.eventSystem.currentSelectedGameObject != null && this.eventSystem.currentSelectedGameObject.activeInHierarchy)))
			{
				this.lastFocussedInputField = this.eventSystem.currentSelectedGameObject;
				if (this.lastFocussedInputField != null)
				{
					this.FocusKeyboard(this.lastFocussedInputField);
				}
			}
			else if (this.keyboardObject == this.lastFocussedInputField && ((this.lastFocussedInputField != null && !this.lastFocussedInputField.activeInHierarchy) || this.eventSystem.currentSelectedGameObject == null || !this.IsInputFieldOrKey(this.eventSystem.currentSelectedGameObject)))
			{
				this.lastFocussedInputField = null;
				this.CloseKeyboard();
			}
			
		}

		private void FocusKeyboard(GameObject newKeyboardObject)
		{
			this.keyboardObject = newKeyboardObject;
			if (!this._keyboardParent.activeSelf && this._enableVirtualKeyboard)
			{
				this._keyboardParent.SetActive(true);
			}
		}

		private void AutoSetKeyboardPosition()
		{
			Vector3 position = this._keyboardObject.transform.position - this._keyboardObject.transform.forward * 0.01f - this._keyboardObject.transform.up * 0.02f;
			this._keyboardParent.transform.position = position;
			this._keyboardParent.transform.rotation = this._keyboardObject.transform.rotation;
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
					
				}
			}
		}

		private void Unshift()
		{
			if (this._shiftKey.isOn)
			{
				this._shiftKey.isOn = false;
			}
		}

		public void ENTER(bool realKeyboard = false)
		{
			if (this.keyboardObject != null)
			{
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
				this._keyboardParent.transform.position = pos;
			}
		}

		public void SetKeyboardRotation(GameObject keyboardObject, Quaternion rot)
		{
			if (this._keyboardObject == keyboardObject)
			{
				this._keyboardParent.transform.rotation = rot;
			}
		}

		private bool IsInputFieldOrKey(GameObject obj)
		{
			if (obj != null)
			{
				if (obj.transform.parent != null && obj.transform.parent.parent != null && obj.transform.parent.parent.parent != null && obj.transform.parent.parent.parent.parent != null && obj.transform.parent.parent.parent.parent.GetComponent<MetaKeyboard>() != null)
				{
					return true;
				}
			}
			return false;
		}

		private void Awake()
		{
			this._keyboardParent = base.transform.GetChild(0).gameObject;
			this.eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			this._shiftKey = base.transform.GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton SHIFT").GetComponent<Toggle>();
			this._shiftKey = base.transform.GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton SHIFT").GetComponent<Toggle>();
			this._capsKey = base.transform.GetChild(0).FindChild("MGUI.Canvas").FindChild("MGUI.Panel").FindChild("MGUI.ToggleButton CAPS").GetComponent<Toggle>();
		}

		private void Update()
		{
			this.UpdateKeyboardObject();
		}

		private void LateUpdate()
		{
			
			if (Input.GetKeyDown((KeyCode)13))
			{
				this.ENTER(true);
			}
		}
	}
}
