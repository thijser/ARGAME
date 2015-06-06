using UnityEngine;
using UnityEditor;

/// <summary>
/// Allows users to easily make a marker the new parent/level marker.
/// </summary>
[CustomEditor(typeof(MarkerTransformerMarker))]
public class MarkerTransformEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MarkerTransformerMarker marker = (MarkerTransformerMarker) target;
        if (GUILayout.Button("Make level marker"))
        {
            // Find any current level marker and make it a normal marker again.
            GameObject currentLevelMarker = GameObject.FindGameObjectWithTag("LevelMarker");
            if (currentLevelMarker != null)
            {
                currentLevelMarker.tag = "Untagged";
            }

            // Make the linked marker the new level marker.
            marker.tag = "LevelMarker";
        }
    }
}
