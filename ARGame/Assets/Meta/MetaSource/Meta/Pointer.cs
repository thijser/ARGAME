using System;

namespace Meta
{
	public class Pointer : HandGameEntity
	{
		internal void CopyTo(ref Pointer pointer)
		{
			pointer = (Pointer)base.Clone();
		}
	}
}
