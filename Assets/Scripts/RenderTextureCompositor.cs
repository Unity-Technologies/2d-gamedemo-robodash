using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Publishes the low res world and UI render textures for the Hidden/LowResCompositor full screen pass,
// which is set up as a renderer feature on Assets/Settings/Renderer2D.asset
[ExecuteInEditMode]
public class RenderTextureCompositor : MonoBehaviour {

    static readonly int k_LowResTexture = Shader.PropertyToID("_LowResTexture");
    static readonly int k_UITexture = Shader.PropertyToID("_UITexture");

    public void SetRenderTexture(RenderTexture rt) {
        Shader.SetGlobalTexture(k_LowResTexture, rt);
    }

    public void SetUIRenderTexture(RenderTexture rt) {
        Shader.SetGlobalTexture(k_UITexture, rt);
    }
}
