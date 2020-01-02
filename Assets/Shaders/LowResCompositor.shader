// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/LowResCompositor"
{
    Properties
    {
        _LowResTexture ("Texture", 2D) = "white" {}
        _UITexture("UI Texture", 2D) = "clear" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _LowResTexture;
            sampler2D _UITexture;

            fixed4 frag (v2f i) : SV_Target
            {
                //calculate proper uv coordinates of the pixel perfect low res texture & add ui texture on top
                float aspect = _ScreenParams.x / _ScreenParams.y;
                i.uv.y = ((i.uv.y * 2.0 - 1.0) / aspect) / 2.0 + .5;

                fixed4 col = tex2D(_LowResTexture, i.uv);
                fixed4 ui = tex2D(_UITexture, i.uv);
                
                return fixed4(col.rgb * (1-ui.a) + ui.rgb * ui.a, 1);
            }
            ENDCG
        }
    }
}
