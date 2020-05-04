Shader "Custom/LeafShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_RimColor("Rim Color", Color) = (0.26, 0.19, 0.17, 0.0)
		_RimPower("Rim Power", Range(0.5, 8.0)) = 3.0

		[PerRendererData]_wind_dir ("Wind Direction", Vector) = (0.5,0.05,0.5,0)
        [PerRendererData]_wind_size ("Wind Wave Size", range(5,50)) = 15

        _tree_sway_stutter_influence("Tree Sway Stutter Influence", range(0,1)) = 0.2
        [PerRendererData]_tree_sway_stutter ("Tree Sway Stutter", range(0,10)) = 1.5
        [PerRendererData]_tree_sway_speed ("Tree Sway Speed", range(0,10)) = 1
        _tree_sway_disp ("Tree Sway Displacement", range(0,1)) = 0.3

        _branches_disp ("Branches Displacement", range(0,0.5)) = 0.3

        _leaves_wiggle_disp ("Leaves Wiggle Displacement", float) = 0.07
        _leaves_wiggle_speed ("Leaves Wiggle Speed", float) = 0.01

        _r_influence ("Red Vertex Influence", range(0,1)) = 1
        _b_influence ("Blue Vertex Influence", range(0,1)) = 1

		_Transparency("Transparency", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert vertex:vert addshadow

		sampler2D _MainTex;

		float4 _wind_dir;
		float _wind_size;
		float _tree_sway_speed;
		float _tree_sway_disp;
		float _leaves_wiggle_disp;
		float _leaves_wiggle_speed;
		float _branches_disp;
		float _tree_sway_stutter;
		float _tree_sway_stutter_influence;
		float _r_influence;
		float _b_influence;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;

			float4 screenPos;
			float3 worldPos;
		};

		sampler2D _BumpMap;
		float4 _RimColor;
		float _RimPower;
		fixed4 _Color;
		uniform float4 _Obstacle;
		float3 worldPos;
		half _Transparency;

		void vert (inout appdata_full i) {
			worldPos = mul (unity_ObjectToWorld, i.vertex).xyz;

			//Tree Movement and Wiggle
			i.vertex.x += (cos(_Time.z * _tree_sway_speed + (worldPos.x / _wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.x/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.x * (i.vertex.y / 10) + 
			cos(_Time.w * i.vertex.x * _leaves_wiggle_speed + (worldPos.x / _wind_size)) * _leaves_wiggle_disp * _wind_dir.x * i.color.b * _b_influence;

			i.vertex.z += (cos(_Time.z * _tree_sway_speed + (worldPos.z / _wind_size) + (sin(_Time.z * _tree_sway_stutter * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_stutter_influence) ) + 1)/2 * _tree_sway_disp * _wind_dir.z * (i.vertex.y / 10) + 
			cos(_Time.w * i.vertex.z * _leaves_wiggle_speed + (worldPos.x / _wind_size)) * _leaves_wiggle_disp * _wind_dir.z * i.color.b * _b_influence;

			i.vertex.y += cos(_Time.z * _tree_sway_speed + (worldPos.z/_wind_size)) * _tree_sway_disp * _wind_dir.y * (i.vertex.y / 10);

			//Branches Movement
			i.vertex.y += sin(_Time.w * _tree_sway_speed + _wind_dir.x + (worldPos.z/_wind_size)) * _branches_disp  * i.color.r * _r_influence;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			o.Alpha = c.a;

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
	FallBack "Diffuse"
}
