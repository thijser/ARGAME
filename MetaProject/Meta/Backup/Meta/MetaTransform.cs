// Decompiled with JetBrains decompiler
// Type: Meta.MetaTransform
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class MetaTransform : MonoBehaviour
  {
    private MetaBody _metaBody;
    private Transform _hudLockObject;
    private Vector3 _orbitalDirection;
    private int _prevLayer;
    private bool _prevLayerSet;
    private float _movementSmoothing;
    private Vector3 _prevPos;
    private Quaternion _prevRot;
    private Vector3 _prevFramePos;

    public MetaTransform()
    {
      base.\u002Ector();
    }

    private void Awake()
    {
      this._metaBody = (MetaBody) ((Component) this).GetComponent<MetaBody>();
    }

    private void Update()
    {
    }

    private void LateUpdate()
    {
      if (!Object.op_Inequality((Object) this._metaBody, (Object) null))
        return;
      if (this._metaBody.hud)
        this.UpdateHUDLock();
      else if (this._metaBody.orbital)
      {
        this.UpdateOrbitalLock();
      }
      else
      {
        if (!this._metaBody.palmLock && !this._metaBody.fingerLock)
          return;
        this.UpdatePalmOrFingerLock();
      }
    }

    public void SetupHUDLock()
    {
      if (!Application.get_isPlaying())
        return;
      this.CreateDestroyHUDLockObject();
      this.ToggleHUDLayer();
    }

    private void CreateDestroyHUDLockObject()
    {
      if (Object.op_Inequality((Object) this._metaBody, (Object) null) && this._metaBody.hud)
      {
        if (!Object.op_Equality((Object) this._hudLockObject, (Object) null))
          return;
        this._hudLockObject = new GameObject().get_transform();
        this._hudLockObject.set_position(((Component) this).get_transform().get_position());
        this._hudLockObject.set_rotation(((Component) this).get_transform().get_rotation());
        this._hudLockObject.set_parent(((Component) Camera.get_main()).get_transform());
        ((Object) this._hudLockObject).set_name(((Object) ((Component) this).get_gameObject()).get_name() + ".HUDLockObject");
      }
      else
      {
        if (!Object.op_Inequality((Object) this._hudLockObject, (Object) null))
          return;
        Object.Destroy((Object) ((Component) this._hudLockObject).get_gameObject());
      }
    }

    public void UpdateLockObject()
    {
      if (!Object.op_Inequality((Object) this._hudLockObject, (Object) null))
        return;
      this._hudLockObject.set_position(((Component) this).get_transform().get_position());
      this._hudLockObject.set_rotation(((Component) this).get_transform().get_rotation());
    }

    private void ToggleHUDLayer()
    {
      if (Object.op_Inequality((Object) this._metaBody, (Object) null) && this._metaBody.hud && (this._metaBody.hudLayer || this._metaBody.useDefaultHUDSettings))
      {
        if (((Component) this).get_gameObject().get_layer() == 9 || ((Component) this).get_gameObject().get_layer() == 10)
          return;
        this._prevLayer = ((Component) this).get_gameObject().get_layer();
        this._prevLayerSet = true;
        if (this._prevLayer != 5)
          ((Component) this).get_gameObject().set_layer(9);
        else
          ((Component) this).get_gameObject().set_layer(10);
      }
      else
      {
        if (!this._prevLayerSet)
          return;
        ((Component) this).get_gameObject().set_layer(this._prevLayer);
        this._prevLayerSet = false;
      }
    }

    private void UpdateHUDLock()
    {
      if (!Object.op_Inequality((Object) this._metaBody, (Object) null))
        return;
      if (this._metaBody.useDefaultHUDSettings)
      {
        ((Component) this).get_transform().set_position(this._hudLockObject.get_position());
        ((Component) this).get_transform().set_rotation(this._hudLockObject.get_rotation());
      }
      else
      {
        if (this._metaBody.hudLockPosition)
        {
          Vector3 position = this._hudLockObject.get_position();
          if (!this._metaBody.hudLockPositionX)
            position.x = ((Component) this).get_transform().get_position().x;
          if (!this._metaBody.hudLockPositionY)
            position.y = ((Component) this).get_transform().get_position().y;
          if (!this._metaBody.hudLockPositionZ)
            position.z = ((Component) this).get_transform().get_position().z;
          ((Component) this).get_transform().set_position(position);
        }
        if (!this._metaBody.hudLockRotation)
          return;
        Quaternion rotation1 = this._hudLockObject.get_rotation();
        // ISSUE: explicit reference operation
        Vector3 eulerAngles = ((Quaternion) @rotation1).get_eulerAngles();
        if (!this._metaBody.hudLockRotationX)
        {
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          Vector3& local1 = @eulerAngles;
          Quaternion rotation2 = ((Component) this).get_transform().get_rotation();
          // ISSUE: explicit reference operation
          // ISSUE: variable of the null type
          __Null local2 = ((Quaternion) @rotation2).get_eulerAngles().x;
          // ISSUE: explicit reference operation
          (^local1).x = local2;
        }
        if (!this._metaBody.hudLockRotationY)
        {
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          Vector3& local1 = @eulerAngles;
          Quaternion rotation2 = ((Component) this).get_transform().get_rotation();
          // ISSUE: explicit reference operation
          // ISSUE: variable of the null type
          __Null local2 = ((Quaternion) @rotation2).get_eulerAngles().y;
          // ISSUE: explicit reference operation
          (^local1).y = local2;
        }
        if (!this._metaBody.hudLockRotationZ)
        {
          // ISSUE: explicit reference operation
          // ISSUE: variable of a reference type
          Vector3& local1 = @eulerAngles;
          Quaternion rotation2 = ((Component) this).get_transform().get_rotation();
          // ISSUE: explicit reference operation
          // ISSUE: variable of the null type
          __Null local2 = ((Quaternion) @rotation2).get_eulerAngles().z;
          // ISSUE: explicit reference operation
          (^local1).z = local2;
        }
        ((Component) this).get_transform().set_rotation(Quaternion.Euler(eulerAngles));
      }
    }

    private void UpdateOrbitalLock()
    {
      if (Vector3.op_Inequality(this._prevPos, ((Component) this).get_transform().get_position()) || Quaternion.op_Inequality(this._prevRot, ((Component) this).get_transform().get_rotation()) || Vector3.op_Inequality(this._prevFramePos, ((Component) MetaCore.Instance).get_transform().get_position()))
      {
        this._orbitalDirection = !Vector3.op_Inequality(((Component) this).get_transform().get_position(), ((Component) MetaCore.Instance).get_transform().get_position()) ? ((Component) MetaCore.Instance).get_transform().get_forward() : Vector3.Normalize(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), ((Component) MetaCore.Instance).get_transform().get_position()));
        if (this._metaBody.orbitalLockDistance || this._metaBody.useDefaultOrbitalSettings)
        {
          ((Component) this).get_transform().set_position(((Component) MetaCore.Instance).get_transform().get_position());
          ((Component) this).get_transform().Translate(Vector3.op_Multiply(this._orbitalDirection, this._metaBody.useDefaultOrbitalSettings || this._metaBody.userReachDistance ? MetaSingleton<MetaUserSettingsManager>.Instance.GetReachDistance() : this._metaBody.lockDistance), (Space) 0);
        }
        if (this._metaBody.orbitalLookAtCamera || this._metaBody.useDefaultOrbitalSettings)
        {
          ((Component) this).get_transform().LookAt(((Component) Camera.get_main()).get_transform());
          if (this._metaBody.useDefaultOrbitalSettings || this._metaBody.orbitalLookAtCameraFlipY)
            ((Component) this).get_transform().Rotate(new Vector3(0.0f, 180f, 0.0f));
        }
      }
      this._prevPos = ((Component) this).get_transform().get_position();
      this._prevRot = ((Component) this).get_transform().get_rotation();
      this._prevFramePos = ((Component) MetaCore.Instance).get_transform().get_position();
    }

    private void UpdatePalmOrFingerLock()
    {
      HandType handType = this._metaBody.lockHandType;
      switch (handType)
      {
        case HandType.UNKNOWN:
          return;
        case HandType.EITHER:
          if (Hands.right != null && Hands.right.isValid)
          {
            handType = HandType.RIGHT;
            break;
          }
          if (Hands.left == null || !Hands.left.isValid)
            return;
          handType = HandType.LEFT;
          break;
      }
      if (Hands.GetHands()[(int) handType] == null || !Hands.GetHands()[(int) handType].isValid)
        return;
      Vector3 position = ((Component) this).get_transform().get_position();
      if (this._metaBody.palmLock)
        position = Hands.GetHands()[(int) handType].palm.position;
      else if (this._metaBody.fingerLock)
        position = Hands.GetHands()[(int) handType].pointer.position;
      ((Component) this).get_transform().set_position(Vector3.Lerp(((Component) this).get_transform().get_position(), position, this._movementSmoothing * Time.get_deltaTime()));
    }
  }
}
