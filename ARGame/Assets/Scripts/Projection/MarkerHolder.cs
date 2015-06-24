//----------------------------------------------------------------------------
// <copyright file="MarkerHolder.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;
    
    /// <summary>
    /// Container class for Markers.
    /// </summary>
    /// <typeparam name="T">The type of markers this MarkerHolder holds.</typeparam>
    public class MarkerHolder<T> : MonoBehaviour where T : Marker
    {
        /// <summary>
        /// Collection of all registered to this class. 
        /// </summary>
        private Dictionary<int, T> markers = new Dictionary<int, T>();

        /// <summary>
        /// Gets an <see cref="IEnumerable"/> over all Markers in
        /// this <see cref="MarkerHolder"/>.
        /// </summary>
        public IEnumerable<T> Markers 
        {
            get
            {
                return this.markers.Values;
            }
        }

        /// <summary>
        /// Removes a Marker with the given id, if it exists.
        /// </summary>
        /// <param name="id">The id of the Marker to remove.</param>
        public void RemoveMarker(int id)
        {
            this.markers.Remove(id);
        }

        /// <summary>
        /// Adds a Marker to this MarkerHolder.
        /// </summary>
        /// <param name="marker">The Marker to add, not null.</param>
        public void AddMarker(T marker)
        {
            Assert.IsNotNull(marker);
            this.markers.Add(marker.Id, marker);
        }

        /// <summary>
        /// Returns whether this MarkerHolder contains a Marker with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>True if a Marker with the given id exists, false otherwise.</returns>
        public bool Contains(int id)
        {
            return this.markers.ContainsKey(id);
        }

        /// <summary>
        /// Returns the Marker with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The Marker with the given id.</returns>
        /// <exception cref="KeyNotFoundException">If no Marker with that id exists.</exception>
        public T GetMarker(int id)
        {
            return this.markers[id];
        }

        /// <summary>
        /// Applies the given transformMatrix to all Markers in this MarkerHolder.
        /// </summary>
        /// <param name="transformMatrix">The transformMatrix to apply.</param>
        public void UpdateMarkerPositions(Matrix4x4 transformMatrix)
        {
            foreach (Marker marker in this.Markers)
            {
                marker.UpdatePosition(transformMatrix);
            }
        }
    }
}
