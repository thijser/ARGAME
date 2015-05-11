//----------------------------------------------------------------------------
// <copyright file="AndGate.cs" company="Delft University of Technology">
//     Copyright 2015, Delft University of Technology
//     
//     This software is licensed under the terms of the MIT License.
//     A copy of the license should be included with this software. If not, 
//     see http://opensource.org/licenses/MIT for the full license.
// </copyright>
//----------------------------------------------------------------------------
namespace Laser 
{	
	/// <summary>
	/// An AND-gate that outputs a laser beam if two other beams hit it.
	/// </summary>
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	public class AndGate : MonoBehaviour, ILaserReceiver 
	{
		/// <summary>
		/// A variable storing whether or not a previous laser hit the gate.
		/// </summary>
		private bool hit = false;

		/// <summary>
		/// A variable storing whether or not a beam has already been
		/// created this tick.
		/// </summary>
		private bool beamcreated = false;


		/// <summary>
		/// The lasers that hit the gate.
		/// </summary>
		private IList<Laser> lasers = new List<Laser>();

		/// <summary>
		/// Creates the resulting beam.
		/// </summary>
		/// <returns>The reflected Laser beam segment.</returns>
		/// <param name="laser">The Laser beam.</param>
		/// <param name="surfaceNormal">The surface normal.</param>
		public Laser CreateBeam(Laser laser)
		{
			if (laser == null) 
			{
				throw new ArgumentNullException ("laser");
			}

			Vector3 newDir = new Vector3 (0, 0, 0);
			Vector3 newOrig = new Vector3 (0, 0, 0);
			foreach(Laser l in lasers) 
			{
				newDir = newDir + l.Direction;
				newOrig = newOrig + l.Endpoint;
			}
			return laser.Extend (this.transform.position, newDir/lasers.Count); 
		}
		
		/// <summary>
		/// Creates a new laser beam if two existing laser beams hit  
		/// the gate.
		/// </summary>
		/// <param name="sender">The sender of the event, ignored here.</param>
		/// <param name="args">The EventArgs object that describes the event.</param>
		public void OnLaserHit(object sender, HitEventArgs args) 
		{
			if (args == null) 
			{
				throw new ArgumentNullException("args");
			}

			lasers.Add(args.Laser);
			if (hit && !beamcreated) 
			{
				CreateBeam(args.Laser);
				beamcreated = true;
			}

			if (!hit) 
			{
				hit = true;
			}
		}

		/// <summary>
		/// Resets the variables for use in the next tick.
		/// </summary>
		public void LateUpdate() 
		{
			hit = false;
			beamcreated = false;
			lasers.Clear();
		}
	}
}
