using UnityEngine;
using System.Collections;
using UNetwork = UnityEngine.Network;

public class CubeMover : MonoBehaviour {
    void Update() {
        if (UNetwork.isServer) {
            float t = Time.time * 3;
            transform.position = new Vector3(Mathf.Cos(t), 0, Mathf.Sin(t));
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        if (stream.isWriting) {
            Vector3 pos = transform.position;
            stream.Serialize(ref pos);
        } else {
            Vector3 receivedPosition = Vector3.zero;
            stream.Serialize(ref receivedPosition);
            transform.position = receivedPosition;
        }
    }
}
