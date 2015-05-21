using UnityEngine;
using System.Collections;
using Network;

public class PositionPreviewer : MonoBehaviour {
	void OnPositionUpdate(PositionUpdate update) {
        GameObject.Find("Marker" + update.ID).transform.position = new Vector3(update.X / 10.0f, 0, 72 - update.Y / 10.0f);
    }
}
