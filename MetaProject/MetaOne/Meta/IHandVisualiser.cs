using System;

namespace Meta
{
	internal interface IHandVisualiser<T>
	{
		bool currentlyActive
		{
			get;
		}

		void GetDisplayData(ref T leftHandDisplay, ref T rightHandDisplay);

		void Enable();

		void Disable();
	}
}
