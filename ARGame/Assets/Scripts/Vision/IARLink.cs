//----------------------------------------------------------------------------
// <copyright file="IARLink.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Vision
{
    using System.Collections.ObjectModel;
    using Projection;

    /// <summary>
    /// Interface for providers of AR functionality.
    /// <para>
    /// Implementations of this interface detect markers in the world 
    /// and returns a list of current markers when the <c>GetMarkerPositions()</c>
    /// method is called.
    /// </para>
    /// </summary>
    public interface IARLink 
    {
        /// <summary>
        /// return a list of all visible markers and their locations, can be empty if none are visible. 
        /// </summary>
        /// <returns>The marker positions.</returns>
         Collection<MarkerPosition> GetMarkerPositions();
    }
}