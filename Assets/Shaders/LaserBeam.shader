Shader "Unlit/LaserBeam"
{
    Properties
    {
        _Color("Middle color", Color) = (1,1,1,1)
        _EdgeColor("Edge color", Color) = (1,0,0,1)
        _Noise("Noise texture", 2D) = "gray"{}
        _NoiseIntensity("Noise intensity", Float) = 1
        _ScrollSpeed("Scroll speed", Float) = 1
        _StartBoost("Start boost", Float) = .5
        _EndBoost("End boost", Float) = .5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend One OneMinusSrcAlpha
        ZWrite Off

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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Noise;
            float4 _Noise_ST;

            fixed4 _Color;
            fixed4 _EdgeColor;
            half _NoiseIntensity;
            half _ScrollSpeed;
            half _StartBoost;
            half _EndBoost;

            half _LineLength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                //perlin noise texture
                fixed noise = tex2D(_Noise, (i.worldPos.xy + _Noise_ST.zw + _Time.x * _ScrollSpeed) * _Noise_ST.xy).r;
                noise = noise * 2 - 1;
                //distort uv coordinate with noise
                i.uv.y += noise.r * _NoiseIntensity;

                //fill middle and edges with appropriate textures, add a bit of random animation to both
                half centerness = 1 - abs(i.uv.y - .5) * 2;
                float middle = step(.85, centerness);
                middle *= 0 + abs(sin(15 + i.uv.x * 15 + _Time.x *235));
                float edge = step(.5, centerness) - middle;
                edge *= 1 + abs(sin(i.uv.x * 15 + _Time.x * 135));
                
                //highlight beam start and end
                fixed4 col = _Color * middle + _EdgeColor * (edge);
                col.rgb += _StartBoost * smoothstep(1, 0, (i.uv.x * _LineLength));
                col.rgb += _EndBoost * smoothstep(_LineLength - 1, _LineLength, (i.uv.x * _LineLength));

                col *= i.color;

                col.rgb *= col.a;
                col.rgb *= 2;
            
                col.a *= .5;
                
                return col;
            }
            ENDCG
        }
    }
}
