using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Level
{
    /// <summary>
    /// Descriptor of level.
    /// </summary>
    public class LevelDescriptor
    {
        /// <summary>
        /// Width of the level in tiles.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the level in tiles.
        /// </summary>
        public int Height;

        /// <summary>
        /// Width of a single tile in pixels.
        /// </summary>
        public int TileWidth;

        /// <summary>
        /// Height of a single tile in pixels.
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// Amount of tiles in horizontal direction.
        /// </summary>
        public int HorizontalTiles;

        /// <summary>
        /// Amount of tiles in vertical direction.
        /// </summary>
        public int VerticalTiles;
    }
}
