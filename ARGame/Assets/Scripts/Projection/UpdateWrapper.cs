//----------------------------------------------------------------------------
// <copyright file="UpdateWrapper.cs" company="Delft University of Technology">
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
    using Network;
    using UnityEngine;

    /// <summary>
    /// <c>MonoBehaviour</c> wrapper around a <c>PositionUpdate</c> instance so
    /// that they can be assigned to GameObjects.
    /// </summary>
    public class UpdateWrapper : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the position update used for assignment to gameObjects
        /// </summary>
        public PositionUpdate Wrapped { get; set; }
    }
}