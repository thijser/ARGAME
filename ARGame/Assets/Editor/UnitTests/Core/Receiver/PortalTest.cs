//----------------------------------------------------------------------------
// <copyright file="PortalTest.cs" company="Delft University of Technology">
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
    using NUnit.Framework;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// A test class for the AndGate object.
    /// </summary>
    [TestFixture]
    public class PortalTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if an unlinked portal stays unlinked after
        /// initialization.
        /// </summary>
        [Test]
        public void StartUnlinkedTest()
        {
            Portal p = Create<Portal>();
            Assert.Null(p.PortalEmitter);
            p.Start();
            Assert.Null(p.LinkedPortal);
            Assert.NotNull(p.PortalEmitter);
        }

        /// <summary>
        /// Tests if a linked portal also links its link to itself.
        /// </summary>
        [Test]
        public void StartLinkedTest()
        {
            Portal p = CreateLinkedPortal();
            p.Start();
            Assert.NotNull(p.LinkedPortal);
            Assert.True(p == p.LinkedPortal.LinkedPortal);
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// an invalid HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidOnLaserHit()
        {
            Portal p = CreateLinkedPortal();
            p.OnLaserHit(null, new HitEventArgs());
        }

        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null reference HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullOnLaserHit()
        {
            Portal p = CreateLinkedPortal();
            p.OnLaserHit(null, null);
        }
    }
}