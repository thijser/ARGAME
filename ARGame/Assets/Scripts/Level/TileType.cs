﻿namespace Level
{
    /// <summary>
    /// Type of a tile loaded from level file.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// Indicates an empty tile.
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Indicates a wall tile.
        /// </summary>
        Wall = 1,

        /// <summary>
        /// Indicates a green emitter tile.
        /// </summary>
        EmitterG = 2,

        /// <summary>
        /// Indicates a red emitter tile.
        /// </summary>
        EmitterR = 3,

        /// <summary>
        /// Indicates a blue emitter tile.
        /// </summary>
        EmitterB = 4,

        /// <summary>
        /// Indicates a green target tile.
        /// </summary>
        TargetG = 5,

        /// <summary>
        /// Indicates a red target tile.
        /// </summary>
        TargetR = 6,

        /// <summary>
        /// Indicates a blue target tile.
        /// </summary>
        TargetB = 7,

        /// <summary>
        /// Indicates a mirror tile.
        /// </summary>
        Mirror = 8,

        /// <summary>
        /// Indicates a splitter tile.
        /// </summary>
        Splitter = 9,

        /// <summary>
        /// Indicates an emitter tile.
        /// </summary>
        Elevator = 10,

        /// <summary>
        /// Indicates a portal entry tile of pair 1.
        /// </summary>
        PortalEntryOne = 11,

        /// <summary>
        /// Indicates a portal exit tile of pair 1.
        /// </summary>
        PortalExitOne = 12,

        /// <summary>
        /// Indicates a portal entry tile of pair 2.
        /// </summary>
        PortalEntryTwo = 13,

        /// <summary>
        /// Indicates a portal exit tile of pair 2.
        /// </summary>
        PortalExitTwo = 14,

        /// <summary>
        /// Indicates a portal entry tile of pair 3.
        /// </summary>
        PortalEntryThree = 15,

        /// <summary>
        /// Indicates a portal exit tile of pair 3.
        /// </summary>
        PortalExitThree = 16
    }
}
