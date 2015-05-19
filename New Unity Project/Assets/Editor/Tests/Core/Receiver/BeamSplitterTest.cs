//----------------------------------------------------------------------------
// <copyright file="BeamSplitterTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    using System;
    using Core.Emitter;
    using NSubstitute;
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// A test class for the OrGate object.
    /// </summary>
    [TestFixture]
    public class BeamSplitterTest : MirrorsUnitTest
    {
        /// <summary>
        /// A mock emitter, useful for testing purposes.
        /// </summary>
        private MultiEmitter mockedEmitter;

        /// <summary>
        /// Verifies the scene is empty before running the test. 
        /// Fails the test otherwise.
        /// Also sets up the behaviour of the mocked emitter.
        /// </summary>
        [SetUp]
        public void SetUpTest()
        {
            if (Array.Exists(
                    UnityObject.FindObjectsOfType<GameObject>(),
                    e => !this.IsNonInterfering(e)))
            {
                Debug.LogError("Unit tests are being run in a non-empty scene. Clear the scene and try again.");
                Assert.Fail("Failed because current scene is not empty.");
            }
        }

        /// <summary>
        /// Tests if a MultiEmitter object is added when 
        /// the object is instantiated in Unity.
        /// </summary>
        [Test]
        public void StartTest()
        {
            BeamSplitter bs = CreateBeamSplitter();
            Assert.Null(bs.PassThroughEmitter);
            bs.Start();
            Assert.NotNull(bs.PassThroughEmitter);
        }

        /// <summary>
        /// Tests if the correct exception is shown
        /// if a null reference is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest()
        {
            BeamSplitter bs = CreateBeamSplitter();
            bs.Start();
            bs.OnLaserHit(null, null);
        }

        /// <summary>
        /// Tests if the correct exception is shown
        /// if an invalid HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidTest()
        {
            BeamSplitter bs = CreateBeamSplitter();
            bs.Start();
            bs.OnLaserHit(null, new HitEventArgs());
        }
    }
}