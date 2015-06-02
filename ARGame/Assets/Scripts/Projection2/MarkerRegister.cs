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
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Use this message for registering a marker. 
    /// </summary>
    public class MarkerRegister  
    {
        Marker marker;

        public MarkerRegister(Marker mark)
        {
            this.marker = mark;
        }

        public Marker getMarker()
        {
            return marker;
        }
    }
}