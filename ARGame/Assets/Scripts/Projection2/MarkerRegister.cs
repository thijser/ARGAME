using UnityEngine;
using System.Collections;
namespace Projection{
	/// <summary>
	/// Use this message for registering a marker. 
	/// </summary>
	public class MarkerRegister  {
		Marker marker;
		public MarkerRegister(Marker mark){
			marker = mark;
		}
		public Marker getMarker(){
			return marker;
		}
	}

}