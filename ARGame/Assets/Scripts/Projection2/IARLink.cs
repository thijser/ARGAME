//----------------------------------------------------------------------------
// <copyright file="IARLink.cs" company="Delft University of Technology">
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

    public interface IARLink 
    {
        /// <summary>
        /// return a list of all vissible markers and their locations, can be empty if none are visible. 
        /// </summary>
        /// <returns>The marker positions.</returns>
         List<MarkerPosition> GetMarkerPositions();
    }
}