//----------------------------------------------------------------------------
// <copyright file="Vertex.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A Vertex in a SquareGraph.
    /// </summary>
    public class Vertex 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomLevel.Vertex"/> class.
        /// </summary>
        /// <param name="coordinate">The Coordinate of this Vertex.</param>
        public Vertex(Coordinate coordinate) 
        {
            if (coordinate == null)
            {
                throw new ArgumentNullException("coordinate");
            }

            this.Property = Property.EMPTY;
            this.Coordinate = coordinate;
        }
        
        /// <summary>
        /// Gets or sets a value indicating the property of this point on the map.
        /// </summary>
        public Property Property { get; set; }
        
        /// <summary>
        /// Gets the Coordinate of this Vertex.
        /// </summary>
        public Coordinate Coordinate { get; private set; }
    }
}