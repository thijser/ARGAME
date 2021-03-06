﻿//----------------------------------------------------------------------------
// <copyright file="LevelObject.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Description of object found in the level.
    /// </summary>
    public class LevelObject
    {
        /// <summary>
        /// Type of object.
        /// </summary>
        public readonly TileType Type;

        /// <summary>
        /// position of object in level coordinates.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// Rotation of object in level.
        /// </summary>
        public readonly float Rotation;

        /// <summary>
        /// Gets or sets the game object instantiated from this level object.
        /// </summary>
        public GameObject Instance { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelObject"/> class.
        /// </summary>
        /// <param name="type">The <see cref="TileType"/> of the object.</param>
        /// <param name="position">The position of the object in level coordinates.</param>
        /// <param name="rotation">The rotation of the object in the level.</param>
        public LevelObject(TileType type, Vector2 position, float rotation)
        {
            this.Type = type;
            this.Position = position;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Gets the pair Id of portal if this object is one.
        /// <para>
        /// This property is <c>-1</c> if this <see cref="LevelObject"/> is not 
        /// a portal.
        /// </para>
        /// </summary>
        /// <seealso cref="LevelObject.IsPortal"/>
        public int PortalPair
        {
            get
            {
                Dictionary<TileType, int> portals = new Dictionary<TileType, int>()
                {
                    { TileType.PortalEntryOne, 0 },
                    { TileType.PortalEntryTwo, 1 },
                    { TileType.PortalEntryThree, 2 },
                    { TileType.PortalExitOne, 0 },
                    { TileType.PortalExitTwo, 1 },
                    { TileType.PortalExitThree, 2 }
                };

                try
                {
                    return portals[this.Type];
                }
                catch (KeyNotFoundException)
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Check if this level object is a portal.
        /// </summary>
        /// <returns>True if this level object is a portal entry or exit.</returns>
        public bool IsPortal()
        {
            return this.Type >= TileType.PortalEntryOne && this.Type <= TileType.PortalExitThree;
        }

        /// <summary>
        /// Returns a string representation of this <see cref="LevelObject"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="LevelObject"/>.</returns>
        public override string ToString()
        {
            return "LevelObject[Type = " + this.Type + ", Position = " + this.Position + ", Rotation = " + this.Rotation + "]";
        }
    }
}
