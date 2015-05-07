//----------------------------------------------------------------------------
// <copyright file="ProjectThis.cs" company="Delft University of Technology">
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
    /// Projects a game object using a Projector instance.
    /// </summary>
    public class ProjectThis : MonoBehaviour
    {
        /// <summary>
        /// The Projector.
        /// </summary>
        public projectioncode Projector;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Start()
        {
            // Projector.addPosition (transform);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            // Projector.projectTransform(transform);
            // Projector.rotate (transform);
        }
    }
}
