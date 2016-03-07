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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
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
        /// The default scale of the Meta glasses with respect to the world.
        /// </summary>
        public const float DefaultMetaScale = 0.053f;

        /// <summary>
        /// The default depth correction factor for the Meta glasses.
        /// </summary>
        public const float DefaultMetaDepth = 0.84f;

        /// <summary>
        /// The step size of the Meta scale configuration keys.
        /// </summary>
        public const float ScaleConfigurationStepSize = 0.001f;

        /// <summary>
        /// The step size of the Meta depth configuration keys.
        /// </summary>
        public const float DepthConfigurationStepSize = 0.01f;

        /// <summary>
        /// The <see cref="Transform"/> used by the Meta's <see cref="MarkerTracker"/> to set positions of tracked markers.
        /// </summary>
        private Transform lamb;

        /// <summary>
        /// The Meta <see cref="MarkerDetector"/> instance used for tracking markers.
        /// </summary>
        private MarkerDetector markerDetector;

        /// <summary>
        /// Gets or sets the scale of the Meta glasses with respect to the world.
        /// </summary>
        public float MetaScale { get; set; }

        /// <summary>
        /// Gets or sets the depth correction factor for the Meta glasses.
        /// </summary>
        public float MetaDepth { get; set; }

        /// <summary>
        /// Gets the offset of Meta One positions with respect to the marker center.
        /// </summary>
        public Vector3 PositionOffset
        {
            get { return new Vector3(-0.5f * MetaScale, MetaScale, 0); }
        }

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
            this.MetaScale = DefaultMetaScale;
            this.MetaDepth = DefaultMetaDepth;

            if (this.markerDetector == null)
            {
                throw new InvalidOperationException("The marker detector cannot be loaded. Is the Meta connected?");
            }

            // Hide the markerindicator
            MarkerTargetIndicator indicator = this.markerDetector.GetComponent<MarkerTargetIndicator>();
            indicator.enabled = false;
        }

        /// <summary>
        /// Checks if a configuration key is pressed and updates the Meta depth and scale accordingly.
        /// </summary>
        public void FixedUpdate()
        {
            float newDepth = this.MetaDepth;
            float newScale = this.MetaScale;
            if (Input.GetKeyUp(KeyCode.W))
            {
                newDepth += DepthConfigurationStepSize;
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                newDepth -= DepthConfigurationStepSize;
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                newScale += ScaleConfigurationStepSize;
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                newScale -= ScaleConfigurationStepSize;
            }

            if (newDepth != this.MetaDepth || newScale != this.MetaScale)
            {
                File.AppendAllText("MetaConfiguration.txt", "\r\nMetaLink: Scale = " + newScale + ", Depth = " + newDepth);
            }

            this.MetaScale = newScale;
            this.MetaDepth = newDepth;
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

            List<int> updates = this.markerDetector.updatedMarkerTransforms;
            foreach (int id in updates)
            {
                this.markerDetector.GetMarkerTransform(id, ref this.lamb);
                Vector3 position = this.lamb.localPosition + PositionOffset;
                position.Scale(new Vector3(1, 1, MetaDepth));
                MarkerPosition pos = new MarkerPosition(
                    position,
                    this.lamb.localRotation,
                    DateTime.Now,
                    MetaScale * this.lamb.localScale,
                    id);
                list.Add(pos);
            }

            return list;
        }
    }
}
