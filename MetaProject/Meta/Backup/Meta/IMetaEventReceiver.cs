// Decompiled with JetBrains decompiler
// Type: Meta.IMetaEventReceiver
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

namespace Meta
{
  internal interface IMetaEventReceiver
  {
    void MetaInit();

    void MetaUpdate();

    void MetaLateUpdate();

    void MetaOnDestroy();
  }
}
