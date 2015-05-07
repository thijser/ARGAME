//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using UnityEngine;
namespace RandomLevel
{
	/// <summary>
	/// This class forms te bridge between a randomly created level and a randomly rendered level.
	/// It creates a randomly generated level, and then renders that level in the game world.
	/// </summary>
	public class RandomLevelRenderer : MonoBehaviour
	{
		private SquareGraph sg;
		private Vector3 targetVec;
		public GameObject emitterPrefab, wallPrefab, targetPrefab;
		public int rows, cols;
		private Quadrant quad;
		public const float ScaleFact = 15f;
		private void Render() 
		{
			for (int i = 0; i < sg.Maxrow; i++) 
			{
				for (int j = 0; j < sg.Maxcol; j++) 
				{
					Coordinate c = new Coordinate(i,j);
					Vertex v = sg.GetVertexAtCoordinate(c);
					Vector3 spawnVec = CoordToVector(c) - targetVec;
					InstantiateObject(v,spawnVec);
				}
			}
		}
		private static Vector3 CoordToVector(Coordinate c) 
		{
			return new Vector3 (c.col*ScaleFact, 0f, c.row*-ScaleFact);
		}
		private void InstantiateObject(Vertex v, Vector3 spawnVec)
		{

			if (v.Prop == Property.LASER) 
			{
				int i = DetermineQuadRotation (quad);
				Quaternion q = Quaternion.Euler(0, i*90, 0);
				Instantiate (emitterPrefab, spawnVec, q);
			} 
			else if (v.Prop == Property.WALL) 
			{
				Quaternion q = Quaternion.Euler(0, UnityEngine.Random.Range(0,4)*90, 0);
				Instantiate (wallPrefab, spawnVec, q);
			} 
			else if (v.Prop == Property.TARGET) 
			{
				Instantiate (targetPrefab, spawnVec, Quaternion.identity);
			}
		}
		public void Start() 
		{
			RandomLevelGenerator rlg = new RandomLevelGenerator (rows, cols);
			quad = rlg.Quad;
			sg = rlg.ReturnRandomMap ();
			targetVec = CoordToVector (rlg.TargetCoord);
			Render ();
		}
		private static int DetermineQuadRotation(Quadrant q) 
		{
			switch (q) 
			{
			case Quadrant.NORTHWEST: return 3;
			case Quadrant.SOUTHWEST: return 0;
			case Quadrant.SOUTHEAST: return 1;
			case Quadrant.NORTHEAST: return 2;
			default: throw new ArgumentException("The parameter should be a valid quadrant value.");
			}
		}
	}
}

