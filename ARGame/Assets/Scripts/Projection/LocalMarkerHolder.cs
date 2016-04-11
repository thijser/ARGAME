//----------------------------------------------------------------------------
// <copyright file="LocalMarkerHolder.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System;
    using System.IO;
    using Network;
    using UnityEngine;
    using Vision;

    /// <summary>
    /// A class that handles marker registration and updates positions.
    /// </summary>
    public class LocalMarkerHolder : MarkerHolder<LocalMarker>
    {
        /// <summary>
        /// How long are we willing to wait after losing track of a marker. 
        /// </summary>
        public const long Patience = 1;

        /// <summary>
        /// The threshold value for moving.
        /// <para>
        /// Movements smaller than this (squared) distance are discarded.
        /// This reduces image vibrations caused by inaccuracies in tracking.
        /// </para>
        /// </summary>
        public const float MoveThreshold = 0.2f;

        /// <summary>
        /// The threshold value for rotating, in degrees.
        /// <para>
        /// Rotation jumps larger than this value are assumed 
        /// to be tracking errors, and they are ignored if the position
        /// has not changed a lot.</para>
        /// </summary>
        public const float RotateThreshold = 30f;

        /// <summary>
        /// Gets or sets the central level marker, this should be visible. 
        /// </summary>
        public LocalMarker Parent { get; set; }
        
        /// <summary>
        /// Retrieves the scale of the AR glasses used for scaling positions.
        /// </summary>
        public override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Updates the position of all markers.
        /// </summary>
        public void Update()
        {
            if (this.Parent == null || this.Parent.LocalPosition == null)
            {
                Debug.Log("No marker is visible");
                return;
            }

            // We perform a linear transform on markers using three bases:
            //   - the Remote base, which is based on remote positions.
            //   - the Local base, which is based on local positions.
            //   - and the zero base, which is the base with the level marker at the origin.
            //
            // Through these bases, a Matrix 'remote to local' can be constructed to transform 
            // remote positions to local positions because we have the 'remote to zero' and 
            // 'zero to local' transformations.
            Matrix4x4 remoteToZero = this.Parent.RemotePosition.Matrix.inverse;
            Matrix4x4 zeroToLocal = this.Parent.LocalPosition.Matrix;
            Matrix4x4 remoteToLocal = zeroToLocal * remoteToZero;;

            this.SendPositionUpdate(ref remoteToLocal);
            this.UpdateMarkerPositions(remoteToLocal);
        }

        /// <summary>
        /// Sends an <see cref="ARViewUpdate"/> based on the given Matrix transform.
        /// </summary>
        /// <param name="remoteToLocal">The remote-to-local matrix transformation,
        /// passed by reference for performance.</param>
        public void SendPositionUpdate(ref Matrix4x4 remoteToLocal)
        {
            Matrix4x4 inverse = remoteToLocal.inverse;
            Vector4 intersection = inverse * Camera.main.transform.forward.ToVec4();
            Vector4 position = inverse * new Vector4(0, 0, 0, 1);
            this.SendMessage("OnSendPosition", new ARViewUpdate(-1, position, intersection));
        }

        /// <summary>
        /// Called whenever a marker is seen by the detector.
        /// <para>
        /// The <c>position</c> argument should not be null.
        /// </para>
        /// <para>
        /// This method updates the local position of the marker, and possibly changes the parent of 
        /// all markers to the indicated marker if the current parent is no longer visible.
        /// </para>
        /// </summary>
        /// <param name="position">The marker position, not null.</param>
        /// <exception cref="ArgumentNullException">If <c>position == null</c>.</exception>
        public void OnMarkerSeen(MarkerPosition position)
        {
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }

            LocalMarker marker = this.GetMarkerOrCreate(position.ID);
            this.SelectParent(marker);

            if (marker.LocalPosition != null)
            {
                float posDifference = (position.Position - marker.LocalPosition.Position).sqrMagnitude;
                float dirDifference = (position.Rotation * Quaternion.Inverse(marker.LocalPosition.Rotation)).eulerAngles.sqrMagnitude;
                if (posDifference < MoveThreshold || (posDifference < 2 * MoveThreshold && dirDifference > RotateThreshold))
                {
                    // Do not update the position when there is not enough difference.
                    return;
                }
            }

            marker.LocalPosition = position;
        }

        /// <summary>
        /// Sees if the marker is more suited for being the level marker then the old marker. 
        /// If updatedMarker has been seen more recently then the parent+patience and the updateMarker is complete then replace.
        /// </summary>
        /// <param name="updatedMarker">The new parent Marker, not null.</param>
        public void SelectParent(LocalMarker updatedMarker)
        {
            if (updatedMarker == null)
            {
                throw new ArgumentNullException("updatedMarker");
            }

            if (updatedMarker.LocalPosition == null || updatedMarker.RemotePosition == null)
            {
                return;
            }

            if (this.Parent == null || this.Parent.LocalPosition == null ||
                this.Parent.LocalPosition.TimeStamp.AddMilliseconds(Patience) < updatedMarker.LocalPosition.TimeStamp)
            {
                this.Parent = updatedMarker;
            }
        }
    }
}
