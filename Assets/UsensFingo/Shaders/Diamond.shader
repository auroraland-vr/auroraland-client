// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

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

Shader "Fingo/Diamond"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_ReflectTex ("Reflection Texture", Cube) = ""
		_RefractTex ("Refraction Texture", Cube) = ""
	}
  
	SubShader
	{
		Tags { "Queue" = "Transparent" }
   
		// First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
		// convex objects, this is effectively rendering the inside of them.
		Pass
		{
			Name "BACKFACES"

			Cull Front
			ZWrite Off
         
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
         
			float4 _Color;
			samplerCUBE _ReflectTex;
			samplerCUBE _RefractTex;
         
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
			};
         
			v2f vert (float4 v : POSITION, float3 n : NORMAL)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v);
               
				// TexGen CubeReflect: Reflect view direction along the normal, in view space
				float3 viewDir = normalize(ObjSpaceViewDir(v));
				o.uv = reflect(-viewDir, n);
				o.uv = mul(UNITY_MATRIX_MV, float4(o.uv, 0));
				return o;
			}
         
			half4 frag (v2f i) : SV_Target
			{
				float4 col = texCUBE(_RefractTex, i.uv) * _Color;               
				return col;
			}
			ENDCG
		}
		   
		// Second pass - here we render the front faces of the diamonds.
		Pass
		{
			Name "FRONTFACES"

			Cull Back
			ZWrite On
			Blend One One
         
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
         
			float4 _Color;
			samplerCUBE _ReflectTex;
			samplerCUBE _RefractTex;
         
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
				half fresnel : TEXCOORD1;
			};
         
			v2f vert (float4 v : POSITION, float3 n : NORMAL)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v);
               
				// TexGen CubeReflect: Reflect view direction along the normal, in view space
				float3 viewDir = normalize(ObjSpaceViewDir(v));
				o.uv = reflect(-viewDir, n);
				o.uv = mul(UNITY_MATRIX_MV, float4(o.uv, 0));
				o.fresnel = 1.0 - saturate(dot(n,viewDir));
				return o;
			}
         
			half4 frag (v2f i) : SV_Target
			{
				float4 col = texCUBE(_RefractTex, i.uv) * _Color + texCUBE(_ReflectTex, i.uv) * i.fresnel;               
				return col;
			}
			ENDCG
		}
	}

	FallBack "Diffuse"
}