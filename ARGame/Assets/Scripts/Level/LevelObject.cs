//----------------------------------------------------------------------------
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
    using UnityEngine;
    
    /// <summary>
    /// Type of a tile loaded from level file.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// Indicates a wall tile.
        /// </summary>
        Wall = 0,

        /// <summary>
        /// Indicates a green emitter tile.
        /// </summary>
        EmitterG = 1,

        /// <summary>
        /// Indicates a red emitter tile.
        /// </summary>
        EmitterR = 2,
        
        /// <summary>
        /// Indicates a blue emitter tile.
        /// </summary>
        EmitterB = 3,

        /// <summary>
        /// Indicates a green target tile.
        /// </summary>
        TargetG = 4,

        /// <summary>
        /// Indicates a red target tile.
        /// </summary>
        TargetR = 5,

        /// <summary>
        /// Indicates a blue target tile.
        /// </summary>
        TargetB = 6,

        /// <summary>
        /// Indicates a mirror tile.
        /// </summary>
        Mirror = 7,

        /// <summary>
        /// Indicates a splitter tile.
        /// </summary>
        Splitter = 8,

        /// <summary>
        /// Indicates an emitter tile.
        /// </summary>
        Elevator = 9,

        /// <summary>
        /// Indicates a portal tile.
        /// </summary>
        Portal = 10,

        /// <summary>
        /// Indicates an empty tile.
        /// </summary>
        Nothing = 11
    }

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
        /// Position of object in level coordinates.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// Rotation of object in level.
        /// </summary>
        public readonly float Rotation;

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
        /// Returns a string representation of this <see cref="LevelObject"/>.
        /// </summary>
        /// <returns>A string describing this <see cref="LevelObject"/>.</returns>
        public override string ToString()
        {
            return "LevelObject[Type = " + this.Type + ", Position = " + this.Position + ", Rotation = " + this.Rotation + "]";
        }
    }
}
