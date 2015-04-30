using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace RandomLevel{
	class Vertex {
		private int i = 0;
		private int isWall = 0;
		private IList<Vertex> adjacent = new List<Vertex>();
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
		public void AddAdjacent(Vertex v)
		{
			adjacent.Add(v);
		}
		void SetWall(int w) {
			isWall = w;
		}
		int GetWall() {
			return isWall;
		}
	}
}