using UnityEngine;
using System.Collections.Generic;
using Network;

namespace Projection{
	public class PositionUpdater : MonoBehaviour {
		/// <summary>
		/// Central level marker, this should be visible. 
		/// </summary>
		Marker parent;

		/// <summary>
		/// Collection of all registered to this clas. 
		/// </summary>
		private Dictionary<int, Marker> markerTable;
		/// <summary>
		/// register a new marker
		/// </summary>
		public void OnMarkerRegister(MarkerRegister reg){
			markerTable.Add(reg.getMarker().id,reg.getMarker());
		}
		/// <summary>
		/// gets marker with Identifier  
		/// </summary>
		/// <returns>The marker.</returns>
		/// <param name="id">Identifier.</param>
		/// <exception cref="KeyNotFoundException"> thrown when the marker is not (yet) registered</exception>
		public Marker GetMarker(int id){
			if(markerTable.ContainsKey(id)){
				return markerTable[id];
			}else{
				throw new KeyNotFoundException("this marker is not registered");
			}
		}
		/// <summary>
		/// this marker has been seen by remote, informs the marker of this 
		/// </summary>
		/// <param name="mp">marker position.</param>
		/// <param name="id">Identifier.</param>
		public void OnMarkerSeen(MarkerPosition mp,int id){
			this.GetMarker(id).SetLocalPosition(mp);
		}

		public void onRotationUpdate(RotationUpdate ru){
			this.GetMarker(ru.ID).SetObjectRotation(ru.Rotation);
		}
		

		/// <summary>
		/// set the location of the marker based on the remote position. 
		/// </summary>
		/// <param name="pu">position update received over the net.</param>
		public void OnPositionUpdate (PositionUpdate pu){
			this.GetMarker(pu.ID).SetRemotePosition(new MarkerPosition(pu));
		}
	}

}