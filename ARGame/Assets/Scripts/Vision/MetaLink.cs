//----------------------------------------------------------------------------
// <copyright file="MetaLink.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Vision
{
    using System;
    using System.Collections.ObjectModel;
    using Meta;
    using Projection;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Class responsible for linking the Meta to the game world.
    /// </summary>
    public class MetaLink : MonoBehaviour, IARLink
    {
        /// <summary>
        /// The scale of the Meta glasses with respect to the world.
        /// </summary>
        public const float MetaScale = 0.0057f;

        /// <summary>
        /// like cattle this class is driven all around it's very position consumed by the meta, please no cow tipping with the lamb
        /// </summary>
        private Transform lamb;

        /// <summary>
        /// meta object required for tracking 
        /// </summary>
        private MarkerDetector markerDetector;

        /// <summary>
        /// Returns the scale of the Meta One glasses.
        /// </summary>
        /// <returns>The Meta One scale.</returns>
        public float GetScale()
        {
            return MetaScale;
        }

        /// <summary>
        /// Sets up the detector and marker indicator to find this marker.
        /// </summary>
        public void Start()
        {
            this.lamb = new GameObject("lamb").transform;
            this.markerDetector = MarkerDetector.Instance;

            if (this.markerDetector == null)
            {
                throw new InvalidOperationException("The MarkerDetector cannot be loaded. Is the Meta connected?");
            }

            // Hide the markerindicator
            MarkerTargetIndicator indicator = this.markerDetector.GetComponent<MarkerTargetIndicator>();
            indicator.enabled = false;
        }

        /// <summary>
        /// Checks if the MarkerDetector instance of the Meta is available.
        /// Throws a <see cref="MissingComponentException"/> otherwise.
        /// </summary>
        public void EnsureMeta()
        {
            Assert.IsNotNull(this.markerDetector);
            if (!this.markerDetector.gameObject.activeSelf)
            {
                this.markerDetector.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Builds and returns the MarkerPositions detected by the Meta detector.
        /// </summary>
        /// <returns>The List of MarkerPositions.</returns>
        public Collection<MarkerPosition> GetMarkerPositions()
        {
            this.EnsureMeta();
            Collection<MarkerPosition> list = new Collection<MarkerPosition>();

            foreach (int id in this.markerDetector.updatedMarkerTransforms)
            {
                this.markerDetector.GetMarkerTransform(id, ref this.lamb);
                MarkerPosition pos = new MarkerPosition(
                    this.lamb.position,
                    this.lamb.rotation,
                    DateTime.Now,
                    MetaScale * this.lamb.lossyScale,
                    id);
                list.Add(pos);
            }

            return list;
        }
    }
}
