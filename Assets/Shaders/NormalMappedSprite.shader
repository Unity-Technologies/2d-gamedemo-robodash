Shader "Custom/NormalMappedSprite" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        [PerRendererData]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MormalMap("Mormal map", 2D) = "white" {}
        _DitherPattern("Dither pattern", 2D) = "gray"{}
        _EmissionMap("Emission map", 2D) = "black"{}
        _DarknessColor("Darkness color", Color) = (0,0,0,1)
        _EdgeLightBoost("Edge light boost", Range(0, 1)) = 1
        _EmissionColor("Emission tint", Color) = (1,1,1,1)
        _TintMapIntensity("Tint map intensity", Range(0, 2)) = 1
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

        TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex);
        TEXTURE2D(_MormalMap);      SAMPLER(sampler_MormalMap);
        TEXTURE2D(_DitherPattern);  SAMPLER(sampler_DitherPattern);
        TEXTURE2D(_EmissionMap);    SAMPLER(sampler_EmissionMap);
        // Set globally by TintTextureGenerator
        TEXTURE2D(_TintMap);        SAMPLER(sampler_TintMap);
        // Set globally by RenderTextureCamera
        int _UnevenResolution;

        CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            half4 _DarknessColor;
            half4 _EmissionColor;
            half _EdgeLightBoost;
            half _TintMapIntensity;
        CBUFFER_END

        float2 SpriteUV(float2 uv)
        {
            //nudge uv a little bit if the target resolution width is not an even number, fixes pixel perfectness bugs
            if (_UnevenResolution == 1) uv += 1.0 / 1024.0;
            return uv;
        }

        // Albedo after the tint map overlay blend
        half4 TintedAlbedo(float2 uv, half4 vertexColor, float2 positionWS)
        {
            half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * vertexColor;
            half4 tintmap = SAMPLE_TEXTURE2D(_TintMap, sampler_TintMap, positionWS / 256 + .5);
            half albedoValue = (c.r + c.g + c.b) / 3;

            half3 tinted;
            if (albedoValue < .5) tinted = 2 * c.rgb * tintmap.rgb;
            else tinted = 1 - 2 * (1 - c.rgb) * (1 - tintmap.rgb);
            c.rgb = saturate(tinted) * _TintMapIntensity + c.rgb * (1 - _TintMapIntensity);
            return c;
        }

        half3 Emission(float2 uv)
        {
            half4 e = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv);
            return e.rgb * e.a * 2 * _EmissionColor.rgb;
        }
        ENDHLSL

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex LitVertex
            #pragma fragment LitFragment
            #pragma multi_compile_instancing

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4 color       : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 screenUV   : TEXCOORD1;
                float2 positionWS : TEXCOORD2;
                half4 color       : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings LitVertex(Attributes input)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                o.positionCS = TransformObjectToHClip(input.positionOS);
                o.positionWS = TransformObjectToWorld(input.positionOS).xy;
                o.screenUV = ComputeScreenPos(o.positionCS / o.positionCS.w).xy;
                o.uv = input.uv;
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 LitFragment(Varyings input) : SV_Target
            {
                float2 uv = SpriteUV(input.uv);
                half4 c = TintedAlbedo(uv, input.color, input.positionWS);
                if (c.a == 0.0)
                    discard;

                // Blend style 0 carries the dynamic point lights, blend style 1 the global ambient light
                half3 light = 0;
                half3 ambient = 0;
            #if USE_SHAPE_LIGHT_TYPE_0
                light = SAMPLE_TEXTURE2D(_ShapeLightTexture0, sampler_ShapeLightTexture0, input.screenUV).rgb;
            #endif
            #if USE_SHAPE_LIGHT_TYPE_1
                ambient = SAMPLE_TEXTURE2D(_ShapeLightTexture1, sampler_ShapeLightTexture1, input.screenUV).rgb;
            #endif

                // Quantize the light to a few steps, using the dither pattern to pick between neighbouring steps
                float ditherPattern = SAMPLE_TEXTURE2D(_DitherPattern, sampler_DitherPattern, input.screenUV * _ScreenParams.xy / 8 + _WorldSpaceCameraPos.xy * -1).r;
                int res = 4;

                light *= 1.5;
                light = clamp(light, 0, 2);
                float vall = (light.r + light.g + light.b) / 3;

                float clampedLight = floor(vall * res) / res;
                float nextLight = ceil(vall * res) / res;
                float fraction = frac(vall * res);
                float stepper = step(ditherPattern, fraction);
                light *= clampedLight * (1 - stepper) + nextLight * stepper;

                half3 color = c.rgb * (ambient + light) + Emission(uv);
                return half4(color, c.a);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering" }

            HLSLPROGRAM
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normal     : NORMAL;
                float4 tangent    : TANGENT;
                half4 color       : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half3 normalWS     : TEXCOORD1;
                half3 tangentWS    : TEXCOORD2;
                half3 bitangentWS  : TEXCOORD3;
                half4 color        : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings NormalsRenderingVertex(Attributes input)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                o.positionCS = TransformObjectToHClip(input.positionOS);
                o.uv = input.uv;
                o.normalWS = TransformObjectToWorldDir(input.normal);
                o.tangentWS = TransformObjectToWorldDir(input.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * input.tangent.w;
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 NormalsRenderingFragment(Varyings input) : SV_Target
            {
                SetUpSpriteInstanceProperties();
                float2 uv = SpriteUV(input.uv);
                half4 mainTex = input.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                //normal format is somewhat custom, uses all three channels
                half3 normalTS = normalize(SAMPLE_TEXTURE2D(_MormalMap, sampler_MormalMap, uv).xyz * 2 - 1);
                return NormalsRenderingShared(mainTex, normalTS, input.tangentWS, input.bitangentWS, input.normalWS);
            }
            ENDHLSL
        }

        Pass
        {
            // Unlit fallback for cameras that do not use the 2D Renderer
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4 color       : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float2 positionWS : TEXCOORD1;
                half4 color       : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings UnlitVertex(Attributes input)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                o.positionCS = TransformObjectToHClip(input.positionOS);
                o.positionWS = TransformObjectToWorld(input.positionOS).xy;
                o.uv = input.uv;
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings input) : SV_Target
            {
                float2 uv = SpriteUV(input.uv);
                half4 c = TintedAlbedo(uv, input.color, input.positionWS);
                return half4(c.rgb + Emission(uv), c.a);
            }
            ENDHLSL
        }
    }
}
