using UnityEngine;
using System.Collections;
using Network;
using Projection;
using System;
public class MessageDistributer : MonoBehaviour {
	/// <summary>
	/// Receives and handles all server updates, by sending them forward to other handlers
	/// </summary>
	/// <param name="update">The update to be handled.</param>

	public void OnServerUpdate(AbstractUpdate update)
	{
		if (update == null)
		{
			throw new ArgumentNullException("update");
		}
		
		if (update.Type == UpdateType.UpdatePosition)
		{
			this.SendMessage("OnPositionUpdate", update as PositionUpdate);                
			return;
		}
	 	if (update.Type == UpdateType.UpdateRotation)
		{
			this.SendMessage("OnRotationUpdate", update as RotationUpdate);                
			return;
		}
		if(update.Type==UpdateType.Level){
			this.SendMessage("LevelUpdate", update as LevelUpdate);                
			return;
		}
	}
}
