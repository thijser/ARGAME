//----------------------------------------------------------------------------
// <copyright file="UsedCardManager.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using UnityEngine;

    /// <summary>
    /// Indicates the BaseForLevel instance used for synchronizing positions.
    /// </summary>
    public class UsedCardManager : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the currently used BaseForLevel instance.
        /// </summary>
        public BaseForLevel CurrentlyUsed { get; set; }
    }
}
