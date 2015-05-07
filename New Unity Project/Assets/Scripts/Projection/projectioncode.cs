//----------------------------------------------------------------------------
// <copyright file="projectioncode.cs" company="Delft University of Technology">
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
    using UnityEngine;

    public class projectioncode : MonoBehaviour
    {
        public List<Transform> Positions;
        
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;
        private Vector3 v1;
        private Vector3 v2;
        private Vector3 normal;

        public void Update()
        {
            SetPlaneByPoints(Positions[0].position, Positions[1].position, Positions[2].position);
        }

        public void AddPosition(Transform t)
        {
            Positions.Add(t);
        }

        /*
         * Use a plane with the following 3 coordinates 
         */
        public void Rotate(Transform t)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.right, normal);
        }

        public void SetPlaneByPoints(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            p1 = point1;
            p2 = point2;
            p3 = point3;

            v1 = p2 - p1;
            v2 = p3 - p1;
            normal = ComputeNormal(v1, v2);
        }

        public Transform ProjectTransform(Transform input, Transform home)
        {
            input.position = ProjectPoint(home.position);
            return input;
        }

        /*
         * projects a 3d point into space; 
         */
        public Vector3 ProjectPoint(Vector3 point)
        {
            // The relative point is the point relative to our origin.
            Vector3 relativePoint = point - p1;
            float dist = Vector3.Dot(relativePoint, normal); // distance plane to point 
            return point - dist * normal;
        }

        /*
         * Calculates the normal of 2 vectors
         */
        public Vector3 ComputeNormal(Vector3 v1, Vector3 v2)
        {
            Vector3 res = (Vector3.Cross(v1, v2));
            res.Normalize();
            return res;
        }
    }
}