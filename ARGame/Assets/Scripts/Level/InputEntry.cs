//----------------------------------------------------------------------------
// <copyright file="InputEntry.cs" company="Delft University of Technology">
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
    /// only for use in the levelLoader
    /// </summary>
    public class InputEntry
    {
        /// <summary>
        /// Gets or sets the type of this InputEntry as a character.
        /// </summary>
        public char Type { get; set; }
        
        /// <summary>
        /// Gets or sets the direction of this InputEntry as a character.
        /// </summary>
        public char Direction { get; set; }
        
        /// <summary>
        /// Gets or sets the position of this InputEntry.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets the angle of this InputEntry in degrees.
        /// </summary>
        public int Angle
        {
            get
            {
                return 45 * ('0' - this.Direction);
            }
        }
    }
}
