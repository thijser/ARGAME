namespace Projection
{
    using Meta;
    using Projection;
    using UnityEngine;

    public class MarkerTrackerExample : MonoBehaviour
    {
        /// <summary>
        /// The marker ID to use for the attached transform.
        /// </summary>
        public int id = -1;

        private GameObject markerdetectorGO;
        private MarkerTargetIndicator marketTargetindicator;

        public void Start()
        {
            this.markerdetectorGO = MarkerDetector.Instance.gameObject;

            // hide markerindicator
            this.marketTargetindicator = this.markerdetectorGO.GetComponent<MarkerTargetIndicator>();
            this.marketTargetindicator.enabled = false;
        }

        /// <summary>
        /// Use in LateUpdate, for better performance
        /// </summary>
        public void LateUpdate()
        {
            // enable marker gameObject (disabled by default)
            if (!this.markerdetectorGO.activeSelf)
            {
                this.markerdetectorGO.SetActive(true);
            }

            Transform newTransform = this.transform;
            if (MarkerDetector.Instance != null)
            {
                Debug.Log("seeing" + MarkerDetector.Instance.GetNumberOfVisibleMarkers() + "markers");
                if (MarkerDetector.Instance.updatedMarkerTransforms.Contains(this.id))
                {
                    MarkerDetector.Instance.GetMarkerTransform(this.id, ref newTransform);
                    BaseForLevel bfl;
                    if ((bfl = newTransform.gameObject.GetComponent<BaseForLevel>()) != null)
                    {
                        bfl.Seen();
                    }
                }
            }
        }
    }
}