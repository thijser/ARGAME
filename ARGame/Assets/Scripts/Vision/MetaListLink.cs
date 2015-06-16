//----------------------------------------------------------------------------
// <copyright file="MetaListLink.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Vision
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Meta;
    using Projection;
    using UnityEngine;


    /// <summary>
    /// <see cref="IARLink"/> implementation that uses the Meta One glasses for providing
    /// AR functionality.
    /// </summary>
    public class MetaListLink : MonoBehaviour, IARLink
    {
        /// <summary>
        /// The size of a Meta marker.
        /// </summary>
        public const float MetaScale = 0.0057f;
		public float GetScale(){
			return MetaScale;
		}
        /// <summary>
        /// Gets the list of virtual markers used by the Meta for tracking.
        /// <para>
        /// This parameter cannot be initialized in Start as doing so causes a race condition as 
        /// the markers start registering themselves before start is done. 
        /// </para>
        /// </summary>
        private List<MetaBody> virtualMarkers = new List<MetaBody>();

        /// <summary>
        /// Creates a virtual marker for the given ID.
        /// </summary>
        /// <param name="id">The Marker ID.</param>
        public void CreateVirtualMarker(int id)
        {
            GameObject marker = new GameObject("virtual marker" + id);
            MetaBody metabody = marker.AddComponent<MetaBody>();

            metabody.markerTarget = true;
            metabody.markerTargetID = id;
            //// metabody.transform.position = new Vector3(13, 666, 1337);
            metabody.markerTargetPlaceable = true;
            this.virtualMarkers.Add(metabody);
        }

        /// <summary>
        /// Creates a virtual Marker for the marker registered in the given MarkerRegister.
        /// </summary>
        /// <param name="register">The MarkerRegister, not null.</param>
        public void OnMarkerRegister(MarkerRegister register)
        {
            if (register == null)
            {
                throw new ArgumentNullException("register");
            }

            this.CreateVirtualMarker(register.RegisteredMarker.ID);
        }

        /// <summary>
        /// Retrieves and returns the positions of all Markers.
        /// </summary>
        /// <returns>A Collection of the MarkerPositions detected by the Meta.</returns>
        public Collection<MarkerPosition> GetMarkerPositions()
        {
            Collection<MarkerPosition> list = new Collection<MarkerPosition>();
            foreach (MetaBody body in this.virtualMarkers)
            {
                int id = body.markerTargetID;

                MarkerPosition mp = new MarkerPosition(
                        body.transform.position,
                        body.transform.rotation,
                        DateTime.Now,
                        MetaScale * body.transform.localScale,
                        id);
                list.Add(mp);
            }

            return list;
        }
    }
}