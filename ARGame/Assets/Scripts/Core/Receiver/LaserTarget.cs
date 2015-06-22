//----------------------------------------------------------------------------
// <copyright file="LaserTarget.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Core.Receiver
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine;

    /// <summary>
    /// A Laser Target that loads the next level when Hit with a Laser beam.
    /// </summary>
    public class LaserTarget : MonoBehaviour, ILaserReceiver
    {
        /// <summary>
        /// The name of the opening animation clip.
        /// </summary>
        public const string OpenClipName = "Open";

        /// <summary>
        /// The name of the opening animation state.
        /// </summary>
        public const string OpenedStateName = "Opened";

        /// <summary>
        /// The original crystal material.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Material OriginalMaterial;

        /// <summary>
        /// The model that defines the crystal.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public MeshRenderer Crystal;

        /// <summary>
        /// The required laser color of this Target.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Unity Property")]
        public Color TargetColor;

        /// <summary>
        /// Gets a value indicating whether the Target is opening.
        /// </summary>
        /// <value><c>true</c> if the Target is opening; otherwise, <c>false</c>.</value>
        public bool IsOpening { get; private set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Start()
        {
            this.SetOpening(false);
        }

        /// <summary>
        /// Updates the animation properties.
        /// </summary>
        public void Reset()
        {
            this.SetOpening(false);
            this.StartCoroutine(this.ResetState());
        }

        /// <summary>
        /// Consumes the Laser beam and loads the next level if the Target is fully opened.
        /// </summary>
        /// <param name="sender">The object that sent this event</param>
        /// <param name="args">The arguments that describe the event</param>
        public void OnLaserHit(object sender, HitEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!args.IsValid)
            {
                throw new ArgumentException("The supplied HitEventArgs object was invalid.");
            }

            if (this.IsHitColorSufficient(args.Laser.Emitter.Properties.LaserColor))
            {
                this.SetOpening(true);
            }
        }

        /// <summary>
        /// Sets whether this LaserTarget is opening.
        /// </summary>
        /// <param name="opening">True to set that this Target is opening, false otherwise.</param>
        public void SetOpening(bool opening)
        {
            this.IsOpening = opening;
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("LaserHit", opening);
            }
        }

        /// <summary>
        /// Ensures the color of the crystal is correct.
        /// </summary>
        public void Update()
        {
            this.EnsureCrystalColor();
        }

        /// <summary>
        /// Tests whether the <c>actual</c> color channel value is close enough 
        /// to the <c>expected</c> value to be considered equal.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <returns>True if they are roughly equal, false otherwise.</returns>
        private static bool MatchExpectedChannel(float expected, float actual)
        {
            return actual >= expected * 0.9f && actual <= expected * 1.1f;
        }

        /// <summary>
        /// Ensures the color of the crystal is correct.
        /// </summary>
        private void EnsureCrystalColor()
        {
            this.Crystal.material.SetColor("_Color", this.TargetColor);
        }

        /// <summary>
        /// Resets the shadowed animation state at the end of the current frame.
        /// </summary>
        /// <returns>The IEnumerator instance used for waiting.</returns>
        private IEnumerator ResetState()
        {
            yield return new WaitForEndOfFrame();
            this.IsOpening = false;
        }

        /// <summary>
        /// Determines if the color of the laser hitting the Target is sufficient.
        /// </summary>
        /// <param name="hit">The color of the beam hitting the Target.</param>
        /// <returns>True if the supplied color is within range (10% off at most
        /// on all RGB strengths), false otherwise.</returns>
        private bool IsHitColorSufficient(Color hit)
        {
            return MatchExpectedChannel(this.TargetColor.r, hit.r)
                && MatchExpectedChannel(this.TargetColor.g, hit.g)
                && MatchExpectedChannel(this.TargetColor.b, hit.b);
        }
    }
}