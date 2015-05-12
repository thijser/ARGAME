//----------------------------------------------------------------------------
// <copyright file="LaserType.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Describes a Laser type.
    /// <para>
    /// A LaserType is a representation of a type of Laser beam and determines
    /// all of its properties. It can be applied to a LineRenderer to influence
    /// the appearance of the Laser beam.
    /// </para>
    /// </summary>
    public class LaserType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LaserType"/> class.
        /// </summary>
        /// <param name="typeName">The name of this LaserType, not null and not empty.</param>
        /// <param name="material">The Material for the Laser beam, not null.</param>
        public LaserType(string typeName, Material material)
        {
            if (material == null)
            {
                throw new ArgumentNullException("material");
            }

            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("Type name must be non-empty");
            }

            this.LaserMaterial = material;
            this.TypeName = typeName;
        }

        /// <summary>
        /// Gets the Material used for the Laser beam.
        /// </summary>
        public Material LaserMaterial { get; private set; }

        /// <summary>
        /// Gets a unique name for the LaserType.
        /// <para>
        /// LaserType instances with the same name are considered equal,
        /// even if their Materials differ.
        /// </para>
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Applies this Laser type to the given LineRenderer.
        /// </summary>
        /// <param name="renderer">The LineRenderer to apply this LaserType to, not null.</param>
        public void Apply(LineRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException("renderer");
            }

            renderer.material = this.LaserMaterial;
        }

        /// <summary>
        /// Tests if this <see cref="LaserType"/> is equal to <c>obj</c>.
        /// </summary>
        /// <param name="obj">The object to test for equality.</param>
        /// <returns>True if this object is equal to <c>obj</c>, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            LaserType that = obj as LaserType;
            return this.TypeName.Equals(that.TypeName);
        }

        /// <summary>
        /// Gets a hash code for this LaserType.
        /// </summary>
        /// <returns>The hash code for this object.</returns>
        public override int GetHashCode()
        {
            return this.TypeName.GetHashCode();
        }
    }
}
