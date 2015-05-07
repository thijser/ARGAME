namespace RandomLevel
{
	using System;

	/// <summary>
	/// A coordinate class, to specify a coordinate in the map
	/// </summary>
	public class Coordinate
	{
		/// <summary>
		/// Gets or sets the row coordinate.
		/// </summary>
		public int row { get; private set;}

		/// <summary>
		/// Gets or sets the column coordinate.
		/// </summary>
		public int col { get; private set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomLevel.Coordinate"/> class.
		/// </summary>
		/// <param name="_row"> The row coordinate.</param>
		/// <param name="_col"> The column coordinate.</param>
		public Coordinate(int row, int col)
		{
			this.row = row;
			this.col = col;
		}
	}
}
