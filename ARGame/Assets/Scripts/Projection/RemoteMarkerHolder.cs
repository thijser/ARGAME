//----------------------------------------------------------------------------
// <copyright file="RemoteMarkerHolder.cs" company="Delft University of Technology">
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

    /// <summary>
    /// Previews the position of PositionUpdate objects.
    /// </summary>
    public class RemoteMarkerHolder : MarkerHolder<RemoteMarker>
    {
        /// <summary>
        /// Gets or sets the player to follow.
        /// </summary>
        public RemotePlayerMarker PlayerToFollow { get; set; }

        /// <summary>
        /// Updates the positions of the markers.
        /// <para>
        /// If <c>PlayerToFollow</c> is set, then the board is 
        /// projected as seen by that player. Otherwise, the 
        /// board is projection on the xz-plane.
        /// </para>
        /// </summary>
        public void Update()
        {
            if (this.PlayerToFollow == null)
            {
                this.UpdateMarkerPositions(Matrix4x4.identity);
            }
            else
            {
                this.UpdateMarkerPositions(this.PlayerToFollow.RemotePosition.Matrix.inverse);
            }
        }
    }
}
