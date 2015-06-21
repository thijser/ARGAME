//----------------------------------------------------------------------------
// <copyright file="LevelLoader.cs" company="Delft University of Technology">
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
    using System.Text.RegularExpressions;
    using Projection;
    using UnityEngine;
    using StreamReader = System.IO.StreamReader;

    /// <summary>
    /// Loads plain text levels using a grid-based encoding.
    /// </summary>
    public class LevelLoader
    {
        /// <summary>
        /// The ID of the virtual level marker.
        /// </summary>
        public const int LevelMarkerID = 13379001;

        /// <summary>
        /// The Dictionary containing the Prefab objects for instantiation.
        /// </summary>
        private Dictionary<char, GameObject> indexes;

        /// <summary>
        /// The entries in the level.
        /// </summary>
        private List<InputEntry> entries;

        /// <summary>
        /// The GameObject to which the level components are added as children.
        /// </summary>
        private GameObject level;

        /// <summary>
        /// The size of the level.
        /// </summary>
        private Vector2 size = new Vector2(1, 1);

        /// <summary>
        /// Gets the size of the level.
        /// </summary>
        public Vector2 LevelSize
        {
            get
            {
                return this.size;
            }
        }

        /// <summary>
        /// Gets or sets the parent of the level GameObject.
        /// </summary>
        public Transform LevelParent { get; set; }

        /// <summary>
        /// Gets or sets the size of the board, used for aligning the level.
        /// </summary>
        public Vector2 BoardSize { get; set; }

        /// <summary>
        /// Initializes the Prefab GameObject Dictionary.
        /// </summary>
        public void LoadObjects()
        {
            this.indexes = new Dictionary<char, GameObject>();
            this.indexes.Add('w', (GameObject)Resources.Load("prefabs/BOXWALL"));
            this.indexes.Add('e', (GameObject)Resources.Load("prefabs/Emitter"));
            this.indexes.Add('t', (GameObject)Resources.Load("prefabs/Laser Target"));
            this.indexes.Add('m', (GameObject)Resources.Load("prefabs/Mirror"));
        }

        /// <summary>
        /// Loads and creates a level from the file at the given path.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns>The created level GameObject.</returns>
        public GameObject CreateLevel(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("The file path name is null.");
            }

            this.LoadLetters(path);
            if (this.indexes == null)
            {
                this.LoadObjects();
            }

            return this.ConstructLevel();
        }

        /// <summary>
        /// Parses the contents of the file and stores the contents as a list of
        /// <see cref="InputEntry"/> objects.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        public void LoadLetters(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                int y = 0;
                this.entries = new List<InputEntry>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    line = Regex.Replace(line, @"\s+", string.Empty);
                    for (int x = 0; x < line.Length; x += 2)
                    {
                        InputEntry ie = new InputEntry();
                        ie.Type = line[x];

                        ie.Direction = line[x + 1];
                        ie.Position = new Vector2(x / 2, y);

                        if (x / 2 > this.size.x)
                        {
                            this.size.x = x / 2;
                        }

                        if (y > this.size.y)
                        {
                            this.size.y = y;
                        }

                        if (ie.Type != '.')
                        {
                            this.entries.Add(ie);
                        }
                    }

                    y++;
                }
            }
        }

        /// <summary>
        /// Constructs a GameObject based on the provided <see cref="InputEntry"/>.
        /// </summary>
        /// <param name="entry">The <see cref="InputEntry"/>.</param>
        /// <returns>The created GameObject.</returns>
        public GameObject ConstructEntry(InputEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            GameObject prefab = this.indexes[entry.Type];
            GameObject levelObject = GameObject.Instantiate(prefab);
            levelObject.transform.localRotation = Quaternion.Euler(0f, entry.Angle, 0f);
            return levelObject;
        }

        /// <summary>
        /// Constructs the level object from a previously parsed list of 
        /// <see cref="InputEntry"/> instances.
        /// </summary>
        /// <returns>The created level GameObject.</returns>
        public GameObject ConstructLevel()
        {
            this.level = new GameObject("Level");
            this.level.transform.parent = this.LevelParent;

            this.level.AddComponent<Levelcomp>();
            Levelcomp levelcomp = this.level.GetComponent<Levelcomp>();
            levelcomp.Size = this.size;

            foreach (InputEntry ie in this.entries)
            {
                GameObject go = this.ConstructEntry(ie);
                go.transform.SetParent(this.level.transform);
                go.transform.localPosition = new Vector3((int)ie.Position.x, 0, (int)ie.Position.y);
            }

            Vector2 origin = (this.BoardSize - this.size) / 2f;
            Vector3 position = new Vector3(-this.BoardSize.x + origin.x, 0, origin.y);

            // The remote player does not use the `Marker.RemotePosition`, but we can update that case by simply 
            // setting the position here.
            this.level.transform.localPosition = position;

            position.x = -position.x;
            position.z = -position.z;
            this.level.AddComponent<Marker>();
            Marker marker = this.level.GetComponent<Marker>();
            marker.ID = LevelMarkerID;
            marker.RemotePosition = new MarkerPosition(8f * position, Quaternion.Euler(0, 0, 0), DateTime.Now, 8f * new Vector3(-1, 1, -1), 13379001);

            return this.level;
        }
    }
}