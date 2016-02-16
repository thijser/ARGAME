// Decompiled with JetBrains decompiler
// Type: Meta.MetaLocalization
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meta
{
  [ExecuteInEditMode]
  public class MetaLocalization : MetaSingleton<MetaLocalization>
  {
    [SerializeField]
    private Localizer[] _localizers = new Localizer[0];
    protected GameObject _targetGO;
    [SerializeField]
    private int _currentLocalization;

    public GameObject targetGO
    {
      get
      {
        return this._targetGO;
      }
      private set
      {
        this._targetGO = value;
      }
    }

    public string[] localizers
    {
      get
      {
        return Enumerable.ToArray<string>(Enumerable.Select<Localizer, string>((IEnumerable<Localizer>) this._localizers, (Func<Localizer, string>) (x => ((Object) x).get_name())));
      }
    }

    public string currentLocalization
    {
      get
      {
        return ((Object) this._localizers[this._currentLocalization]).get_name();
      }
      set
      {
        this.GetLocalizers();
        this._currentLocalization = 0;
        bool flag = false;
        for (int index = 0; index < this._localizers.Length; ++index)
        {
          if (value == ((Object) this._localizers[index]).get_name())
          {
            this._currentLocalization = index;
            flag = true;
            break;
          }
        }
        this.SwitchLocalizer();
        if (flag)
          return;
        Debug.LogError((object) ("Localizer \"" + value + "\" not found."));
      }
    }

    public IMULocalizer ImuLocalizer
    {
      get
      {
        return this.GetLocalizer("IMULocalizer") as IMULocalizer;
      }
      set
      {
      }
    }

    private void OnValidate()
    {
      this.GetLocalizers();
      this.SwitchLocalizer();
    }

    private void Start()
    {
      this.GetLocalizers();
      this.SwitchLocalizer();
    }

    private void SwitchLocalizer()
    {
      for (int index = 0; index < this._localizers.Length; ++index)
      {
        if (Object.op_Inequality((Object) this._localizers[index], (Object) null))
        {
          if (index != this._currentLocalization)
          {
            ((Behaviour) this._localizers[index]).set_enabled(false);
          }
          else
          {
            ((Behaviour) this._localizers[index]).set_enabled(true);
            if (Object.op_Inequality((Object) this._targetGO, (Object) null))
              this._localizers[index].targetGO = this._targetGO;
          }
        }
      }
    }

    public Localizer GetLocalizer(string localizerName)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: reference to a compiler-generated method
      Localizer[] localizerArray = Enumerable.ToArray<Localizer>(Enumerable.Where<Localizer>((IEnumerable<Localizer>) this._localizers, new Func<Localizer, bool>(new MetaLocalization.\u003CGetLocalizer\u003Ec__AnonStoreyA()
      {
        localizerName = localizerName
      }.\u003C\u003Em__1)));
      if (localizerArray.Length > 0)
        return localizerArray[0];
      return (Localizer) null;
    }

    public void GetLocalizers()
    {
      this._localizers = (Localizer[]) ((Component) ((Component) this).get_transform()).GetComponentsInChildren<Localizer>();
    }

    public void ResetLocalizer()
    {
      if (!Object.op_Inequality((Object) this._localizers[this._currentLocalization], (Object) null))
        return;
      this._localizers[this._currentLocalization].ResetLocalizer();
    }

    public Quaternion GetRotation()
    {
      if (Object.op_Inequality((Object) this._localizers[this._currentLocalization], (Object) null))
        return this._localizers[this._currentLocalization].GetRotation();
      return Quaternion.get_identity();
    }

    public Vector3 GetPosition()
    {
      if (Object.op_Inequality((Object) this._localizers[this._currentLocalization], (Object) null))
        return this._localizers[this._currentLocalization].GetPosition();
      return new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void UseMouseLocalizer()
    {
      this.currentLocalization = "MouseLocalizer";
    }
  }
}
