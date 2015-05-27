//----------------------------------------------------------------------------
// <copyright file="RemoteObjectSyncer.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{	
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Network;
    using UnityEngine;

	/// <summary>
	/// Synchronizes the location of an object from PositionUpdates.
	/// </summary>
    public class RemoteObjectSyncer : MonoBehaviour
    {
        /// <summary>
        /// The GameObjects to register on startup.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public List<GameObject> RegisterObjectsOnStartup;
		public GameObject levelMarker; 
		public float scale; 
        /// <summary>
        /// The Dictionary containing the ID to GameObject mappings.
        /// </summary>
        private Dictionary<int, GameObject> objectTable;

		/// <summary>
		/// Updates the position of the GameObject with the ID of the update.
		/// </summary>
        /// <param name="update">The PositionUpdate.</param>
        public void OnPositionUpdate(PositionUpdate update)
        {
            Debug.Log("received:" + update.ToString());

            if (!this.objectTable.ContainsKey(update.ID))
            {
                throw new KeyNotFoundException("ID of not yet registered object");
            }

            GameObject toMove = this.objectTable[update.ID];
            Transform transToMove = toMove.GetComponent<Transform>();
          	Transform par= levelMarker.GetComponent<Transform>();
			transToMove.position= new Vector3(update.X*scale, 0, update.Y*scale)+par.position;
        }

		/// <summary>
		/// Registers a new object with a given id.
		/// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <param name="obj">The GameObject.</param>
        public void RegisterObject(int id, GameObject obj)
        {
            this.objectTable.Add(id, obj);
        }

		/// <summary>
		/// Loads the List of objects into the dictionary.
		/// </summary>
        public void Start()
        {
            this.objectTable = new Dictionary<int, GameObject>();
            int i = 0;
            foreach (GameObject go in this.RegisterObjectsOnStartup)
            {
                i++;
                this.objectTable.Add(i, go);
                Transform transGo = go.GetComponent<Transform>();
				Transform parent = levelMarker.GetComponent<Transform>();
				transGo.SetParent(parent, false);
            }
        }
    }
}
