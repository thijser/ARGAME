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

		[HideInInspector, SerializeField]
		private Component[] blackListComponents;

		[HideInInspector]
		public static MetaHider Instance
		{
			get;
			private set;
		}

		private void Awake()
		{
			if (MetaHider.Instance != null)
			{
				Debug.LogWarning("There is already a MetaWorld in the scene!");
                UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			MetaHider.Instance = this;
		}

		private void OnDestroy()
		{
			if (MetaHider.Instance == this)
			{
				MetaHider.Instance = null;
			}
		}

		internal void AddBlackListComponents(List<Component> components)
		{
			int i;
			for (i = components.Count - 1; i >= 0; i--)
			{
				if (this.blackListComponents.Contains(components[i]))
				{
					components.RemoveAt(i);
				}
			}
			int num = this.blackListComponents.Length;
			int num2 = num + components.Count;
			Component[] array = new Component[num2];
			this.blackListComponents.CopyTo(array, 0);
			this.blackListComponents = array;
			i = num;
			foreach (Component current in components)
			{
				this.blackListComponents.SetValue(current, i);
				i++;
			}
		}
	}
}
