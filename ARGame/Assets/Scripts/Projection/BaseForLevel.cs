//----------------------------------------------------------------------------
// <copyright file="BaseForLevel.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;
	using Meta;
	/// <summary>
	/// Marks which marker is used for the basis of the level by the meta one
	/// </summary>
    public class BaseForLevel : MonoBehaviour
	{
		/// <summary>
		/// The time the base marker may be missing before another marker is used.
		/// </summary>
		public const int Patience = 10;

		public int ID = -1;
		public float RemoteX;
		public float RemoteY;
		GameObject markerdetectorGO;
		MarkerTargetIndicator marketTargetindicator;

		/// <summary>
		/// hide the markerindicator
		/// </summary>
		private void Start() {
			
			markerdetectorGO = MarkerDetector.Instance.gameObject;
			
			//hide markerindicator
			marketTargetindicator = markerdetectorGO.GetComponent<MarkerTargetIndicator>();
			marketTargetindicator.enabled = false;
		}
		private void LateUpdate()
		{
			//enable marker gameObject (disbaled by default)
			if (!markerdetectorGO.activeSelf)
			{
				markerdetectorGO.SetActive(true);
			}
			Transform newTransform = this.transform;
			if (MarkerDetector.Instance != null)
			{
				Debug.Log("seeing" + MarkerDetector.Instance.GetNumberOfVisibleMarkers()+ "markers");
				if (MarkerDetector.Instance.updatedMarkerTransforms.Contains(ID)){
					MarkerDetector.Instance.GetMarkerTransform(ID, ref newTransform);
					BaseForLevel bfl;
					if((bfl= newTransform.gameObject.GetComponent<BaseForLevel>())!=null){
						bfl.Seen();
						Debug.Log ("transforming");
						if(gameObject.GetComponent<UsedCardManager>()){
							UpdateWrapper wrapper=gameObject.GetComponent<UpdateWrapper>();
							Debug.Log ("locking");
							if(wrapper!=null&&wrapper.Wrapped!=null){
								Debug.Log("done");
								transform.RotateAround(transform.position,transform.up,-1*wrapper.Wrapped.Rotation);
						}
					}
				}
			}
		}

		}

		[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]

        /// <summary>
        /// Gets or sets the time stamp for this BaseForLevel instance.
        /// </summary>
        public long Timestamp { get; set; }

		/// <summary>
		/// The marked marker has been seen. It determines by
		/// determining if this marker is now the dominant one if the
	 	/// other one has not been seen for some time.
		/// </summary>
        public void Seen()
        {
			Debug.Log ("saw:" + ID);
            this.Timestamp = Time.frameCount;

            UsedCardManager holder = this.GetComponentInParent<UsedCardManager>();
			BaseForLevel holderbfl;
			if (holder!=null){
				holderbfl=holder.GetComponent<BaseForLevel>();
				if(holderbfl.Timestamp + Patience < this.Timestamp)
            {
				Destroy(holder);
                Transform p = transform.parent;
                transform.parent = null;
                p.parent = this.transform;
				gameObject.AddComponent<UsedCardManager>(); 
				UsedCardManager newHolder=gameObject.GetComponent<UsedCardManager>();
				newHolder.CurrentlyUsed=this;
				
				}
			}

        }
    }
}
