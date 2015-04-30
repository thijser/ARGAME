using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RandomLevel{
	public class Vertex {
		private int i = 0;
		private int x,y;
		private IList<Edge> edges = new List<Edge>();
		public Vertex(int row, int col){
			x = row;
			y = col;
		}
		void SetVisited()
		{
			i = 1;
		}
		void RevertVisited()
		{
			i = 0;
		}
		void AddEdge(Edge e)
		{
			edges.Add (e);
		}
	}
}