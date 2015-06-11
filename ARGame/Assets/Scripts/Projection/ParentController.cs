using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Projection{ 
public class ParentController : MonoBehaviour {
		private MarkerHolder holder; 
		private List<Marker> UsedMarkers;
		public long patience=1000*1000;
		GameObject slave;
		// Use this for initialization
		void Start () {
			slave = new GameObject("slave");
			slave.AddComponent<Marker>();
			holder=gameObject.GetComponent<MarkerHolder>();
			if (holder==null){
				throw new ArgumentNullException("no markerholder found");
			}
		}
		
		// Update is called once per frame
		void Update () {
			ConstructUsedMarkerList();	
			holder.Parent=ConstructMarker();
		}

		Marker ConstructMarker(){
			Marker m = slave.GetComponent<marker>();
			m.LocalPosition = ConstructLocalPosition();
			m.RemotePosition = ConstructRemotePosition();
			m.ID=421337666;
			return m;
		}
		Quaternion getLocalRotation(){
			Vector3 euler = new Vector3(0,0,0);
			foreach (Marker m in UsedMarkers){
				euler=euler+m.LocalPosition.Rotation.eulerAngles;
			}
			euler=euler/UsedMarkers.Count;
			return Quaternion.Euler(euler);

		}
		Vector3 getLocalcoordinates(){
			Vector3 coor = new Vector3(0,0,0);
			foreach (Marker m in UsedMarkers){
				coor=coor+m.LocalPosition.Position;
			}
			coor=coor/UsedMarkers.Count;
			return coor;
		}
		Quaternion getRemoteRotation(){
			Vector3 coor = new Vector3(0,0,0);
			foreach (Marker m in UsedMarkers){
				coor=coor+m.RemotePosition.Rotation.eulerAngles;
			}
			coor=coor/UsedMarkers.Count;
			return Quaternion.Euler(coor);
		}
		Vector3 getRemoteCoordinates(){
			Vector3 coor = new Vector3(0,0,0);
			foreach (Marker m in UsedMarkers){
				coor=coor+m.RemotePosition.Position;
			}
			coor=coor/UsedMarkers.Count;
			return coor;
		}
		MarkerPosition ConstructLocalPosition(){
			MarkerPosition mp = new MarkerPosition(getLocalcoordinates(),getLocalRotation(),DateTime.Now,new Vector3(1,1,1),421337666);
			return mp;
		}
		MarkerPosition ConstructRemotePosition(){
			MarkerPosition mp = new MarkerPosition(getRemoteCoordinates(),getRemoteRotation(),DateTime.Now,new Vector3(1,1,1),421337666);
			return mp;
		}
		/// <summary>
		/// puts all markers that can be used as parent marker 
		/// (meaning seen by both local and remote) in usedMarkers 
		/// </summary>
		void ConstructUsedMarkerList(){
			UsedMarkers =new List<Marker>();
            Debug.Log("call.");
			Dictionary<int, Marker> dict = holder.markerTable;
			foreach(KeyValuePair<int,Marker> pair in dict){
				Marker mark = pair.Value;
				if(mark.LocalPosition!=null&&mark.RemotePosition!=null){
                    Debug.Log("Marker seen.");
					if(mark.LocalPosition.TimeStamp.Ticks+patience>DateTime.Now.Ticks&&mark.RemotePosition.TimeStamp.Ticks>patience){
						UsedMarkers.Add(mark);
					}
				}
			}
		}
	}

}