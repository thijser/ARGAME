//----------------------------------------------------------------------------
// <copyright file="MarkerRegister.cs" company="Delft University of Technology">
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
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Event message for registration of Marker instances.
    /// </summary>
    public class MarkerRegister
    {
        /// <summary>
        /// The Marker that needs to be reigstered.
        /// </summary>
        public Marker RegisteredMarker { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerRegister"/> class.
        /// </summary>
        /// <param name="mark">The Marker to be registered.</param>
        public MarkerRegister(Marker mark)
        {
            this.RegisteredMarker = mark;
        }
    }
}