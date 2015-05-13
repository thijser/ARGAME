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
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// This class forms the bridge between a randomly created level and a randomly rendered level.
    /// It creates a randomly generated level, and then renders that level in the game world.
    /// </summary>
    public class RandomLevelRenderer : MonoBehaviour
    {
        /// <summary>
        /// The size of a single field on the grid.
        /// </summary>
        public const float ScaleFactor = 15f;
        
        /// <summary>
        /// Prefab object for the Emitter.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject EmitterPrefab;
        
        /// <summary>
        /// Prefab object for the Wall.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject WallPrefab;
        
        /// <summary>
        /// Prefab object for the Laser Target.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject TargetPrefab;

        /// <summary>
        /// The amount of rows in the level.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int RowCount;
        
        /// <summary>
        /// The amount of columns in the level.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int ColumnCount;

        /// <summary>
        /// The current Quadrant of the grid.
        /// </summary>
        private Quadrant quad;

        /// <summary>
        /// The SquareGraph acting as the playing field.
        /// </summary>
        private SquareGraph sg;

        /// <summary>
        /// The relative origin of the level. Equal to the location of the target.
        /// </summary>
        private Vector3 targetVec;

        /// <summary>
        /// Generates a randomized level and generates the game objects in the level.
        /// </summary>
        public void Start()
        {
            RandomLevelGenerator rlg = new RandomLevelGenerator(this.RowCount, this.ColumnCount);
            this.quad = rlg.Quad;
            this.sg = rlg.ReturnRandomMap();
            this.targetVec = CoordToVector(rlg.TargetCoord);
            this.Render();
        }

        /// <summary>
        /// Translates a Coordinate instance to a Vector3 instance.
        /// </summary>
        /// <param name="c">The Coordinate.</param>
        /// <returns>The Vector3.</returns>
        private static Vector3 CoordToVector(Coordinate c)
        {
            return new Vector3(c.Col * ScaleFactor, 0f, c.Row * -ScaleFactor);
        }

        /// <summary>
        /// Determines the rotation of the Quadrant.
        /// </summary>
        /// <param name="q">The Quadrant</param>
        /// <returns>The rotation, as an integer between 0 and 3.</returns>
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
        
        /// <summary>
        /// Renders the generated map, creating the game object in the level.
        /// </summary>
        private void Render()
        {
            this.sg.ForEach(vertex =>
            {
                Coordinate c = vertex.Coordinate;
                Vector3 spawnVec = CoordToVector(c) - this.targetVec;
                this.InstantiateObject(vertex, spawnVec);
            });
        }

        /// <summary>
        /// Instantiates the correct object at the indicated Vertex.
        /// </summary>
        /// <param name="v">The Vertex</param>
        /// <param name="spawnVec">The position to create the object</param>
        private void InstantiateObject(Vertex v, Vector3 spawnVec)
        {
            if (v.Property == Property.LASER)
            {
                int i = DetermineQuadRotation(this.quad);
                Quaternion q = Quaternion.Euler(0, i * 90, 0);
                UnityEngine.Object.Instantiate(this.EmitterPrefab, spawnVec, q);
            }
            else if (v.Property == Property.WALL)
            {
                Quaternion q = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90, 0);
                UnityEngine.Object.Instantiate(this.WallPrefab, spawnVec, q);
            }
            else if (v.Property == Property.TARGET)
            {
                UnityEngine.Object.Instantiate(this.TargetPrefab, spawnVec, Quaternion.identity);
            }
        }
    }
}
