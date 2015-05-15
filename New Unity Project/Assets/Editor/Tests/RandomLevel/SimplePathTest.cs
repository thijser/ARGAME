//----------------------------------------------------------------------------
// <copyright file="SimplePathTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace RandomLevel
{
    using System;
    using NUnit.Framework;
    using TestUtilities;

    /// <summary>
    /// Test class for the SimplePathBuilder class. 
    /// All options are tested to test the maximum amount of code.
    /// </summary>
    [TestFixture]
    public class SimplePathTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests of the correct argument is thrown when a null
        /// reference is used in the construction of the object.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructArgsNullTest()
        {
            new SimplePathBuilder(null);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the first
        /// parameter of the PathFromToRow-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructRowInvalidParam1()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToRow(100, 5, 10);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the second
        /// parameter of the PathFromToRow-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructRowInvalidParam2()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToRow(5, 100, 10);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the third
        /// parameter of the PathFromToRow-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructRowInvalidParam3()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToRow(5, 15, 100);
        }

        /// <summary>
        /// Tests if the PathFromToRow method labels vertices correctly,
        /// given that the parameters are valid with respect to this 
        /// square graph.
        /// </summary>
        [Test]
        public void ConstructRowValidParams()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToRow(5, 5, 5);
            Assert.That(spb.Graph.GetVertexAtCoordinate(new Coordinate(5, 5)).Property == Property.PARTOFPATH);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the first
        /// parameter of the PathFromToCol-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructColInvalidParam1()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToCol(100, 5, 5);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the second
        /// parameter of the PathFromToCol-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructColInvalidParam2()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToCol(5, 100, 5);
        }

        /// <summary>
        /// Tests if the correct exception is thrown if the third
        /// parameter of the PathFromToCol-method is invalid, given 
        /// the supplied square graph.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructColInvalidParam3()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToCol(5, 15, 100);
        }

        /// <summary>
        /// Tests if the PathFromToCol method labels vertices correctly,
        /// given that the parameters are valid with respect to this 
        /// square graph.
        /// </summary>
        [Test]
        public void ConstructColValidParams()
        {
            SimplePathBuilder spb = new SimplePathBuilder(new SquareGraph(20, 20));
            spb.PathFromToCol(5, 5, 5);
            Assert.That(spb.Graph.GetVertexAtCoordinate(new Coordinate(5, 5)).Property == Property.PARTOFPATH);
        }
    }
}
