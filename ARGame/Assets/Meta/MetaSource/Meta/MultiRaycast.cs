using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public static class MultiRaycast
	{
		public static RaycastHit[] MultiRayCast(Vector3 origin, Vector3 direction, int rows, int raysPerRow, float theta, LayerMask layerMask, bool descend = false)
		{
			int num = layerMask.value;
			if (num < 0)
			{
				num = 0;
			}
			num |= -2147483648;
			num |= 4;
			num |= 65536;
			num = ~num;
			if (rows == 0 || raysPerRow == 0)
			{
				return new RaycastHit[0];
			}
			RaycastHit[] array = new RaycastHit[rows * raysPerRow];
			Vector3 vector = Vector3.up;
			Vector3 vector2 = Vector3.Cross(direction, vector);
			if (vector2.magnitude == 0f)
			{
				vector = Vector3.right;
				vector2 = Vector3.Cross(direction, vector);
			}
			Quaternion quaternion = Quaternion.AngleAxis(360f / (float)raysPerRow, direction);
			for (int i = 0; i < rows; i++)
			{
				Quaternion quaternion2 = Quaternion.AngleAxis((1f + (float)i) / (float)rows * theta, vector2);
				Vector3 vector3 = quaternion2 * direction;
				for (int j = 0; j < raysPerRow; j++)
				{
					if (descend)
					{
						RaycastHit[] array2 = Physics.RaycastAll(origin, vector3, float.PositiveInfinity, num);
						if (array2.Length != 0)
						{
							array[i * raysPerRow + j] = MultiRaycast.CollidedDescendant(array2);
						}
					}
					else
					{
					}
					vector3 = quaternion * vector3;
				}
			}
			return array;
		}

		public static GameObject MostHit(RaycastHit[] hits)
		{
			Dictionary<Transform, int> dictionary = new Dictionary<Transform, int>();
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit raycastHit = hits[i];
				if (raycastHit.collider != null)
				{
					if (!dictionary.ContainsKey(raycastHit.transform))
					{
						dictionary[raycastHit.transform] = 0;
					}
					Dictionary<Transform, int> dictionary2;
					Dictionary<Transform, int> expr_4F = dictionary2 = dictionary;
					Transform transform;
					Transform expr_59 = transform = raycastHit.transform;
					int num = dictionary2[transform];
					expr_4F[expr_59] = num + 1;
				}
			}
			Transform transform2 = null;
			int num2 = 0;
			foreach (KeyValuePair<Transform, int> current in dictionary)
			{
				if (current.Value > num2)
				{
					transform2 = current.Key;
					num2 = current.Value;
				}
			}
			return (!(transform2 != null)) ? null : transform2.gameObject;
		}

		public static GameObject MostHitWithWeights(RaycastHit[] hits, float[] rowWeights)
		{
			if (hits.Length % rowWeights.Length != 0)
			{
				Debug.LogError("Invalid number of row weights.");
				return null;
			}
			Dictionary<Transform, float> dictionary = new Dictionary<Transform, float>();
			int num = 0;
			int num2 = 0;
			int num3 = hits.Length / rowWeights.Length;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit raycastHit = hits[i];
				if (raycastHit.collider != null)
				{
					if (!dictionary.ContainsKey(raycastHit.transform))
					{
						dictionary[raycastHit.transform] = 0f;
					}
					Dictionary<Transform, float> dictionary2;
					Dictionary<Transform, float> expr_7C = dictionary2 = dictionary;
					Transform transform;
					Transform expr_86 = transform = raycastHit.transform;
					float num4 = dictionary2[transform];
					expr_7C[expr_86] = num4 + rowWeights[num2];
				}
				num++;
				if (num == num3)
				{
					num2++;
					num = 0;
				}
			}
			Transform transform2 = null;
			float num5 = 0f;
			foreach (KeyValuePair<Transform, float> current in dictionary)
			{
				if (current.Value > num5)
				{
					transform2 = current.Key;
					num5 = current.Value;
				}
			}
			return (!(transform2 != null)) ? null : transform2.gameObject;
		}

		private static RaycastHit CollidedDescendant(RaycastHit[] hits)
		{
			RaycastHit result = hits[0];
			for (int i = 1; i < hits.Length; i++)
			{
				if ((hits[i].distance < result.distance && !result.transform.IsChildOf(hits[i].transform)) || (hits[i].transform.IsChildOf(result.transform) && result.collider.bounds.Contains(hits[i].point)))
				{
					result = hits[i];
				}
			}
			return result;
		}
	}
}
