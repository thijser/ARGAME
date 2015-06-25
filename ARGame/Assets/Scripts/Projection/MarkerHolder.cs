//----------------------------------------------------------------------------
// <copyright file="MarkerHolder.cs" company="Delft University of Technology">
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
    using Level;
    using Network;
    using UnityEngine;
    using UnityEngine.Assertions;
    
    /// <summary>
    /// Container class for Markers.
    /// </summary>
    /// <typeparam name="T">The type of markers this MarkerHolder holds.</typeparam>
    public class MarkerHolder<T> : MonoBehaviour where T : Marker
    {
        /// <summary>
        /// The GameObject to use as a template for Markers.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public T ReferenceMarker;

        /// <summary>
        /// Collection of all registered to this class. 
        /// </summary>
        private Dictionary<int, T> markers = new Dictionary<int, T>();


	
        /// <summary>
        /// Gets an <see cref="IEnumerable"/> over all Markers in
        /// this <see cref="MarkerHolder"/>.
        /// </summary>
        public IEnumerable<T> Markers 
        {
            get
            {
                return this.markers.Values;
            }
        }

        /// <summary>
        /// Deactivates the Reference Marker.
        /// </summary>
        public virtual void Start()
        {
            this.ReferenceMarker.gameObject.SetActive(false);
        }

        /// <summary>
        /// Removes a Marker with the given id, if it exists.
        /// </summary>
        /// <param name="id">The id of the Marker to remove.</param>
        public void RemoveMarker(int id)
        {
            this.markers.Remove(id);
        }

        /// <summary>
        /// Adds a Marker to this MarkerHolder.
        /// <para>
        /// If a Marker with the same Id already exists, it is overwritten.
        /// </para>
        /// </summary>
        /// <param name="marker">The Marker to add, not null.</param>
        public void AddMarker(T marker)
        {
            Assert.IsNotNull(marker);
            this.markers.Remove(marker.Id);
            this.markers.Add(marker.Id, marker);
        }

        /// <summary>
        /// Returns whether this MarkerHolder contains a Marker with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>True if a Marker with the given id exists, false otherwise.</returns>
        public bool Contains(int id)
        {
            return this.markers.ContainsKey(id);
        }

        /// <summary>
        /// Returns the Marker with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The Marker with the given id.</returns>
        /// <exception cref="KeyNotFoundException">If no Marker with that id exists.</exception>
        public T GetMarker(int id)
        {
            if (this.Contains(id))
            {
                return this.markers[id];
            }
            else
            {
                throw new KeyNotFoundException("No such Marker exists: " + id);
            }
        }

        /// <summary>
        /// Returns the Marker with the given id, creating one if it doesn't exist.
        /// </summary>
        /// <param name="id">The marker id.</param>
        /// <returns>The Marker with the requested id.</returns>
        public T GetMarkerOrCreate(int id)
        {
            if (this.Contains(id))
            {
                return this.markers[id];
            }
            else
            {
                T marker = GameObject.Instantiate(this.ReferenceMarker);
                marker.Id = id;
                marker.transform.parent = this.transform;
                this.AddMarker(marker);
                return marker;
            }
        }

        /// <summary>
        /// Applies the given transformMatrix to all Markers in this MarkerHolder.
        /// </summary>
        /// <param name="transformMatrix">The transformMatrix to apply.</param>
        public void UpdateMarkerPositions(Matrix4x4 transformMatrix)
        {
            foreach (Marker marker in this.Markers)
            {
                marker.UpdatePosition(transformMatrix);
            }
        }

        /// <summary>
        /// Applies the <see cref="RotationUpdate"/> to the corresponding <see cref="Marker"/>
        /// in this <see cref="MarkerHolder"/>.
        /// </summary>
        /// <param name="update">The <see cref="RotationUpdate"/>, not null.</param>
        public virtual void OnRotationUpdate(RotationUpdate update)
        {
            Assert.IsNotNull(update);
            this.GetMarkerOrCreate(update.Id).ObjectRotation = update.Rotation;
        }

        /// <summary>
        /// Applies the <see cref="PositionUpdate"/> to the corresponding <see cref="Marker"/>
        /// in this <see cref="MarkerHolder"/>.
        /// </summary>
        /// <param name="update">The <see cref="PositionUpdate"/>, not null.</param>
        public virtual void OnPositionUpdate(PositionUpdate update)
        {
            Assert.IsNotNull(update);
            T marker = this.GetMarkerOrCreate(update.Id);
            if (update.Type == UpdateType.UpdatePosition)
            {
                marker.RemotePosition = new MarkerPosition(update);
                marker.gameObject.SetActive(true);
            }
            else
            {
                Assert.AreEqual(UpdateType.DeletePosition, update.Type);
                marker.gameObject.SetActive(false);
            }
        }
    }
}
