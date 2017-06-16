using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class UIRenderTextureCamera : MonoBehaviour {

    int targetWidth = 512;
    RenderTexture rt;
    Camera cam;
    Camera mainCam;
    RenderTextureCompositor compositor;
    RenderTextureCamera renderTextureCamera;

    void OnValidate() {
        UpdateCamera();
    }

    void Update() {
        UpdateCamera();
    }

    public void UpdateCamera() {
        if (cam == null) cam = GetComponent<Camera>();
        if (mainCam == null) mainCam = GetComponentInParent<Camera>();
        if (compositor == null) compositor = FindObjectOfType<RenderTextureCompositor>();
        if (renderTextureCamera == null) renderTextureCamera = FindObjectOfType<RenderTextureCamera>();
        if (cam == null || mainCam == null || compositor == null || renderTextureCamera == null) return;

        targetWidth = renderTextureCamera.GetCurrentTargetWidth();
        if (targetWidth == 0) targetWidth = 411;
        if (rt == null || rt.width != targetWidth) {
            rt = new RenderTexture(targetWidth, targetWidth, 1, RenderTextureFormat.ARGB32);
            rt.filterMode = FilterMode.Point;
            rt.useMipMap = false;
            rt.wrapMode = TextureWrapMode.Clamp;

        }
        cam.targetTexture = rt;
        cam.orthographicSize = (targetWidth / 48f);
        compositor.SetUIRenderTexture(rt);
    }

    //void LateUpdate() {
    //    transform.localPosition = Vector3.zero;
    //    float moveStep = 1f / 24f;

    //    transform.position = new Vector3(Mathf.Round(transform.position.x / moveStep),
    //                                        Mathf.Round(transform.position.y / moveStep),
    //                                        Mathf.Round(transform.position.z / moveStep)) * moveStep;
    //    transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    //}
}
