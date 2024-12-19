Shader "Key mouse/Fresnel effect"
{
	Properties
	{
		_PatternTex("Pattern", 2D) = "white" {}
		[HDR] _Emission("Color", color) = (0,0,0)
		_FresnelPow("Fresnel Power", Range(0.25, 4)) = 1
		_FresnelVolume("Fresnel Volume",Range(0,0.01)) = 0.001
		_Speed("Speed", Range(0, 2)) = 0.1
	}
		SubShader
		{
			Tags { "Queue" = "Transparent" }
			LOD 100

			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				Name "OUTLINEPASS"

				ZWrite off

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					//float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
					float4 vertex : SV_POSITION;
					float3 viewDir : TEXCOORD2;
				};

				float4 _Emission;
				float _FresnelVolume;
				sampler2D _MainTex;
				float _FresnelPow;
				sampler2D _PatternTex;
				float4 _PatternTex_ST;
				float _Speed;

				v2f vert(appdata IN)
				{
					v2f o;
					float camDist = distance(UnityObjectToWorldDir(IN.vertex), _WorldSpaceCameraPos);
					IN.vertex.xyz += normalize(IN.normal) * camDist * _FresnelVolume;
					o.vertex = UnityObjectToClipPos(IN.vertex);

					//Calculate the normals based in world position
					float3 worldNormal = mul(IN.normal, (float3x3)unity_WorldToObject);

					o.worldPos = normalize(worldNormal);
					o.uv = TRANSFORM_TEX(IN.uv, _PatternTex);

					//Calculate the view direction based on each vertex
					o.viewDir = normalize(WorldSpaceViewDir(IN.vertex));

					return o;
				}

				float4 frag(v2f IN) : SV_TARGET
				{
					fixed4 pattern = tex2D(_PatternTex, IN.uv + _Speed * _Time.y);
				    // dot product between worldNormal and viewDirection
				    float fresnelInfluence = dot(IN.worldPos, IN.viewDir);
				    float saturatedFresnel = saturate(1 - fresnelInfluence);
				    
				    return pow(saturatedFresnel, _FresnelPow) * _Emission * pattern;
				}

				ENDCG
			}

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
						float3 normal: NORMAL;
					};

					struct v2f
					{
						float2 uv : TEXCOORD0;
						float3 worldPos : TEXCOORD1;
						float4 vertex : SV_POSITION;
						float3 viewDir : TEXCOORD2;
					};

					sampler2D _MainTex;
					float _FresnelPow;
					sampler2D _PatternTex;
					float4 _PatternTex_ST;
					float _Speed;
					float4 _TransparentColor = (0, 0, 0, 0);

					v2f vert(appdata v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);

						//Calculate the normals based in world position
						float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);

						o.worldPos = normalize(worldNormal);
						o.uv = TRANSFORM_TEX(v.uv, _PatternTex);

						//Calculate the view direction based on each vertex
						o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 pattern = tex2D(_PatternTex, i.uv + _Speed * _Time.y);
					// dot product between worldNormal and viewDirection
					float fresnelInfluence = dot(i.worldPos, i.viewDir);
					float saturatedFresnel = saturate(1 - fresnelInfluence);

					return pow(saturatedFresnel, _FresnelPow) * _TransparentColor * pattern;
				}
				ENDCG
		}
		}
}