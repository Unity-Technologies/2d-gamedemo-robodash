using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class RenderTextureCamera : MonoBehaviour {

    int targetWidth = 512;
    RenderTexture rt;
    Camera cam;
    Camera mainCam;
    RenderTextureCompositor compositor;

    public int tilesPerScreen = 12;

    int currentTargetWidth;

    void Update() {
        if (cam == null) cam = GetComponent<Camera>();
        if (mainCam == null) mainCam = GetComponentInParent<Camera>();
        if (compositor == null) compositor = GetComponentInParent<RenderTextureCompositor>();

        if (cam == null || mainCam == null || compositor == null) return;

        CalculateTargetWidth();

        if(rt == null || rt.width != targetWidth) {
            rt = new RenderTexture(targetWidth, targetWidth, 1, RenderTextureFormat.ARGB32);
            rt.filterMode = FilterMode.Point;
            rt.useMipMap = false;
            rt.wrapMode = TextureWrapMode.Clamp;
           
        }
        cam.targetTexture = rt;
        cam.orthographicSize = (targetWidth / 48f);
        compositor.SetRenderTexture(rt);
        currentTargetWidth = targetWidth;
        Shader.SetGlobalInt("_UnevenResolution", Application.isPlaying ? (targetWidth % 2) : 0);
    }

    int CalculateTargetWidth() {
        int idealTargetWidth = tilesPerScreen * 24;
        float ratio = (float)Screen.width / (float)idealTargetWidth;
        if (Mathf.RoundToInt(ratio) == 0) {
            targetWidth = 400;
        } else targetWidth = Screen.width / Mathf.RoundToInt(ratio);
        return targetWidth;

    }

    void LateUpdate() {
        transform.localPosition = Vector3.zero;
        float moveStep = 1f / 24f;

        transform.position = new Vector3(Mathf.Round(transform.position.x / moveStep),
                                            Mathf.Round(transform.position.y / moveStep),
                                            Mathf.Round(transform.position.z / moveStep)) * moveStep;
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    public int GetCurrentTargetWidth() {
        if (!Application.isPlaying) return CalculateTargetWidth();
        return currentTargetWidth;
    }
}
