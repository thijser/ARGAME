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
		public Vertex() {
			prop = Property.EMPTY;
		}
		///<summary>
		///Adds the given Vertex as a neighbour.
		///</summary>
		///<param name="vertex">The Vertex to add, not null.</param>
	}
}