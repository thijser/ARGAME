using UnityEngine;
using System.Collections;

public class projectioncode : MonoBehaviour {
	Vector3 p1;
	Vector3 p2;
	Vector3 p3;
	Vector3 v1;
	Vector3 v2;
	Vector3 normal;


	/*
	 *use a plane with the following 3 coordinates 
	 */ 
	public void setPlaneByPoints(Vector3 point1,Vector3 point2, Vector3 point3){
		p1 = point1;
		p2 = point2;
		p3 = point3;

		v1=p2-p1;
		v2=p3-p1;
		normal=getNormal(v1,v2);
	}

	public Transform projectTransform(Transform input){
		input.position = projectPoint (input.position);
		return input;
	}
	/*
	 *  projects a 3d point into space; 
	 */
	Vector3 projectPoint(Vector3 point){
		Vector3 vOToPoint = point - p1;//vector between origin and point 
		float dist = Vector3.Dot (vOToPoint, normal); //distance plane to point 
		return point - dist * normal;
	}

	/*
	 * caclulates the normal of 2 vectors
	 */ 
	Vector3 getNormal(Vector3 v1,Vector3 v2){
		Vector3 res= (Vector3.Cross (v1, v2));
		res.Normalize();
		return res;
	}
}
