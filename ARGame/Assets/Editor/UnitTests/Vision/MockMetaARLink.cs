//----------------------------------------------------------------------------
// <copyright file="MockMetaARLink.cs" company="Delft University of Technology">
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
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Projection;
    using UnityEngine;

    /// <summary>
    /// Mock implementation of IARLink that replicates the 
    /// expected behavior of the Meta One glasses.
    /// <para>
    /// This allows for easier testing of code that depends on 
    /// a Meta-like ARLink implementation.
    /// </para>
    /// </summary>
    public class MockMetaARLink : MonoBehaviour, IARLink
    {
        /// <summary>
        /// The scale to use for the Meta.
        /// <para>
        /// This value replicates the Meta scale defined in MetaLink.
        /// </para>
        /// </summary>
        public const float MetaScale = MetaLink.MetaScale;

        /// <summary>
        /// Gets or sets the Positions returned by a call to the
        /// <c>GetMarkerPositions()</c> method of this MockMetaARLink.
        /// </summary>
        public Collection<MarkerPosition> MarkerPositions { get; set; }

        /// <summary>
        /// Sets the position, rotation and scale of a marker.
        /// <para>
        /// If no position exists yet for the Marker, a new MarkerPosition is created.
        /// </para>
        /// <para>
        /// This method allows tests to place GameObjects in any way in the world, and
        /// pass them into this method one by one to simulate markers as seen by the Meta.
        /// </para>
        /// </summary>
        /// <param name="id">The Id of the Marker.</param>
        /// <param name="transform">The Transform for the Marker, not null.</param>
        public void SetMarker(int id, Transform t)
        {
            if (this.MarkerPositions == null)
            {
                this.MarkerPositions = new Collection<MarkerPosition>();
            }

            MarkerPosition marker = this.MarkerPositions
                .SkipWhile(pos => pos.ID != id)
                .FirstOrDefault();

            if (marker == null)
            {
                marker = new MarkerPosition(t.position, t.rotation, DateTime.Now, t.lossyScale, id);
                this.MarkerPositions.Add(marker);
            }
            else
            {
                marker.Position = t.position;
                marker.Rotation = t.rotation;
                marker.Scale = t.lossyScale;
                marker.TimeStamp = DateTime.Now;
            }
        }

        /// <summary>
        /// Returns a list of all visible markers and their locations.
        /// <para>
        /// The result can be empty if no markers are visible. 
        /// </para>
        /// <para>
        /// This mock implementation returns the value of the 
        /// <c>MarkerPositions</c> property.
        /// </para>
        /// </summary>
        /// <returns>The marker positions.</returns>
        public Collection<MarkerPosition> GetMarkerPositions()
        {
            return this.MarkerPositions;
        }

        /// <summary>
        /// Gets the scale by which objects need to be scaled to fit the
        /// AR glasses.
        /// </summary>
        /// <returns>The Meta AR Link scale.</returns>
        public float GetScale()
        {
            return MetaScale;
        }
    }
}