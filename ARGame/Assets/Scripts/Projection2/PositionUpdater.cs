using UnityEngine;
using System.Collections.Generic;
using Network;

namespace Projection{
	public class PositionUpdater : MonoBehaviour {
		/// <summary>
		/// How long are we willing to wait after losing track of a marker. 
		/// </summary>
		public long patience=(1000*10000);//1000 milliseconds 

		/// <summary>
		/// Central level marker, this should be visible. 
		/// </summary>
		Marker parent;
		public float scale=1; 
		/// <summary>
		/// Collection of all registered to this clas. 
		/// </summary>
		private Dictionary<int, Marker> markerTable;
		/// <summary>
		/// register a new marker
		/// </summary>
		public void OnMarkerRegister(MarkerRegister reg){
			markerTable.Add(reg.getMarker().id,reg.getMarker());
			if(parent==null)
				parent=reg.getMarker();
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
			if(parent.localPosition.timeStamp.Ticks+patience<mp.timeStamp.Ticks){
				reparent(this.GetMarker(id));
			}
		}
		/// <summary>
		/// inform marker that it has received an rotationUpdate 
		/// </summary>
		/// <param name="ru">rotation update.</param>
		public void onRotationUpdate(RotationUpdate ru){
			this.GetMarker(ru.ID).SetObjectRotation(ru.Rotation);
		}
		/// <summary>
		/// Update position of all markers .
		/// </summary>
		public void Update(){
			foreach(KeyValuePair<int, Marker> entry in markerTable)
			{
				updatePosition(entry.Value);
			}
		}
		/// <summary>
		/// uses the market target and parent to set the transform of target
		/// </summary>
		/// <param name="target">Target</param>
		public void updatePosition(Marker target)
        {
            if (target == parent)
            {
                UpdateParentPosition(target);
            }
            else
            {
                UpdateChildPosition(target);
            }
		}

        /// <summary>
        /// Updates position if supplied target is the parent.
        /// </summary>
        /// <param name="target">The supplied target.</param>
        public void UpdateParentPosition(Marker target)
        {
            target.gameObject.transform.position = target.localPosition.Position;
            Vector3 localrotation = target.localPosition.Rotation.eulerAngles;
            Vector3 remoterotation = target.remotePosition.Rotation.eulerAngles;
            Vector3 finalrotation = localrotation - remoterotation;
            target.gameObject.transform.rotation = Quaternion.Euler(finalrotation);
        }

        /// <summary>
        /// Updates position if supplied target is not the parent.
        /// </summary>
        /// <param name="target">The supplied target.</param>
        public void UpdateChildPosition(Marker target)
        {
            target.gameObject.transform.position = target.remotePosition.Position - parent.remotePosition.Position;
            // TODO: If mirrored then swap operation params.
        }
		public void reparent(Marker target){
			parent=target;
			foreach(KeyValuePair<int, Marker> entry in markerTable){
				if(entry.Value!=parent){
					entry.Value.transform.SetParent(target.transform);
					updatePosition(entry.Value);
				}
			}
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