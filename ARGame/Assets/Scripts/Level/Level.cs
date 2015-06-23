namespace Level
{
    using System.Collections.Generic;

    /// <summary>
    /// Information about a level.
    /// </summary>
    public class Level
    {
        /// <summary>
        /// Description of level properties.
        /// </summary>
        public readonly LevelProperties Properties;

        /// <summary>
        /// List of objects within the level.
        /// </summary>
        public readonly List<LevelObject> Objects;

        /// <summary>
        /// Create a new level description object.
        /// </summary>
        /// <param name="properties">Level properties.</param>
        /// <param name="objects">Objects within level.</param>
        public Level(LevelProperties properties, List<LevelObject> objects)
        {
            this.Properties = properties;
            this.Objects = objects;
        }
    }
}
