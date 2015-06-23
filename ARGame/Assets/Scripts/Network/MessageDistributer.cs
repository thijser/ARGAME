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
    using UnityEngine.Assertions;

    /// <summary>
    /// Distributes general messages from the server as specific messages.
    /// </summary>
    public class MessageDistributer : MonoBehaviour
    {
        /// <summary>
        /// Receives and handles all server updates, by sending them forward to other handlers
        /// </summary>
<<<<<<< HEAD
        /// <param name="update">The serverUpdate to be handled, not null.</param>
=======
        /// <param name="update">The serverUpdate to be handled.</param>
>>>>>>> master
        public void OnServerUpdate(AbstractUpdate update)
        {
            Assert.IsNotNull(update);

            switch (update.Type)
            {
                case UpdateType.UpdatePosition:
                case UpdateType.DeletePosition:
                    this.SendMessage("OnPositionUpdate", update as PositionUpdate);
                    break;
                case UpdateType.UpdateRotation:
                    this.SendMessage("OnRotationUpdate", update as RotationUpdate);
                    break;
                case UpdateType.UpdateLevel:
                    this.SendMessage("OnLevelUpdate", update as LevelUpdate);
                    break;
                case UpdateType.UpdateARView:
                    this.SendMessage("OnARViewUpdate", update as ARViewUpdate);
                    break;
                default:
                    Assert.IsTrue(false, "Reached unreachable default case in MessageDistributer");
                    break;
            }
        }
    }
}
