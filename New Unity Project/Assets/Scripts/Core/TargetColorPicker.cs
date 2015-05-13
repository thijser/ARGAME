//----------------------------------------------------------------------------
// <copyright file="TargetColorPicker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Updates the Renderer in this GameObject with a specific color.
    /// </summary>
    public class TargetColorPicker : MonoBehaviour
    {
        /// <summary>
        /// The color to update the Renderer with.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Color Color;

        /// <summary>
        /// Updates the color in a Renderer Component of this GameObject.
        /// </summary>
        public void Update()
        {
            Renderer ren = gameObject.GetComponent<Renderer>();
            ren.material.SetColor("Color", this.Color);
        }
    }
}
