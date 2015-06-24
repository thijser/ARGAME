//----------------------------------------------------------------------------
// <copyright file="RemoteMarkerHolder.cs" company="Delft University of Technology">
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
    using Network;

    /// <summary>
    /// Previews the position of PositionUpdate objects.
    /// TODO: Make sure very first update is handled too (can be RotationUpdate!)
    /// </summary>
    public class RemoteMarkerHolder : MonoBehaviour
    {
        /// <summary>
        /// Object representing the mesh to use for markers.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject ReferenceMarker;

        /// <summary>
        /// Collection of markers that have already been instantiated.
        /// </summary>
        private Dictionary<int, RemoteMarker> instantiatedMarkers = new Dictionary<int, RemoteMarker>();

        /// <summary>
        /// Creates a marker when the first update is received for it (that is not a delete).
        /// </summary>
        /// <param name="update">The server update to be handled.</param>
        public void OnServerUpdate(AbstractUpdate update)
        {
            if (update.Type == UpdateType.DeletePosition)
            {
                instantiatedMarkers.Remove(update.Id);
            }
            else if (!instantiatedMarkers.ContainsKey(update.Id))
            {
                RemoteMarker marker = Instantiate(this.ReferenceMarker).GetComponent<RemoteMarker>();
                marker.Id = update.Id;

                marker.transform.parent = this.transform;
                marker.gameObject.SetActive(true);

                instantiatedMarkers.Add(update.Id, marker);
            }
        }

        /// <summary>
        /// Handle position updates to move or delete marker.
        /// </summary>
        /// <param name="update">Position update to handle.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            if (update.Type == UpdateType.DeletePosition)
            {
                instantiatedMarkers[update.Id].RemoveObject();
            }
            else
            {
                instantiatedMarkers[update.Id].MoveObject(update.Coordinate);
            }
        }

        /// <summary>
        /// Handle rotation update to rotate marker.
        /// </summary>
        /// <param name="update">Rotation update to handle.</param>
        public void OnRotationUpdate(RotationUpdate update)
        {
            instantiatedMarkers[update.Id].RotateObject(update.Rotation);
        }

        /// <summary>
        /// Disable reference marker, because it's only used as template.
        /// </summary>
        public void Start()
        {
            this.ReferenceMarker.SetActive(false);
        }
    }
}
