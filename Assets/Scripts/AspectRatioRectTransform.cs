using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AspectRatioRectTransform : MonoBehaviour {

    RectTransform rectTransform;
    AspectRatioFitter aspectRatioFitter;
    void Update() {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (aspectRatioFitter == null) aspectRatioFitter = GetComponent<AspectRatioFitter>();

        aspectRatioFitter.aspectRatio = (float)Screen.width / (float)Screen.height;

    }
}
