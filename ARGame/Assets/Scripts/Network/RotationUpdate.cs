﻿//----------------------------------------------------------------------------
// <copyright file="PositionUpdate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
	public class RotationUpdate : AbstractUpdate 
    {
        /// <summary>
        /// Gets the rotation if this update object.
        /// </summary>
		public float Rotation { get; private set; }

        public RotationUpdate(UpdateType type, float rotation, int id)
        {
            this.Type = type;
            this.Rotation = rotation;
            this.ID = id;
        }
	}
}
