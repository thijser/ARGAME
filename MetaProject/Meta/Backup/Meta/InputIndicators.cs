// Decompiled with JetBrains decompiler
// Type: Meta.InputIndicators
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Meta
{
  public class InputIndicators : MetaSingleton<InputIndicators>
  {
    [SerializeField]
    private bool _fingertipIndicators = true;
    [SerializeField]
    private bool _hoverIndicators = true;
    [SerializeField]
    private bool _pressIndicators = true;
    [SerializeField]
    private bool _pressSounds = true;
    [SerializeField]
    private bool _dwellIndicators = true;
    [SerializeField]
    private bool _grabCircles = true;
    private Transform[] _fingerPointerIndicatorTransforms = new Transform[2];
    private Transform[] _fingertipIndicatorTransforms = new Transform[10];
    private Transform[] _fingerPointerHoverTransforms = new Transform[2];
    private Transform[] _fingertipHoverTransforms = new Transform[10];
    private Transform[] _grabCircleSolidTransforms = new Transform[2];
    private Transform[] _grabCircleDottedTransforms = new Transform[2];
    private float _minHandOpenness = -100f;
    private float _maxHandOpenness = 20f;
    private bool[] _dwellSet = new bool[2];
    [SerializeField]
    private bool _handCloud;
    [SerializeField]
    private bool _grabCirclesAlways;
    private bool _handCloudPrev;
    private bool _fingertipIndicatorsPrev;
    private bool _fingertipsPrev;
    [SerializeField]
    [HideInInspector]
    private GameObject _handCloudObject;
    [HideInInspector]
    [SerializeField]
    private GameObject _fingertipIndicator;
    [HideInInspector]
    [SerializeField]
    private GameObject _hoverIndicator;
    [SerializeField]
    [HideInInspector]
    private GameObject _pressIndicator;
    [SerializeField]
    [HideInInspector]
    private GameObject _pressSound;
    [SerializeField]
    [HideInInspector]
    private GameObject _grabCircleSolid;
    [HideInInspector]
    [SerializeField]
    private GameObject _grabCircleDotted;
    [SerializeField]
    [HideInInspector]
    private GameObject _dwellIndicator;
    [HideInInspector]
    [SerializeField]
    private Texture2D[] _dwellIndicatorTextures;

    public bool handCloud
    {
      get
      {
        return this._handCloud;
      }
      set
      {
        this._handCloud = value;
      }
    }

    public bool fingertipIndicators
    {
      get
      {
        return this._fingertipIndicators;
      }
      set
      {
        this._fingertipIndicators = value;
      }
    }

    public bool hoverIndicators
    {
      get
      {
        return this._hoverIndicators;
      }
      set
      {
        this._hoverIndicators = value;
      }
    }

    public bool pressIndicators
    {
      get
      {
        return this._pressIndicators;
      }
      set
      {
        this._pressIndicators = value;
      }
    }

    public bool pressSounds
    {
      get
      {
        return this._pressSounds;
      }
      set
      {
        this._pressSounds = value;
      }
    }

    public bool dwellIndicators
    {
      get
      {
        return this._dwellIndicators;
      }
      set
      {
        this._dwellIndicators = value;
      }
    }

    public bool grabCircles
    {
      get
      {
        return this._grabCircles;
      }
      set
      {
        this._grabCircles = value;
      }
    }

    public bool grabCirclesAlways
    {
      get
      {
        return this._grabCirclesAlways;
      }
      set
      {
        this._grabCirclesAlways = value;
      }
    }

    private void Start()
    {
      this.InitFingertipIndicatorTransforms();
      this.InitGrabCircleTransforms();
      this.SelectFingerIndicators();
      this.SetHandCloud();
    }

    private void Update()
    {
      this.CheckIndicatorSettings();
    }

    private void LateUpdate()
    {
      this._dwellSet = new bool[2];
    }

    private void InitFingertipIndicatorTransforms()
    {
      for (int index1 = 0; index1 < 2; ++index1)
      {
        if (Hands.GetHands()[index1] != null)
        {
          this._fingerPointerIndicatorTransforms[index1] = this.InstantiatePrefab(this._fingertipIndicator, "FingertipIndicator", Hands.GetHands()[index1].pointer.gameObject.get_transform(), new Vector3(1.0 / 1000.0, 1.0 / 1000.0, 1.0 / 1000.0), new Color?());
          this._fingerPointerHoverTransforms[index1] = this.InstantiatePrefab(this._hoverIndicator, "HoverIndicator", Hands.GetHands()[index1].pointer.gameObject.get_transform(), new Vector3(1.0 / 1000.0, 1.0 / 1000.0, 1.0 / 1000.0), new Color?());
          this._fingerPointerHoverTransforms[index1] = this.InstantiatePrefab(this._dwellIndicator, "PointerDwellIndicator", Hands.GetHands()[index1].pointer.gameObject.get_transform(), new Vector3(1.0 / 500.0, 1.0 / 500.0, 1.0 / 500.0), new Color?());
          for (int index2 = 0; index2 < 5; ++index2)
          {
            this._fingertipIndicatorTransforms[index2 + index1 * 5] = this.InstantiatePrefab(this._fingertipIndicator, "FingertipIndicator", Hands.GetHands()[index1].fingers[index2].gameObject.get_transform(), new Vector3(1.0 / 1000.0, 1.0 / 1000.0, 1.0 / 1000.0), new Color?());
            this._fingertipHoverTransforms[index2 + index1 * 5] = this.InstantiatePrefab(this._hoverIndicator, "HoverIndicator", Hands.GetHands()[index1].fingers[index2].gameObject.get_transform(), new Vector3(1.0 / 1000.0, 1.0 / 1000.0, 1.0 / 1000.0), new Color?());
          }
        }
      }
    }

    private void InitGrabCircleTransforms()
    {
      for (int index = 0; index < 2; ++index)
      {
        if (Hands.GetHands()[index] != null)
        {
          this._grabCircleSolidTransforms[index] = this.InstantiatePrefab(this._grabCircleSolid, "GrabCircleSolid", Hands.GetHands()[index].palm.gameObject.get_transform(), new Vector3(3.0 / 1000.0, 3.0 / 1000.0, 3.0 / 1000.0), new Color?(new Color(1f, 0.6470588f, 0.0f, 1f)));
          this._grabCircleDottedTransforms[index] = this.InstantiatePrefab(this._grabCircleDotted, "GrabCircleDotted", Hands.GetHands()[index].palm.gameObject.get_transform(), new Vector3(3.0 / 1000.0, 3.0 / 1000.0, 3.0 / 1000.0), new Color?(new Color(1f, 1f, 0.3921569f, 1f)));
        }
      }
    }

    internal void UpdateDwellIndicators(HandType hand, float completion)
    {
      Transform child = Hands.GetHands()[(int) hand].pointer.gameObject.get_transform().FindChild("PointerDwellIndicator");
      ((Renderer) ((Component) child).GetComponent<Renderer>()).set_enabled(true);
      int index = (int) ((double) (this._dwellIndicatorTextures.Length - 1) * (double) completion);
      if (index <= 0)
        index = 0;
      if (index >= this._dwellIndicatorTextures.Length - 1)
        index = this._dwellIndicatorTextures.Length - 1;
      ((Renderer) ((Component) child).GetComponent<Renderer>()).get_material().set_mainTexture((Texture) this._dwellIndicatorTextures[index]);
      this._dwellSet[(int) hand] = true;
    }

    internal void HideDwellIndicators(HandType hand)
    {
      if (this._dwellSet[(int) hand])
        return;
      ((Renderer) ((Component) Hands.GetHands()[(int) hand].pointer.gameObject.get_transform().FindChild("PointerDwellIndicator")).GetComponent<Renderer>()).set_enabled(false);
    }

    private Transform InstantiatePrefab(GameObject prefab, string name, Transform parent, Vector3 localScale, Color? color = null)
    {
      Transform transform = ((GameObject) Object.Instantiate((Object) prefab)).get_transform();
      ((Object) transform).set_name(name);
      transform.set_parent(parent);
      transform.set_localPosition(Vector3.get_zero());
      transform.set_localScale(localScale);
      if (color.HasValue)
      {
        ((Renderer) ((Component) transform).GetComponent<Renderer>()).get_material().set_color(color.Value);
        ((Renderer) ((Component) transform).GetComponent<Renderer>()).get_material().SetColor("_SpecColor", color.Value);
      }
      return transform;
    }

    private void CheckIndicatorSettings()
    {
      if (this._fingertipsPrev != Hands.handConfig.fingertips || this._fingertipIndicators != this._fingertipIndicatorsPrev)
      {
        Hands.handConfig.fingertips = Hands.handConfig.fingertips;
        this.SelectFingerIndicators();
      }
      if (this._handCloudPrev == this._handCloud)
        return;
      this.SetHandCloud();
    }

    private void SelectFingerIndicators()
    {
      this._fingertipsPrev = Hands.handConfig.fingertips;
      this._fingertipIndicatorsPrev = this._fingertipIndicators;
      if (Hands.handConfig.fingertips)
        this.SetFingertipRenderers(false, this._fingertipIndicators);
      else
        this.SetFingertipRenderers(this._fingertipIndicators, false);
    }

    private void SetHandCloud()
    {
      Hands.handConfig.SetAllParameters();
      this._handCloudPrev = this._handCloud;
      this._handCloudObject.SetActive(this._handCloud);
    }

    private void SetFingertipRenderers(bool pointers, bool fingertips)
    {
      for (int index = 0; index < 2; ++index)
      {
        if (Object.op_Inequality((Object) this._fingerPointerIndicatorTransforms[index], (Object) null))
        {
          ((Renderer) ((Component) this._fingerPointerIndicatorTransforms[index]).GetComponent<Renderer>()).set_enabled(pointers);
          if (!pointers)
            ((Renderer) ((Component) this._fingerPointerHoverTransforms[index]).GetComponent<Renderer>()).set_enabled(false);
        }
      }
      for (int index = 0; index < 10; ++index)
      {
        if (Object.op_Inequality((Object) this._fingertipIndicatorTransforms[index], (Object) null))
          ((Renderer) ((Component) this._fingertipIndicatorTransforms[index]).GetComponent<Renderer>()).set_enabled(fingertips);
        if (!fingertips && Object.op_Inequality((Object) this._fingertipHoverTransforms[index], (Object) null))
          ((Renderer) ((Component) this._fingertipHoverTransforms[index]).GetComponent<Renderer>()).set_enabled(false);
      }
    }

    internal void DisableHoverIndicator(Transform hoverIndicator)
    {
      ((Renderer) ((Component) hoverIndicator).GetComponent<Renderer>()).set_enabled(false);
    }

    internal void UpdateHoverIndicator(Transform hoverIndicator, float distance)
    {
      if (!this._fingertipIndicators || !this._hoverIndicators)
        return;
      float num1 = 0.0125f;
      if ((double) distance < 0.200000002980232)
        num1 += (float) (0.0924999937415123 * (1.0 - (double) distance / 0.200000002980232));
      float num2 = num1 / 100f;
      hoverIndicator.set_localScale(new Vector3(num2, num2, num2));
      ((Renderer) ((Component) hoverIndicator).GetComponent<Renderer>()).set_enabled(true);
    }

    internal void ColorHoverIndicator(Transform hoverIndicator, Color color)
    {
      ((Renderer) ((Component) hoverIndicator).GetComponent<Renderer>()).get_material().set_color(color);
    }

    public void InstantiatePressIndicator(Vector3 pos, Quaternion rot)
    {
      if (!this._pressIndicators || !Object.op_Inequality((Object) this._pressIndicator, (Object) null))
        return;
      Object.Instantiate((Object) this._pressIndicator, pos, rot);
    }

    public void InstantiatePressSound(Vector3 pos, AudioClip pressSound)
    {
      if (!this._pressSounds || !Object.op_Inequality((Object) this._pressSound, (Object) null))
        return;
      GameObject gameObject = Object.Instantiate((Object) this._pressSound, pos, Quaternion.get_identity()) as GameObject;
      if (Object.op_Inequality((Object) pressSound, (Object) null))
        ((AudioSource) gameObject.GetComponent<AudioSource>()).set_clip(pressSound);
      ((AudioSource) gameObject.GetComponent<AudioSource>()).set_loop(false);
      ((AudioSource) gameObject.GetComponent<AudioSource>()).Play();
      this.StartCoroutine("DestroySound", (object) gameObject);
    }

    [DebuggerHidden]
    private IEnumerator DestroySound(GameObject sound)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InputIndicators.\u003CDestroySound\u003Ec__Iterator4()
      {
        sound = sound,
        \u003C\u0024\u003Esound = sound
      };
    }

    internal void UpdateGrabCircles(int hand, bool objectWithinRange)
    {
      if (this._grabCircles && (objectWithinRange || this._grabCirclesAlways))
      {
        if (Hands.GetHands()[hand] == null)
          return;
        float num1 = Hands.GetHands()[hand].handOpenness;
        if (Hands.GetHands()[hand].gesture.type == MetaGesture.GRAB)
        {
          float num2 = this._maxHandOpenness - Hands.handConfig.grabThreshold;
          if ((double) num1 > (double) this._maxHandOpenness)
            num1 = this._maxHandOpenness;
          float num3 = Mathf.Abs(num1 - Hands.handConfig.grabThreshold) / num2;
          if ((double) num1 < (double) Hands.handConfig.grabThreshold)
            num3 = 0.0f;
          this.SetCircleScale(hand, (float) ((1.0 - (double) num3) * 0.00240000011399388 + 0.000600000028498471));
        }
        else
        {
          float num2 = Hands.handConfig.grabThreshold - this._minHandOpenness;
          if ((double) num1 < (double) this._minHandOpenness)
            num1 = this._minHandOpenness;
          float num3 = (float) (1.0 - (double) Mathf.Abs(num1 - Hands.handConfig.grabThreshold) / (double) num2);
          if ((double) num1 > (double) Hands.handConfig.grabThreshold)
            num3 = 1f;
          this.SetCircleScale(hand, (float) ((1.0 - (double) num3) * (3.0 / 500.0) + 3.0 / 1000.0));
        }
        this._grabCircleSolidTransforms[hand].LookAt(((Component) Camera.get_main()).get_transform());
        this._grabCircleSolidTransforms[hand].Rotate(new Vector3(90f, 0.0f, 0.0f));
        this._grabCircleDottedTransforms[hand].set_rotation(this._grabCircleSolidTransforms[hand].get_rotation());
        this.SetCircleGrabbedColor(hand, objectWithinRange);
        this.ToggleGrabCircle(hand, true);
      }
      else
        this.ToggleGrabCircle(hand, false);
    }

    private void SetCircleGrabbedColor(int hand, bool objectWithinRange)
    {
      if (objectWithinRange)
      {
        ((Renderer) ((Component) this._grabCircleSolidTransforms[hand]).GetComponent<Renderer>()).get_material().set_color(Color.get_green());
        ((Renderer) ((Component) this._grabCircleSolidTransforms[hand]).GetComponent<Renderer>()).get_material().SetColor("_Emission", Color.get_green());
      }
      else
      {
        ((Renderer) ((Component) this._grabCircleSolidTransforms[hand]).GetComponent<Renderer>()).get_material().set_color(Color.get_gray());
        ((Renderer) ((Component) this._grabCircleSolidTransforms[hand]).GetComponent<Renderer>()).get_material().SetColor("_Emission", Color.get_gray());
      }
    }

    private void SetCircleScale(int hand, float scale)
    {
      Vector3 vector3_1;
      // ISSUE: explicit reference operation
      ((Vector3) @vector3_1).\u002Ector(scale, scale, scale);
      Vector3 vector3_2 = Vector3.Lerp(this._grabCircleDottedTransforms[hand].get_localScale(), vector3_1, 10f * Time.get_deltaTime());
      this._grabCircleDottedTransforms[hand].set_localScale(vector3_2);
    }

    private void ToggleGrabCircle(int hand, bool visibility)
    {
      if (Object.op_Inequality((Object) this._grabCircleSolidTransforms[hand], (Object) null))
        ((Renderer) ((Component) this._grabCircleSolidTransforms[hand]).GetComponent<Renderer>()).set_enabled(visibility);
      if (!Object.op_Inequality((Object) this._grabCircleDottedTransforms[hand], (Object) null))
        return;
      ((Renderer) ((Component) this._grabCircleDottedTransforms[hand]).GetComponent<Renderer>()).set_enabled(visibility);
    }
  }
}
