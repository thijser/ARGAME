//----------------------------------------------------------------------------
// <copyright file="PositionPreviewer.cs" company="Delft University of Technology">
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
    /// Previews the position of PositionUpdate objects.
    /// </summary>
    public class PositionPreviewer : MonoBehaviour
    {
        /// <summary>
        /// Moves the marker object with the ID of the given PositionUpdate
        /// to the location as indicated by the update.
        /// </summary>
        /// <param name="update">The PositionUpdate object to show.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            if (update == null)
            {
                throw new ArgumentNullException("update");
            }

            GameObject marker = GameObject.Find("Marker" + update.ID);
            marker.transform.position = new Vector3(update.X / 10.0f, 0, 72 - (update.Y / 10.0f));
        }
    }
}
