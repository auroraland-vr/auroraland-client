// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AVP/Height_Blend_2_Layer"
{
	Properties
	{
		_Tile1("Tile1", Float) = 0
		_Tile2("Tile2", Float) = 0
		_Color1("Color1", Color) = (0,0,0,0)
		_Albedo1("Albedo1", 2D) = "white" {}
		_Metallic1("Metallic1", 2D) = "white" {}
		_Metallic1_Power("Metallic1_Power", Range( 0 , 1)) = 0
		_Smoothness1("Smoothness1", Range( 0 , 1)) = 0.5
		_Normal1("Normal1", 2D) = "bump" {}
		_Normal1_Power("Normal1_Power", Range( -4 , 4)) = 0
		_Height1("Height1", 2D) = "white" {}
		_Color2("Color2", Color) = (1,1,1,0)
		_Albedo2("Albedo2", 2D) = "white" {}
		_Metallic2("Metallic2", 2D) = "white" {}
		_Metallic2_Power("Metallic2_Power", Range( 0 , 1)) = 0
		_Normal2("Normal2", 2D) = "bump" {}
		_Normal2_Power("Normal2_Power", Range( -4 , 4)) = 0
		_Smoothness2("Smoothness2", Range( 0 , 1)) = 0.5
		_Height2("Height2", 2D) = "white" {}
		_Height2Scale("Height2 Scale", Range( 0 , 10)) = 1
		_Height2Offset("Height2 Offset", Range( -10 , 10)) = 1
		_Color2Scale("Color2 Scale", Range( 0 , 20)) = 1
		_Color2Offset("Color2 Offset", Range( -20 , 20)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _Normal1_Power;
		uniform sampler2D _Normal1;
		uniform float _Tile1;
		uniform float _Normal2_Power;
		uniform sampler2D _Normal2;
		uniform float _Tile2;
		uniform sampler2D _Height1;
		uniform sampler2D _Height2;
		uniform float _Height2Scale;
		uniform float _Height2Offset;
		uniform float _Color2Scale;
		uniform float _Color2Offset;
		uniform float4 _Color1;
		uniform sampler2D _Albedo1;
		uniform float4 _Color2;
		uniform sampler2D _Albedo2;
		uniform float _Metallic1_Power;
		uniform sampler2D _Metallic1;
		uniform float _Metallic2_Power;
		uniform sampler2D _Metallic2;
		uniform float _Smoothness1;
		uniform float _Smoothness2;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_Tile1).xx;
			float2 uv_TexCoord179 = i.uv_texcoord * temp_cast_0;
			float2 temp_cast_1 = (_Tile2).xx;
			float2 uv_TexCoord180 = i.uv_texcoord * temp_cast_1;
			float4 appendResult135 = (float4(tex2D( _Height1, uv_TexCoord179 ).r , tex2D( _Height2, uv_TexCoord180 ).r , 0.0 , 0.0));
			float4 appendResult111 = (float4(0.0 , _Height2Scale , 0.0 , 0.0));
			float4 appendResult114 = (float4(0.0 , _Height2Offset , 0.0 , 0.0));
			float4 temp_output_101_0 = (( 1.0 - appendResult135 )*appendResult111 + appendResult114);
			float4 appendResult120 = (float4(0.0 , _Color2Scale , 0.0 , 0.0));
			float4 appendResult121 = (float4(0.0 , _Color2Offset , 0.0 , 0.0));
			float4 temp_output_97_0 = (i.vertexColor*appendResult120 + appendResult121);
			float4 Blender124 = ( 1.0 - saturate( ( temp_output_101_0 + temp_output_97_0 ) ) );
			float3 lerpResult72 = lerp( UnpackScaleNormal( tex2D( _Normal1, uv_TexCoord179 ) ,_Normal1_Power ) , UnpackScaleNormal( tex2D( _Normal2, uv_TexCoord180 ) ,_Normal2_Power ) , (Blender124).y);
			o.Normal = lerpResult72;
			float4 lerpResult29 = lerp( ( _Color1 * tex2D( _Albedo1, uv_TexCoord179 ) ) , ( _Color2 * tex2D( _Albedo2, uv_TexCoord180 ) ) , (Blender124).y);
			o.Albedo = lerpResult29.rgb;
			float4 tex2DNode140 = tex2D( _Metallic1, uv_TexCoord179 );
			float4 tex2DNode141 = tex2D( _Metallic2, uv_TexCoord180 );
			float temp_output_156_0 = (Blender124).y;
			float lerpResult143 = lerp( ( _Metallic1_Power * tex2DNode140.r ) , ( _Metallic2_Power * tex2DNode141.r ) , temp_output_156_0);
			o.Metallic = lerpResult143;
			float lerpResult144 = lerp( ( tex2DNode140.a * _Smoothness1 ) , ( tex2DNode141.a * _Smoothness2 ) , temp_output_156_0);
			o.Smoothness = lerpResult144;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
-1913;237;1906;1004;3296.468;-250.8705;2.098435;True;True
Node;AmplifyShaderEditor.RangedFloatNode;181;-2136.797,502.2938;Float;False;Property;_Tile2;Tile2;1;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-2144.407,341.0961;Float;False;Property;_Tile1;Tile1;0;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;179;-1925.563,326.0983;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;180;-1923.21,478.1493;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;177;-821.4088,2052.778;Float;True;Property;_Height1;Height1;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;175;-824.4773,2267.355;Float;True;Property;_Height2;Height2;17;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;118;34.99989,2194.801;Float;False;Property;_Color2Scale;Color2 Scale;20;0;Create;True;0;0;False;0;1;2.5;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;34.99989,2002.8;Float;False;Property;_Height2Offset;Height2 Offset;19;0;Create;True;0;0;False;0;1;-2;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;34.99989,2354.801;Float;False;Property;_Color2Offset;Color2 Offset;21;0;Create;True;0;0;False;0;1;0.8;-20;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;135;320,1680;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;113;34.99989,1842.8;Float;False;Property;_Height2Scale;Height2 Scale;18;0;Create;True;0;0;False;0;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;114;323.0016,2002.8;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;111;323.0016,1858.8;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;102;483.0016,1714.8;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;121;323.0016,2354.801;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;94;-159.002,2128.801;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;120;323.0016,2210.801;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;97;515.0016,2130.801;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;101;515.0016,1842.8;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;107;867.0016,1986.8;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;108;1010.601,1982.8;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;109;1154.601,1982.8;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;145;-1536,1728;Float;False;124;0;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;-868.4028,341.0201;Float;False;124;0;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;186;-1955.737,188.6399;Float;False;Property;_Normal1_Power;Normal1_Power;8;0;Create;True;0;0;False;0;0;0;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-1536,1632;Float;False;Property;_Smoothness2;Smoothness2;16;0;Create;True;0;0;False;0;0.5;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;140;-1539.435,1071.943;Float;True;Property;_Metallic1;Metallic1;4;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;137;-1534.855,981.6395;Float;False;Property;_Metallic1_Power;Metallic1_Power;5;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-1966.737,638.6399;Float;False;Property;_Normal2_Power;Normal2_Power;15;0;Create;True;0;0;False;0;0;0;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-1536,1280;Float;False;Property;_Smoothness1;Smoothness1;6;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-1536,1376;Float;False;Property;_Metallic2_Power;Metallic2_Power;13;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;1298.601,1982.8;Float;False;Blender;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;141;-1536,1456;Float;True;Property;_Metallic2;Metallic2;12;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;183;-681.8011,-26.71973;Float;False;Property;_Color2;Color2;10;0;Create;True;0;0;False;0;1,1,1,0;0.3921568,0.3921568,0.3921568,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-767,-247;Float;True;Property;_Albedo1;Albedo1;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;182;-674.8011,-419.7197;Float;False;Property;_Color1;Color1;2;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;126;-1536,668;Float;False;124;0;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;2;-768,144;Float;True;Property;_Albedo2;Albedo2;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-1154.139,1233.565;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-1150.135,1598.313;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-1150.135,1404.271;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-1155.448,1029.034;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;-384.8011,-286.7197;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;156;-1360,1728;Float;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-385.8011,65.28027;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;125;-1344,668;Float;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;69;-1536,464;Float;True;Property;_Normal2;Normal2;14;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;68;-1536,276;Float;True;Property;_Normal1;Normal1;7;0;Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;129;-676.4028,341.0201;Float;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;105;707.0016,2050.801;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;143;-899.7291,1294.739;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;144;-894.1356,1433.927;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;72;-1136,496;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;106;707.0016,1970.8;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;29;-92.83109,-114.5518;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,672;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;AVP/Height_Blend_2_Layer;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;179;0;178;0
WireConnection;180;0;181;0
WireConnection;177;1;179;0
WireConnection;175;1;180;0
WireConnection;135;0;177;1
WireConnection;135;1;175;1
WireConnection;114;1;116;0
WireConnection;111;1;113;0
WireConnection;102;0;135;0
WireConnection;121;1;122;0
WireConnection;120;1;118;0
WireConnection;97;0;94;0
WireConnection;97;1;120;0
WireConnection;97;2;121;0
WireConnection;101;0;102;0
WireConnection;101;1;111;0
WireConnection;101;2;114;0
WireConnection;107;0;101;0
WireConnection;107;1;97;0
WireConnection;108;0;107;0
WireConnection;109;0;108;0
WireConnection;140;1;179;0
WireConnection;124;0;109;0
WireConnection;141;1;180;0
WireConnection;28;1;179;0
WireConnection;2;1;180;0
WireConnection;139;0;140;4
WireConnection;139;1;147;0
WireConnection;152;0;141;4
WireConnection;152;1;154;0
WireConnection;142;0;146;0
WireConnection;142;1;141;1
WireConnection;138;0;137;0
WireConnection;138;1;140;1
WireConnection;184;0;182;0
WireConnection;184;1;28;0
WireConnection;156;0;145;0
WireConnection;185;0;183;0
WireConnection;185;1;2;0
WireConnection;125;0;126;0
WireConnection;69;1;180;0
WireConnection;69;5;187;0
WireConnection;68;1;179;0
WireConnection;68;5;186;0
WireConnection;129;0;130;0
WireConnection;105;0;97;0
WireConnection;143;0;138;0
WireConnection;143;1;142;0
WireConnection;143;2;156;0
WireConnection;144;0;139;0
WireConnection;144;1;152;0
WireConnection;144;2;156;0
WireConnection;72;0;68;0
WireConnection;72;1;69;0
WireConnection;72;2;125;0
WireConnection;106;0;101;0
WireConnection;29;0;184;0
WireConnection;29;1;185;0
WireConnection;29;2;129;0
WireConnection;0;0;29;0
WireConnection;0;1;72;0
WireConnection;0;3;143;0
WireConnection;0;4;144;0
ASEEND*/
//CHKSM=C12F95A8B0329C2FDE5FC62DE121890687707D38