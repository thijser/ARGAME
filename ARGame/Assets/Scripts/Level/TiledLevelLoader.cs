using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class TiledLevelLoader : MonoBehaviour
{
    public string Level = "basic";

    private enum TileType
    {
        Wall,
        Emitter,
        Target,
        Mirror,
        Nothing
    }

    private class LevelObject
    {
        public readonly TileType Type;
        public readonly Vector2 Position;
        public readonly float Rotation;

        public LevelObject(TileType type, Vector2 position, float rotation)
        {
            this.Type = type;
            this.Position = position;
            this.Rotation = rotation;
        }

        public override string ToString()
        {
            return "LevelObject[Type = " + Type + ", Position = " + Position + ", Rotation = " + Rotation + "]";
        }
    }

    private Dictionary<TileType, GameObject> objectPrefabs = new Dictionary<TileType, GameObject>();

    void Start()
    {
        // Load prefabs for each tile type
        objectPrefabs[TileType.Wall] = Resources.Load("Prefabs/BOXWALL") as GameObject;
        objectPrefabs[TileType.Emitter] = Resources.Load("Prefabs/Emitter") as GameObject;
        objectPrefabs[TileType.Target] = Resources.Load("Prefabs/Laser Target") as GameObject;
        objectPrefabs[TileType.Mirror] = Resources.Load("Prefabs/Mirror") as GameObject;

        // Parse and instantiate level
        string xml = (Resources.Load("levels/" + Level) as TextAsset).text;
        List<LevelObject> levelObjects = ParseLevel(xml);

        foreach (LevelObject obj in levelObjects)
        {
            InstantiateLevelObject(obj, objectPrefabs);
        }
    }

    private static GameObject InstantiateLevelObject(LevelObject levelObject, Dictionary<TileType, GameObject> objectPrefabs)
    {
        GameObject obj = GameObject.Instantiate(objectPrefabs[levelObject.Type]);

        obj.transform.position = new Vector3(-levelObject.Position.x, 0, levelObject.Position.y);
        obj.transform.rotation = Quaternion.AngleAxis(levelObject.Rotation, Vector3.up);

        return obj;
    }

    private static List<LevelObject> ParseLevel(string xml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        // Find level size
        var mapNode = doc.SelectSingleNode("map");
        int width = int.Parse(mapNode.Attributes["width"].Value);
        int height = int.Parse(mapNode.Attributes["height"].Value);

        // Determine amount of tile types
        var imageNode = doc.SelectSingleNode("//image");
        int tileTypes = int.Parse(imageNode.Attributes["width"].Value) / 5;

        // Load tiles
        int x = 0;
        int y = 0;

        var levelObjects = new List<LevelObject>();

        foreach (XmlNode tileNode in doc.SelectNodes("//tile"))
        {
            int gid = int.Parse(tileNode.Attributes["gid"].Value);

            // Determine rotation of object
            int rotation = (gid / tileTypes) * 45;

            // Determine type of object
            int rawType = (gid - 1) % tileTypes;
            TileType type = TileType.Nothing;

            switch (rawType)
            {
                case 0: type = TileType.Wall; break;
                case 1: type = TileType.Emitter; rotation += 90; break;
                case 2: type = TileType.Target; break;
                case 3: type = TileType.Mirror; rotation += 90; break;
                default: type = TileType.Nothing; break;
            }

            // Determine position of object in world coordinates
            Vector2 pos = new Vector2(x, y);

            if (type != TileType.Nothing)
            {
                levelObjects.Add(new LevelObject(type, pos, rotation));
            }

            // Update X, Y position
            if (x + 1 == width) y++;
            x = (x + 1) % width;
        }

        return levelObjects;
    }
}
