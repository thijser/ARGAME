//FIXME: Replace all tabs in this file with sequences of spaces
﻿using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
    public int maxReflections = 10;
    public LineRenderer lineRenderer;

  //FIXME: Empty method
  //       Either:
  //         - Remove this method
  //         - Explain by a comment inside the method why it is empty
	void Start() {

	}

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
                    //FIXME: Refactor the method to remove this "break" statement
                    break;
                }
            } else {
                positions.Add(pos + dir * 20);

                //FIXME: Refactor the method to remove this "break" statement
                break;
            }
        }

        lineRenderer.SetVertexCount(positions.Count);

        for (int i = 0; i < positions.Count; i++) {
            lineRenderer.SetPosition(i, (Vector3) positions[i]);
        }
	}
}
