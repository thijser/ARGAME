namespace Network
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Update sent by the server whenever the board size is changed.
    /// <para>
    /// Usually, this message is sent only once just after the client connects.
    /// </para>
    /// </summary>
    public class LevelUpdate : AbstractUpdate
    {
        /// <summary>
        /// Initializes a new object of the <see cref="BoardSizeUpdate"/> class.
        /// </summary>
        /// <param name="size">The new size of the board.</param>
        /// <param name="index">The index of the next level.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If one of the values in the supplied <see cref="Vector2"/> is not positive,
        /// or if the supplied level index is not positive.
        /// </exception>
        public LevelUpdate(Vector2 size, int index)
        {
            if (size.x <= 0 || size.y <= 0)
            {
                throw new ArgumentOutOfRangeException("size", size, "The board must have positive dimensions");
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "The index of the next level must be a positive integer.");
            }

            this.Size = size;
            this.NextLevelIndex = index;
        }

        /// <summary>
        /// Gets the new board size.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// Gets the index of the next level.
        /// </summary>
        public int NextLevelIndex { get; private set; }
    }
}
