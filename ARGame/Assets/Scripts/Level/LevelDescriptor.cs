//----------------------------------------------------------------------------
// <copyright file="LevelDescriptor.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    /// <summary>
    /// Container class describing details about a level.
    /// </summary>
    public class LevelProperties
    {
        /// <summary>
        /// Gets or sets the width of the level in tiles.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the level in tiles.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width of a single tile in pixels.
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of a single tile in pixels.
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        /// Gets or sets the amount of tiles in horizontal direction.
        /// </summary>
        public int HorizontalTiles { get; set; }

        /// <summary>
        /// Gets or sets the amount of tiles in vertical direction.
        /// </summary>
        public int VerticalTiles { get; set; }
    }
}
