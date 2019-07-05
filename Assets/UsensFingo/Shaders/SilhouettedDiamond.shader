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

Shader "Fingo/SilhouettedDiamond"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineScale ("Outline Scale", Range (1.0, 10.0)) = 1.1
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_ReflectTex ("Reflection Texture", Cube) = ""
		_RefractTex ("Refraction Texture", Cube) = ""
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		
		// Pass to render outline (a scaled-up version)
		Pass
		{
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }

			Cull Front
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : POSITION;
				float4 color : COLOR;
			};
			
			float4 _OutlineColor;
			float _OutlineScale;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex * _OutlineScale);
				o.color = _OutlineColor;
				return o;
			}

			half4 frag(v2f i) : COLOR {
				return i.color;
			}
			ENDCG
		}
   
		// Diamond pass 1 - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
		// convex objects, this is effectively rendering the inside of them.
		UsePass "Fingo/Diamond/BACKFACES"

		// Diamond pass 2 - here we render the front faces of the diamonds.
		UsePass "Fingo/Diamond/FRONTFACES"
      }

      FallBack "Diffuse"
}