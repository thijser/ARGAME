//----------------------------------------------------------------------------
// <copyright file="TiledLevelLoader.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Level
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Core;
    using Core.Receiver;
    using UnityEngine;

    /// <summary>
    /// Level loader that loads levels created with the Tiled map editor.
    /// </summary>
    public class TiledLevelLoader : MonoBehaviour
    {
        /// <summary>
        /// Level to load in the Resources/Level directory.
        /// </summary>
        public string Level = "basic";

        /// <summary>
        /// Mapping of level object types to prefabs.
        /// </summary>
        private Dictionary<TileType, GameObject> objectPrefabs = new Dictionary<TileType, GameObject>();

        /// <summary>
        /// Loads the level prefabs and the level itself.
        /// </summary>
        public void Start()
        {
            // Load prefabs for each tile type
            this.objectPrefabs[TileType.Wall] = Resources.Load("Prefabs/BOXWALL") as GameObject;
            this.objectPrefabs[TileType.EmitterR] = Resources.Load("Prefabs/Emitter") as GameObject;
            this.objectPrefabs[TileType.EmitterG] = Resources.Load("Prefabs/Emitter") as GameObject;
            this.objectPrefabs[TileType.EmitterB] = Resources.Load("Prefabs/Emitter") as GameObject;
            this.objectPrefabs[TileType.TargetR] = Resources.Load("Prefabs/Laser Target") as GameObject;
            this.objectPrefabs[TileType.TargetG] = Resources.Load("Prefabs/Laser Target") as GameObject;
            this.objectPrefabs[TileType.TargetB] = Resources.Load("Prefabs/Laser Target") as GameObject;
            this.objectPrefabs[TileType.Mirror] = Resources.Load("Prefabs/Mirror") as GameObject;

            // Parse and instantiate level
            string xml = (Resources.Load("levels/" + this.Level) as TextAsset).text;
            List<LevelObject> levelObjects = ParseLevel(xml);

            foreach (LevelObject obj in levelObjects)
            {
                try
                {
                    InstantiateLevelObject(obj, this.objectPrefabs);
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogError("No prefab for " + obj.Type);
                }
            }
        }

        /// <summary>
        /// Instantiates a GameObject from a level object using the prefab mapping.
        /// </summary>
        /// <param name="levelObject">Level object to turn into GameObject.</param>
        /// <param name="objectPrefabs">Mapping of level object types to prefabs.</param>
        /// <returns>The created level GameObject.</returns>
        private static GameObject InstantiateLevelObject(LevelObject levelObject, Dictionary<TileType, GameObject> objectPrefabs)
        {
            GameObject obj = GameObject.Instantiate(objectPrefabs[levelObject.Type]);

            obj.transform.position = new Vector3(-levelObject.Position.x, 0, levelObject.Position.y);
            obj.transform.rotation = Quaternion.AngleAxis(levelObject.Rotation, Vector3.up);

            InitializeObjectColor(obj, levelObject);

            return obj;
        }

        /// <summary>
        /// Sets the object color of the given GameObject, if applicable.
        /// </summary>
        /// <param name="obj">The GameObject.</param>
        /// <param name="levelObject">The LevelObject describing the type of the GameObject.</param>
        /// <returns>True if the color could be changed for the GameObject, false if the operation was not applicable.</returns>
        private static bool InitializeObjectColor(GameObject obj, LevelObject levelObject)
        {
            switch (levelObject.Type)
            {
                case TileType.EmitterR:
                    obj.GetComponentInChildren<LaserProperties>().RGBStrengths = new Vector3(0.2f, 0f, 0f);
                    break;
                case TileType.EmitterG:
                    obj.GetComponentInChildren<LaserProperties>().RGBStrengths = new Vector3(0f, 0.2f, 0f);
                    break;
                case TileType.EmitterB:
                    obj.GetComponentInChildren<LaserProperties>().RGBStrengths = new Vector3(0f, 0f, 0.2f);
                    break;
                case TileType.TargetR:
                    obj.GetComponent<LaserTarget>().TargetColor = new Color(1, 0, 0);
                    break;
                case TileType.TargetG:
                    obj.GetComponent<LaserTarget>().TargetColor = new Color(0, 1, 0);
                    break;
                case TileType.TargetB:
                    obj.GetComponent<LaserTarget>().TargetColor = new Color(0, 0, 1);
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Parse a level created with Tiled and the objects within.
        /// </summary>
        /// <param name="xml">XML representation of level as written by Tiled editor.</param>
        /// <returns>Descriptors of objects within the level.</returns>
        private static List<LevelObject> ParseLevel(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            LevelDescriptor level = ParseLevelHeader(doc);

            return ParseLevelTiles(doc, level);
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
            levelDescriptor.Width = int.Parse(mapNode.Attributes["width"].Value);
            levelDescriptor.Height = int.Parse(mapNode.Attributes["height"].Value);

            levelDescriptor.TileWidth = int.Parse(tilesetNode.Attributes["tilewidth"].Value);
            levelDescriptor.TileHeight = int.Parse(tilesetNode.Attributes["tileheight"].Value);

            levelDescriptor.HorizontalTiles = int.Parse(imageNode.Attributes["width"].Value) / levelDescriptor.TileWidth;
            levelDescriptor.VerticalTiles = int.Parse(imageNode.Attributes["height"].Value) / levelDescriptor.TileHeight;

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
                int gid = int.Parse(tileNode.Attributes["gid"].Value);

                // Bug in Tiled where it sometimes outputs tiles with gid 1 as 0
                gid = Math.Max(1, gid);

                // Determine rotation of object
                int rotation = (gid / level.HorizontalTiles) * 45;

                // Determine type of object
                int rawType = (gid - 1) % level.HorizontalTiles;
                TileType type = (TileType)rawType;

                // Determine position of object in world coordinates
                Vector2 pos = new Vector2(x, y);

                if (type != TileType.Nothing)
                {
                    levelObjects.Add(new LevelObject(type, pos, rotation));
                }

                // Update X, Y position
                if (x + 1 == level.Width)
                {
                    y++;
                }

                x = (x + 1) % level.Width;
            }

            return levelObjects;
        }
    }
}