Shader "Hidden/LowResCompositor"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "LowResCompositor"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Set globally by RenderTextureCompositor
            TEXTURE2D(_LowResTexture);
            TEXTURE2D(_UITexture);

            half4 frag (Varyings i) : SV_Target
            {
                //calculate proper uv coordinates of the pixel perfect low res texture & add ui texture on top
                float2 uv = i.texcoord;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                uv.y = ((uv.y * 2.0 - 1.0) / aspect) / 2.0 + .5;

                half4 col = SAMPLE_TEXTURE2D(_LowResTexture, sampler_PointClamp, uv);
                half4 ui = SAMPLE_TEXTURE2D(_UITexture, sampler_PointClamp, uv);

                return half4(col.rgb * (1-ui.a) + ui.rgb * ui.a, 1);
            }
            ENDHLSL
        }
    }
}
