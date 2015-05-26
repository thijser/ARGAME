//----------------------------------------------------------------------------
// <copyright file="LaserProperties.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Updates the Color of a LineRenderer.
    /// </summary>
    public class LaserProperties : MonoBehaviour
    {
        /// <summary>
        /// The strengths of the RGB components as a Vector3.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Vector3 RGBStrengths;

        /// <summary>
        /// Gets or sets the LineRenderer to use for the laser.
        /// </summary>
        public LineRenderer LineRenderer { get; set; }

        /// <summary>
        /// Gets the color of the Laser beam.
        /// </summary>
        public Color LaserColor
        {
            get
            {
                Color c = new Color(0, 0, 0, 1);
                float highest = Mathf.Max(this.RGBStrengths.x, this.RGBStrengths.y, this.RGBStrengths.z);
                if (highest > 0)
                {
                    c.r = this.RGBStrengths.x / highest;
                    c.g = this.RGBStrengths.y / highest;
                    c.b = this.RGBStrengths.z / highest;
                }

                return c;
            }
        }

        /// <summary>
        /// Gets the magnitude of the Laser beam.
        /// </summary>
        public float Strength
        {
            get
            {
                return this.RGBStrengths.magnitude;
            }
        }

        /// <summary>
        /// Initializes the LineRenderer in this LaserProperties.
        /// </summary>
        public void Start() 
        {
            if (LineRenderer == null) 
            {
                LineRenderer = this.GetComponent<LineRenderer>();
            }
        }

        /// <summary>
        /// Updates the Laser beam indicated by the LineRenderer.
        /// Called by Unity every step.
        /// </summary>
        public void Update()
        {
            this.UpdateBeam();
        }

        /// <summary>
        /// Updates the Laser beam indicated by the LineRenderer.
        /// </summary>
        public void UpdateBeam()
        {
            float scale = transform.lossyScale.x;
            Color color = this.LaserColor;
            this.LineRenderer.SetWidth(this.Strength * scale, this.Strength * scale);
            this.LineRenderer.material.color = color;
            this.LineRenderer.material.SetColor("_Albedo", color);
            this.LineRenderer.material.SetColor("_Emission", color);
            this.LineRenderer.material.SetColor("Main Color", color);
        }
    }
}
