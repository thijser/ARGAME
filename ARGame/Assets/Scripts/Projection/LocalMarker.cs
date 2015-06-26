//----------------------------------------------------------------------------
// <copyright file="LocalMarker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    /// <summary>
    /// Represents a marker detected in the world.
    /// <para>
    /// This class acts as a container for the position and rotation data 
    /// coming from both the remote server as well as the local IARLink 
    /// implementation.
    /// </para>
    /// </summary>
    public class LocalMarker : Marker
    {
        /// <summary>
        /// Gets or sets the local position of this marker.
        /// </summary>
        public MarkerPosition LocalPosition { get; set; }

        /// <summary>
        /// Returns a string representation of this Marker.
        /// </summary>
        /// <returns>A string describing this Marker.</returns>
        public override string ToString()
        {
            return "<LocalMarker:id=" + this.Id +
                ", RemotePosition=" + this.RemotePosition +
                ", LocalPosition=" + this.LocalPosition +
                ", ObjectRotation=" + this.ObjectRotation + ">";
        }
    }
}