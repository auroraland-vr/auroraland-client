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

Shader "Fingo/FingoRGBImage" { 
	Properties { 
		_Color ("Main Color", Color) = (1, 1, 1, 1) 
		_MainTex ("Main Texture", 2D) = "white" {} 
		_CutTex ("Cutout (A) Texture", 2D) = "white" {} 
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
		_Alpha ("Transparency", Range(0,1)) = 0.3
		_Exponent ("Exponent", Range(0,1)) = 0.55
	}

 
	SubShader { 
		Tags {"Queue"="Background" "IgnoreProjector"="True" "RenderType"="Transparent"} 
		
     	Cull Off Zwrite On Ztest Always fog { mode off }
    	Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM #pragma surface surf Lambert alpha vertex:vert

 
		sampler2D _MainTex; 
		sampler2D _CutTex; 
		fixed4 _Color; 
		float _Cutoff;
		float _Alpha;
		float _Exponent;

 
		struct Input { 
			float2 uv_MainTex; 
			float3 localPos;
			float3 worldPos;
			float4 color : COLOR;
		};
       
     	void vert (inout appdata_full v, out Input o) {

   			UNITY_INITIALIZE_OUTPUT(Input,o);
        	v.texcoord.y = 1 - v.texcoord.y;
		}
    
		void surf (Input IN, inout SurfaceOutput o) { 
			
			fixed4 color = tex2D(_MainTex, IN.uv_MainTex); 	

 			o.Emission = color.bgr;
 			o.Alpha = 1;

		} 
		ENDCG 
		
	}

 
	Fallback "Diffuse" 
} 