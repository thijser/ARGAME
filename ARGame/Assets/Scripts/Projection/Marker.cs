//----------------------------------------------------------------------------
// <copyright file="Marker.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not,
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Projection
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// Represents a marker detected in the world.
    /// <para>
    /// This class acts as a container for the position and rotation data 
    /// coming from both the remote server as well as the local IARLink 
    /// implementation.
    /// </para>
    /// </summary>
    public class Marker : MonoBehaviour
    {
        /// <summary>
        /// The ID of the Marker.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public int id = -1;

        /// <summary>
        /// Gets or sets the ID of this Marker.
        /// <para>
        /// If the ID is already set to a valid ID, the setter does nothing.
        /// </para>
        /// </summary>
        public int ID
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id < 0)
                {
                    this.id = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the remote position of this marker.
        /// </summary>
        public MarkerPosition RemotePosition { get; set; }

        /// <summary>
        /// Gets or sets the local position of this marker.
        /// </summary>
        public MarkerPosition LocalPosition { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the object on this marker.
        /// </summary>
        public float ObjectRotation { get; set; }

        /// <summary>
        /// Registers this marker.
        /// </summary>
        public void Start()
        {
			this.SendMessageUpwards("OnMarkerRegister", new MarkerRegister(this));
        }

		public void UpdatePosition(Marker parent){
			if(this!=parent){
				// Position relative to level marker in board space
				Vector3 rel = this.RemotePosition.Position - parent.RemotePosition.Position;
				rel.Scale(parent.LocalPosition.Scale);
				// Rotate position to Meta space rotation
				this.transform.position = parent.LocalPosition.Position + parent.LocalPosition.Rotation  *Quaternion.Inverse(parent.RemotePosition.Rotation)*rel;
				// Give child markers the same rotation and scale as the level marker
				Quaternion relativeRotation = Quaternion.AngleAxis(-parent.RemotePosition.Rotation.eulerAngles.y,parent.gameObject.transform.up);
				this.transform.rotation = relativeRotation * parent.LocalPosition.Rotation;
			}else{
				this.transform.position=this.LocalPosition.Position;
			}
		
		}
		/// <summary>
		/// Returns a string representation of this Marker.
		/// </summary>
		/// <returns>A string describing this Marker.</returns>
		public override string ToString()
        {
            return "<marker:id=" + this.ID +
                ", RemotePosition=" + this.RemotePosition.ToString() +
                ", LocalPosition=" + this.LocalPosition.ToString() +
                ", ObjectRotation=" + this.ObjectRotation + ">";
        }
    }
}