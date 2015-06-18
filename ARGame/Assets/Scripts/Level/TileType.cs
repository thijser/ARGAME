namespace Level
{
    /// <summary>
    /// Type of a tile loaded from level file.
    /// </summary>
    public enum TileType
    {
        Nothing = 0,
        Wall = 1,
        EmitterG = 2,
        EmitterR = 3,
        EmitterB = 4,
        TargetG = 5,
        TargetR = 6,
        TargetB = 7,
        Mirror = 8,
        Splitter = 9,
        Elevator = 10,
        Portal = 11
    }
}
