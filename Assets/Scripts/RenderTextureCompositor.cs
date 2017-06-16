using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class RenderTextureCompositor : ImageEffectBase {


    RenderTexture rt;
    RenderTexture uiRT;

    public void SetRenderTexture(RenderTexture rt) {
        this.rt = rt;
    }

    public void SetUIRenderTexture(RenderTexture rt) {
        this.uiRT = rt;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {

        material.SetTexture("_LowResTexture", rt);
        material.SetTexture("_UITexture", uiRT);
        Graphics.Blit(source, destination, material);
    }

    void Update() {
        //Shader.SetGlobalInt("_UnevenResolution", Screen.width % 2);
        //Debug.Log("" + Screen.width + " " + Screen.height);
    }
}
