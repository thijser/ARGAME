﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class projectioncode : MonoBehaviour {
	Vector3 p1;
	Vector3 p2;
	Vector3 p3;
	Vector3 v1;
	Vector3 v2;
	Vector3 normal;

	public List<Transform> positions;

	void Update () {
		setPlaneByPoints (positions [0].position,positions [1].position,positions [2].position);
	}

	public void addPosition(Transform t){
		positions.Add (t);
	}
	/*
	 *use a plane with the following 3 coordinates 
	 */ 

	public void rotate(Transform t){
		transform.rotation = Quaternion.FromToRotation(Vector3.right, normal);
	}

	public void setPlaneByPoints(Vector3 point1,Vector3 point2, Vector3 point3){
		p1 = point1;
		p2 = point2;
		p3 = point3;

		v1=p2-p1;
		v2=p3-p1;
		normal=getNormal(v1,v2);
	}

	public Transform projectTransform(Transform input, Transform home){
		input.position = projectPoint (home.position);
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
