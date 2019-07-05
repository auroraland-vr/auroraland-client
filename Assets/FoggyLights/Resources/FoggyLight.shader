// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FoggyLight"
{
	Properties
		{
			[HideInInspector] _SrcBlend ("__src", Float) = 1.0
			[HideInInspector] _DstBlend ("__dst", Float) = 1.0
		}

    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
 
		Lighting Off 
		ZWrite Off 
		ZTest Always
		Fog { Mode Off }

       Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            
			CGINCLUDE      
            #pragma vertex PointLightVert
            #pragma fragment PointLight
            #pragma target 3.0		
			#pragma multi_compile _ _FOG_CONTAINER  	
			#pragma multi_compile _ _ADDITIVE  
			#pragma multi_compile _ TONEMAP
            sampler2D _CameraDepthTexture;
            #include "UnityCG.cginc"
	
  
            float4 
				PointLightPosition,
				PointLightColor;
            float 
				PointLightExponent,  
				Offset, 
				_Visibility,
				IntersectionRange,
				PointLightIntensity;

			ENDCG

			CGPROGRAM     
			struct PointLightv2f
            {
                float4 pos         : SV_POSITION;
                float3 Wpos        : TEXCOORD0;                
                float3 ViewPos     : TEXCOORD1;
				float4 ScreenUVs   : TEXCOORD2;
				float depth : DEPTH;
            };
            void PointLightVert (appdata_full v, out PointLightv2f o)
            {
               
				UNITY_INITIALIZE_OUTPUT(PointLightv2f, o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.Wpos.xyz = mul((float4x4)unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                float4 ScreenPos = ComputeScreenPos(o.pos);
				o.ScreenUVs.xy = ScreenPos.xy / ScreenPos.w;
				o.ScreenUVs.w = ScreenPos.w;
                o.ViewPos = mul((float4x4)UNITY_MATRIX_MV, float4(v.vertex.xyz, 1)).xyz;  	
				o.depth = COMPUTE_DEPTH_01;
            }

			

            float4 PointLight(PointLightv2f i) : COLOR
            {
                float3  Wpos = i.Wpos, q;
                float PointInscattering = 0;
                float  c, s, b;
                float3 dir = (Wpos - _WorldSpaceCameraPos);
                float l = length(dir);
                dir /=l;
                q = _WorldSpaceCameraPos - PointLightPosition.rgb ;				
                b = dot(dir, q );
                c = dot(q , q );
				
                // evaluate integral
				s = 1.0f / sqrt(c - b *b );
                PointInscattering = min(max(0, s * (atan( (l + b ) * s ) - atan( b *s ))), 100);
		
				PointInscattering *= PointLightIntensity * 0.5;
				#ifdef TONEMAP
				PointInscattering /=(1+PointInscattering );//reinhard style
				//PointInscattering = 1-exp2(-PointInscattering );//filmic style
				#endif
                PointInscattering = pow(PointInscattering , PointLightExponent );
                	
				float2 ScreenUVs = i.ScreenUVs.xy;
                //float Depth =  length(DECODE_EYEDEPTH(tex2D(_CameraDepthTexture, ScreenUVs))/normalize(i.ViewPos).z);																				
				float Depth= LinearEyeDepth(tex2D(_CameraDepthTexture, ScreenUVs));
				//return Depth;
                //Soft interesection & offset:
				//float depth = i.depth;
				//float SoftEdges = 1-saturate((length(Wpos -_WorldSpaceCameraPos) - Depth) / IntersectionRange);
				//return SoftEdges;
				float InscatteringRange = saturate( (Depth - length(q) ) / IntersectionRange - (Offset- IntersectionRange/40));
				//return InscatteringRange;
                PointInscattering *= InscatteringRange;
				
				#ifdef _FOG_CONTAINER
				half FogVolumeAtten = exp(-i.ScreenUVs.w/_Visibility);
				PointInscattering *= FogVolumeAtten;
				#endif
				//PointLightColor.xyz *= PointLightIntensity * 0.5;
				#ifdef _ADDITIVE
				return float4(PointLightColor.xyz * PointLightColor.a * PointInscattering, 1);
				#else
                return float4(PointLightColor.xyz, PointLightColor.a * min(1, PointInscattering));
				#endif
            }

              ENDCG
        }

	} 
	Fallback off
}