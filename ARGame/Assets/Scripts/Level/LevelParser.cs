namespace Level
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;
    using UnityEngine;

    /// <summary>
    /// Parses level from Tiled XML.
    /// </summary>
    public static class LevelParser
    {
        /// <summary>
        /// Parse a level created with Tiled and the objects within.
        /// </summary>
        /// <param name="xml">XML representation of level as written by Tiled editor.</param>
        /// <returns>Level descriptor and descriptors of objects within the level.</returns>
        private static KeyValuePair<LevelDescriptor, List<LevelObject>> ParseLevel(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            LevelDescriptor level = ParseLevelHeader(doc);
            List<LevelObject> levelObjects = ParseLevelTiles(doc, level);

            return new KeyValuePair<LevelDescriptor, List<LevelObject>>(level, levelObjects);
        }

        /// <summary>
        /// Read level info from XML exported by Tiled.
        /// </summary>
        /// <param name="levelDoc">XML document of level.</param>
        /// <returns>Info about level.</returns>
        private static LevelDescriptor ParseLevelHeader(XmlDocument levelDoc)
        {
            LevelDescriptor levelDescriptor = new LevelDescriptor();

            // Find nodes containing relevant attributes
            var mapNode = levelDoc.SelectSingleNode("map");
            var imageNode = levelDoc.SelectSingleNode("//image");
            var tilesetNode = levelDoc.SelectSingleNode("//tileset");

            // Read data from them
            levelDescriptor.Width = int.Parse(mapNode.Attributes["width"].Value, CultureInfo.InvariantCulture);
            levelDescriptor.Height = int.Parse(mapNode.Attributes["height"].Value, CultureInfo.InvariantCulture);

            levelDescriptor.TileWidth = int.Parse(tilesetNode.Attributes["tilewidth"].Value, CultureInfo.InvariantCulture);
            levelDescriptor.TileHeight = int.Parse(tilesetNode.Attributes["tileheight"].Value, CultureInfo.InvariantCulture);

            levelDescriptor.HorizontalTiles = int.Parse(imageNode.Attributes["width"].Value, CultureInfo.InvariantCulture) / levelDescriptor.TileWidth;
            levelDescriptor.VerticalTiles = int.Parse(imageNode.Attributes["height"].Value, CultureInfo.InvariantCulture) / levelDescriptor.TileHeight;

            return levelDescriptor;
        }

        /// <summary>
        /// Load objects from tiles in level exported to XML.
        /// </summary>
        /// <param name="levelDoc">XML document of level.</param>
        /// <param name="level">Level descriptor returned by ParseLevelHeader.</param>
        /// <returns>List of objects placed within level.</returns>
        private static List<LevelObject> ParseLevelTiles(XmlDocument levelDoc, LevelDescriptor level)
        {
            int x = 0;
            int y = 0;

            var levelObjects = new List<LevelObject>();

            foreach (XmlNode tileNode in levelDoc.SelectNodes("//tile"))
            {
                int gid = int.Parse(tileNode.Attributes["gid"].Value, CultureInfo.InvariantCulture);

                // Bug in Tiled where it sometimes outputs tiles with gid 1 as 0
                gid = Math.Max(1, gid);

                // Determine rotation of object
                int rotation = ((gid - 1) / level.HorizontalTiles) * 45;

                // Determine type of object
                int rawType = (gid - 1) % level.HorizontalTiles;
                TileType type = (TileType)rawType;

                // Determine position of object in world coordinates
                Vector2 pos = new Vector2(x, y);

                if (type != TileType.Nothing)
                {
                    levelObjects.Add(new LevelObject(type, pos, rotation));
                }

                // UpdatePosition X, Y position
                if (x + 1 == level.Width)
                {
                    y++;
                }

                x = (x + 1) % level.Width;
            }

            return levelObjects;
        }

        /// <summary>
        /// Loads the Tiled level given by the specified path.
        /// </summary>
        /// <param name="path">Path to level file in Resources.</param>
        /// <returns>Info about parsed level.</returns>
        public static KeyValuePair<LevelDescriptor, List<LevelObject>> LoadLevel(string path)
        {
            try
            {
                string xml = (Resources.Load(path) as TextAsset).text;
                return ParseLevel(xml);
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException("Invalid level path (" + path + ").");
            }
        }
    }
}
