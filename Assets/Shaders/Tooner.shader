Shader "Custom/Tooner" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		[HDR] _Emission("Emission", Color) = (0, 0, 0, 1)

		_ShadowTint("Shadow Tint", Color) = (0.5, 0.5, 0.5, 1)

		_TextureBlend("Tex Blend", Range(0, 1)) = 0.5
		[HideInInspector] _Transparency("Transparency", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "Queue" = "Geometry"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Ramp fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		half3 _Emission;
		float3 _ShadowTint;
		float _TextureBlend;
		float3 worldPos;
		half _Transparency;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
		};

		fixed4 _Color;

		void vert (inout appdata_full i) {
			worldPos = mul (unity_ObjectToWorld, i.vertex).xyz;
		}

        float4 LightingRamp(SurfaceOutput s, float3 lightDir, half3 viewDir, float shadowAttenuation) {
            //how much does the normal point towards the light?
            float towardsLight = dot(s.Normal, lightDir);
            // make the lighting a hard cut
            float towardsLightChange = fwidth(towardsLight);
            float lightIntensity = smoothstep(0, towardsLightChange, towardsLight);

        #ifdef USING_DIRECTIONAL_LIGHT
            //for directional lights, get a hard vut in the middle of the shadow attenuation
            float attenuationChange = fwidth(shadowAttenuation) * 0.5;
            float shadow = smoothstep(0.5 - attenuationChange, 0.5 + attenuationChange, shadowAttenuation);
        #else
            //for other light types (point, spot), put the cutoff near black, so the falloff doesn't affect the range
            float attenuationChange = fwidth(shadowAttenuation);
            float shadow = smoothstep(0, attenuationChange, shadowAttenuation);
        #endif
            lightIntensity = lightIntensity * shadow;

            //calculate shadow color and mix light and shadow based on the light. Then taint it based on the light color
            float3 shadowColor = s.Albedo * _ShadowTint;
            float4 color;
            color.rgb = lerp(shadowColor, s.Albedo, lightIntensity) * _LightColor0.rgb;
            color.a = s.Alpha;
            return color;
        }

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Emission = _Emission;
			o.Albedo = lerp(c.rgb, _Color, _TextureBlend);

			const float4x4 thresholdMatrix = {
				1, 9, 3, 11,
				13, 5, 15, 7,
				4, 12, 2, 10,
				16, 8, 14, 6
			};

			float2 pos = IN.screenPos.xy / IN.screenPos.w * _ScreenParams.xy;
			float threshold = thresholdMatrix[pos.x % 4][pos.y % 4] / 17;
			half trans = distance(_WorldSpaceCameraPos, IN.worldPos.xyz) - 0.5;
			_Transparency = trans;
			clip(_Transparency - threshold);
		}
		ENDCG
	}
	FallBack "Standard"
}
