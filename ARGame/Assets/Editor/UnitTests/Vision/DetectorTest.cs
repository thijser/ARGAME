//----------------------------------------------------------------------------
// <copyright file="DetectorTest.cs" company="Delft University of Technology">
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
    using NUnit.Framework;
    using Projection;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="Detector"/> class.
    /// </summary>
    [TestFixture]
    public class DetectorTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if calling the <c>Start()</c> method has no effect 
        /// if there is no ARLink present.
        /// </summary>
        [Test]
        public void TestStartNoARLink()
        {
            Detector detector = GameObjectFactory.Create<Detector>();
            detector.Start();
            Assert.IsNull(detector.Link);
        }

        /// <summary>
        /// Tests if calling the <c>Start()</c> method with multiple 
        /// IARLink instances present has no effect.
        /// </summary>
        [Test]
        public void TestStartTwoARLinks()
        {
            Detector detector = GameObjectFactory.Create<Detector>();
            detector.gameObject.AddComponent<MockMetaARLink>();
            detector.gameObject.AddComponent<MockMetaARLink>();
            detector.Start();
            Assert.IsNull(detector.Link);
        }

        /// <summary>
        /// Tests if starting the Detector with a single IARLink sets that
        /// IARLink in the <c>Link</c> property of the Detector.
        /// </summary>
        [Test]
        public void TestStartTypical()
        {
            Detector detector = GameObjectFactory.Create<Detector>();
            IARLink link = detector.gameObject.AddComponent<MockMetaARLink>();
            detector.Start();
            Assert.IsNotNull(link);
            Assert.AreEqual(link, detector.Link);
        }

        /// <summary>
        /// Tests if calling <c>EmitMarkerSeen(null)</c> throws the expected
        /// exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmitMarkerSeenNull()
        {
            Detector detector = GameObjectFactory.Create<Detector>();
            detector.EmitMarkerSeen(null);
        }

        /// <summary>
        /// Tests if calling <c>EmitMarkerSeen</c> sends the <c>OnMarkerSeen</c> message
        /// properly.
        /// </summary>
        [Test]
        public void TestEmitMarkerSeen()
        {
            Detector detector = GameObjectFactory.Create<Detector>();
            MarkerSeenListener listener = detector.gameObject.AddComponent<MarkerSeenListener>();
            MarkerPosition position = new MarkerPosition(new Vector3(2, 3, 4), Quaternion.Euler(15, 45, 30), DateTime.Now, new Vector3(4, 7, 6), 8);
            detector.EmitMarkerSeen(position);
            Assert.AreEqual(position, listener.Position);
        }

        /// <summary>
        /// Listener class for the <c>OnMarkerSeen(MarkerPosition)</c> message.
        /// </summary>
        public class MarkerSeenListener : MonoBehaviour
        {
            /// <summary>
            /// Gets the last MarkerPosition for which <c>OnMarkerSeen</c> was called.
            /// </summary>
            public MarkerPosition Position { get; private set; }

            /// <summary>
            /// Sets the <c>Position</c> property to the argument <see cref="MarkerPosition"/>.
            /// </summary>
            /// <param name="position">The <see cref="MarkerPosition"/>.</param>
            public void OnMarkerSeen(MarkerPosition position)
            {
                this.Position = position;
            }
        }
    }
}
