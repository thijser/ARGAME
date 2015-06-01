namespace Projection{
using System.Collections.Generic;

	public interface IARLink {
		/// <summary>
		/// return a list of all vissible markers and their locations, can be empty if none are visible. 
		/// </summary>
		/// <returns>The marker positions.</returns>
		 List<MarkerPosition>GetMarkerPositions();
	}
}