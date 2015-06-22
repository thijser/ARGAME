//----------------------------------------------------------------------------
// <copyright file="RemoteMarker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using UnityEngine;

    /// <summary>
    /// Represents a marker for a remote player.
    /// </summary>
    public class RemoteMarker : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the <see cref="MarkerState"/> for this <see cref="RemoteMarker"/>.
        /// </summary>
        public MarkerState State { get; set; }
    }
}