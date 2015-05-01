using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RandomLevel{
	class Vertex {
		public bool Visited { get; set; }
		public bool IsWall { get; set; }

		private List<Vertex> adjacent = new List<Vertex>();

		public void AddAdjacent(Vertex v)
		{
			adjacent.Add(v);
		}
	}
}
