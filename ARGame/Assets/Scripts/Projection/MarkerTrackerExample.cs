﻿using Meta;
using UnityEngine;
using System.Collections;
using Projection;

public class MarkerTrackerExample : MonoBehaviour
{
    GameObject markerdetectorGO;
    MarkerTargetIndicator marketTargetindicator;

    /// <summary>
    /// The marker ID to use for the attached transform.
    /// </summary>
    public int id = -1;

    private void Start() {

        markerdetectorGO = MarkerDetector.Instance.gameObject;

        //hide markerindicator
        marketTargetindicator = markerdetectorGO.GetComponent<MarkerTargetIndicator>();
        marketTargetindicator.enabled = false;
    }

    /// <summary>
    /// Use in LateUpdate, for better performance
    /// </summary>
    private void LateUpdate()
    {
        //enable marker gameObject (disbaled by default)
        if (!markerdetectorGO.activeSelf)
        {
            markerdetectorGO.SetActive(true);
        }
        Transform newTransform = this.transform;
        if (MarkerDetector.Instance != null)
        {
			Debug.Log("seeing" + MarkerDetector.Instance.GetNumberOfVisibleMarkers()+ "markers");
            if (MarkerDetector.Instance.updatedMarkerTransforms.Contains(id)){
                MarkerDetector.Instance.GetMarkerTransform(id, ref newTransform);
				BaseForLevel bfl;
				if((bfl= newTransform.gameObject.GetComponent<BaseForLevel>())!=null){
					bfl.Seen();
				}
			}
        }
    }

}