//----------------------------------------------------------------------------
// <copyright file="Marker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections;
    using UnityEngine;

	public class Marker : MonoBehaviour 
    {
		public int id = -1;
		public MarkerPosition remotePosition;
		public MarkerPosition localPosition;
		float objectRotation; 
	
		public void SetObjectRotation(float rotation)
        {
			objectRotation = rotation;
		}
	
		public void SetRemotePosition(MarkerPosition rem)
        {
			remotePosition = rem;
		}

		public void SetLocalPosition(MarkerPosition rem)
        {
			localPosition = rem;
		}

		void Start()
        {
			this.SendMessageUpwards("OnMarkerRegister", new MarkerRegister(this));
		}
	}
}