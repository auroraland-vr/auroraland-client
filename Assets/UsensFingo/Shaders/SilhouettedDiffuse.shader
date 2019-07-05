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

Shader "Fingo/SilhouettedDiffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline Width", Range (0.0, 1.0)) = 0.01
		_Metallic("Metallic", Range(0,1)) = 0
		_Smoothness("Smoothness", Range(0,1)) = 0.5
		_MainTex ("Base (RGB)", 2D) = "white" { }
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		// Pass to render outline
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

			float _Outline;
			float4 _OutlineColor;

			v2f vert(appdata v) {
				v2f o;

				// Scale vertex along normal direction in clip space
				// so the outline width does not change as the object moves closer/further
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(norm.xy);
				o.pos.xy += offset * o.pos.z * _Outline;
				
				// Scale vertex along normal direction in object space
				// so the outline width changes as the object moves closer/further
//				float4 posObject = v.vertex; 
//				posObject.xyz += v.normal * _Outline;
//				o.pos = UnityObjectToClipPos(posObject);

				o.color = _OutlineColor;
				return o;
			}

			half4 frag(v2f i) :COLOR {
				return i.color;
			}
			ENDCG
		}

		// Pass to render model
		Name "BASE"
		
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Standard

		fixed4 _Color;
		sampler2D _MainTex;
		float _Metallic;
		float _Smoothness;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout  SurfaceOutputStandard o) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a * _Color.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
		}
		ENDCG
	}

	Fallback "Diffuse"
}