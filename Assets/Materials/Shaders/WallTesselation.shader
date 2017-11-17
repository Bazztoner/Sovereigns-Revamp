// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WallTesselation"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 1
		_InnerWalls_InnerWalls_AlbedoTransparency_DISP("InnerWalls_InnerWalls_AlbedoTransparency_DISP", 2D) = "white" {}
		_InnerWalls_InnerWalls_MetallicSmoothness("InnerWalls_InnerWalls_MetallicSmoothness", 2D) = "white" {}
		_InnerWalls_InnerWalls_Normal("InnerWalls_InnerWalls_Normal", 2D) = "bump" {}
		_InnerWalls_InnerWalls_AlbedoTransparency("InnerWalls_InnerWalls_AlbedoTransparency", 2D) = "white" {}
		_Intensity("Intensity", Range( -1 , 1)) = 0
		_SubDivisions("SubDivisions", Range( 1 , 50)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction tessphong:_TessPhongStrength 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform sampler2D _InnerWalls_InnerWalls_Normal;
		uniform float4 _InnerWalls_InnerWalls_Normal_ST;
		uniform sampler2D _InnerWalls_InnerWalls_AlbedoTransparency;
		uniform float4 _InnerWalls_InnerWalls_AlbedoTransparency_ST;
		uniform sampler2D _InnerWalls_InnerWalls_MetallicSmoothness;
		uniform float4 _InnerWalls_InnerWalls_MetallicSmoothness_ST;
		uniform sampler2D _InnerWalls_InnerWalls_AlbedoTransparency_DISP;
		uniform float4 _InnerWalls_InnerWalls_AlbedoTransparency_DISP_ST;
		uniform float _Intensity;
		uniform float _SubDivisions;
		uniform float _TessPhongStrength;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			float4 temp_cast_2 = (_SubDivisions).xxxx;
			return temp_cast_2;
		}

		void vertexDataFunc( inout appdata v )
		{
			float3 ase_vertexNormal = v.normal.xyz;
			float4 uv_InnerWalls_InnerWalls_AlbedoTransparency_DISP = float4(v.texcoord * _InnerWalls_InnerWalls_AlbedoTransparency_DISP_ST.xy + _InnerWalls_InnerWalls_AlbedoTransparency_DISP_ST.zw, 0 ,0);
			v.vertex.xyz += ( ( float4( ase_vertexNormal , 0.0 ) * tex2Dlod( _InnerWalls_InnerWalls_AlbedoTransparency_DISP, uv_InnerWalls_InnerWalls_AlbedoTransparency_DISP ) ) * _Intensity ).xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_InnerWalls_InnerWalls_Normal = i.uv_texcoord * _InnerWalls_InnerWalls_Normal_ST.xy + _InnerWalls_InnerWalls_Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _InnerWalls_InnerWalls_Normal, uv_InnerWalls_InnerWalls_Normal ) );
			float2 uv_InnerWalls_InnerWalls_AlbedoTransparency = i.uv_texcoord * _InnerWalls_InnerWalls_AlbedoTransparency_ST.xy + _InnerWalls_InnerWalls_AlbedoTransparency_ST.zw;
			o.Albedo = tex2D( _InnerWalls_InnerWalls_AlbedoTransparency, uv_InnerWalls_InnerWalls_AlbedoTransparency ).xyz;
			float2 uv_InnerWalls_InnerWalls_MetallicSmoothness = i.uv_texcoord * _InnerWalls_InnerWalls_MetallicSmoothness_ST.xy + _InnerWalls_InnerWalls_MetallicSmoothness_ST.zw;
			float4 tex2DNode2 = tex2D( _InnerWalls_InnerWalls_MetallicSmoothness, uv_InnerWalls_InnerWalls_MetallicSmoothness );
			o.Metallic = tex2DNode2.r;
			o.Smoothness = tex2DNode2.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
886;91;480;615;1214.707;-3.25036;1.896888;False;False
Node;AmplifyShaderEditor.NormalVertexDataNode;10;-696.9233,359.4114;Float;True;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-845.1219,641.5093;Float;True;Property;_InnerWalls_InnerWalls_AlbedoTransparency_DISP;InnerWalls_InnerWalls_AlbedoTransparency_DISP;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-336.1458,513.8731;Float;False;2;2;0;FLOAT3;0.0;False;1;FLOAT4;0.0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.RangedFloatNode;8;-380.0162,680.4315;Float;False;Property;_Intensity;Intensity;4;0;0;-1;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;4;-538.8605,-124.5617;Float;True;Property;_InnerWalls_InnerWalls_AlbedoTransparency;InnerWalls_InnerWalls_AlbedoTransparency;3;0;Assets/Materials/Temple/InnerWalls/InnerWalls_InnerWalls_AlbedoTransparency.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-448.786,265.9014;Float;True;Property;_InnerWalls_InnerWalls_Normal;InnerWalls_InnerWalls_Normal;2;0;Assets/Materials/Temple/InnerWalls/InnerWalls_InnerWalls_Normal.tga;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;9;-359.5432,760.1909;Float;False;Property;_SubDivisions;SubDivisions;5;0;1;1;50;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-543.7574,70.85086;Float;True;Property;_InnerWalls_InnerWalls_MetallicSmoothness;InnerWalls_InnerWalls_MetallicSmoothness;1;0;Assets/Materials/Temple/InnerWalls/InnerWalls_InnerWalls_MetallicSmoothness.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-187.8116,551.4521;Float;False;2;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-2,0;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;WallTesselation;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;0;4;10;25;True;1;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;10;0
WireConnection;6;1;1;0
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;0;0;4;0
WireConnection;0;1;3;0
WireConnection;0;3;2;1
WireConnection;0;4;2;4
WireConnection;0;11;7;0
WireConnection;0;14;9;0
ASEEND*/
//CHKSM=717F311CBED98D917C514F7D244C40B32A94BBF5