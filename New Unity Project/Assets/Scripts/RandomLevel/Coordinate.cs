//----------------------------------------------------------------------------
// <copyright file="Coordinate.cs" company="Delft University of Technology">
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
