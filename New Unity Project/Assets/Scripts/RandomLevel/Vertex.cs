using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RandomLevel{

	///<summary>
	///A Vertex in a SquareGraph.
	///</summary>
	public class Vertex {
		///<summary>
		///Flag indicating whether this Vertex is visited.
		///</summary>
		public bool Visited { get; set; }
		///<summary>
		///Flag indicating whether this Vertex is a wall.
		///</summary>
		public bool IsWall { get; set; }

		private List<Vertex> adjacent = new List<Vertex>();

		///<summary>
		///Adds the given Vertex as a neighbour.
		///</summary>
		///<param name="vertex">The Vertex to add, not null.</param>
		public void AddAdjacent(Vertex vertex)
		{
			if (vertex == null)
			{
				throw new ArgumentNullException("vertex");
			}
			adjacent.Add(vertex);
		}
	}
}
