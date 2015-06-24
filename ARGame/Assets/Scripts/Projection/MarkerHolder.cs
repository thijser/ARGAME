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
    using UnityEngine;
    
    /// <summary>
    /// Container class for Markers.
    /// </summary>
    /// <typeparam name="T">The type of markers this MarkerHolder holds.</typeparam>
    public class MarkerHolder<T> : MonoBehaviour where T : Marker
    {

    }
}
