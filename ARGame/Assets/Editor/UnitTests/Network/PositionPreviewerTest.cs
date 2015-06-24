//----------------------------------------------------------------------------
// <copyright file="PositionPreviewerTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Network
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using TestUtilities;
    using Projection;

    /// <summary>
    /// Test class for PositionPreviewer.
    /// </summary>
    [TestFixture]
    public class PositionPreviewerTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null reference is used.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullTest()
        {
            GameObjectFactory.Create<RemoteMarkerHolder>().OnPositionUpdate(null);
        }

        /// <summary>
        /// Tests if the start behavior is correct.
        /// </summary>
        [Test]
        public void StartTest()
        {
            RemoteMarkerHolder pp = GameObjectFactory.Create<RemoteMarkerHolder>();
            pp.ReferenceMarker = GameObjectFactory.Create<RemoteMarker>();
            pp.Start();
            Assert.False(pp.ReferenceMarker.gameObject.activeSelf);
            pp.ReferenceMarker.gameObject.SetActive(true);
        }
    }
}
