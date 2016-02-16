// Decompiled with JetBrains decompiler
// Type: Meta.HandInputBuffer
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System.Runtime.InteropServices;

namespace Meta
{
  internal class HandInputBuffer
  {
    private CppHandData[] _cppHandData = new CppHandData[2];

    public HandInputBuffer()
    {
      for (int index = 0; index < 2; ++index)
      {
        this._cppHandData[index] = new CppHandData();
        this._cppHandData[index].Init();
      }
    }

    [DllImport("MetaVisionDLL", EntryPoint = "getHandData")]
    private static extern void GetHandData(ref CppHandData leftHand, ref CppHandData rightHand);

    public void GetHandData()
    {
      if (Hands.useFaker)
        MetaOldDLLMetaInputFaker.GetHandData(ref this._cppHandData[0], ref this._cppHandData[1]);
      else
        HandInputBuffer.GetHandData(ref this._cppHandData[0], ref this._cppHandData[1]);
    }

    public void UpdateHandInput(ref Hand[] hands)
    {
      for (int index = 0; index < 2; ++index)
        hands[index].Update(this._cppHandData[index]);
    }
  }
}
