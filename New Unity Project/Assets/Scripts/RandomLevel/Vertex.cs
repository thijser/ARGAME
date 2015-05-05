using System.Collections;
using System.Collections.Generic;
using System;
namespace RandomLevel{
	
	/// <summary>
	/// A Vertex in a SquareGraph.
	/// </summary>
	public class Vertex {
		///<summary>
		///Value indicating the property of this point on the map.
		///</summary>
		public Property prop { get; set; } 
		/// <summary>
		/// Initializes a new instance of the <see cref="RandomLevel.Vertex"/> class.
		/// </summary>
		public Vertex() {
			prop = Property.EMPTY;
		}
	}
}