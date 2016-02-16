using System;
using UnityEngine;

namespace Meta
{
	internal class MetaPhysics
	{
		private float movementSmoothing = 10f;

		public void MoveObj(Transform obj, Vector3 newPosition, Vector3 offset)
		{
			obj.position = Vector3.Lerp(obj.position, newPosition - offset, this.movementSmoothing * Time.deltaTime);
		}

		public void RotateObj(Transform obj, ref Quaternion prevObjRotation, ref Vector3 prevHandsVector, Vector3 handsVector)
		{
			Vector3 vector = obj.InverseTransformDirection(handsVector);
			Vector3 vector2 = obj.InverseTransformDirection(prevHandsVector);
			Quaternion quaternion = Quaternion.FromToRotation(vector2, vector);
			obj.rotation = prevObjRotation * quaternion;
			prevHandsVector = handsVector;
			prevObjRotation = obj.rotation;
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
			Vector3 localScale = Vector3.Lerp(obj.localScale, vector, Time.deltaTime * this.movementSmoothing);
			if (component.useMinScale && localScale.x < component.minScale.x)
			{
				localScale = component.minScale;
			}
			else if (component.useMaxScale && localScale.x > component.maxScale.x)
			{
				localScale = component.maxScale;
			}
			obj.localScale = localScale;
		}
	}
}
