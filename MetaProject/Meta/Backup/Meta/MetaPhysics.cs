// Decompiled with JetBrains decompiler
// Type: Meta.MetaPhysics
// Assembly: Meta, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A97142E9-99B1-4A5E-AB7A-F4FDDF65AE91
// Assembly location: C:\cygwin64\home\ptrck\ARGame\ARGame\Assets\Meta\Meta.dll

using UnityEngine;

namespace Meta
{
  internal class MetaPhysics
  {
    private float movementSmoothing = 10f;

    public void MoveObj(Transform obj, Vector3 newPosition, Vector3 offset)
    {
      obj.set_position(Vector3.Lerp(obj.get_position(), Vector3.op_Subtraction(newPosition, offset), this.movementSmoothing * Time.get_deltaTime()));
    }

    public void RotateObj(Transform obj, ref Quaternion prevObjRotation, ref Vector3 prevHandsVector, Vector3 handsVector)
    {
      Vector3 vector3 = obj.InverseTransformDirection(handsVector);
      Quaternion quaternion = Quaternion.FromToRotation(obj.InverseTransformDirection(prevHandsVector), vector3);
      obj.set_rotation(Quaternion.op_Multiply(prevObjRotation, quaternion));
      prevHandsVector = handsVector;
      prevObjRotation = obj.get_rotation();
    }

    public void ScaleObj(Transform obj, float initialHandDist, float handDist, Vector3 initialScale, bool fartherIsLarger = true)
    {
      MetaBody metaBody = (MetaBody) ((Component) obj).GetComponent<MetaBody>();
      float num = handDist - initialHandDist;
      if (!fartherIsLarger)
        num = -num;
      Vector3 vector3_1 = Vector3.op_Addition(initialScale, Vector3.op_Multiply(Vector3.op_Multiply(num, initialScale), 5f));
      Vector3 vector3_2 = Vector3.Lerp(obj.get_localScale(), vector3_1, Time.get_deltaTime() * this.movementSmoothing);
      if (metaBody.useMinScale && vector3_2.x < metaBody.minScale.x)
        vector3_2 = metaBody.minScale;
      else if (metaBody.useMaxScale && vector3_2.x > metaBody.maxScale.x)
        vector3_2 = metaBody.maxScale;
      obj.set_localScale(vector3_2);
    }
  }
}
