﻿//----------------------------------------------------------------------------
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
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using NSubstitute;
    using TestUtilities;
    using UnityEngine;

    /// <summary>
    /// Test class for PositionPreviewer.
    /// </summary>
    [TestFixture]
    public class PositionPreviewerTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the correct exception is thrown when
        /// a null refrence is used.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public static void ArgumentNullTest()
        {
            GameObjectFactory.CreatePreviewer().OnPositionUpdate(null);
        }

        /// <summary>
        /// Tests if none of the marker states have been initialized after creation.
        /// </summary>
        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public static void NullMarkerStateTest()
        {
            GameObjectFactory.CreatePreviewer().GetMarkerState(0);
        }

        /// <summary>
        /// Tests if the start behaviour is correct.
        /// </summary>
        [Test]
        public static void StartTest()
        {
            PositionPreviewer pp = GameObjectFactory.CreatePreviewer();
            pp.ReferenceMarker = new UnityEngine.GameObject();
            pp.Start();
            Assert.False(pp.ReferenceMarker.activeSelf);
        }
    }
}