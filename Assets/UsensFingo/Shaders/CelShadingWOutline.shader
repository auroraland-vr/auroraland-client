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

Shader "Fingo/CelShadingWOutline"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[KeywordEnum(Off, On)] _Outline ("Outline?", Float) = 0 
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1		// outline thickness for fully lit areas
		_UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) = 0.4	// outline thickness for unlit areas
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }

		Pass
		{
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _OUTLINE_OFF _OUTLINE_ON
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};

			fixed4 _Color;
			fixed4 _OutlineColor;
			float _LitOutlineThickness;
			float _UnlitOutlineThickness;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				return o;
			}
		
			fixed4 frag (v2f i) : COLOR
			{
				float3 normalDirection = normalize(i.normalDir);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz); //directional light
				float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);

				// Calculate a ratio factor based on intensity (alternatively, you could read 
				// the desired values from a preset ramp texture).
				float intensity = max(0.0, dot(normalDirection, lightDirection));
				float factor = max(ceil(intensity - 0.95), min(0.6, floor(intensity / 0.25) * 0.2));
				
				// If a fragment is determined to be close enough to a silhouette, it is set to the color of the outline.
				// The thickness of the outline is an interpolation between _UnlitOutlineThickness (unlit) and _LitOutlineThickness (fully lit).
#ifdef _OUTLINE_ON
				if (dot(viewDirection, normalDirection) 
					< lerp(_UnlitOutlineThickness, _LitOutlineThickness, intensity)) {
					return _OutlineColor;
				}			
				else {
					return fixed4(_Color.r * factor, _Color.g * factor, _Color.b * factor, _Color.a);
				}
#else
				return fixed4(_Color.r * factor, _Color.g * factor, _Color.b * factor, _Color.a);		
#endif
			}
			ENDCG
		}
	}

	FallBack "Diffuse"
}