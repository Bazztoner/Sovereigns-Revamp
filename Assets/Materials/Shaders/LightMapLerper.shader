// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LightmapLerper"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_LightmapA("Lightmap A", 2D) = "white" {}
		_LightmapB("Lightmap B", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_MetallicSmoothness("MetallicSmoothness", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_alphaLerp("alphaLerp", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _LightmapA;
		uniform float4 _LightmapA_ST;
		uniform sampler2D _LightmapB;
		uniform float4 _LightmapB_ST;
		uniform float _alphaLerp;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode5 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = tex2DNode5.rgb;
			float2 uv2_LightmapA = i.uv2_texcoord2 * _LightmapA_ST.xy + _LightmapA_ST.zw;
			float2 uv2_LightmapB = i.uv2_texcoord2 * _LightmapB_ST.xy + _LightmapB_ST.zw;
			float4 lerpResult8 = lerp( ( tex2DNode5 * tex2D( _LightmapA, uv2_LightmapA ) ) , ( tex2DNode5 * tex2D( _LightmapB, uv2_LightmapB ) ) , _alphaLerp);
			o.Emission = lerpResult8.rgb;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode6 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			o.Metallic = tex2DNode6.r;
			o.Smoothness = tex2DNode6.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
342;105;810;556;1323.917;515.7402;2.077846;True;False
Node;AmplifyShaderEditor.SamplerNode;5;-673.434,-347.9379;Float;True;Property;_Albedo;Albedo;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-581.2831,-65.16912;Float;True;Property;_LightmapB;Lightmap B;1;0;None;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-612.9963,-622.1502;Float;True;Property;_LightmapA;Lightmap A;0;0;None;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-246.7214,-448.4525;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;9;20.99776,-199.0816;Float;False;Property;_alphaLerp;alphaLerp;5;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-264.0548,-147.4062;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.LerpOp;8;84.53159,-401.8558;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;7;-28.90847,107.6865;Float;True;Property;_Normal;Normal;4;0;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-39.85936,-79.33571;Float;True;Property;_MetallicSmoothness;MetallicSmoothness;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;399.6431,-253.4144;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;LightmapLerper;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;5;0
WireConnection;3;1;1;0
WireConnection;4;0;5;0
WireConnection;4;1;2;0
WireConnection;8;0;3;0
WireConnection;8;1;4;0
WireConnection;8;2;9;0
WireConnection;0;0;5;0
WireConnection;0;1;7;0
WireConnection;0;2;8;0
WireConnection;0;3;6;1
WireConnection;0;4;6;4
ASEEND*/
//CHKSM=D5512C3BFC2E236EC1FE1042CC8BD92466F88F16