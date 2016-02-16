// Decompiled with JetBrains decompiler
// Type: Meta.PinchGesture
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

namespace Meta
{
  public class PinchGesture : Gesture
  {
    internal PinchGesture(CppGestureData cppGesture)
    {
      this._position = MetaUtils.FloatToVector3(cppGesture.gesturePoint);
      this._isValid = cppGesture.valid;
      this._type = cppGesture.manipulationGesture;
    }
  }
}
