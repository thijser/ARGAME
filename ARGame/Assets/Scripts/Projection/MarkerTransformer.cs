//----------------------------------------------------------------------------
// <copyright file="MarkerTransformer.cs" company="Delft University of Technology">
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
    /// Handles transformations between board markers and Meta markers.
    /// </summary>
    [ExecuteInEditMode]
    public class MarkerTransformer : MonoBehaviour
    {
        /// <summary>
        /// Board markers.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Transform[] BoardMarkers;

        /// <summary>
        /// Meta markers that board markers map to.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Transform[] MetaMarkers;

        /// <summary>
        /// Updates the relative Meta marker positions based on the level marker and board layout.
        /// </summary>
        public void Update()
        {
            // Find index of level marker
            int levelMarkerIdx = -1;

            for (int i = 0; i < this.MetaMarkers.Length; i++)
            {
                if (this.MetaMarkers[i].tag == "LevelMarker")
                {
                    levelMarkerIdx = i;
                }
            }

            // Move the other markers relative to it if there is one
            if (levelMarkerIdx != -1)
            {
                for (int i = 0; i < this.MetaMarkers.Length; i++)
                {
                    if (i == levelMarkerIdx)
                    {
                        continue;
                    }
                    
                    // Position relative to level marker in board space
                    Vector3 rel = this.BoardMarkers[i].position - this.BoardMarkers[levelMarkerIdx].position;

                    // Transform position to Meta space scale
                    Vector3 inverseScale = this.BoardMarkers[levelMarkerIdx].localScale;
                    inverseScale.x = 1.0f / inverseScale.x;
                    inverseScale.y = 1.0f / inverseScale.y;
                    inverseScale.z = 1.0f / inverseScale.z;

                    rel.Scale(inverseScale);
                    rel.Scale(this.MetaMarkers[levelMarkerIdx].localScale);

                    // Rotate position to Meta space rotation
                    this.MetaMarkers[i].position = 
                        this.MetaMarkers[levelMarkerIdx].position + 
                        (this.MetaMarkers[levelMarkerIdx].rotation * 
                        Quaternion.Inverse(this.BoardMarkers[levelMarkerIdx].rotation) * rel);

                    // Give child markers the same rotation and scale as the level marker
                    this.MetaMarkers[i].rotation = Quaternion.AngleAxis(
                        -this.BoardMarkers[levelMarkerIdx].rotation.eulerAngles.y, 
                        this.MetaMarkers[levelMarkerIdx].up) * this.MetaMarkers[levelMarkerIdx].rotation;

                    this.MetaMarkers[i].localScale = this.MetaMarkers[levelMarkerIdx].localScale;
                }
            }
        }
    }
}