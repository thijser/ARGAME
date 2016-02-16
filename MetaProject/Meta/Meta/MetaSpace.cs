// Decompiled with JetBrains decompiler
// Type: Meta.MetaSpace
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using System;
using UnityEngine;

namespace Meta
{
  [Obsolete("Not used anymore", true)]
  public class MetaSpace : MetaSingleton<MetaSpace>
  {
    public GameObject keyboardObject
    {
      get
      {
        Debug.LogError((object) "MetaSpace is deprecated. Please use MetaKeybaord.Instance.keyboardObject instead");
        return (GameObject) null;
      }
    }

    public static void LoadScene(string sceneName, bool displayLoading = true)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaCore.LoadScene() instead");
    }

    public static void LoadScene(int sceneNum, bool displayLoading = true)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaCore.LoadScene() instead");
    }

    public static void LoadSceneAdditive(string sceneName, bool displayLoading = true)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaCore.LoadSceneAdditive() instead");
    }

    public static void LoadSceneAdditive(int sceneNum, bool displayLoading = true)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaCore.LoadSceneAdditive() instead");
    }

    public void RequestKeyboard(GameObject keyboardObject)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaKeyboard.Instance.RequestKeyboard() instead");
    }

    public void ReleaseKeyboard(GameObject keyboardObject)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaKeyboard.Instance.ReleaseKeyboard() instead");
    }

    public void SetKeyboardPosition(GameObject keyboardObject, Vector3 pos)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaKeyboard.Instance.SetKeyboardPosition() instead");
    }

    public void SetKeyboardRotation(GameObject keyboardObject, Quaternion rot)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaKeyboard.Instance.SetKeyboardRotation() instead");
    }

    public static bool IsVisible(Bounds bounds)
    {
      Debug.LogError((object) "MetaSpace is deprecated. Please use MetaCamera.IsVisible() instead");
      return false;
    }
  }
}
