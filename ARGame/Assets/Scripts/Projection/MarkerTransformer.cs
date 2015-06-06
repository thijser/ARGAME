using UnityEngine;
using System.Collections;

/// <summary>
/// Handles transformations between board markers and Meta markers.
/// </summary>
[ExecuteInEditMode]
public class MarkerTransformer : MonoBehaviour {
    /// <summary>
    /// Board markers.
    /// </summary>
    public Transform[] BoardMarkers;

    /// <summary>
    /// Meta markers that board markers map to.
    /// </summary>
    public Transform[] MetaMarkers;
	
    /// <summary>
    /// Updates the relative Meta marker positions based on the level marker and board layout.
    /// </summary>
	void Update() {
        // Find index of level marker
        int levelMarkerIdx = -1;

        for (int i = 0; i < MetaMarkers.Length; i++)
        {
            if (MetaMarkers[i].tag == "LevelMarker")
            {
                levelMarkerIdx = i;
            }
        }

        // Move the other markers relative to it if there is one
        if (levelMarkerIdx != -1)
        {
            for (int i = 0; i < MetaMarkers.Length; i++)
            {
                // Position relative to level marker in board space
                Vector3 rel = BoardMarkers[i].position - BoardMarkers[levelMarkerIdx].position;

                // Transform position to Meta space scale
                Vector3 inverseScale = BoardMarkers[levelMarkerIdx].localScale;
                inverseScale.x = 1.0f / inverseScale.x;
                inverseScale.y = 1.0f / inverseScale.y;
                inverseScale.z = 1.0f / inverseScale.z;

                rel.Scale(inverseScale);
                rel.Scale(MetaMarkers[levelMarkerIdx].localScale);

                // Rotate position to Meta space rotation
                MetaMarkers[i].position = MetaMarkers[levelMarkerIdx].position + MetaMarkers[levelMarkerIdx].rotation * rel;

                // Give child markers the same rotation and scale as the level marker
                MetaMarkers[i].rotation = MetaMarkers[levelMarkerIdx].rotation;
                MetaMarkers[i].localScale = MetaMarkers[levelMarkerIdx].localScale;
            }
        }
	}
}
