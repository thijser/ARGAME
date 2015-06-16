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
    /// Implementation of <see cref="IARLink"/> that allows setting
    /// Marker positions from code.
    /// <para>
    /// This class can be used to create an <see cref="IARLink"/>
    /// implementation that uses a backing Collection for storing
    /// the Marker positions, and implementations for which the
    /// Marker position can be set using 
    /// </para>
    /// </summary>
    public class ARLinkAdapter : MonoBehaviour, IARLink
    {
        /// <summary>
        /// Gets or sets the Positions returned by a call to the
        /// <c>GetMarkerPositions()</c> method of this ARLinkAdapter.
        /// </summary>
        public Collection<MarkerPosition> MarkerPositions { get; set; }

        /// <summary>
        /// Gets or sets the scale to use for the AR objects.
        /// </summary>
        public float ARScale { get; set; }

        /// <summary>
        /// Sets the position, rotation and scale of a marker.
        /// <para>
        /// If no position exists yet for the Marker, a new MarkerPosition is created.
        /// </para>
        /// <para>
        /// This method allows placing GameObjects in any way in the world, and
        /// update their corresponding MarkerPositions by calling this method
        /// for all markers.
        /// </para>
        /// </summary>
        /// <param name="id">The ID of the Marker.</param>
        /// <param name="t">The Transform for the Marker, not null.</param>
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
        /// This implementation returns the value of the 
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
        /// <returns>The AR scale.</returns>
        public float GetScale()
        {
            return this.ARScale;
        }
    }
}