//----------------------------------------------------------------------------
// <copyright file="BaseForLevel.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
	using UnityEngine;

	public class BaseForLevel : MonoBehaviour 
	{
		public const int Patience = 10;

		public GameObject Basepoint;

		public long Timestamp = 0;

		public void Seen()
		{
			Timestamp = Time.frameCount;
			UsedCardManager holder = this.Basepoint.GetComponent<UsedCardManager>();
			if(holder.CurrentlyUsed.Timestamp + Patience < this.Timestamp)
			{
				Transform p = transform.parent;
				transform.parent = null;
				p.parent = transform;
				holder.CurrentlyUsed = this;
			}
		}

	}
}