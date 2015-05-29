//----------------------------------------------------------------------------
// <copyright file="LaserBeam.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core
{
    using System.Collections;
    using Core.Emitter;
    using UnityEngine;

    /// <summary>
    /// A Laser beam that is emitted into the world.
    /// </summary>
    public class LaserBeam
    {
        /// <summary>
        /// The maximum distance a laser beam travels.
        /// </summary>
        public const float MaxLaserLength = 1000.0f;

        /// <summary>
        /// The maximum distance the laser beam will check for collisions.
        /// </summary>
        public const float MaxRaycastDist = 1000.0f;

        /// <summary>
        /// The LaserEmitter that emitted this Laser beam.
        /// </summary>
        private LaserEmitter emitter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaserBeam"/> class.
        /// </summary>
        /// <param name="origin">The origin of this Laser beam.</param>
        /// <param name="direction">The direction vector of this Laser beam.</param>
        /// <param name="emitter">The LaserEmitter that caused this Laser beam.</param>
        public LaserBeam(Vector3 origin, Vector3 direction, LaserEmitter emitter)
        {
            this.Origin = origin;
            this.Direction = direction;
            this.emitter = emitter;
        }

        /// <summary>
        /// Gets the LaserEmitter that (possibly indirectly) emitted this Laser beam.
        /// </summary>
        /// <value>The LaserEmitter.</value>
        public LaserEmitter Emitter
        {
            get
            {
                return this.emitter;
            }
        }

        /// <summary>
        /// Gets or sets the origin point of this Laser beam.
        /// </summary>
        public Vector3 Origin { get; set; }

        /// <summary>
        /// Gets or sets the direction of this Laser beam.
        /// </summary>
        public Vector3 Direction { get; set; }

        /// <summary>
        /// Gets or sets the endpoint of this Laser beam.
        /// This value is available only after this Laser's <c>Create()</c>
        /// is invoked.
        /// </summary>
        public Vector3 Endpoint { get; set; }

        /// <summary>
        /// Determines the end point of this Laser beam.
        /// The Laser beam can only be drawn after this method is invoked.
        /// Also, until this method is invoked, the value of the <c>endpoint</c>
        /// parameter is undefined.
        /// </summary>
        public void Create()
        {
            HitEventArgs args = this.DoRaycast();
            ILaserReceiver receiver = args.Receiver;
            if (receiver != null)
            {
                receiver.OnLaserHit(this, args);
            }
        }

        /// <summary>
        /// Finds the Receiver this Laser collided with.
        /// </summary>
        /// <returns>The HitEventArgs containing details about the collision.</returns>
        public HitEventArgs DoRaycast()
        {
            RaycastHit[] hits = Physics.RaycastAll(this.Origin, this.Direction, MaxRaycastDist);
            foreach (RaycastHit hit in hits)
            {
                ILaserReceiver receiver = GetValidReceiver(hit);
                if (receiver != null)
                {
                    this.Endpoint = hit.point;
                    this.emitter.AddLaser(this);
                    return new HitEventArgs(this, hit.point, hit.normal, receiver);
                }
            }

            this.Endpoint = this.Origin + (MaxLaserLength * this.Direction);
            this.Emitter.AddLaser(this);
            return new HitEventArgs();
        }

        /// <summary>
        /// Extends this Laser beam in the specified direction.
        /// The returned Laser is already initialized. That is,
        /// its <c>Create()</c> method is invoked.
        /// </summary>
        /// <param name="newOrigin">The origin of the new Laser beam.</param>
        /// <param name="newDirection">The direction of the new Laser beam.</param>
        /// <returns>The created Laser instance.</returns>
        public LaserBeam Extend(Vector3 newOrigin, Vector3 newDirection)
        {
            LaserBeam laser = new LaserBeam(newOrigin, newDirection, this.emitter);
            laser.Create();
            return laser;
        }

        /// <summary>
        /// Returns a valid ILaserReceiver from the given <c>RaycastHit</c>.
        /// <para>
        /// An ILaserReceiver is valid if the argument <c>RaycastHit</c> is not null
        /// and the Collider of the hit has an ILaseReceiver Component that
        /// also extends <c>MonoBehaviour</c>. Furthermore, the <c>MonoBehaviour</c> needs 
        /// to be enabled to be valid.
        /// </para>
        /// </summary>
        /// <returns>The ILaserReceiver, or null if no valid ILaserReceiver is found.</returns>
        /// <param name="hit">The <c>RaycastHit</c>.</param>
        private static ILaserReceiver GetValidReceiver(RaycastHit hit)
        {
            if (hit.collider == null)
            {
                return null;
            }

            ILaserReceiver receiver = hit.collider.GetComponent<ILaserReceiver>();
            MonoBehaviour script = receiver as MonoBehaviour;
            if (script == null || !script.enabled)
            {
                return null;
            }

            return receiver;
        }
    }
}
