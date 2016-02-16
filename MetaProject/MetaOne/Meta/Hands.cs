using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta
{
	public class Hands : MetaSingleton<Hands>, IMetaEventReceiver
	{
		internal static bool useFaker = true;

		private HandInputBuffer _handInputBuffer = new HandInputBuffer();

		[HideInInspector, SerializeField]
		private HandObjects _handObjects;

		[SerializeField]
		internal HandConfig _handConfig = new HandConfig();

		private Hand[] _hands = new Hand[2];

		private DynamicGesture _dynamicGesture;

		internal static HandConfig handConfig
		{
			get
			{
				return MetaSingleton<Hands>.Instance._handConfig;
			}
		}

		public static Hand left
		{
			get
			{
				return MetaSingleton<Hands>.Instance._hands[0];
			}
		}

		public static Hand right
		{
			get
			{
				return MetaSingleton<Hands>.Instance._hands[1];
			}
		}

		internal static DynamicGesture dynamicGesture
		{
			get
			{
				return MetaSingleton<Hands>.Instance._dynamicGesture;
			}
		}

		public static Hand[] GetHands()
		{
			return MetaSingleton<Hands>.Instance._hands;
		}

		public void MetaLateUpdate()
		{
			this._handInputBuffer.UpdateHandInput(ref this._hands);
			for (int i = 0; i < 2; i++)
			{
				this._hands[i].LocalToWorldCoordinate(base.get_transform());
			}
			this._handObjects.UpdateHandGO(ref this._hands);
			MetaOldDLLMetaInputFaker.GetDynamicHandGestureData(ref this._dynamicGesture);
		}

		private void OnValidate()
		{
			if (Application.get_isPlaying())
			{
				this._handConfig.SetAllParameters();
			}
		}

		private void OnDestroy()
		{
			if (this._handObjects != null)
			{
				this._handObjects.DestroyHandGO();
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "registerHands")]
		private static extern void RegisterHands();

		[DllImport("MetaVisionDLL", EntryPoint = "enableHands")]
		private static extern void EnableHands();

		public void MetaInit()
		{
			Hands.RegisterHands();
			Hands.EnableHands();
			MetaOldDLLMetaInputFaker.InitializeHandData();
			this._handConfig.SetAllParameters();
			for (int i = 0; i < 2; i++)
			{
				this._hands[i] = new Hand();
			}
			this._handObjects.InitHandGO(ref this._hands, base.get_transform());
		}

		public void MetaUpdate()
		{
			MetaOldDLLMetaInputFaker.UpdateData();
			this._handInputBuffer.GetHandData();
		}

		public void MetaOnDestroy()
		{
		}
	}
}
