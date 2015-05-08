//----------------------------------------------------------------------------
// <copyright file="RandomLevelRenderer.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This class forms the bridge between a randomly created level and a randomly rendered level.
    /// It creates a randomly generated level, and then renders that level in the game world.
    /// </summary>
    public class RandomLevelRenderer : MonoBehaviour
    {
        public const float ScaleFactor = 15f;
        
        public GameObject EmitterPrefab, WallPrefab, TargetPrefab;

        public int RowCount, ColumnCount;

        private Quadrant quad;

        private SquareGraph sg;

        private Vector3 targetVec;

        public void Start()
        {
            RandomLevelGenerator rlg = new RandomLevelGenerator(this.RowCount, this.ColumnCount);
            this.quad = rlg.Quad;
            this.sg = rlg.ReturnRandomMap();
            this.targetVec = CoordToVector(rlg.TargetCoord);
            this.Render();
        }

        private static Vector3 CoordToVector(Coordinate c)
        {
            return new Vector3(c.Col * ScaleFactor, 0f, c.Row * -ScaleFactor);
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
        
        private void Render()
        {
            for (int i = 0; i < this.sg.Maxrow; i++)
            {
                for (int j = 0; j < this.sg.Maxcol; j++)
                {
                    Coordinate c = new Coordinate(i, j);
                    Vertex v = this.sg.GetVertexAtCoordinate(c);
                    Vector3 spawnVec = CoordToVector(c) - this.targetVec;
                    this.InstantiateObject(v, spawnVec);
                }
            }
        }

        private void InstantiateObject(Vertex v, Vector3 spawnVec)
        {
            if (v.Prop == Property.LASER)
            {
                int i = DetermineQuadRotation(this.quad);
                Quaternion q = Quaternion.Euler(0, i * 90, 0);
                UnityEngine.Object.Instantiate(this.EmitterPrefab, spawnVec, q);
            }
            else if (v.Prop == Property.WALL)
            {
                Quaternion q = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90, 0);
                UnityEngine.Object.Instantiate(this.WallPrefab, spawnVec, q);
            }
            else if (v.Prop == Property.TARGET)
            {
                UnityEngine.Object.Instantiate(this.TargetPrefab, spawnVec, Quaternion.identity);
            }
        }
    }
}
