using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RandomLevel{
	public class Vertex {
		private int i = 0;
		private int isWall = 0;
		private IList<Edge> edges = new List<Edge>();
		public Vertex(){
		}
		void SetVisited()
		{
			i = 1;
		}
		int GetVisited() {
			return i;
		}
		void RevertVisited()
		{
			i = 0;
		}
		void AddEdge(Edge e)
		{
			edges.Add(e);
		}
		void SetWall(int w) {
			isWall = w;
		}
		int GetWall() {
			return isWall;
		}
	}
}