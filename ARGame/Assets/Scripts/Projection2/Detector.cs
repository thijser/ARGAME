//----------------------------------------------------------------------------
// <copyright file="Detector.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Detector : MonoBehaviour 
    {
        IARLink link;

        void Start()
        {
            this.link = new MetaLink();
        }

        void LateUpdate()
        {
            List<MarkerPosition> list = this.link.GetMarkerPositions();
            foreach (MarkerPosition mp in list)
            {

            }
        }
    }
}