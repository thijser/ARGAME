//----------------------------------------------------------------------------
// <copyright file="UpdateType.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    /// <summary>
    /// Indicates the type of the PositionUpdate message.
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// Indicates the position of the object is updated.
        /// </summary>
        UpdatePosition = 0,

        /// <summary>
        /// Indicates the object is removed from the field.
        /// </summary>
        DeletePosition = 1,

        /// <summary>
        /// Indicates a ping message indicating the server is still alive.
        /// </summary>
        Ping = 2,

        /// <summary>
        /// Indicates that the rotation of the object is updated.
        /// </summary>
        UpdateRotation = 3,

        /// <summary>
        /// Indicates that the level has changed.
        /// </summary>
        Level = 4
    }
}
