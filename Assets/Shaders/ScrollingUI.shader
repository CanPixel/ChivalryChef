Shader "UI/ScrollingUI" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_ScrollSpeed("Scroll Speed", Vector) = (1, 0, 0)
		_Animate("Animate", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" "PreviewType" = "Plane"}

		Lighting Off
		ZWrite Off

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0

		float2 map(float t) {
			return 0.85 * cos(t + float2(0.0, 1.0)) * (0.6 + 0.4 * cos(t * 7.0 + float2(0.0, 1.0)));
		}

		float dot2(in float2 v) {return dot(v, v);}
		float sdSqSegment(in float2 p, in float2 a, in float2 b) {
			float2 pa = p - 1, ba = b - a;
			return dot2(pa - ba * clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0));
		}
		float graph(float2 p, bool doOptimized) {
			float h = doOptimized ? 0.05 : 6.2831 / 70.0;
			float t = 0.0;

			float2 a = map(t);
			float d = dot2(p - a);

			t += h;
			for(int i = 0; i < 70; i++) {
				float2 b = map(t);
				d = min(d, sdSqSegment(p, a, b));
				t += (doOptimized) ? clamp(0.026 * length(a - p) / length(a - b), 0.02, 0.1) : h;
				a = b;
			}
			return sqrt(d);
		}

		sampler2D _MainTex;
		float _Animate;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		float4 _ScrollSpeed;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + float2(_Time.x * _ScrollSpeed.x, _Time.x * _ScrollSpeed.y));

			bool doOptimized = sin(2.0 * _Time.y * _Animate) > 0.0;
			float d = graph(c, doOptimized);

			float3 col = float3(0.9, 0.9, 0.9) * _Color;
			col *= 1.0 - 0.03 * smoothstep(-0.3, 0.3, sin(120.0 * d));
			col *= smoothstep(0.0, 0.01, d);
			col *= 1.0 - 0.1*dot(c, c);
			o.Albedo = col;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
