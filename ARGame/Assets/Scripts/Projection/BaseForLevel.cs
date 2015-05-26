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
    
	/// <summary>
	/// Marks which marker is used for the basis of the level by the meta one
	/// </summary>
    public class BaseForLevel : MonoBehaviour
    {
        /// <summary>
        /// The time the base marker may be missing before another marker is used.
        /// </summary>
        public const int Patience = 10;

        /// <summary>
        /// The GameObject to use as a base point.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public GameObject Basepoint;

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
            this.Timestamp = Time.frameCount;
            UsedCardManager holder = this.Basepoint.GetComponent<UsedCardManager>();
            if (holder.CurrentlyUsed.Timestamp + Patience < this.Timestamp)
            {
                Transform p = transform.parent;
                transform.parent = null;
                p.parent = this.transform;
                holder.CurrentlyUsed = this;
            }
        }
    }
}
