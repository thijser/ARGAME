//----------------------------------------------------------------------------
// <copyright file="PlaneProjector.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
    
    /// <summary>
    /// Projects points onto a plane.
    /// </summary>
    public class PlaneProjector : MonoBehaviour
    {
        /// <summary>
        /// The List of positions on the plane.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public List<Transform> Positions;
        
        /// <summary>
        /// The first reference point on the plane.
        /// </summary>
        private Vector3 p1;

        /// <summary>
        /// The second reference point on the plane.
        /// </summary>
        private Vector3 p2;

        /// <summary>
        /// The third reference point on the plane.
        /// </summary>
        private Vector3 p3;

        /// <summary>
        /// The first reference vector of the plane.
        /// </summary>
        private Vector3 v1;

        /// <summary>
        /// The second reference vector of the plane.
        /// </summary>
        private Vector3 v2;

        /// <summary>
        /// The normal of the plane.
        /// </summary>
        private Vector3 normal;

        /// <summary>
        /// Updates the plane of points.
        /// </summary>
        public void Update()
        {
            if (this.Positions.Count >= 3)
            {
                this.SetPlaneByPoints(
                    this.Positions[0].position, 
                    this.Positions[1].position, 
                    this.Positions[2].position);
            }
        }

        /// <summary>
        /// Adds the given Transform to this PlaneProjector.
        /// </summary>
        /// <param name="transform">The Transform to add.</param>
        public void AddPosition(Transform transform)
        {
            this.Positions.Add(transform);
        }

        /// <summary>
        /// Rotates the given Transform to match this PlaneProjector's plane.
        /// </summary>
        /// <param name="transform">The Transform</param>
        public void Rotate(Transform transform)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, this.normal);
        }

        /// <summary>
        /// Sets the plane of this PlaneProjector by three points.
        /// </summary>
        /// <param name="point1">The first position</param>
        /// <param name="point2">The second position</param>
        /// <param name="point3">The third position</param>
        public void SetPlaneByPoints(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            this.p1 = point1;
            this.p2 = point2;
            this.p3 = point3;

            this.v1 = this.p2 - this.p1;
            this.v2 = this.p3 - this.p1;
            this.normal = this.ComputeNormal(this.v1, this.v2);
        }

        /// <summary>
        /// Projects the given Transform to this plane
        /// </summary>
        /// <param name="input">The input Transform</param>
        /// <param name="home">The Transform of the original object.</param>
        /// <returns>The translated input Transform</returns>
        public Transform ProjectTransform(Transform input, Transform home)
        {
            input.position = this.ProjectPoint(home.position);
            return input;
        }

        /// <summary>
        /// Projects a point onto the plane.
        /// </summary>
        /// <param name="point">The point to project.</param>
        /// <returns>The projection of the point.</returns>
        public Vector3 ProjectPoint(Vector3 point)
        {
            // The relative point is the point relative to our origin.
            Vector3 relativePoint = point - this.p1;
            float dist = Vector3.Dot(relativePoint, this.normal); // distance plane to point 
            return point - (dist * this.normal);
        }

        /// <summary>
        /// Computes the Normal of two Vectors in 3D space.
        /// </summary>
        /// <param name="v1">The first Vector3</param>
        /// <param name="v2">The second Vector3</param>
        /// <returns>The normal of the plane given by the span of <c>v1</c> and <c>v2</c>.</returns>
        public Vector3 ComputeNormal(Vector3 v1, Vector3 v2)
        {
            Vector3 res = Vector3.Cross(v1, v2);
            res.Normalize();
            return res;
        }
    }
}