using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace RandomLevel{

	///<summary>
	///A Vertex in a SquareGraph.
	///</summary>
	public class Vertex {
		///<summary>
		///Flag indicating whether this Vertex is visited.
		///</summary>
		private bool Visited { get; set; }
		///<summary>
		///Flag indicating whether this Vertex is a wall.
		///</summary>
		private bool IsWall { get; set; }
		private bool IsTarget { get; set; }
		private bool IsLaser { get; set; }
		private bool PartOfPath { get; set; }

		public Vertex() {
			IsWall = false;
			IsTarget = false;
			IsLaser = false;
			PartOfPath = false;
		}
		///<summary>
		///Adds the given Vertex as a neighbour.
		///</summary>
		///<param name="vertex">The Vertex to add, not null.</param>
	}
}
