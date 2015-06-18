using UnityEngine;

namespace Level
{
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
        /// Position of object in tilespace.
        /// </summary>
        public readonly Vector2 Position;

        /// <summary>
        /// Rotation of object in level.
        /// </summary>
        public readonly float Rotation;

        /// <summary>
        /// Create description of object found in level.
        /// </summary>
        /// <param name="type">Type of object.</param>
        /// <param name="position">Position of object in tilespace.</param>
        /// <param name="rotation">Rotation of object in level.</param>
        public LevelObject(TileType type, Vector2 position, float rotation)
        {
            this.Type = type;
            this.Position = position;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Returns string representation of level object descriptor.
        /// </summary>
        /// <returns>String representation of level object descriptor.</returns>
        public override string ToString()
        {
            return "LevelObject[Type = " + Type + ", Position = " + Position + ", Rotation = " + Rotation + "]";
        }
    }
}
