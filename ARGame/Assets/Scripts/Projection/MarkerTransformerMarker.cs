using UnityEngine;
using System.Collections;

/// <summary>
/// Changes the material of a marker to indicate its type (normal or level marker).
/// </summary>
[ExecuteInEditMode]
public class MarkerTransformerMarker : MonoBehaviour {
    /// <summary>
    /// Material for normal markers.
    /// </summary>
    public Material NormalMarkerMaterial;

    /// <summary>
    /// Material for level/parent marker.
    /// </summary>
    public Material LevelMarkerMaterial;

	void Update() {
        if (tag == "LevelMarker")
        {
            GetComponent<Renderer>().material = LevelMarkerMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = NormalMarkerMaterial;
        }
	}
}
