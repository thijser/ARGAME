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
    using System;
    using Network;
    using UnityEngine;

    /// <summary>
    /// Keeps data from either remote or local on the position of the marker.
    /// </summary>
    public class MarkerPosition
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public DateTime timeStamp;
        public Vector3 scale;
        public int id=-1;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPosition"/> class.
        /// </summary>
        /// <param name="update">The update.</param>
        public MarkerPosition(PositionUpdate update)
        {
            this.scale = new Vector3(1, 1, 1);
            this.timeStamp = DateTime.Now;
            this.Position = new Vector3(update.Coordinate[0], 0, update.Coordinate[1]);
            this.scale = new Vector3(1, 1, 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkerPosition"/> class.
        /// </summary>
        /// <param name="pos">The position.</param>
  
		/// <param name="rot">The rotation.</param>
        /// <param name="stamp">The timestamp of the update.</param>
        /// <param name="scale">The scale of the object.</param>
        public MarkerPosition(Vector3 pos, Quaternion rot, DateTime stamp, Vector3 scale)
        {
            this.Position = pos;
            this.Rotation = rot;
            this.timeStamp = stamp;
            this.scale = scale;
        }
		public override String ToString ()
		{
			String ret = "MarkerPosition:< Position="+Position+", Rotation="+Rotation+", timeStamp="+timeStamp+", scale="+scale+", id=" + id;
			return ret;
		}
    }
}
