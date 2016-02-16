// Decompiled with JetBrains decompiler
// Type: Meta.Hands
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
  public class Hands : MetaSingleton<Hands>, IMetaEventReceiver
  {
    internal static bool useFaker = true;
    private HandInputBuffer _handInputBuffer = new HandInputBuffer();
    [SerializeField]
    internal HandConfig _handConfig = new HandConfig();
    private Hand[] _hands = new Hand[2];
    [SerializeField]
    [HideInInspector]
    private HandObjects _handObjects;
    private DynamicGesture _dynamicGesture;

    internal static HandConfig handConfig
    {
      get
      {
        return MetaSingleton<Hands>.Instance._handConfig;
      }
    }

    public static Hand left
    {
      get
      {
        return MetaSingleton<Hands>.Instance._hands[0];
      }
    }

    public static Hand right
    {
      get
      {
        return MetaSingleton<Hands>.Instance._hands[1];
      }
    }

    internal static DynamicGesture dynamicGesture
    {
      get
      {
        return MetaSingleton<Hands>.Instance._dynamicGesture;
      }
    }

    public static Hand[] GetHands()
    {
      return MetaSingleton<Hands>.Instance._hands;
    }

    public void MetaLateUpdate()
    {
      this._handInputBuffer.UpdateHandInput(ref this._hands);
      for (int index = 0; index < 2; ++index)
        this._hands[index].LocalToWorldCoordinate(((Component) this).get_transform());
      this._handObjects.UpdateHandGO(ref this._hands);
      MetaOldDLLMetaInputFaker.GetDynamicHandGestureData(ref this._dynamicGesture);
    }

    private void OnValidate()
    {
      if (!Application.get_isPlaying())
        return;
      this._handConfig.SetAllParameters();
    }

    private void OnDestroy()
    {
      if (this._handObjects == null)
        return;
      this._handObjects.DestroyHandGO();
    }

    [DllImport("MetaVisionDLL", EntryPoint = "registerHands")]
    private static extern void RegisterHands();

    [DllImport("MetaVisionDLL", EntryPoint = "enableHands")]
    private static extern void EnableHands();

    public void MetaInit()
    {
      Hands.RegisterHands();
      Hands.EnableHands();
      MetaOldDLLMetaInputFaker.InitializeHandData();
      this._handConfig.SetAllParameters();
      for (int index = 0; index < 2; ++index)
        this._hands[index] = new Hand();
      this._handObjects.InitHandGO(ref this._hands, ((Component) this).get_transform());
    }

    public void MetaUpdate()
    {
      MetaOldDLLMetaInputFaker.UpdateData();
      this._handInputBuffer.GetHandData();
    }

    public void MetaOnDestroy()
    {
    }
  }
}
