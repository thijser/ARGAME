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

    public class RemoteObjectSyncer : MonoBehaviour
    {
        /// <summary>
        /// The GameObjects to register on startup.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public List<GameObject> RegisterObjectsOnStartup;

        private Dictionary<int, GameObject> objectTable;

        public void OnPositionUpdate(PositionUpdate update)
        {
            Debug.Log("received:" + update.ToString());
            if (!this.objectTable.ContainsKey(update.ID))
            {
                throw new KeyNotFoundException("ID of not yet registered object");
            }

            GameObject toMove = this.objectTable[update.ID];
            Transform transToMove = toMove.GetComponent<Transform>();
            transToMove.localPosition = new Vector3(update.X, 0, update.Y);
        }

        public void RegisterObject(int id, GameObject obj)
        {
            this.objectTable.Add(id, obj);
        }

        public void Start()
        {
            this.objectTable = new Dictionary<int, GameObject>();
            int i = 0;
            foreach (GameObject go in this.RegisterObjectsOnStartup)
            {
                i++;
                this.objectTable.Add(i, go);
                Transform transGo = go.GetComponent<Transform>();
                transGo.SetParent(transform.parent, false);
            }
        }
    }
}
