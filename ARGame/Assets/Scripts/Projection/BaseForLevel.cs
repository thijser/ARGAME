//----------------------------------------------------------------------------
// <copyright file="BaseForLevel.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Diagnostics.CodeAnalysis;
    using Meta;
    using UnityEngine;

    /// <summary>
    /// Marks which marker is used for the basis of the level by the meta one
    /// </summary>
    public class BaseForLevel : MonoBehaviour
    {
        /// <summary>
        /// The time the base marker may be missing before another marker is used.
        /// </summary>
        public const int Patience = 10;

        /// <summary>
        /// Meta ID of this marker.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int ID = -1;

        /// <summary>
        /// meta object required for tracking 
        /// </summary>
        private GameObject markerdetectorGO;

        /// <summary>
        /// meta object required for tracking 
        /// </summary>
        private MarkerTargetIndicator marketTargetindicator;

        /// <summary>
        /// Gets or sets a value indicating whether this base is currently the level marker used by the Meta.
        /// </summary>
        public bool LevelMarker { get; set; }

        /// <summary>
        /// Gets or sets the x position from the remote.
        /// </summary>
        public float RemoteX { get; set; }

        /// <summary>
        /// Gets or sets the y position from the remote.
        /// </summary>
        public float RemoteY { get; set; }

        /// <summary>
        /// Gets or sets the time stamp for this BaseForLevel instance.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// hide the marker indicator and other meta activities required for tracking 
        /// </summary>
        public void Start()
        {
            this.LevelMarker = false;
            this.markerdetectorGO = MarkerDetector.Instance.gameObject;

            // hide markerindicator
            this.marketTargetindicator = this.markerdetectorGO.GetComponent<MarkerTargetIndicator>();
            this.marketTargetindicator.enabled = false;
        }

        /// <summary>
        /// Updates the positions of detected markers.
        /// </summary>
        public void LateUpdate()
        {
            // enable marker gameObject (disbaled by default)
            if (!this.markerdetectorGO.activeSelf)
            {
                this.markerdetectorGO.SetActive(true);
            }

            // get transform if we have to move this object
            Transform newTransform = this.transform;

            // check if there is a marker detector (if there isn't the meta isn't working)
            if (MarkerDetector.Instance != null)
            {
                // check if we can see this marker 
                if (MarkerDetector.Instance.updatedMarkerTransforms.Contains(this.ID))
                {
                    // if we can then move this marker to that position 
                    MarkerDetector.Instance.GetMarkerTransform(this.ID, ref newTransform);

                    // we have seen this marker 
                    this.Seen();
                    Debug.Log("transforming");
                    if (this.LevelMarker)
                    {
                        UpdateWrapper wrapper = gameObject.GetComponent<UpdateWrapper>();
                        Debug.Log("locking");
                        if (wrapper != null && wrapper.Wrapped != null)
                        {
                            Debug.Log("done");

                            // rotate object to correct position 
                            transform.RotateAround(transform.position, transform.up, -1 * wrapper.Wrapped.Rotation);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The marked marker has been seen. It determines by
        /// determining if this marker is now the dominant one if the
        /// other one has not been seen for some time.
        /// </summary>
        public void Seen()
        {
            // note the timestamp so we know this marker has been seen by the meta at a given point in time
            Debug.Log("saw:" + this.ID);
            this.Timestamp = Time.frameCount;

            // get the currently active level marker 
            BaseForLevel holder = this.GetComponentInParent<BaseForLevel>();

            // check if there is indeed one or if this marker is the highest level marker (and thus we have nothing to check against)
            if (holder != null && holder.LevelMarker)
            {
                // check if we are within patience of the other marker if not switch over
                if (holder.Timestamp + Patience < this.Timestamp)
                {
                    // exchange parrents 
                    Transform p = transform.parent;
                    transform.parent = null;
                    p.parent = this.transform;

                    // mark the other marker as no longer leading and mark this one as the level marker
                    holder.LevelMarker = false;
                    this.LevelMarker = true;
                }
            }
        }
    }
}
