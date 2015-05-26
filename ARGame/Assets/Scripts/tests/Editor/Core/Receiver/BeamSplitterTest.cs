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
    /// A test class for the beam splitter.
    /// </summary>
    [TestFixture]
    public class BeamSplitterTest : MirrorsUnitTest
    {
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