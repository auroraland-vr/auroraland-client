// Upgrade NOTE: removed variant '__' where variant LOD_FADE_PERCENTAGE is used.

Shader "Nature/SpeedTree Custom"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _HueVariation ("Hue Variation", Color) = (1.0,0.5,0.0,0.1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _DetailTex ("Detail", 2D) = "black" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.333
        [MaterialEnum(Off,0,Front,1,Back,2)] _Cull ("Cull", Int) = 2
        [MaterialEnum(None,0,Fastest,1,Fast,2,Better,3,Best,4,Palm,5)] _WindQuality ("Wind Quality", Range(0,5)) = 0
    }
 
    // targeting SM3.0+
    SubShader
    {
        Tags
        {
            "Queue"="Geometry"
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
            "DisableBatching"="LODFading"
        }
        LOD 400
        Cull [_Cull]
 
        CGPROGRAM
            #pragma surface surf Lambert vertex:SpeedTreeVertKritoa nodirlightmap nodynlightmap noshadowmask
            #pragma target 3.0
            #pragma multi_compile  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
            #pragma instancing_options assumeuniformscaling lodfade maxcount:50
            #pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
            #pragma shader_feature EFFECT_BUMP
            #pragma shader_feature EFFECT_HUE_VARIATION
            #define ENABLE_WIND
            #include "SpeedTreeCommon.cginc"
 
            void OffsetSpeedTreeVertexKritoa(inout SpeedTreeVB data, float lodValue)
            {
                float3 finalPosition = data.vertex.xyz;
 
                #ifdef ENABLE_WIND
                    half windQuality = _WindQuality * _WindEnabled;
 
                    float3 rotatedWindVector, rotatedBranchAnchor;
                    if (windQuality <= WIND_QUALITY_NONE)
                    {
                        rotatedWindVector = float3(0.0f, 0.0f, 0.0f);
                        rotatedBranchAnchor = float3(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        // compute rotated wind parameters
                        rotatedWindVector = normalize(mul(_ST_WindVector.xyz, (float3x3)unity_ObjectToWorld));
                        rotatedBranchAnchor = normalize(mul(_ST_WindBranchAnchor.xyz, (float3x3)unity_ObjectToWorld)) * _ST_WindBranchAnchor.w;
                    }
                #endif
 
                #if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_FROND)
 
                    // smooth LOD
                    #ifdef LOD_FADE_PERCENTAGE
                        finalPosition = lerp(finalPosition, data.texcoord1.xyz, lodValue);
                    #endif
 
                    // frond wind, if needed
                    #if defined(ENABLE_WIND) && defined(GEOM_TYPE_FROND)
                        if (windQuality == WIND_QUALITY_PALM)
                            finalPosition = RippleFrond(finalPosition, data.normal, data.texcoord.x, data.texcoord.y, data.texcoord2.x, data.texcoord2.y, data.texcoord2.z);
                    #endif
 
                #elif defined(GEOM_TYPE_LEAF)
 
                    // remove anchor position
                    finalPosition -= data.texcoord1.xyz;
 
                    bool isFacingLeaf = data.color.a == 0;
                    if (isFacingLeaf)
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            finalPosition *= lerp(1.0, data.texcoord1.w, lodValue);
                        #endif
 
                        // face camera-facing leaf to camera                
//                      float offsetLen = length(finalPosition);
//                      finalPosition = mul(finalPosition.xyz, (float3x3)UNITY_MATRIX_IT_MV); // inv(MV) * finalPosition    // old camera facing (orients to view matrix, bad!!)
 
                        // KRITOA: can't get this to work - somebody help
 
                        // orient to camera position with up vector instead now
//                      float3 camPos = _WorldSpaceCameraPos.xyz;
//                      float3 objectOrigin = data.texcoord1.xyz;  //mul(unity_ObjectToWorld, float3(0,0,0));
//                      float3 objectForward = normalize(camPos-objectOrigin);
//                      float3 objectRight = normalize(cross(float3(0,1,0),objectForward));
//                      float3 objectUp = normalize(cross(objectForward,objectRight));
//                      finalPosition = mul(float3x3(objectRight, objectUp, objectForward),finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z),
//                                                                      finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z),
//                                                                      finalPosition.xyz);
 
//                      finalPosition = normalize(finalPosition) * offsetLen; // make sure the offset vector is still scaled
                    }
                    else
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            float3 lodPosition = float3(data.texcoord1.w, data.texcoord3.x, data.texcoord3.y);
                            finalPosition = lerp(finalPosition, lodPosition, lodValue);
                        #endif
                    }
 
                    #ifdef ENABLE_WIND
                        // leaf wind
                        if (windQuality > WIND_QUALITY_FASTEST && windQuality < WIND_QUALITY_PALM)
                        {
                            float leafWindTrigOffset = data.texcoord1.x + data.texcoord1.y;
                            finalPosition = LeafWind(windQuality == WIND_QUALITY_BEST, data.texcoord2.w > 0.0, finalPosition, data.normal, data.texcoord2.x, float3(0,0,0), data.texcoord2.y, data.texcoord2.z, leafWindTrigOffset, rotatedWindVector);
                        }
                    #endif
 
                    // move back out to anchor
                    finalPosition += data.texcoord1.xyz;
 
                #endif
 
                #ifdef ENABLE_WIND
                    float3 treePos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
 
                    #ifndef GEOM_TYPE_MESH
                        if (windQuality >= WIND_QUALITY_BETTER)
                        {
                            // branch wind (applies to all 3D geometry)
                            finalPosition = BranchWind(windQuality == WIND_QUALITY_PALM, finalPosition, treePos, float4(data.texcoord.zw, 0, 0), rotatedWindVector, rotatedBranchAnchor);
                        }
                    #endif
 
                    if (windQuality > WIND_QUALITY_NONE)
                    {
                        // global wind
                        finalPosition = GlobalWind(finalPosition, treePos, true, rotatedWindVector, _ST_WindGlobal.x);
                    }
                #endif
 
                data.vertex.xyz = finalPosition;
            }
 
            void SpeedTreeVertKritoa(inout SpeedTreeVB IN, out Input OUT)
            {
                UNITY_INITIALIZE_OUTPUT(Input, OUT);
 
                OUT.mainTexUV = IN.texcoord.xy;
                OUT.color = _Color;
                OUT.color.rgb *= IN.color.r; // ambient occlusion factor
 
                #ifdef EFFECT_HUE_VARIATION
                    float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
                    hueVariationAmount += frac(IN.vertex.x + IN.normal.y + IN.normal.x) * 0.5 - 0.3;
                    OUT.HueVariationAmount = saturate(hueVariationAmount * _HueVariation.a);
                #endif
 
                #ifdef GEOM_TYPE_BRANCH_DETAIL
                    // The two types are always in different sub-range of the mesh so no interpolation (between detail and blend) problem.
                    OUT.Detail.xy = IN.texcoord2.xy;
                    if (IN.color.a == 0) // Blend
                        OUT.Detail.z = IN.texcoord2.z;
                    else // Detail texture
                        OUT.Detail.z = 2.5f; // stay out of Blend's .z range
                #endif
 
                OffsetSpeedTreeVertexKritoa(IN, unity_LODFade.x);
 
                UNITY_TRANSFER_DITHER_CROSSFADE(OUT, IN.vertex)
            }
 
 
            void surf(Input IN, inout SurfaceOutput OUT)
            {
                SpeedTreeFragOut o;
                SpeedTreeFrag(IN, o);
                SPEEDTREE_COPY_FRAG(OUT, o)
            }
        ENDCG
 
        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
                #pragma multi_compile_instancing
                #pragma instancing_options assumeuniformscaling lodfade maxcount:50
                #pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
                #pragma multi_compile_shadowcaster
                #define ENABLE_WIND
                #include "SpeedTreeCommon.cginc"
 
            void OffsetSpeedTreeVertexKritoa(inout SpeedTreeVB data, float lodValue)
            {
                float3 finalPosition = data.vertex.xyz;
 
                #ifdef ENABLE_WIND
                    half windQuality = _WindQuality * _WindEnabled;
 
                    float3 rotatedWindVector, rotatedBranchAnchor;
                    if (windQuality <= WIND_QUALITY_NONE)
                    {
                        rotatedWindVector = float3(0.0f, 0.0f, 0.0f);
                        rotatedBranchAnchor = float3(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        // compute rotated wind parameters
                        rotatedWindVector = normalize(mul(_ST_WindVector.xyz, (float3x3)unity_ObjectToWorld));
                        rotatedBranchAnchor = normalize(mul(_ST_WindBranchAnchor.xyz, (float3x3)unity_ObjectToWorld)) * _ST_WindBranchAnchor.w;
                    }
                #endif
 
                #if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_FROND)
 
                    // smooth LOD
                    #ifdef LOD_FADE_PERCENTAGE
                        finalPosition = lerp(finalPosition, data.texcoord1.xyz, lodValue);
                    #endif
 
                    // frond wind, if needed
                    #if defined(ENABLE_WIND) && defined(GEOM_TYPE_FROND)
                        if (windQuality == WIND_QUALITY_PALM)
                            finalPosition = RippleFrond(finalPosition, data.normal, data.texcoord.x, data.texcoord.y, data.texcoord2.x, data.texcoord2.y, data.texcoord2.z);
                    #endif
 
                #elif defined(GEOM_TYPE_LEAF)
 
                    // remove anchor position
                    finalPosition -= data.texcoord1.xyz;
 
                    bool isFacingLeaf = data.color.a == 0;
                    if (isFacingLeaf)
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            finalPosition *= lerp(1.0, data.texcoord1.w, lodValue);
                        #endif
 
                        // face camera-facing leaf to camera
//                      float offsetLen = length(finalPosition);
//                      finalPosition = mul(finalPosition.xyz, (float3x3)UNITY_MATRIX_IT_MV); // inv(MV) * finalPosition    // old camera facing (orients to view matrix, bad!)
 
                        //KRITOA: can't get this to work. Somebody help.
 
                        // orient to camera position with up vector instead now
//                      float3 camPos = _WorldSpaceCameraPos.xyz;
//                      float3 objectOrigin = data.texcoord1.xyz;  //mul(unity_ObjectToWorld, float3(0,0,0));
//                      float3 objectForward = normalize(camPos-objectOrigin);
//                      float3 objectRight = normalize(cross(float3(0,1,0),objectForward));
//                      float3 objectUp = normalize(cross(objectForward,objectRight));
//                      finalPosition = mul(float3x3(objectRight, objectUp, objectForward),finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z),
//                                                                      finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z),
//                                                                      finalPosition.xyz);
 
//                      finalPosition = normalize(finalPosition) * offsetLen; // make sure the offset vector is still scaled
                    }
                    else
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            float3 lodPosition = float3(data.texcoord1.w, data.texcoord3.x, data.texcoord3.y);
                            finalPosition = lerp(finalPosition, lodPosition, lodValue);
                        #endif
                    }
 
                    #ifdef ENABLE_WIND
                        // leaf wind
                        if (windQuality > WIND_QUALITY_FASTEST && windQuality < WIND_QUALITY_PALM)
                        {
                            float leafWindTrigOffset = data.texcoord1.x + data.texcoord1.y;
                            finalPosition = LeafWind(windQuality == WIND_QUALITY_BEST, data.texcoord2.w > 0.0, finalPosition, data.normal, data.texcoord2.x, float3(0,0,0), data.texcoord2.y, data.texcoord2.z, leafWindTrigOffset, rotatedWindVector);
                        }
                    #endif
 
                    // move back out to anchor
                    finalPosition += data.texcoord1.xyz;
 
                #endif
 
                #ifdef ENABLE_WIND
                    float3 treePos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
 
                    #ifndef GEOM_TYPE_MESH
                        if (windQuality >= WIND_QUALITY_BETTER)
                        {
                            // branch wind (applies to all 3D geometry)
                            finalPosition = BranchWind(windQuality == WIND_QUALITY_PALM, finalPosition, treePos, float4(data.texcoord.zw, 0, 0), rotatedWindVector, rotatedBranchAnchor);
                        }
                    #endif
 
                    if (windQuality > WIND_QUALITY_NONE)
                    {
                        // global wind
                        finalPosition = GlobalWind(finalPosition, treePos, true, rotatedWindVector, _ST_WindGlobal.x);
                    }
                #endif
 
                data.vertex.xyz = finalPosition;
            }
 
            void SpeedTreeVertKritoa(inout SpeedTreeVB IN, out Input OUT)
            {
                UNITY_INITIALIZE_OUTPUT(Input, OUT);
 
                OUT.mainTexUV = IN.texcoord.xy;
                OUT.color = _Color;
                OUT.color.rgb *= IN.color.r; // ambient occlusion factor
 
                #ifdef EFFECT_HUE_VARIATION
                    float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
                    hueVariationAmount += frac(IN.vertex.x + IN.normal.y + IN.normal.x) * 0.5 - 0.3;
                    OUT.HueVariationAmount = saturate(hueVariationAmount * _HueVariation.a);
                #endif
 
                #ifdef GEOM_TYPE_BRANCH_DETAIL
                    // The two types are always in different sub-range of the mesh so no interpolation (between detail and blend) problem.
                    OUT.Detail.xy = IN.texcoord2.xy;
                    if (IN.color.a == 0) // Blend
                        OUT.Detail.z = IN.texcoord2.z;
                    else // Detail texture
                        OUT.Detail.z = 2.5f; // stay out of Blend's .z range
                #endif
 
                OffsetSpeedTreeVertexKritoa(IN, unity_LODFade.x);
 
                UNITY_TRANSFER_DITHER_CROSSFADE(OUT, IN.vertex)
            }
 
 
                struct v2f
                {
                    V2F_SHADOW_CASTER;
                    #ifdef SPEEDTREE_ALPHATEST
                        float2 uv : TEXCOORD1;
                    #endif
                    UNITY_DITHER_CROSSFADE_COORDS_IDX(2)
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };
 
                v2f vert(SpeedTreeVB v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    #ifdef SPEEDTREE_ALPHATEST
                        o.uv = v.texcoord.xy;
                    #endif
                    OffsetSpeedTreeVertexKritoa(v, unity_LODFade.x);
                    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                    UNITY_TRANSFER_DITHER_CROSSFADE_HPOS(o, o.pos)
 
                    return o;
                }
 
                float4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(i);
                    #ifdef SPEEDTREE_ALPHATEST
                        clip(tex2D(_MainTex, i.uv).a * _Color.a - _Cutoff);
                    #endif
                    // UNITY_APPLY_DITHER_CROSSFADE(i)
                    SHADOW_CASTER_FRAGMENT(i)
                }
            ENDCG
        }
 
        Pass
        {
            Tags { "LightMode" = "Vertex" }
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_fog
                #pragma multi_compile  LOD_FADE_PERCENTAGE LOD_FADE_CROSSFADE
                #pragma multi_compile_instancing
                #pragma instancing_options assumeuniformscaling lodfade maxcount:50
                #pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
                #pragma shader_feature EFFECT_HUE_VARIATION
                #define ENABLE_WIND
                #include "SpeedTreeCommon.cginc"
 
                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    UNITY_FOG_COORDS(0)
                    Input data      : TEXCOORD1;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };
 
                v2f vert(SpeedTreeVB v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    SpeedTreeVert(v, o.data);
                    o.data.color.rgb *= ShadeVertexLightsFull(v.vertex, v.normal, 4, true);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }
 
                fixed4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(i);
                    SpeedTreeFragOut o;
                    SpeedTreeFrag(i.data, o);
                    fixed4 c = fixed4(o.Albedo, o.Alpha);
                    UNITY_APPLY_FOG(i.fogCoord, c);
                    return c;
                }
            ENDCG
        }
    }
 
    // targeting SM2.0: Cross-fading, Normal-mapping, Hue variation and Wind animation are turned off for less instructions
    SubShader
    {
        Tags
        {
            "Queue"="Geometry"
            "IgnoreProjector"="True"
            "RenderType"="Opaque"
            "DisableBatching"="LODFading"
        }
        LOD 400
        Cull [_Cull]
 
        CGPROGRAM
            #pragma surface surf Lambert vertex:SpeedTreeVertKritoa nodirlightmap nodynlightmap noshadowmask
            #pragma multi_compile  LOD_FADE_PERCENTAGE
            #pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
            #include "SpeedTreeCommon.cginc"
 
                        void OffsetSpeedTreeVertexKritoa(inout SpeedTreeVB data, float lodValue)
            {
                float3 finalPosition = data.vertex.xyz;
 
                #ifdef ENABLE_WIND
                    half windQuality = _WindQuality * _WindEnabled;
 
                    float3 rotatedWindVector, rotatedBranchAnchor;
                    if (windQuality <= WIND_QUALITY_NONE)
                    {
                        rotatedWindVector = float3(0.0f, 0.0f, 0.0f);
                        rotatedBranchAnchor = float3(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        // compute rotated wind parameters
                        rotatedWindVector = normalize(mul(_ST_WindVector.xyz, (float3x3)unity_ObjectToWorld));
                        rotatedBranchAnchor = normalize(mul(_ST_WindBranchAnchor.xyz, (float3x3)unity_ObjectToWorld)) * _ST_WindBranchAnchor.w;
                    }
                #endif
 
                #if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_FROND)
 
                    // smooth LOD
                    #ifdef LOD_FADE_PERCENTAGE
                        finalPosition = lerp(finalPosition, data.texcoord1.xyz, lodValue);
                    #endif
 
                    // frond wind, if needed
                    #if defined(ENABLE_WIND) && defined(GEOM_TYPE_FROND)
                        if (windQuality == WIND_QUALITY_PALM)
                            finalPosition = RippleFrond(finalPosition, data.normal, data.texcoord.x, data.texcoord.y, data.texcoord2.x, data.texcoord2.y, data.texcoord2.z);
                    #endif
 
                #elif defined(GEOM_TYPE_LEAF)
 
                    // remove anchor position
                    finalPosition -= data.texcoord1.xyz;
 
                    bool isFacingLeaf = data.color.a == 0;
                    if (isFacingLeaf)
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            finalPosition *= lerp(1.0, data.texcoord1.w, lodValue);
                        #endif
 
                        // face camera-facing leaf to camera
//                      float offsetLen = length(finalPosition);
//                      finalPosition = mul(finalPosition.xyz, (float3x3)UNITY_MATRIX_IT_MV); // inv(MV) * finalPosition    // old camera facing (orients to view matrix, bad!)
 
                        // KRITOA: can't get this to work. Somebody help.
 
                        // orient to camera position with up vector instead now
//                      float3 camPos = _WorldSpaceCameraPos.xyz;
//                      float3 objectOrigin = data.texcoord1.xyz;  //mul(unity_ObjectToWorld, float3(0,0,0));
//                      float3 objectForward = normalize(camPos-objectOrigin);
//                      float3 objectRight = normalize(cross(float3(0,1,0),objectForward));
//                      float3 objectUp = normalize(cross(objectForward,objectRight));
//                      finalPosition = mul(float3x3(objectRight, objectUp, objectForward),finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z),
//                                                                      finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z),
//                                                                      finalPosition.xyz);
 
//                      finalPosition = normalize(finalPosition) * offsetLen; // make sure the offset vector is still scaled
                    }
                    else
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            float3 lodPosition = float3(data.texcoord1.w, data.texcoord3.x, data.texcoord3.y);
                            finalPosition = lerp(finalPosition, lodPosition, lodValue);
                        #endif
                    }
 
                    #ifdef ENABLE_WIND
                        // leaf wind
                        if (windQuality > WIND_QUALITY_FASTEST && windQuality < WIND_QUALITY_PALM)
                        {
                            float leafWindTrigOffset = data.texcoord1.x + data.texcoord1.y;
                            finalPosition = LeafWind(windQuality == WIND_QUALITY_BEST, data.texcoord2.w > 0.0, finalPosition, data.normal, data.texcoord2.x, float3(0,0,0), data.texcoord2.y, data.texcoord2.z, leafWindTrigOffset, rotatedWindVector);
                        }
                    #endif
 
                    // move back out to anchor
                    finalPosition += data.texcoord1.xyz;
 
                #endif
 
                #ifdef ENABLE_WIND
                    float3 treePos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
 
                    #ifndef GEOM_TYPE_MESH
                        if (windQuality >= WIND_QUALITY_BETTER)
                        {
                            // branch wind (applies to all 3D geometry)
                            finalPosition = BranchWind(windQuality == WIND_QUALITY_PALM, finalPosition, treePos, float4(data.texcoord.zw, 0, 0), rotatedWindVector, rotatedBranchAnchor);
                        }
                    #endif
 
                    if (windQuality > WIND_QUALITY_NONE)
                    {
                        // global wind
                        finalPosition = GlobalWind(finalPosition, treePos, true, rotatedWindVector, _ST_WindGlobal.x);
                    }
                #endif
 
                data.vertex.xyz = finalPosition;
            }
 
            void SpeedTreeVertKritoa(inout SpeedTreeVB IN, out Input OUT)
            {
                UNITY_INITIALIZE_OUTPUT(Input, OUT);
 
                OUT.mainTexUV = IN.texcoord.xy;
                OUT.color = _Color;
                OUT.color.rgb *= IN.color.r; // ambient occlusion factor
 
                #ifdef EFFECT_HUE_VARIATION
                    float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
                    hueVariationAmount += frac(IN.vertex.x + IN.normal.y + IN.normal.x) * 0.5 - 0.3;
                    OUT.HueVariationAmount = saturate(hueVariationAmount * _HueVariation.a);
                #endif
 
                #ifdef GEOM_TYPE_BRANCH_DETAIL
                    // The two types are always in different sub-range of the mesh so no interpolation (between detail and blend) problem.
                    OUT.Detail.xy = IN.texcoord2.xy;
                    if (IN.color.a == 0) // Blend
                        OUT.Detail.z = IN.texcoord2.z;
                    else // Detail texture
                        OUT.Detail.z = 2.5f; // stay out of Blend's .z range
                #endif
 
                OffsetSpeedTreeVertexKritoa(IN, unity_LODFade.x);
 
                UNITY_TRANSFER_DITHER_CROSSFADE(OUT, IN.vertex)
            }
 
            void surf(Input IN, inout SurfaceOutput OUT)
            {
                SpeedTreeFragOut o;
                SpeedTreeFrag(IN, o);
                SPEEDTREE_COPY_FRAG(OUT, o)
            }
        ENDCG
 
        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile  LOD_FADE_PERCENTAGE
                #pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
                #pragma multi_compile_shadowcaster
                #include "SpeedTreeCommon.cginc"
 
                            void OffsetSpeedTreeVertexKritoa(inout SpeedTreeVB data, float lodValue)
            {
                float3 finalPosition = data.vertex.xyz;
 
                #ifdef ENABLE_WIND
                    half windQuality = _WindQuality * _WindEnabled;
 
                    float3 rotatedWindVector, rotatedBranchAnchor;
                    if (windQuality <= WIND_QUALITY_NONE)
                    {
                        rotatedWindVector = float3(0.0f, 0.0f, 0.0f);
                        rotatedBranchAnchor = float3(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        // compute rotated wind parameters
                        rotatedWindVector = normalize(mul(_ST_WindVector.xyz, (float3x3)unity_ObjectToWorld));
                        rotatedBranchAnchor = normalize(mul(_ST_WindBranchAnchor.xyz, (float3x3)unity_ObjectToWorld)) * _ST_WindBranchAnchor.w;
                    }
                #endif
 
                #if defined(GEOM_TYPE_BRANCH) || defined(GEOM_TYPE_FROND)
 
                    // smooth LOD
                    #ifdef LOD_FADE_PERCENTAGE
                        finalPosition = lerp(finalPosition, data.texcoord1.xyz, lodValue);
                    #endif
 
                    // frond wind, if needed
                    #if defined(ENABLE_WIND) && defined(GEOM_TYPE_FROND)
                        if (windQuality == WIND_QUALITY_PALM)
                            finalPosition = RippleFrond(finalPosition, data.normal, data.texcoord.x, data.texcoord.y, data.texcoord2.x, data.texcoord2.y, data.texcoord2.z);
                    #endif
 
                #elif defined(GEOM_TYPE_LEAF)
 
                    // remove anchor position
                    finalPosition -= data.texcoord1.xyz;
 
                    bool isFacingLeaf = data.color.a == 0;
                    if (isFacingLeaf)
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            finalPosition *= lerp(1.0, data.texcoord1.w, lodValue);
                        #endif
 
                        // face camera-facing leaf to camera
//                      float offsetLen = length(finalPosition);
//                      finalPosition = mul(finalPosition.xyz, (float3x3)UNITY_MATRIX_IT_MV); // inv(MV) * finalPosition    // old camera facing (orients to view matrix, bad!)
 
                        //KRITOA: Can't get this to work. Somebody help.
 
                        // orient to camera position with up vector instead now
//                      float3 camPos = _WorldSpaceCameraPos.xyz;
//                      float3 objectOrigin = data.texcoord1.xyz;  //mul(unity_ObjectToWorld, float3(0,0,0));
//                      float3 objectForward = normalize(camPos-objectOrigin);
//                      float3 objectRight = normalize(cross(float3(0,1,0),objectForward));
//                      float3 objectUp = normalize(cross(objectForward,objectRight));
//                      finalPosition = mul(float3x3(objectRight, objectUp, objectForward),finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectUp.x, objectForward.x,
//                                                                      objectRight.y, objectUp.y, objectForward.y,
//                                                                      objectRight.z, objectUp.z, objectForward.z),
//                                                                      finalPosition.xyz);
//                      finalPosition = mul(finalPosition.xyz,float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z));
//                      finalPosition = mul(float3x3(objectRight.x, objectRight.y, objectRight.z,
//                                                                      objectUp.x, objectUp.y, objectUp.z,
//                                                                      objectForward.x, objectForward.y, objectForward.z),
//                                                                      finalPosition.xyz);
 
//                      finalPosition = normalize(finalPosition) * offsetLen; // make sure the offset vector is still scaled
                    }
                    else
                    {
                        #ifdef LOD_FADE_PERCENTAGE
                            float3 lodPosition = float3(data.texcoord1.w, data.texcoord3.x, data.texcoord3.y);
                            finalPosition = lerp(finalPosition, lodPosition, lodValue);
                        #endif
                    }
 
                    #ifdef ENABLE_WIND
                        // leaf wind
                        if (windQuality > WIND_QUALITY_FASTEST && windQuality < WIND_QUALITY_PALM)
                        {
                            float leafWindTrigOffset = data.texcoord1.x + data.texcoord1.y;
                            finalPosition = LeafWind(windQuality == WIND_QUALITY_BEST, data.texcoord2.w > 0.0, finalPosition, data.normal, data.texcoord2.x, float3(0,0,0), data.texcoord2.y, data.texcoord2.z, leafWindTrigOffset, rotatedWindVector);
                        }
                    #endif
 
                    // move back out to anchor
                    finalPosition += data.texcoord1.xyz;
 
                #endif
 
                #ifdef ENABLE_WIND
                    float3 treePos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
 
                    #ifndef GEOM_TYPE_MESH
                        if (windQuality >= WIND_QUALITY_BETTER)
                        {
                            // branch wind (applies to all 3D geometry)
                            finalPosition = BranchWind(windQuality == WIND_QUALITY_PALM, finalPosition, treePos, float4(data.texcoord.zw, 0, 0), rotatedWindVector, rotatedBranchAnchor);
                        }
                    #endif
 
                    if (windQuality > WIND_QUALITY_NONE)
                    {
                        // global wind
                        finalPosition = GlobalWind(finalPosition, treePos, true, rotatedWindVector, _ST_WindGlobal.x);
                    }
                #endif
 
                data.vertex.xyz = finalPosition;
            }
 
            void SpeedTreeVertKritoa(inout SpeedTreeVB IN, out Input OUT)
            {
                UNITY_INITIALIZE_OUTPUT(Input, OUT);
 
                OUT.mainTexUV = IN.texcoord.xy;
                OUT.color = _Color;
                OUT.color.rgb *= IN.color.r; // ambient occlusion factor
 
                #ifdef EFFECT_HUE_VARIATION
                    float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
                    hueVariationAmount += frac(IN.vertex.x + IN.normal.y + IN.normal.x) * 0.5 - 0.3;
                    OUT.HueVariationAmount = saturate(hueVariationAmount * _HueVariation.a);
                #endif
 
                #ifdef GEOM_TYPE_BRANCH_DETAIL
                    // The two types are always in different sub-range of the mesh so no interpolation (between detail and blend) problem.
                    OUT.Detail.xy = IN.texcoord2.xy;
                    if (IN.color.a == 0) // Blend
                        OUT.Detail.z = IN.texcoord2.z;
                    else // Detail texture
                        OUT.Detail.z = 2.5f; // stay out of Blend's .z range
                #endif
 
                OffsetSpeedTreeVertexKritoa(IN, unity_LODFade.x);
 
                UNITY_TRANSFER_DITHER_CROSSFADE(OUT, IN.vertex)
            }
 
                struct v2f
                {
                    V2F_SHADOW_CASTER;
                    #ifdef SPEEDTREE_ALPHATEST
                        float2 uv : TEXCOORD1;
                    #endif
                };
 
                v2f vert(SpeedTreeVB v)
                {
                    v2f o;
                    #ifdef SPEEDTREE_ALPHATEST
                        o.uv = v.texcoord.xy;
                    #endif
                    OffsetSpeedTreeVertexKritoa(v, unity_LODFade.x);
                    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                    return o;
                }
 
                float4 frag(v2f i) : SV_Target
                {
                    #ifdef SPEEDTREE_ALPHATEST
                        clip(tex2D(_MainTex, i.uv).a * _Color.a - _Cutoff);
                    #endif
                    SHADOW_CASTER_FRAGMENT(i)
                }
            ENDCG
        }
 
        Pass
        {
            Tags { "LightMode" = "Vertex" }
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog
                #pragma multi_compile  LOD_FADE_PERCENTAGE
                #pragma shader_feature GEOM_TYPE_BRANCH GEOM_TYPE_BRANCH_DETAIL GEOM_TYPE_FROND GEOM_TYPE_LEAF GEOM_TYPE_MESH
                #include "SpeedTreeCommon.cginc"
 
                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    UNITY_FOG_COORDS(0)
                    Input data      : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };
 
                v2f vert(SpeedTreeVB v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    SpeedTreeVert(v, o.data);
                    o.data.color.rgb *= ShadeVertexLightsFull(v.vertex, v.normal, 2, false);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }
 
                fixed4 frag(v2f i) : SV_Target
                {
                    SpeedTreeFragOut o;
                    SpeedTreeFrag(i.data, o);
                    fixed4 c = fixed4(o.Albedo, o.Alpha);
                    UNITY_APPLY_FOG(i.fogCoord, c);
                    return c;
                }
            ENDCG
        }
    }
 
    FallBack "Transparent/Cutout/VertexLit"
    CustomEditor "SpeedTreeMaterialInspector"
}