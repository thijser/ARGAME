//----------------------------------------------------------------------------
// <copyright file="Detector.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections.ObjectModel;
    using UnityEngine;

    /// <summary>
    /// Broadcasts the MarkerPositions from an <see cref="IARLink"/> instance
    /// as Unity messages.
    /// </summary>
    public class Detector : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the IARLink instance used to get MarkerPositions from.
        /// </summary>
        public IARLink Link { get; set; }

        /// <summary>
        /// Initializes the Detector with a MetaLink instance.
        /// </summary>
        public void Start()
        {
            this.Link = new GameObject("MetaLink", typeof(MetaLink)).GetComponent<MetaLink>();
        }

        /// <summary>
        /// Retrieves the MarkerPositions from the <see cref="IARLink"/> 
        /// instance and broadcasts all positions as "OnMarkerSeen" Unity 
        /// messages.
        /// </summary>
        public void LateUpdate()
        {
            Collection<MarkerPosition> list = this.Link.GetMarkerPositions();
            foreach (MarkerPosition mp in list)
            {
                Debug.Log(mp.ToString());
                this.SendMessage(
                    "OnMarkerSeen",
                    mp,
                    SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}