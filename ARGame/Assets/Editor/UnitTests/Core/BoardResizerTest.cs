//----------------------------------------------------------------------------
// <copyright file="BoardResizerTest.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using Network;
using NUnit.Framework;
using TestUtilities;
using UnityEngine;

    /// <summary>
    /// Unit test for the <see cref="BoardResizer"/> class.
    /// </summary>
    [TestFixture]
    public class BoardResizerTest : MirrorsUnitTest
    {
        /// <summary>
        /// Tests if the <c>UpdateBoardSize</c> method returns false 
        /// if no board is found.
        /// </summary>
        [Test]
        public void TestUpdateBoardSizeNoBoard()
        {
            Assert.IsFalse(GameObjectFactory.Create<BoardResizer>().UpdateBoardSize(new Vector2(2, 2)));
        }

        /// <summary>
        /// Tests if a typical invocation of <c>UpdateBoardSize</c> has the desired effect.
        /// </summary>
        [Test]
        public void TestUpdateBoardSizeTypical()
        {
            GameObject board = new GameObject("board");
            board.tag = "PlayingBoard";

            BoardResizer resizer = GameObjectFactory.Create<BoardResizer>();
            board.transform.parent = resizer.transform;
            Assert.IsTrue(resizer.UpdateBoardSize(new Vector2(240, 320)));

            Assert.AreEqual(new Vector3(-240, 1, 320), board.transform.localScale);
        }

        /// <summary>
        /// Tests if calling <c>UpdateBoardSize</c> when the board has a non-default scale
        /// will preserve the scale along the z-axis and overwrite the scale in the x- and y-axes.
        /// </summary>
        [Test]
        public void TestUpdateBoardSizeWithNonDefaultInitialScale()
        {
            GameObject board = new GameObject("board");
            board.tag = "PlayingBoard";
            board.transform.localScale = new Vector3(300, 20, 500);

            BoardResizer resizer = GameObjectFactory.Create<BoardResizer>();
            board.transform.parent = resizer.transform;
            Assert.IsTrue(resizer.UpdateBoardSize(new Vector2(240, 320)));

            Assert.AreEqual(new Vector3(-240, 20, 320), board.transform.localScale);
        }

        /// <summary>
        /// Tests if the <c>OnServerUpdate</c> method performs as expected
        /// with a LevelUpdate argument.
        /// </summary>
        [Test]
        public void TestOnServerUpdateWithLevelUpdate()
        {
            GameObject board = new GameObject("board");
            board.tag = "PlayingBoard";

            BoardResizer resizer = GameObjectFactory.Create<BoardResizer>();
            
            LevelUpdate update = new LevelUpdate(34, new Vector2(240, 320));
            board.transform.parent = resizer.transform;
            resizer.OnServerUpdate(update);

            Assert.AreEqual(new Vector3(-240, 1, 320), board.transform.localScale);
        }

        /// <summary>
        /// Tests if the <c>OnServerUpdate</c> method performs no operation
        /// with a non-LevelUpdate argument.
        /// </summary>
        [Test]
        public void TestOnServerUpdateWithOtherUpdateType()
        {
            GameObject board = new GameObject("board");
            board.tag = "PlayingBoard";

            BoardResizer resizer = GameObjectFactory.Create<BoardResizer>();
            board.transform.parent = resizer.transform;
            resizer.OnServerUpdate(new RotationUpdate(UpdateType.UpdateRotation, 86, 8));

            Assert.AreEqual(Vector3.one, board.transform.localScale);
        }
    }
}
