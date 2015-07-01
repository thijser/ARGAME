//----------------------------------------------------------------------------
// <copyright file="Level.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    using System.Collections.ObjectModel;

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
        public readonly ReadOnlyCollection<LevelObject> Objects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        /// <param name="properties">Level properties.</param>
        /// <param name="objects">Objects within level.</param>
        public Level(LevelProperties properties, ReadOnlyCollection<LevelObject> objects)
        {
            this.Properties = properties;
            this.Objects = objects;
        }
    }
}
