using System;
using UnityEngine;
using System.Collections;
using Meta;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Projection{
public class MetaListLink : MonoBehaviour ,IARLink {
		List<GameObject> VirtualMarkers = new List<GameObject>();
		void MarkerGameObjectFactoryMethod(int id){
			GameObject marker = new GameObject("virtual marker"+id);
			marker.AddComponent<MetaBody>();
			MetaBody metabody = marker.GetComponent<MetaBody>();
			metabody.markerTarget=true;
			metabody.markerTargetID=id;
			marker.transform.position=new Vector3(13,666,1337);//it's dark magic
			metabody.markerTargetPlaceable=false;
			VirtualMarkers.Add(marker);
		}
		void OnMarkerRegister(MarkerRegister register){
			MarkerGameObjectFactoryMethod(register.RegisteredMarker.ID);

		}

		public Collection<MarkerPosition> GetMarkerPositions(){
			Collection<MarkerPosition> list= new Collection<MarkerPosition>();
			foreach (GameObject go in VirtualMarkers){
				int id = go.GetComponent<MetaBody>().markerTargetID;

				MarkerPosition mp = new MarkerPosition(
													go.transform.position,
													go.transform.rotation,
													DateTime.Now,
													go.transform.localScale,
				                                    id
													);
				list.Add (mp);
			}
			return list;

		}

	}
}