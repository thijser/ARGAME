//----------------------------------------------------------------------------
// <copyright file="LensSplitterTest.cs" company="Delft University of Technology">
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

    /// <summary>
    /// A test class for the lens splitter.
    /// </summary>
    [TestFixture]
    public class LensSplitterTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the object does not
        /// raise its hit flag without being hit.
        /// </summary>
        [Test]
        public void StartTest()
        {
            LensSplitter ls = Create<LensSplitter>();
            Assert.False(ls.IsHit());
        }

        /// <summary>
        /// Tests if the correct exception is shown
        /// if a null reference is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest()
        {
            LensSplitter ls = Create<LensSplitter>();
            ls.OnLaserHit(null, null);
        }

        /// <summary>
        /// Tests if the correct exception is shown
        /// if an invalid HitEventArgs is used in OnLaserHit.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidTest()
        {
            LensSplitter ls = Create<LensSplitter>();
            ls.OnLaserHit(null, new HitEventArgs());
        }
    }
}