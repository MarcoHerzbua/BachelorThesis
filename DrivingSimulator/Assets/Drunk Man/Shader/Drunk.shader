Shader "Drunk Man/Drunk" {
	Properties {
		_MainTex          ("Main", 2D) = "white" {}
		_RGBShiftFactor   ("RGB Shift Factor", Float) = 0
		_RGBShiftPower    ("RGB Shift Power", Float) = 3
		_GhostSeeRadius   ("Ghost See Radius", Float) = 0.01
		_GhostSeeMix      ("Ghost See Mix", Float) = 0.5
		_GhostSeeAmplitude("Ghost See Amplitude", Float) = 0.05
		_Dimensions       ("Dark Dimensions", Vector) = (0.5, 0.5, 0, 0)
		_Frequency        ("Refraction Frequency", Float) = 10
		_Period           ("Refraction Period", Float) = 1.5
		_RandomNumber     ("Refraction Random", Float) = 1
		_Amplitude        ("Refraction Amplitude", Float) = 40
	}
	SubShader {
		ZTest Off Cull Off ZWrite Off Blend Off
	  	Fog { Mode off }
		Pass {   // pass 0
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float _RGBShiftFactor, _RGBShiftPower;
			
			float4 frag (v2f_img i) : SV_TARGET
			{
				float2 dist = float2(0.5, 0.5) - i.uv;
				float2 unit = dist / length(dist);

				float ol = length(dist) * _RGBShiftFactor;
				ol = 1.0 - pow(1.0 - ol, _RGBShiftPower);
				float2 offset = unit * ol;
 			
				float4 cr  = tex2D(_MainTex, i.uv + offset);
				float4 cga = tex2D(_MainTex, i.uv);
				float4 cb  = tex2D(_MainTex, i.uv - offset);
				return float4(cr.r, cga.g, cb.b, cga.a);
			}
			ENDCG
		}
		Pass {   // pass 1
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float _GhostSeeRadius, _GhostSeeMix;
			
			float4 frag (v2f_img i) : SV_TARGET
			{
				float angle = _Time.y;
				float2 offset = float2(cos(angle), sin(angle * 2.0)) * _GhostSeeRadius;
				float4 original = tex2D(_MainTex, i.uv);
				float4 shifted = tex2D(_MainTex, i.uv + offset);
				return lerp(original, shifted, _GhostSeeMix);
			}
			ENDCG
		}
		Pass {   // pass 2
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float _GhostSeeAmplitude;

			float4 frag (v2f_img i) : SV_TARGET
			{
				float sq = sin(_Time.y) * _GhostSeeAmplitude;
				float4 tc = tex2D(_MainTex, i.uv);
				float4 tl = tex2D(_MainTex, i.uv - float2(sin(sq), 0));
				float4 tR = tex2D(_MainTex, i.uv + float2(sin(sq), 0));
				float4 tD = tex2D(_MainTex, i.uv - float2(0, sin(sq)));
				float4 tU = tex2D(_MainTex, i.uv + float2(0, sin(sq)));
				return (tc + tl + tR + tD + tU) / 5;
			}
			ENDCG
		}
		Pass {   // pass 3
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float2 _Dimensions;

			float4 frag (v2f_img i) : SV_TARGET
			{
				float2 gradient = 0.5 - i.uv;
				gradient.x = gradient.x * (1 / _Dimensions.x);
				gradient.y = gradient.y * (1 / _Dimensions.y);
				float dist = length(gradient);
				float4 tc = tex2D(_MainTex, i.uv);
				return lerp(tc, float4(0, 0, 0, 1), dist);
			}
			ENDCG
		}
		Pass {   // pass 4
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Utils.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _Frequency, _Period, _RandomNumber, _Amplitude;

			float4 frag (v2f_img i) : SV_TARGET
			{
				const float PI = 3.141592;
				float n = snoise(float3((i.uv * _Frequency), _RandomNumber * _Period + _Time.y)) * PI;
				float2 offset = float2(cos(n), sin(n)) * _Amplitude * _MainTex_TexelSize.xy;
				return tex2D(_MainTex, i.uv + offset);
			}
			ENDCG
		}
		Pass {   // pass 5
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _BlurMin, _BlurMax, _BlurSpeed;

			float4 frag (v2f_img i) : SV_TARGET
			{
				float4 c = 0;
				float t = lerp(_BlurMin, _BlurMax, (sin(_Time.y * _BlurSpeed) + 1) / 2);
				for (int n = 0; n <= 25; n++)
				{
					float q = n / 25.0;
					c += tex2D(_MainTex, i.uv + (0.5 - i.uv) * q * t) / 25.0;
				}
				return c;
			}
			ENDCG
		}
	}
	FallBack Off
}