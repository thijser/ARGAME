namespace Level
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using StreamReader = System.IO.StreamReader;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using Projection;
    public class LevelLoader
    {

        private Dictionary<char, GameObject> indexes;

        private List<InputEntry> entries;

        private GameObject level;

        private Vector2 size = new Vector2(1, 1);

        public void LoadObjects()
        {
            this.indexes = new Dictionary<char, GameObject>();
            this.indexes.Add('w', (GameObject)Resources.Load("prefabs/BOXWALL"));
            this.indexes.Add('e', (GameObject)Resources.Load("prefabs/Emitter"));
            this.indexes.Add('t', (GameObject)Resources.Load("prefabs/Laser Target"));
            this.indexes.Add('m', (GameObject)Resources.Load("prefabs/Mirror"));

        }

        public GameObject CreateLevel(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("The file path name is null.");
            }

            LoadLetters(path);
            if (indexes == null)
            {
                LoadObjects();
            }

            return ConstructLevel();
        }

        public void LoadLetters(string path)
        {
            StreamReader reader = new StreamReader(path);
            int y = 0;
            entries = new List<InputEntry>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = Regex.Replace(line, @"\s+", "");
                for (int x = 0; x < line.Length; x++)
                {
                    InputEntry ie = new InputEntry();
                    ie.type = line[x];
                    x++;
                    ie.dir = line[x];
                    ie.pos = new Vector2(x / 2, y);
                    if (x / 2 > size.x)
                        size.x = x / 2;
                    if (y > size.y)
                        size.y = y;
                    if (ie.type != '.')
                    {
                        entries.Add(ie);
                    }
                }
            }
        }

        public GameObject ConstructEntry(InputEntry ie)
        {
            GameObject pref = indexes[ie.type];
            GameObject go = GameObject.Instantiate(pref);
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.Euler(0f, ie.getAngle(), 0f);
            return go;
        }

        public GameObject ConstructLevel()
        {

            level = new GameObject("level");
            level.AddComponent<Levelcomp>();
            Levelcomp levelcomp = level.GetComponent<Levelcomp>();
            levelcomp.size = size;
            foreach (InputEntry ie in entries)
            {
                GameObject go = ConstructEntry(ie);
                go.transform.SetParent(level.transform);
                go.transform.localPosition = new Vector3(ie.pos.x, 0, ie.pos.y);
            }
            level.AddComponent<Marker>();
            Marker m = level.GetComponent<Marker>();
            m.ID = 13379001;
            m.RemotePosition = new MarkerPosition(Vector3.zero, Quaternion.identity, DateTime.Now, Vector3.one, 13379001);
            return level;
        }
    }
}