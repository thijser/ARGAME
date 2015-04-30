using UnityEngine;
using System.Collections;
namespace RandomLevel{
	public class Edge {
		private Vertex from, to;
		public Edge(Vertex f, Vertex t){
			from = f;
			to = t;
		}
	}
}