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
    using System.Globalization;
    using System.Xml;
    using Core;
    using Core.Receiver;
    using UnityEngine;
    using Projection;

    /// <summary>
    /// Level loader that loads levels created with the Tiled map editor.
    /// </summary>
    public class TiledLevelLoader
    {
        /// <summary>
        /// The ID of the virtual level marker.
        /// </summary>
        private const int levelMarkerID = 13379001;

        /// <summary>
        /// Gets or sets the size of the board, used for aligning the level.
        /// </summary>
        public Vector2 BoardSize { get; set; }

        /// <summary>
        /// Mapping of level object types to prefabs.
        /// </summary>
        private Dictionary<TileType, GameObject> objectPrefabs = null;

        /// <summary>
        /// Loads and creates a level from the file at the given path.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns>The created level GameObject.</returns>
        public GameObject CreateLevel(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (this.objectPrefabs == null)
            {
                this.LoadPrefabs();
            }

            KeyValuePair<LevelDescriptor, List<LevelObject>> levelInfo = LoadLevel(path);
            GameObject level = ConstructLevel(levelInfo.Key, levelInfo.Value);

            return level;
        }

        /// <summary>
        /// Constructs the game objects from objects within a level.
        /// </summary>
        /// <param name="level">Level descriptor.</param>
        /// <param name="levelObjects">List of level objects.</param>
        /// <returns>Parent GameObject that represents level.</returns>
        private GameObject ConstructLevel(LevelDescriptor level, List<LevelObject> levelObjects)
        {
            GameObject parent = new GameObject("Level");

            // Instantiate prefab for every level object
            foreach (LevelObject obj in levelObjects)
            {
                try
                {
                    GameObject instance = InstantiateLevelObject(obj, this.objectPrefabs);
                    instance.transform.parent = parent.transform;
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogError("No prefab for " + obj.Type);
                }
            }

            // Link paired portals together
            LinkPortals(levelObjects);

            // Determine offset to center level on board
            Vector3 levelPosition = new Vector3(-(BoardSize.x / 2 - level.Width / 2), 0, BoardSize.y / 2 - level.Height / 2);
            parent.transform.localPosition = levelPosition;

            // Add level marker
            Marker marker = parent.AddComponent<Marker>();
            marker.ID = levelMarkerID;
            marker.RemotePosition = new MarkerPosition(-8f * levelPosition, Quaternion.identity, DateTime.Now, 8f * new Vector3(-1, 1, -1), levelMarkerID);

            return parent;
        }

        /// <summary>
        /// Loads the Tiled level given by the specified path.
        /// </summary>
        /// <param name="path">Path to level file in Resources.</param>
        /// <returns>Info about parsed level.</returns>
        private KeyValuePair<LevelDescriptor, List<LevelObject>> LoadLevel(string path)
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

        /// <summary>
        /// Initializes the Prefab GameObject Dictionary.
        /// </summary>
        private void LoadPrefabs()
        {
            this.objectPrefabs = new Dictionary<TileType, GameObject>();

            this.objectPrefabs[TileType.Wall] = Resources.Load("Prefabs/BOXWALL") as GameObject;
            this.objectPrefabs[TileType.EmitterR] = Resources.Load("Prefabs/Emitter") as GameObject;
            this.objectPrefabs[TileType.EmitterG] = Resources.Load("Prefabs/Emitter") as GameObject;
            this.objectPrefabs[TileType.EmitterB] = Resources.Load("Prefabs/Emitter") as GameObject;
            this.objectPrefabs[TileType.TargetR] = Resources.Load("Prefabs/Laser Target") as GameObject;
            this.objectPrefabs[TileType.TargetG] = Resources.Load("Prefabs/Laser Target") as GameObject;
            this.objectPrefabs[TileType.TargetB] = Resources.Load("Prefabs/Laser Target") as GameObject;
            this.objectPrefabs[TileType.Mirror] = Resources.Load("Prefabs/Mirror") as GameObject;
            this.objectPrefabs[TileType.Splitter] = Resources.Load("Prefabs/LensSplitter") as GameObject;
            this.objectPrefabs[TileType.Checkpoint] = Resources.Load("Prefabs/Checkpoint") as GameObject;

            this.objectPrefabs[TileType.PortalEntryOne] = Resources.Load("Prefabs/Portal") as GameObject;
            this.objectPrefabs[TileType.PortalEntryTwo] = Resources.Load("Prefabs/Portal") as GameObject;
            this.objectPrefabs[TileType.PortalEntryThree] = Resources.Load("Prefabs/Portal") as GameObject;

            this.objectPrefabs[TileType.PortalExitOne] = Resources.Load("Prefabs/Portal") as GameObject;
            this.objectPrefabs[TileType.PortalExitTwo] = Resources.Load("Prefabs/Portal") as GameObject;
            this.objectPrefabs[TileType.PortalExitThree] = Resources.Load("Prefabs/Portal") as GameObject;
        }

        /// <summary>
        /// Link all of the portals in pairs together.
        /// </summary>
        /// <param name="levelObjects">Level objects containing portal pairs.</param>
        private static void LinkPortals(List<LevelObject> levelObjects)
        {
            GameObject[] portals = new GameObject[3];

            foreach (LevelObject obj in levelObjects)
            {
                if (obj.IsPortal())
                {
                    int pair = obj.GetPortalPair();

                    if (portals[pair] == null)
                    {
                        portals[pair] = obj.Instance;
                    }
                    else
                    {
                        Portal a = portals[pair].GetComponentInChildren<Portal>();
                        Portal b = obj.Instance.GetComponentInChildren<Portal>();

                        a.LinkedPortal = b;
                        b.LinkedPortal = a;
                    }
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

            levelObject.Instance = obj;

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
                    obj.GetComponentInChildren<LaserProperties>().RGBStrengths = new Vector3(0.1f, 0f, 0f);
                    break;
                case TileType.EmitterG:
                    obj.GetComponentInChildren<LaserProperties>().RGBStrengths = new Vector3(0f, 0.1f, 0f);
                    break;
                case TileType.EmitterB:
                    obj.GetComponentInChildren<LaserProperties>().RGBStrengths = new Vector3(0f, 0f, 0.1f);
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
    }
}