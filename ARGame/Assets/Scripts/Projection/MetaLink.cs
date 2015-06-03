﻿//----------------------------------------------------------------------------
// <copyright file="MetaLink.cs" company="Delft University of Technology">
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
    using System.Collections;
    using System.Collections.Generic;
    using Meta;
    using UnityEngine;

    /// <summary>
    /// Class responsible for linking the Meta to the game world.
    /// </summary>
    public class MetaLink : MonoBehaviour, IARLink
    {
        /// <summary>
        /// like cattle this class is driven all around it's very position consumed by the meta, please no cow tipping with the lamb
        /// </summary>
        private GameObject lamb = new GameObject();

        /// <summary>
        /// meta object required for tracking 
        /// </summary>
        private GameObject markerdetectorGO;

        /// <summary>
        /// meta object required for tracking 
        /// </summary>
        private MarkerTargetIndicator marketTargetindicator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaLink"/> class.
        /// </summary>
        public MetaLink()
        {
            this.markerdetectorGO = MarkerDetector.Instance.gameObject;

            // hide markerindicator
            this.marketTargetindicator = this.markerdetectorGO.GetComponent<MarkerTargetIndicator>();
            this.marketTargetindicator.enabled = false;
        }

        /// <summary>
        /// Sets up the detector and marker indicator to find this marker.
        /// </summary>
        public void Start()
        {
            this.markerdetectorGO = MarkerDetector.Instance.gameObject;
            //// hide markerindicator
            this.marketTargetindicator = this.markerdetectorGO.GetComponent<MarkerTargetIndicator>();
            this.marketTargetindicator.enabled = false;
        }

        /// <summary>
        /// Checks if the MarkerDetector instance of the Meta is available.
        /// Throws a <see cref="MissingComponentException"/> otherwise.
        /// </summary>
        public void EnsureMeta()
        {
            if (!this.markerdetectorGO.activeSelf)
            {
                this.markerdetectorGO.SetActive(true);
            }

            if (MarkerDetector.Instance == null)
            {
                throw new MissingComponentException("Missing MarkerDetector instance");
            }
        }

        /// <summary>
        /// Builds and returns the MarkerPositions detected by the Meta detector.
        /// </summary>
        /// <returns>The List of MarkerPositions.</returns>
        public List<MarkerPosition> GetMarkerPositions()
        {
            this.EnsureMeta();
            Transform trans = this.lamb.transform;
            List<MarkerPosition> list = new List<MarkerPosition>();

            foreach (int id in MarkerDetector.Instance.updatedMarkerTransforms)
            {
                MarkerDetector.Instance.GetMarkerTransform(id, ref trans);
                MarkerPosition pos = new MarkerPosition(trans.position, trans.rotation, DateTime.Now, trans.localScale, id);
                list.Add(pos);
                pos.ID = id;
            }

            return list;
        }
    }
}