//----------------------------------------------------------------------------
// <copyright file="PingUpdate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;

    /// <summary>
    /// An serverUpdate object that describes a ping message,
    /// causing the server to not time out.
    /// </summary>
    public class PingUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Network.PingUpdate"/> class.
        /// </summary>
        public PingUpdate()
        {
            this.Type = UpdateType.Ping;
            this.ID = -1;
            this.TimeStamp = DateTime.Now.Ticks;
        }
    }
}