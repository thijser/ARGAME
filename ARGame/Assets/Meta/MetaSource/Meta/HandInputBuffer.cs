using System;
using System.Runtime.InteropServices;

namespace Meta
{
	internal class HandInputBuffer
	{
		private CppHandData[] _cppHandData = new CppHandData[2];

		public HandInputBuffer()
		{
			for (int i = 0; i < 2; i++)
			{
				this._cppHandData[i] = default(CppHandData);
				this._cppHandData[i].Init();
			}
		}

		[DllImport("MetaVisionDLL", EntryPoint = "getHandData")]
		private static extern void GetHandData(ref CppHandData leftHand, ref CppHandData rightHand);

		public void GetHandData()
		{
			if (Hands.useFaker)
			{
				MetaOldDLLMetaInputFaker.GetHandData(ref this._cppHandData[0], ref this._cppHandData[1]);
			}
			else
			{
				HandInputBuffer.GetHandData(ref this._cppHandData[0], ref this._cppHandData[1]);
			}
		}

		public void UpdateHandInput(ref Hand[] hands)
		{
			for (int i = 0; i < 2; i++)
			{
				hands[i].Update(this._cppHandData[i]);
			}
		}
	}
}
