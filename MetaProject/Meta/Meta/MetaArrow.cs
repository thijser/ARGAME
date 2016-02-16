// Decompiled with JetBrains decompiler
// Type: Meta.MetaArrow
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  public class MetaArrow : MetaSingleton<MetaArrow>
  {
    public bool twoD = true;
    public Transform targetTransform;
    public bool alwaysOn;

    public bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
      if (Object.op_Inequality((Object) renderer, (Object) null))
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.get_bounds());
      return true;
    }

    public bool IsVisible(Renderer targetRenderer)
    {
      return MetaCamera.GetCameraMode() != CameraType.Monocular ? Object.op_Inequality((Object) GameObject.Find("MetaCameraLeft"), (Object) null) && Object.op_Inequality((Object) GameObject.Find("MetaCameraRight"), (Object) null) && (this.IsVisibleFrom(targetRenderer, (Camera) GameObject.Find("MetaCameraLeft").GetComponent<Camera>()) || this.IsVisibleFrom(targetRenderer, (Camera) GameObject.Find("MetaCameraRight").GetComponent<Camera>())) : this.IsVisibleFrom(targetRenderer, Camera.get_main());
    }

    public bool IsVisible(GameObject targetObject)
    {
      Renderer[] rendererArray = (Renderer[]) targetObject.GetComponentsInChildren<Renderer>();
      bool flag = false;
      foreach (Renderer targetRenderer in rendererArray)
      {
        if (this.IsVisible(targetRenderer))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public bool IsVisible(Transform tTransform)
    {
      return this.IsVisible(((Component) tTransform).get_gameObject());
    }

    private void Start()
    {
      ((Renderer) ((Component) this).GetComponent<Renderer>()).set_enabled(false);
    }

    private void Update()
    {
      if (Object.op_Inequality((Object) this.targetTransform, (Object) null) && ((Component) this.targetTransform).get_gameObject().get_activeInHierarchy())
      {
        if (this.alwaysOn || !this.IsVisible(this.targetTransform))
        {
          ((Renderer) ((Component) this).GetComponent<Renderer>()).set_enabled(true);
          foreach (Renderer renderer in (Renderer[]) ((Component) this).get_gameObject().GetComponentsInChildren<Renderer>())
            renderer.set_enabled(true);
          if (this.twoD)
          {
            Vector3 vector3 = Quaternion.op_Multiply(Quaternion.Inverse(((Component) Camera.get_main()).get_transform().get_rotation()), Vector3.op_Subtraction(this.targetTransform.get_position(), ((Component) this).get_transform().get_position()));
            // ISSUE: explicit reference operation
            ((Vector3) @vector3).Normalize();
            ((Component) this).get_transform().set_localRotation(Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2((float) vector3.y, (float) vector3.x) * 57.29578f - 90f));
          }
          else
            ((Component) this).get_transform().LookAt(this.targetTransform);
        }
        else
        {
          ((Renderer) ((Component) this).GetComponent<Renderer>()).set_enabled(false);
          foreach (Renderer renderer in (Renderer[]) ((Component) this).get_gameObject().GetComponentsInChildren<Renderer>())
            renderer.set_enabled(false);
        }
      }
      else
        ((Renderer) ((Component) this).GetComponent<Renderer>()).set_enabled(false);
    }
  }
}
