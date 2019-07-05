// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/*************************************************************************\
*                           USENS CONFIDENTIAL                            *
* _______________________________________________________________________ *
*                                                                         *
* [2014] - [2017] USENS Incorporated                                      *
* All Rights Reserved.                                                    *
*                                                                         *
* NOTICE:  All information contained herein is, and remains               *
* the property of uSens Incorporated and its suppliers,                   *
* if any.  The intellectual and technical concepts contained              *
* herein are proprietary to uSens Incorporated                            *
* and its suppliers and may be covered by U.S. and Foreign Patents,       *
* patents in process, and are protected by trade secret or copyright law. *
* Dissemination of this information or reproduction of this material      *
* is strictly forbidden unless prior written permission is obtained       *
* from uSens Incorporated.                                                *
*                                                                         *
\*************************************************************************/

Shader "Fingo/TransparentFresnel"
{
	Properties
	{
		_Color ("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_RimColor ("Rim Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_FPOW ("FPOW Fresnel", Float) = 5.0
		_R0 ("R0 Fresnel", Float) = 0.05
		_MainTex ("Base (RGB)", 2D) = "White" { }
	}

	Category
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha One
		ZWrite Off

		SubShader
		{
			Pass
			{
				Name "MAIN"

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
				fixed4 _Color;
				fixed4 _RimColor;
				float _FPOW;
				float _R0;
				sampler2D _MainTex;
				float4 _MainTex_ST;

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float fresnel : TEXCOORD1;
				};

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

					float3 normalWorld = UnityObjectToWorldNormal(v.normal);
					float3 posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
					float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - posWorld);

					// Compute the fresnel term
					half fresnel = saturate(1.0 - dot(normalWorld, viewDir));
					fresnel = pow(fresnel, _FPOW);
					o.fresnel = _R0 + (1.0 - _R0) * fresnel;
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
					fixed4 innerColor = i.fresnel * _Color * tex2D(_MainTex, i.texcoord);
					return 2.0f * innerColor * _RimColor;
				}
				ENDCG
			}
		}
		Fallback "Diffuse"
	}
}