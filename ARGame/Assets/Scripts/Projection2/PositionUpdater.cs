//----------------------------------------------------------------------------
// <copyright file="PositionUpdater.cs" company="Delft University of Technology">
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
    using Network;
    using UnityEngine;

    /// <summary>
    /// A class that handles marker registration and updates positions.
    /// </summary>
    public class PositionUpdater : MonoBehaviour 
    {
        /// <summary>
        /// How long are we willing to wait after losing track of a marker. 
        /// </summary>
        public long patience=(1000*10000);//1000 milliseconds 

        /// <summary>
        /// Central level marker, this should be visible. 
        /// </summary>
        Marker parent;

        /// <summary>
        /// Scale of the object.
        /// </summary>
        public float scale = 1;
 
        /// <summary>
        /// Collection of all registered to this class. 
        /// </summary>
        private Dictionary<int, Marker> markerTable;

        /// <summary>
        /// Register a new marker
        /// <param name="register">The marker register parameter that registers the new marker.</param>
        /// </summary>
        public void OnMarkerRegister(MarkerRegister register)
        {
            markerTable.Add(register.getMarker().id, register.getMarker());
            if (parent == null)
            {
                parent = register.getMarker();
            }  
        }

        /// <summary>
        /// Gets marker with Identifier  
        /// </summary>
        /// <returns>The marker.</returns>
        /// <param name="id">The identifier.</param>
        /// <exception cref="KeyNotFoundException"> thrown when the marker is not (yet) registered</exception>
        public Marker GetMarker(int id)
        {
            if(markerTable.ContainsKey(id))
            {
                return markerTable[id];
            }
            else
            {
                throw new KeyNotFoundException("this marker is not registered");
            }
        }

        /// <summary>
        /// This marker has been seen by remote, informs the marker of this 
        /// </summary>
        /// <param name="mp">marker position.</param>
        /// <param name="id">Identifier.</param>
        public void OnMarkerSeen(MarkerPosition mp,int id)
        {
            this.GetMarker(id).SetLocalPosition(mp);
            if(parent.localPosition.timeStamp.Ticks+patience<mp.timeStamp.Ticks)
            {
                reparent(this.GetMarker(id));
            }
        }

        /// <summary>
        /// inform marker that it has received an rotationUpdate 
        /// </summary>
        /// <param name="ru">rotation update.</param>
        public void onRotationUpdate(RotationUpdate ru)
        {
            this.GetMarker(ru.ID).SetObjectRotation(ru.Rotation);
        }

        /// <summary>
        /// Update position of all markers .
        /// </summary>
        public void Update()
        {
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


        public void reparent(Marker target)
        {
            parent=target;
            foreach(KeyValuePair<int, Marker> entry in markerTable)
            {
                if(entry.Value!=parent)
                {
                    entry.Value.transform.SetParent(target.transform);
                    updatePosition(entry.Value);
                }
            }
        }

        /// <summary>
        /// set the location of the marker based on the remote position. 
        /// </summary>
        /// <param name="pu">position update received over the net.</param>
        public void OnPositionUpdate (PositionUpdate pu)
        {
            this.GetMarker(pu.ID).SetRemotePosition(new MarkerPosition(pu));
        }
    }
}