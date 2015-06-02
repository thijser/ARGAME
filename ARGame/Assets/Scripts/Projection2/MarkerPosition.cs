//----------------------------------------------------------------------------
// <copyright file="MarkerPosition.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections;
    using System;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Keeps data from either remote or local on the position of the marker.
    /// 
    /// </summary>
    public class MarkerPosition : MonoBehaviour
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public DateTime timeStamp;
        public Vector3 scale;


        /// <summary>
        /// create a new markerPosition from a position update 
        /// </summary>
        /// <param name="update">Pu.</param>
        public MarkerPosition(PositionUpdate pu)
        {
            this.scale = new Vector3(1, 1, 1);
            this.timeStamp = DateTime.Now;
            this.Position = new Vector3(pu.Coordinate[0], 0, pu.Coordinate[1]);
            this.scale = new Vector3(1, 1, 1);
        }

        public MarkerPosition(Vector3 pos, Quaternion rot, DateTime stamp, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            timeStamp = stamp;
            this.scale = scale;
        }
    }

}