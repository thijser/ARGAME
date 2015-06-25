﻿//----------------------------------------------------------------------------
// <copyright file="RemoteMarker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Represents a marker for a remote player.
    /// </summary>
    public class RemoteMarker : Marker
    {
        /// <summary>
        /// The scale factor to apply.
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// Initializes the scale factor to 1 if it was not set.
        /// </summary>
        public void Start()
        {
            if (this.ScaleFactor == 0)
            {
                this.ScaleFactor = 2f;
            }
        }

        /// <summary>
        /// Updates the position of this Marker using the provided Matrix.
        /// </summary>
        /// <param name="transformMatrix">The transformation Matrix.</param>
        public override void UpdatePosition(Matrix4x4 transformMatrix)
        {
            this.RemotePosition.Scale = this.ScaleFactor * Vector3.one;
            base.UpdatePosition(transformMatrix);
        }
    }
}
