//----------------------------------------------------------------------------
// <copyright file="MarkerTransformerMarker.cs" company="Delft University of Technology">
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
    /// Changes the material of a marker to indicate its type (normal or level marker).
    /// </summary>
    [ExecuteInEditMode]
    public class MarkerTransformerMarker : MonoBehaviour
    {
        /// <summary>
        /// Material for normal markers.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Material NormalMarkerMaterial;

        /// <summary>
        /// Material for level/parent marker.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Material LevelMarkerMaterial;

        /// <summary>
        /// Updates the Material of this MarkerTransformerMarker to match 
        /// whether this Marker is tagger as a level marker.
        /// </summary>
        public void Update()
        {
            if (this.tag == "LevelMarker")
            {
                this.GetComponent<Renderer>().material = this.LevelMarkerMaterial;
            }
            else
            {
                this.GetComponent<Renderer>().material = this.NormalMarkerMaterial;
            }
        }
    }
}