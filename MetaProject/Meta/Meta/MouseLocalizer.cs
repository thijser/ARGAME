// Decompiled with JetBrains decompiler
// Type: Meta.MouseLocalizer
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class MouseLocalizer : Localizer
  {
    public float sensitivityX = 5f;
    public float sensitivityY = 5f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -90f;
    public float maximumY = 90f;
    public float smoothSpeed = 20f;
    private float rotationX;
    private float smoothRotationX;
    private float rotationY;
    private float smoothRotationY;
    private Vector3 _position;
    private Quaternion _rotation;
    private bool bActive;
    private bool _prevMouseCursorVisibility;
    private bool _prevMouseCursorLockState;
    private bool _stereoMouseEnabled;

    public void Update()
    {
      if (Input.GetMouseButton(1))
      {
        if (!this.bActive)
        {
          this._prevMouseCursorVisibility = ScreenCursor.GetMouseCursorVisibility();
          this._prevMouseCursorLockState = ScreenCursor.GetMouseCursorLockState();
          this._stereoMouseEnabled = MetaSingleton<MetaMouse>.Instance.enableMetaMouse;
          this.bActive = true;
        }
        this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
        this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
        this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
        ScreenCursor.SetMouseCursorVisibility(false);
        ScreenCursor.SetMouseCursorLockState(true);
        MetaSingleton<MetaMouse>.Instance.enableMetaMouse = false;
      }
      else if (this.bActive)
      {
        this.bActive = false;
        ScreenCursor.SetMouseCursorVisibility(this._prevMouseCursorVisibility);
        ScreenCursor.SetMouseCursorLockState(this._prevMouseCursorLockState);
        MetaSingleton<MetaMouse>.Instance.enableMetaMouse = this._stereoMouseEnabled;
      }
      this.smoothRotationX += (this.rotationX - this.smoothRotationX) * this.smoothSpeed * Time.get_smoothDeltaTime();
      this.smoothRotationY += (this.rotationY - this.smoothRotationY) * this.smoothSpeed * Time.get_smoothDeltaTime();
      ((Component) this).get_transform().set_localEulerAngles(new Vector3(-this.smoothRotationY, this.smoothRotationX, 0.0f));
      if (Input.GetMouseButton(1))
      {
        Vector3 vector3_1;
        // ISSUE: explicit reference operation
        ((Vector3) @vector3_1).\u002Ector(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        Vector3 vector3_2 = Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), vector3_1);
        Transform transform1 = ((Component) this).get_transform();
        Vector3 vector3_3 = Vector3.op_Addition(transform1.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, 25f), Time.get_smoothDeltaTime()));
        transform1.set_position(vector3_3);
        Transform transform2 = ((Component) this).get_transform();
        Vector3 vector3_4 = Vector3.op_Addition(transform2.get_position(), Vector3.op_Multiply(Vector3.op_Multiply(Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), Vector3.get_forward()), Input.GetAxis("Mouse ScrollWheel")), 200f));
        transform2.set_position(vector3_4);
      }
      if (Object.op_Equality((Object) this._targetGO, (Object) null))
        this.SetDefaultTargetGO();
      this.UpdateTargetGOTransform();
    }

    public void UpdateTargetGOTransform()
    {
      this._targetGO.get_transform().set_position(((Component) this).get_transform().get_position());
      this._targetGO.get_transform().set_rotation(((Component) this).get_transform().get_rotation());
    }
  }
}
