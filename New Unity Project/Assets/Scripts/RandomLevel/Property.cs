//----------------------------------------------------------------------------
// <copyright file="Property.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    /// <summary>
    /// An enumeration that gives several properties of a point in the map.
    /// </summary>
    public enum Property
    {
        /// <summary>
        /// Indicates the location is empty.
        /// </summary>
        EMPTY = 0,

        /// <summary>
        /// Indicates the location contains a wall.
        /// </summary>
        WALL,

        /// <summary>
        /// Indicates the location contains a laser beam emitter.
        /// </summary>
        LASER,

        /// <summary>
        /// Indicates the location contains a laser target.
        /// </summary>
        TARGET,

        /// <summary>
        /// Indicates the location contains a part of the generated laser beam path.
        /// </summary>
        PARTOFPATH
    }
}
