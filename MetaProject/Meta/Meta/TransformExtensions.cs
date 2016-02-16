// Decompiled with JetBrains decompiler
// Type: Meta.TransformExtensions
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal static class TransformExtensions
  {
    public static GameObject FindChildGameObjectOrDie(this Transform t, string gameObjectName)
    {
      Transform transform = t.Find(gameObjectName);
      GameObject gameObject;
      if (Object.op_Equality((Object) transform, (Object) null))
      {
        Debug.LogError((object) ("No " + gameObjectName + " GameObject found..."));
        gameObject = (GameObject) null;
      }
      else if (!((Component) transform).get_gameObject().get_activeSelf())
      {
        Debug.LogError((object) (gameObjectName + " found but it is inactive..."));
        gameObject = (GameObject) null;
      }
      else
        gameObject = ((Component) transform).get_gameObject();
      if (Object.op_Equality((Object) gameObject, (Object) null))
        MetaUtils.QuitApp();
      return gameObject;
    }
  }
}
