Shader "Custom/MapDrawn"
{
    Properties{
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        _Hatch0("Hatch 0", 2D) = "white" {}
        _Hatch1("Hatch 1", 2D) = "white" {}

        [HDR] _Color("Tint", Color) = (1, 1, 1, 1)
		_AlphaMap("Alpha Map (Greyscale)", 2D) = "white" {}
    
        _Blend("FX Blend", range(0, 1)) = 0.5

        _BrightnessFactor ("Brightness Factor", float) = 1.0
        _Intensity("Intensity", float) = 0

        [Toggle] _Dither("Dither", Float) = 0
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}

        _DitherFade("Dither Fade", Float) = 0

        _Scroll("Scroll", Vector) = (0, 0, 0, 0)
    }

    SubShader {
        Tags { "RenderType"="Opaque"  "LightMode" = "Always" "PassFlags" = "OnlyDirectional"}

        Pass {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex, _AlphaMap, _DitherPattern;
            float4 _MainTex_ST;
		    half4 _Color;
            float _BrightnessFactor;
            float _Intensity;

            sampler2D _Hatch0, _Hatch1;
            float4 _LightColor0;
            float2 _MainTex_TexelSize;
            float _Blend;
            float _Dither;
            float _DitherFade;
            float3 _Scroll;

            fixed4 SampleSpriteTexture (float2 uv) {
                fixed4 col = tex2D (_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                col.a = tex2D (_AlphaTex, uv).r;
                #endif 

                return col;
            }

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				half4 color : COLOR0;
                half3 normal : TEXCOORD1;
            };

            struct v2f {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
				half4 tint : COLOR0;
                half3 normal : NORMAL;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata v){
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                o.tint = v.color * _Color;
                o.normal = mul(float4(v.normal, 0.0), unity_WorldToObject).xyz;
                o.screenPos = ComputeScreenPos(o.position);
                return o;
            }

            fixed3 Hatching(float2 _uv, half _intensity) {
				half3 hatch0 = tex2D(_Hatch0, _uv).rgb;
				half3 hatch1 = tex2D(_Hatch1, _uv).rgb;

				half3 overbright = max(0, _intensity - 1.0);

				half3 weightsA = saturate((_intensity * 6.0) + half3(-0, -1, -2));
				half3 weightsB = saturate((_intensity * 6.0) + half3(-3, -4, -5));

				weightsA.xy -= weightsA.yz;
				weightsA.z -= weightsB.x;
				weightsB.xy -= weightsB.zy;

				hatch0 = hatch0 * weightsA;
				hatch1 = hatch1 * weightsB;

				half3 hatching = overbright + hatch0.r +
					hatch0.g + hatch0.b +
					hatch1.r + hatch1.g +
					hatch1.b;
				return hatching;
			}

            fixed4 frag(v2f i) : SV_TARGET {
                float3 scroll = _Scroll * _Time.y;
                float4 color = tex2D(_MainTex, i.uv);
                float aValue = tex2D(_AlphaMap, i.uv).r;

                half3 diffuse = color.rgb * dot(normalize(mul(unity_WorldToObject, _WorldSpaceLightPos0)), normalize(i.normal));
                half intensity = dot(diffuse, half3(0.2326, 0.7152, 0.0722));

                color.rgb = Hatching(i.uv * 4, intensity);
                color.a *= tex2D(_AlphaMap, i.uv).r;

                float4 fin = color* i.tint;
                fin.a *= aValue;

                fixed4 final = lerp(fin, tex2D(_MainTex, i.uv) * _Color, _Blend);
                final.a *= aValue;

                fixed lum = saturate(Luminance(final.rgb) * _BrightnessFactor);
                fixed4 output;
                output.rgb = lerp(final.rgb, fixed3(lum, lum, lum), _Intensity) * i.tint;
                output.a = aValue * aValue;

                float4 fina = output;
                if(_Dither) {
                    float2 screenPos = i.screenPos.xy / i.screenPos.w;
                    float2 ditherCoord = screenPos * _ScreenParams.xy * _MainTex_TexelSize.xy * 4;
                    float ditherValue = tex2D(_DitherPattern, ditherCoord + scroll).r;
                    float ditheredValue = step(ditherValue, output);
                   fina *= (1.0 - ditheredValue * (_DitherFade - tex2D(_AlphaMap, i.uv).r));
                }
                return fina;
            }
            ENDCG
        }
    }
}