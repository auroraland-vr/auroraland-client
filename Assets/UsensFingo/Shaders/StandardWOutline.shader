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

Shader "Fingo/StandardWOutline"
{
	Properties
	{
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_Metallic("Metallic", Range(0,1)) = 0
		_Smoothness("Smoothness", Range(0,1)) = 0.5
		_MainTex ("Base (RGB)", 2D) = "white" { }

		// Relevant to outline:
		[KeywordEnum(Off, On)] _Outline ("Outline?", Float) = 0 
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1		// outline thickness for fully lit areas
		_UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) = 0.4	// outline thickness for unlit areas
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
		
		ZWrite On

		CGPROGRAM
		#pragma multi_compile _OUTLINE_OFF _OUTLINE_ON
		#pragma surface surf Standard finalcolor:finalColor
		#include "UnityCG.cginc"

		fixed4 _Color;
		float _Metallic;
		float _Smoothness;
		sampler2D _MainTex;
		// Relevant to outline:
		fixed4 _OutlineColor;
		float _LitOutlineThickness;
		float _UnlitOutlineThickness;
		
		struct Input {
			float3 worldNormal;
			float3 viewDir;
			float2 uv_MainTex;
		};

		void finalColor (Input IN, SurfaceOutputStandard o, inout fixed4 color)
		{
#ifdef _OUTLINE_ON
			float3 normalDirection = IN.worldNormal;
			float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz); //directional light
			float3 viewDirection = IN.viewDir;
	
			// If a fragment is determined to be close enough to a silhouette, it is set to the color of the outline.
			// The thickness of the outline is an interpolation between _UnlitOutlineThickness (unlit) and _LitOutlineThickness (fully lit).
			if (dot(viewDirection, normalDirection) 
				< lerp(_UnlitOutlineThickness, _LitOutlineThickness, max(0.0, dot(normalDirection, lightDirection)))) {
				color = _OutlineColor;
			}
#endif
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a * _Color.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
		}
		ENDCG
	}

	Fallback "Diffuse"
}