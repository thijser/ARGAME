// Decompiled with JetBrains decompiler
// Type: Meta.IHandVisualiser`1
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

namespace Meta
{
  internal interface IHandVisualiser<T>
  {
    bool currentlyActive { get; }

    void GetDisplayData(ref T leftHandDisplay, ref T rightHandDisplay);

    void Enable();

    void Disable();
  }
}
