// Decompiled with JetBrains decompiler
// Type: Meta.MetaHider
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
  public class MetaHider : MonoBehaviour
  {
    [SerializeField]
    private Component[] visibleComponents;
    [SerializeField]
    private Component[] visibleAndEditableComponents;
    [SerializeField]
    [HideInInspector]
    private Component[] blackListComponents;

    [HideInInspector]
    public static MetaHider Instance { get; private set; }

    public MetaHider()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      if (Object.op_Inequality((Object) MetaHider.Instance, (Object) null))
      {
        Debug.LogWarning((object) "There is already a MetaWorld in the scene!");
        Object.DestroyImmediate((Object) ((Component) this).get_gameObject());
      }
      else
        MetaHider.Instance = this;
    }

    private void OnDestroy()
    {
      if (!Object.op_Equality((Object) MetaHider.Instance, (Object) this))
        return;
      MetaHider.Instance = (MetaHider) null;
    }

    internal void AddBlackListComponents(List<Component> components)
    {
      for (int index = components.Count - 1; index >= 0; --index)
      {
        if (Enumerable.Contains<Component>((IEnumerable<Component>) this.blackListComponents, components[index]))
          components.RemoveAt(index);
      }
      int length = this.blackListComponents.Length;
      Component[] componentArray = new Component[length + components.Count];
      this.blackListComponents.CopyTo((Array) componentArray, 0);
      this.blackListComponents = componentArray;
      int index1 = length;
      using (List<Component>.Enumerator enumerator = components.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          this.blackListComponents.SetValue((object) enumerator.Current, index1);
          ++index1;
        }
      }
    }
  }
}
