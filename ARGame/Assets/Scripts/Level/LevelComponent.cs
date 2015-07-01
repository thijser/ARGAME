//----------------------------------------------------------------------------
// <copyright file="LevelComponent.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    using UnityEngine;

    /// <summary>
    /// A Level component with a 2D size.
    /// <para>
    /// Instances of this class are used by the LevelLoader class.
    /// </para>
    /// </summary>
    public class LevelComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the size of this component.
        /// </summary>
        public Vector2 Size { get; set; }
    }
}