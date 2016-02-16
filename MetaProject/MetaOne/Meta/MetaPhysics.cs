using System;
using UnityEngine;

namespace Meta
{
	internal class MetaPhysics
	{
		private float movementSmoothing = 10f;

		public void MoveObj(Transform obj, Vector3 newPosition, Vector3 offset)
		{
			obj.set_position(Vector3.Lerp(obj.get_position(), newPosition - offset, this.movementSmoothing * Time.get_deltaTime()));
		}

		public void RotateObj(Transform obj, ref Quaternion prevObjRotation, ref Vector3 prevHandsVector, Vector3 handsVector)
		{
			Vector3 vector = obj.InverseTransformDirection(handsVector);
			Vector3 vector2 = obj.InverseTransformDirection(prevHandsVector);
			Quaternion quaternion = Quaternion.FromToRotation(vector2, vector);
			obj.set_rotation(prevObjRotation * quaternion);
			prevHandsVector = handsVector;
			prevObjRotation = obj.get_rotation();
		}

		public void ScaleObj(Transform obj, float initialHandDist, float handDist, Vector3 initialScale, bool fartherIsLarger = true)
		{
			MetaBody component = obj.GetComponent<MetaBody>();
			float num = handDist - initialHandDist;
			if (!fartherIsLarger)
			{
				num = -num;
			}
			Vector3 vector = initialScale + num * initialScale * 5f;
			Vector3 localScale = Vector3.Lerp(obj.get_localScale(), vector, Time.get_deltaTime() * this.movementSmoothing);
			if (component.useMinScale && localScale.x < component.minScale.x)
			{
				localScale = component.minScale;
			}
			else if (component.useMaxScale && localScale.x > component.maxScale.x)
			{
				localScale = component.maxScale;
			}
			obj.set_localScale(localScale);
		}
	}
}
