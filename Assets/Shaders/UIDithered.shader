Shader "UI/Dithered"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _DitherPattern("Dither pattern", 2D) = "gray"{}
        _DitherColorCount("Dither color count", Int) = 12
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

            Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            int _DitherColorCount;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _DitherPattern;

            fixed4 frag(v2f IN) : SV_Target
            {
                IN.screenPos.xy /= IN.screenPos.w;

                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                int res = _DitherColorCount;

                
                fixed ditherPattern = tex2D(_DitherPattern, (IN.screenPos.xy) * _ScreenParams.xy / 8).r; //repeating dither treshold texture in screen space
                
                float3 clamped = floor(color.rgb * res) / res; // "previous" dither step
                float3 next = ceil(color.rgb * res) / res; // "next" dither step
                float3 lerp = frac(color.rgb * res);
                float3 stepper = step(ditherPattern, lerp);

                color.rgb = clamped * (1 - stepper) + next * (stepper);

                color.a = step(ditherPattern, (color.a * 1.01 - .005));

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}

