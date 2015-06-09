//----------------------------------------------------------------------------
// <copyright file="MarkerTransformEditor.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Allows users to easily make a marker the new parent/level marker.
    /// </summary>
    [CustomEditor(typeof(MarkerTransformerMarker))]
    public class MarkerTransformEditor : Editor
    {
        /// <summary>
        /// Adds a button to the inspector for MarkerTransformerMarker to switch the 
        /// level marker to the selected MarkerTransformerMarker.
        /// </summary>
        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            MarkerTransformerMarker marker = (MarkerTransformerMarker)target;
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
}