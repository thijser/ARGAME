//----------------------------------------------------------------------------
// <copyright file="ProjectionIntegrationTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Testing.Integration
{
    using Projection;
    using UnityEngine;
    using Vision;

    /// <summary>
    /// Integration test behaviour for the Projection namespace.
    /// </summary>
    public class ProjectionIntegrationTest : MonoBehaviour
    {
        public Marker TestMarker { get; set; }
        public ARLinkAdapter ARLink { get; set; }

        /// <summary>
        /// Prepares the Integration Test.
        /// </summary>
        public void Start()
        {
            this.TestMarker = this.GetComponentInChildren(typeof(Marker)) as Marker;
            this.ARLink = this.GetComponentInChildren(typeof(ARLinkAdapter)) as ARLinkAdapter;
        }

        /// <summary>
        /// Sets the positions of markers in the ARLink to the testing positions.
        /// </summary>
        public void SetupARLink()
        {
            // TODO Find positions that mimic the Meta One.
        }

        /// <summary>
        /// Monitors the underlying GameObjects until this test 
        /// is considered passing.
        /// </summary>
        public void Update()
        { 
            // TODO Validate the positions outputted by the Projection namespace.
        }
    }
}
