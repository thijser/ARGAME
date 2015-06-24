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
    using Network;
    using UnityEngine;

    /// <summary>
    /// Previews the position of PositionUpdate objects.
    /// </summary>
    public class RemoteMarkerHolder : MarkerHolder<RemoteMarker>
    {
        /// <summary>
        /// The factor with which to scale the local positions of markers.
        /// </summary>
        public const float LocalScaleFactor = 82;

        /// <summary>
        /// Updates the positions of the markers.
        /// </summary>
        public void Update()
        {
            this.UpdateMarkerPositions(Matrix4x4.identity);
        }

        /// <summary>
        /// Updates the position of a marker as indicated by the given 
        /// <see cref="PositionUpdate"/>.
        /// </summary>
        /// <param name="update">The <see cref="PositionUpdate"/>.</param>
        public override void OnPositionUpdate(PositionUpdate update)
        {
            base.OnPositionUpdate(update);
            this.GetMarker(update.Id).RemotePosition.Scale.Set(LocalScaleFactor, LocalScaleFactor, LocalScaleFactor);
        }
    }
}
