using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToPixelPerfect : MonoBehaviour {

    void LateUpdate() {
        float z = transform.position.z;
        transform.localPosition = Vector3.zero;
        float moveStep = 1f / 24f;

     
        transform.position = new Vector3(Mathf.Round(transform.position.x / moveStep) * moveStep,
                                            Mathf.Round(transform.position.y / moveStep) * moveStep,
                                            z);
     
    }
}
