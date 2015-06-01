//----------------------------------------------------------------------------
// <copyright file="ProjectionIntegrationTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using Network;
    using NUnit.Framework;
    using Projection;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Integration test class for testing whether known marker positions are placed in
    /// expected locations in the Unity scene.
    /// </summary>
    [IntegrationTest.DynamicTest("Untitled")]
    public class ProjectionIntegrationTest : MonoBehaviour
    {
        /// <summary>
        /// Sets up and runs the integration test.
        /// </summary>
        public void Start()
        {
            GameObject marker1 = GameObjectFactory.CreateMarker();
            marker1.transform.position = Vector3.zero;

            GameObject marker2 = GameObjectFactory.CreateMarker();
            marker2.transform.SetParent(marker1.transform);
            GameObject syncer = new GameObject("Syncer", typeof(RemoteObjectSyncer));
            RemoteObjectSyncer sync = syncer.GetComponent<RemoteObjectSyncer>();
            sync.LevelMarker = marker1;
            sync.RegisterObjectsOnStartup.Add(marker1);
            sync.RegisterObjectsOnStartup.Add(marker2);
            sync.Start();

            PositionUpdate update1 = new PositionUpdate(UpdateType.Update, 1, 1, 0, 0);
            sync.OnPositionUpdate(update1);
            Assert.AreEqual(Vector3.zero, marker1.transform.position);
            Assert.AreEqual(Vector3.zero, marker2.transform.position);

            PositionUpdate update2 = new PositionUpdate(UpdateType.Update, 2, 2, 0, 1);
            sync.OnPositionUpdate(update2);
            Assert.AreEqual(new Vector3(1, 0, 1), marker2.transform.position);
        }
    }
}