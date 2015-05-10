//----------------------------------------------------------------------------
// <copyright file="ImageLinker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Projects this game object using a PlaneProjector instance.
    /// </summary>
    public class ImageLinker : MonoBehaviour
    {
        /// <summary>
        /// The PlaneProjector to use.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public PlaneProjector Projector;

        /// <summary>
        /// The Transform to link to.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Transform LinkedTo;

        /// <summary>
        /// Adds this ImageLinker's Transform to the PlaneProjector.
        /// </summary>
        public void Start()
        {
            this.Projector.AddPosition(this.transform);
        }

        /// <summary>
        /// Projects the Transform instance using the PlaneProjector.
        /// </summary>
        public void Update()
        {
            this.Projector.ProjectTransform(this.LinkedTo, this.transform);
            this.Projector.Rotate(this.LinkedTo);
        }
    }
}
