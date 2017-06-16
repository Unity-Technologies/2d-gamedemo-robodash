Shader "Custom/NormalMappedSprite" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MormalMap("Mormal map", 2D) = "white" {}
		_DitherPattern("Dither pattern", 2D) = "gray"{}
		_EmissionMap("Emission map", 2D) = "black"{}
		_DarknessColor("Darkness color", Color) = (0,0,0,1)
		_EdgeLightBoost("Edge light boost", Range(0, 1)) = 1
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		_EmissionColor("Emission tint", Color) = (1,1,1,1)
		_TintMapIntensity("Tint map intensity", Range(0, 2)) = 1
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Cull Off
		
			

		CGPROGRAM
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#pragma surface surf Custom alpha:fade vertex:vert nofog nolightmap noinstancing nodynlightmap
		#pragma target 3.0
		#include "UnityPBSLighting.cginc"
		half _EdgeLightBoost;

		inline half4 LightingCustom(SurfaceOutputStandard s, half3 lightDir, UnityGI gi)
		{
			float ditherPattern = s.Smoothness;
			int res = 4;

			gi.light.color.rgb *= 1.5;
			gi.light.color.rgb = clamp(gi.light.color.rgb,0, 2);
			float vall = gi.light.color.r + gi.light.color.g + gi.light.color.b;
			vall /= 3;

			float clampedLight = floor(vall * res) / res;
			float nextLight = ceil(vall * res) / res;
			float lerp = frac(vall * res);
			float stepper = step(ditherPattern, lerp);
			gi.light.color *= clampedLight * (1 - stepper) + nextLight * stepper;
			s.Smoothness = 0;
			s.Metallic = 0;
			half4 standard = LightingStandard(s, lightDir, gi);
			return standard;
		}
		inline void LightingCustom_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);
		}

		sampler2D _MainTex;
		
		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
			fixed4 color;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _MormalMap;
		half _NormalIntensity;
		sampler2D _DitherPattern;
		sampler2D _EmissionMap;
		fixed4 _DarknessColor;
		fixed4 _RendererColor;
		fixed4 _EmissionColor;
		sampler2D _TintMap;
		half _TintMapIntensity;
		half4 _Flip;
		int _UnevenResolution;
		void vert(inout appdata_full v, out Input o)
		{
			v.vertex.xy *= _Flip.xy;
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color * _RendererColor;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			//nudge uv a little bit if the target resolution width is not an even number, fixes pixel perfectness bugs
			if (_UnevenResolution == 1) IN.uv_MainTex.xy += 1.0 / 1024.0;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = c.rgb;
			fixed4 n = tex2D(_MormalMap, IN.uv_MainTex);
			
			//normal format is somewhat custom, uses all three channels
			fixed3 normal;
			normal.xyz = n.xyz * 2 - 1;
			o.Normal = normal; // normalize(normal);

			o.Alpha = c.a;
			o.Metallic = 0;
			//smoothness channel used to carry dither treshold map information
			o.Smoothness = tex2D(_DitherPattern, IN.screenPos.xy * _ScreenParams.xy / 8 + _WorldSpaceCameraPos.xy * -1).r;
			

			float4 e = tex2D(_EmissionMap, IN.uv_MainTex);
			o.Emission = e.rgb * e.a * 2 * _EmissionColor.rgb;
			fixed4 tintmap = tex2D(_TintMap, (IN.worldPos.xy / 256)  + .5);
			float albedoValue = (c.r + c.g + c.b) / 3;
			
			float3 tinted;
			if (albedoValue < .5) tinted = 2 * o.Albedo * tintmap.rgb;
			else tinted = 1 - 2 * (1 - o.Albedo) * (1 - tintmap.rgb);
			o.Albedo = saturate(tinted) * _TintMapIntensity + o.Albedo * (1 - _TintMapIntensity);
		}

		ENDCG
	}
	FallBack "Diffuse"
}
