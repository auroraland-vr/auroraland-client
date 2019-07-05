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

Shader "Fingo/CelShading"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
			};

			uniform fixed4 _Color;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				float3 normalDirection = normalize(i.normalDir);
				float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz); //directional light

				// Calculate a ratio factor based on intensity (alternatively, you could read 
				// the desired values from a preset ramp texture).
				float intensity = max(0.0, dot(normalDirection, lightDirection));
				float factor = max(ceil(intensity - 0.95), min(0.6, floor(intensity / 0.25) * 0.2));

				return fixed4(_Color.r * factor, _Color.g * factor, _Color.b * factor, _Color.a);
			}
			ENDCG
		}
	}

	FallBack "Diffuse"
}