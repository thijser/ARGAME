﻿using UnityEngine;
using System.Collections;
using VolumetricLines;

/// <summary>
/// Create a sin shaped line strip using a volumetric line strip
/// </summary>
public class CreateSinShapedLineStrip : MonoBehaviour 
{
	public int m_numVertices = 50;
	public Material m_volumetricLineStripMaterial;
	public Color m_color;
	public float m_start = 0f;
	public float m_end = Mathf.PI;

	// Use this for initialization
	void Start () 
	{
		// Create an empty game object
		GameObject go = new GameObject();
		go.transform.parent = transform;

		// Add the MeshFilter component, VolumetricLineStripBehavior requires it
		go.AddComponent<MeshFilter>();

		// Add a MeshRenderer, VolumetricLineStripBehavior requires it, and set the material
		var meshRenderer = go.AddComponent<MeshRenderer>();
		meshRenderer.material = m_volumetricLineStripMaterial;

		// Add the VolumetricLineStripBehavior and set parameters, like color and all the vertices of the line
		var volLineStrip = go.AddComponent<VolumetricLineStripBehavior>();
		volLineStrip.m_setMaterialColor = true;
		volLineStrip.m_lineColor = m_color;
		volLineStrip.m_lineVertices = new Vector3[m_numVertices];
		for (int i=0; i < m_numVertices; ++i)
		{
			float x = Mathf.Lerp(m_start, m_end, (float)i / (float)(m_numVertices-1));
			float y = Mathf.Sin(x);
			volLineStrip.m_lineVertices[i] = gameObject.transform.TransformPoint(new Vector3(x, y, 0f));
		}
	}

	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for (int i=0; i < m_numVertices; ++i)
		{
			float x = Mathf.Lerp(m_start, m_end, (float)i / (float)(m_numVertices-1));
			float y = Mathf.Sin(x);
			Gizmos.DrawSphere(gameObject.transform.TransformPoint(new Vector3(x, y, 0f)), 5f);
		}
	}
}
