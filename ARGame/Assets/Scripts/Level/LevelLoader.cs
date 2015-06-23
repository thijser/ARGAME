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
    using UnityEngine;
    using Core;
    using Core.Receiver;
    using Projection;

    /// <summary>
    /// Level loader that loads levels created with the Tiled map editor.
    /// </summary>
    public class LevelLoader
    {
        /// <summary>
        /// The Id of the virtual level marker.
        /// </summary>
        private const int LevelMarkerID = 13379001;

        /// <summary>
        /// Mapping of level object types to prefabs.
        /// </summary>
        private Dictionary<TileType, GameObject> objectPrefabs = null;

        /// <summary>
        /// Gets or sets the size of the board, used for aligning the level.
        /// </summary>
        public Vector2 BoardSize { get; set; }

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

            Level levelInfo = LevelParser.LoadLevel(path);
            GameObject level = this.ConstructLevel(levelInfo.Properties, levelInfo.Objects);

            return level;
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
                    int pair = obj.PortalPair;

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
        /// Constructs the game objects from objects within a level.
        /// </summary>
        /// <param name="level">Level descriptor.</param>
        /// <param name="levelObjects">List of level objects.</param>
        /// <returns>Parent GameObject that represents level.</returns>
        private GameObject ConstructLevel(LevelProperties level, List<LevelObject> levelObjects)
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
            Vector3 levelPosition = new Vector3(-(this.BoardSize.x - level.Width) / 2, 0, (this.BoardSize.y - level.Height) / 2);
            parent.transform.localPosition = levelPosition;

            // Add level marker if this is a local player
            if (GameObject.Find("MetaWorld") != null)
            {
                Marker marker = parent.AddComponent<Marker>();
                marker.Id = LevelMarkerID;
                marker.RemotePosition = new MarkerPosition(-8f * levelPosition, Quaternion.identity, DateTime.Now, 8f * new Vector3(-1, 1, -1), LevelMarkerID);
            }

            return parent;
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
    }
}