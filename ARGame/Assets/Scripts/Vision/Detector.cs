//----------------------------------------------------------------------------
// <copyright file="Detector.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Vision
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Projection;
    using UnityEngine;
    using System;

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
            IARLink[] links = GetComponents<IARLink>();
            if (links.Length != 1)
            {
                Debug.LogWarning("Expected exactly one IARLink, but got " + links.Length + " (IARLink will be disabled)");
            }
            else
            {
                this.Link = links[0];
            }
        }

        /// <summary>
        /// Retrieves the MarkerPositions from the <see cref="IARLink"/> 
        /// instance and broadcasts all positions as "OnMarkerSeen" Unity 
        /// messages.
        /// </summary>
        public void LateUpdate()
        {
            if (this.Link == null)
            {
                return;
            }

            foreach (MarkerPosition marker in this.Link.GetMarkerPositions())
            {
                this.EmitMarkerSeen(marker);
            }
        }

        /// <summary>
        /// Sends a <c>OnMarkerSeen</c> Unity message upwards.
        /// </summary>
        /// <param name="position">The MarkerPosition describing the message, not null.</param>
        /// <exception cref="ArgumentNullException">If <c>position</c> is null.</exception>
        public void EmitMarkerSeen(MarkerPosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            this.SendMessageUpwards("OnMarkerSeen", position);
        }
    }
}