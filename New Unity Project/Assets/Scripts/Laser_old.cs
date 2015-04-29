using UnityEngine;
using System.Collections;

public class Laser_old : MonoBehaviour {
    public int maxReflections = 10;
    public LineRenderer lineRenderer;

	void Update() {
        Vector3 pos = transform.position;
        Vector3 dir = transform.forward;

        var positions = new ArrayList(maxReflections);
        positions.Add(pos);

        for (int i = 0; i < maxReflections; i++) {
            RaycastHit hitInfo;

            if (Physics.Raycast(pos, dir, out hitInfo, 20.0f)) {
                positions.Add(hitInfo.point);

                pos = hitInfo.point;
                dir = Vector3.Reflect(dir, hitInfo.normal);

                if (hitInfo.collider.tag != "Mirror") {
                    break;
                }
            } else {
                positions.Add(pos + dir * 20);

                break;
            }
        }

        lineRenderer.SetVertexCount(positions.Count);

        for (int i = 0; i < positions.Count; i++) {
            lineRenderer.SetPosition(i, (Vector3) positions[i]);
        }
	}
}
