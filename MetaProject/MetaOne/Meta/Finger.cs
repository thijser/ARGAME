using System;

namespace Meta
{
	internal class Finger : HandGameEntity
	{
		private int _fingerID;

		internal void CopyTo(ref Finger finger)
		{
			finger = (Finger)base.Clone();
		}
	}
}
