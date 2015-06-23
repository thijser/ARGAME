//----------------------------------------------------------------------------
// <copyright file="MessageDistributer.cs" company="Delft University of Technology">
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
    using UnityEngine;

    /// <summary>
    /// Distributes general messages from the server as specific messages.
    /// </summary>
    public class MessageDistributer : MonoBehaviour
    {
        /// <summary>
        /// Receives and handles all server updates, by sending them forward to other handlers
        /// </summary>
        /// <param name="update">The serverUpdate to be handled.</param>
        public void OnServerUpdate(AbstractUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            if (update.Type == UpdateType.UpdatePosition || update.Type == UpdateType.DeletePosition)
            {
                this.SendMessage("OnPositionUpdate", update as PositionUpdate);
            }
            else if (update.Type == UpdateType.UpdateRotation)
            {
                this.SendMessage("OnRotationUpdate", update as RotationUpdate);
            }
            else if (update.Type == UpdateType.Level)
            {
                this.SendMessage("OnLevelUpdate", update as LevelUpdate);
            }
        }
    }
}
