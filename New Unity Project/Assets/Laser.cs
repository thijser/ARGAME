using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
    public int maxReflections = 10;

	void Start() {
        
	}
	
	void Update() {
        Vector3 pos = transform.position;
        Vector3 dir = transform.forward;

        for (int i = 0; i < maxReflections; i++) {
            RaycastHit hitInfo;

            if (Physics.Raycast(pos, dir, out hitInfo, 20.0f)) {
                Debug.DrawLine(pos, hitInfo.point, Color.green);

                pos = hitInfo.point;
                dir = Vector3.Reflect(dir, hitInfo.normal);
            } else {
                Debug.DrawLine(pos, pos + dir * 20, Color.red);

                break;
            }
        }
	}
}
