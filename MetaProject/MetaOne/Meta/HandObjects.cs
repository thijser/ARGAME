using System;
using UnityEngine;

namespace Meta
{
	[Serializable]
	internal class HandObjects
	{
		[SerializeField]
		private GameObject m_colliderPrefab;

		public void InitHandGO(ref Hand[] hands, Transform parentTransform)
		{
			GameObject gameObject = new GameObject();
			gameObject.get_transform().set_parent(parentTransform);
			gameObject.set_name("HandObjects");
			gameObject.get_transform().set_localPosition(Vector3.get_zero());
			gameObject.get_transform().set_localRotation(Quaternion.get_identity());
			for (int i = 0; i < 2; i++)
			{
				string name = (HandType)i + "HandtopLocation";
				hands[i].pointer.InstantiateObject(this.m_colliderPrefab, name, gameObject.get_transform());
				name = (HandType)i + "HandleftLocation";
				hands[i].leftMostPoint.InstantiateObject(this.m_colliderPrefab, name, gameObject.get_transform());
				name = (HandType)i + "HandrightLocation";
				hands[i].rightMostPoint.InstantiateObject(this.m_colliderPrefab, name, gameObject.get_transform());
				name = (HandType)i + "HandpalmLocation";
				hands[i].palm.InstantiateObject(this.m_colliderPrefab, name, gameObject.get_transform());
				for (int j = 0; j < 5; j++)
				{
					name = string.Concat(new object[]
					{
						(HandType)i,
						"Hand",
						(FingerTypes)j,
						"FingerLocation"
					});
					hands[i].fingers[j].InstantiateObject(this.m_colliderPrefab, name, gameObject.get_transform());
				}
			}
		}

		public void UpdateHandGO(ref Hand[] hands)
		{
			Vector3 scale = new Vector3(1f, 1f, 1f);
			for (int i = 0; i < 2; i++)
			{
				hands[i].pointer.SetTransform(hands[i].pointer.position, Quaternion.get_identity(), scale);
				hands[i].leftMostPoint.SetTransform(hands[i].leftMostPoint.position, Quaternion.get_identity(), scale);
				hands[i].rightMostPoint.SetTransform(hands[i].rightMostPoint.position, Quaternion.get_identity(), scale);
				hands[i].palm.SetTransform(hands[i].palm.position, hands[i].palm.localOrientation, scale);
				for (int j = 0; j < 5; j++)
				{
					hands[i].fingers[j].SetTransform(hands[i].fingers[j].position, Quaternion.get_identity(), scale);
				}
				LayerMask layers = -1;
				hands[i].pointer.MultiRayCast(layers);
				if (hands[i].gesture.type == MetaGesture.OPEN)
				{
					hands[i].palm.MultiRayCast(layers);
				}
			}
		}

		public void DestroyHandGO()
		{
		}
	}
}
