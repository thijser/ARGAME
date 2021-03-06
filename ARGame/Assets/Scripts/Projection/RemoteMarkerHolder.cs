﻿//----------------------------------------------------------------------------
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
    using Camera;

    /// <summary>
    /// Previews the position of PositionUpdate objects.
    /// </summary>
    public class RemoteMarkerHolder : MarkerHolder<RemoteMarker>
    {
        private RemotePlayerMarker playerToFollow = null;

        /// <summary>
        /// Gets or sets the player to follow.
        /// </summary>
        public RemotePlayerMarker PlayerToFollow {
            get {
                return playerToFollow;
            }
            set {
                if (DoesMarkerHaveHead(playerToFollow)) {
                    playerToFollow.transform.FindChild("HEAD").GetComponent<MeshRenderer>().enabled = true;
                }

                if (DoesMarkerHaveHead(value)) {
                    value.transform.FindChild("HEAD").GetComponent<MeshRenderer>().enabled = false;
                }

                if (value != null) {
                    if (value.Id == -1) {
                        Camera.main.fieldOfView = 70;
                    } else {
                        Camera.main.fieldOfView = 12;
                    }
                }

                playerToFollow = value;
            }
        }

        private bool DoesMarkerHaveHead(RemotePlayerMarker marker) {
            return marker != null && marker.transform.FindChild("HEAD") != null;
        }

        /// <summary>
        /// Updates the positions of the markers.
        /// <para>
        /// If <c>PlayerToFollow</c> is set, then the board is 
        /// projected as seen by that player. Otherwise, the 
        /// board is projection on the plane spanned by the x-axis 
        /// and the z-axis.
        /// </para>
        /// </summary>
        public void Update()
        {
            if (this.PlayerToFollow == null || this.PlayerToFollow.RemotePosition == null)
            {
                this.UpdateMarkerPositions(Matrix4x4.identity);
            }
            else
            {
                Matrix4x4 matrix = this.PlayerToFollow.RemotePosition.Matrix.inverse;
                this.UpdateMarkerPositions(matrix);
            }
        }
    }
}
