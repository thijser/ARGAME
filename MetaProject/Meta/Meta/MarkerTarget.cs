// Decompiled with JetBrains decompiler
// Type: Meta.MarkerTarget
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class MarkerTarget : MonoBehaviour
  {
    internal int id;

    public MarkerTarget()
    {
      base.\u002Ector();
    }

    internal void MarkerTargetPersistentLoad()
    {
      MetaBody metaBody = (MetaBody) ((Component) this).get_gameObject().GetComponent<MetaBody>();
      if (!Object.op_Inequality((Object) metaBody, (Object) null) || !metaBody.markerTargetPersistent)
        return;
      metaBody.markerTargetID = metaBody.GetPersistentMarkerTargetID();
    }

    internal void MarkerTargetPersistentSave()
    {
      MetaBody metaBody = (MetaBody) ((Component) this).get_gameObject().GetComponent<MetaBody>();
      if (!Object.op_Inequality((Object) metaBody, (Object) null) || !metaBody.markerTargetPersistent)
        return;
      metaBody.SetPersistentMarkerTargetID(metaBody.markerTargetID);
    }

    private void Start()
    {
      this.MarkerTargetPersistentLoad();
    }

    private void LateUpdate()
    {
      MetaBody metaBody = (MetaBody) ((Component) this).get_gameObject().GetComponent<MetaBody>();
      if (!Object.op_Equality((Object) metaBody, (Object) null) && (metaBody.grabbed || metaBody.pinched))
        return;
      Transform transform = ((Component) this).get_transform();
      if (!Object.op_Inequality((Object) MetaSingleton<MarkerDetector>.Instance, (Object) null))
        return;
      MetaSingleton<MarkerDetector>.Instance.GetMarkerTransform(this.id, ref transform);
    }

    private void OnDisable()
    {
      this.MarkerTargetPersistentSave();
    }
  }
}
