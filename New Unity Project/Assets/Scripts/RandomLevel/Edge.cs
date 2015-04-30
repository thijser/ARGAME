using UnityEngine;
using System.Collections;
namespace RandomLevel{
	public class Edge {
		private Vertex from, to;
		public Edge(Vertex f, Vertex t){
			from = f;
			to = t;
		}
		Vertex GetFrom(){
			return from;
		}
		Vertex GetTo(){
			return to;
		}
		void SetFrom(Vertex v){
			from = v;
		}
		void SetTo(Vertex v){
			to = v;
		}
	}
}