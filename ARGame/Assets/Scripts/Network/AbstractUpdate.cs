//----------------------------------------------------------------------------
// <copyright file="AbstractUpdate.cs" company="Delft University of Technology">
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
    /// An abstract class used as an accessibility interface
    /// for parameters for both types of updates.
    /// </summary>
    public abstract class AbstractUpdate
    {
        /// <summary>
        /// Gets or sets the type of this serverUpdate.
        /// </summary>
        public UpdateType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the ID of this serverUpdate.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        /// Gets or sets the timestamp of this serverUpdate.
        /// </summary>
        public long TimeStamp { get; protected set; }
    }
}
