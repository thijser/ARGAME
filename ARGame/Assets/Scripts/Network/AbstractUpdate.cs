//----------------------------------------------------------------------------
// <copyright file="IUpdate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    public abstract class AbstractUpdate
    {
        /// <summary>
        /// Gets the type of this update.
        /// </summary>
        public UpdateType Type { get; protected set; }

        /// <summary>
        /// Gets the ID of this update.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets the timestamp of this update.
        /// </summary>
        public long TimeStamp { get; protected set; }
    }
}
