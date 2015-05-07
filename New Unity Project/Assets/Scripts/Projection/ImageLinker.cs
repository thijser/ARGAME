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
    using UnityEngine;

    public class ImageLinker : MonoBehaviour
    {
        public PlaneProjector Projector;
        public Transform LinkedTo;

        public void Start()
        {
            Projector.AddPosition(transform);
        }

        void Update()
        {
            Projector.ProjectTransform(LinkedTo, transform);
            Projector.Rotate(LinkedTo);
        }
    }
}
