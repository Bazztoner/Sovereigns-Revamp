// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AngelArmor"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Angel_armor_Angel_Armor_AlbedoTransparency("Angel_armor_Angel_Armor_AlbedoTransparency", 2D) = "white" {}
		_Angel_armor_Angel_Armor_MetallicSmoothness("Angel_armor_Angel_Armor_MetallicSmoothness", 2D) = "white" {}
		_Angel_armor_Angel_Armor_Normal("Angel_armor_Angel_Armor_Normal", 2D) = "white" {}
		_ArmorSplatMap("ArmorSplatMap", 2D) = "white" {}
		_FabricMask("FabricMask", 2D) = "white" {}
		_FeatherMask("FeatherMask", 2D) = "white" {}
		_LeatherMask("LeatherMask", 2D) = "white" {}
		_TintAlpha("TintAlpha", Range( 0 , 1)) = 0
		_BaseMetal("BaseMetal", Color) = (0.5882353,0,0,0)
		_EdgePlating("EdgePlating", Color) = (0,0.9632353,0.3653649,0)
		_Fabric("Fabric", Color) = (1,0.9724138,0,0)
		_Leather("Leather", Color) = (0,0,0,0)
		_Feather("Feather", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Angel_armor_Angel_Armor_Normal;
		uniform float4 _Angel_armor_Angel_Armor_Normal_ST;
		uniform float4 _BaseMetal;
		uniform sampler2D _ArmorSplatMap;
		uniform float4 _ArmorSplatMap_ST;
		uniform float4 _EdgePlating;
		uniform float4 _Fabric;
		uniform sampler2D _FabricMask;
		uniform float4 _FabricMask_ST;
		uniform sampler2D _LeatherMask;
		uniform float4 _LeatherMask_ST;
		uniform float4 _Leather;
		uniform sampler2D _FeatherMask;
		uniform float4 _FeatherMask_ST;
		uniform float4 _Feather;
		uniform sampler2D _Angel_armor_Angel_Armor_AlbedoTransparency;
		uniform float4 _Angel_armor_Angel_Armor_AlbedoTransparency_ST;
		uniform float _TintAlpha;
		uniform sampler2D _Angel_armor_Angel_Armor_MetallicSmoothness;
		uniform float4 _Angel_armor_Angel_Armor_MetallicSmoothness_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Angel_armor_Angel_Armor_Normal = i.uv_texcoord * _Angel_armor_Angel_Armor_Normal_ST.xy + _Angel_armor_Angel_Armor_Normal_ST.zw;
			o.Normal = tex2D( _Angel_armor_Angel_Armor_Normal, uv_Angel_armor_Angel_Armor_Normal ).xyz;
			float2 uv_ArmorSplatMap = i.uv_texcoord * _ArmorSplatMap_ST.xy + _ArmorSplatMap_ST.zw;
			float4 tex2DNode1 = tex2D( _ArmorSplatMap, uv_ArmorSplatMap );
			float2 uv_FabricMask = i.uv_texcoord * _FabricMask_ST.xy + _FabricMask_ST.zw;
			float lerpResult27 = lerp( tex2DNode1.b , 0.0 , ( 1.0 - tex2D( _FabricMask, uv_FabricMask ) ).x);
			float2 uv_LeatherMask = i.uv_texcoord * _LeatherMask_ST.xy + _LeatherMask_ST.zw;
			float lerpResult29 = lerp( tex2DNode1.b , 0.0 , ( 1.0 - tex2D( _LeatherMask, uv_LeatherMask ) ).x);
			float2 uv_FeatherMask = i.uv_texcoord * _FeatherMask_ST.xy + _FeatherMask_ST.zw;
			float lerpResult35 = lerp( tex2DNode1.b , 0.0 , ( 1.0 - tex2D( _FeatherMask, uv_FeatherMask ) ).x);
			float2 uv_Angel_armor_Angel_Armor_AlbedoTransparency = i.uv_texcoord * _Angel_armor_Angel_Armor_AlbedoTransparency_ST.xy + _Angel_armor_Angel_Armor_AlbedoTransparency_ST.zw;
			float4 lerpResult19 = lerp( ( ( _BaseMetal * tex2DNode1.r ) + ( _EdgePlating * tex2DNode1.g ) + ( _Fabric * lerpResult27 ) + ( lerpResult29 * _Leather ) + ( lerpResult35 * _Feather ) ) , tex2D( _Angel_armor_Angel_Armor_AlbedoTransparency, uv_Angel_armor_Angel_Armor_AlbedoTransparency ) , _TintAlpha);
			o.Albedo = lerpResult19.xyz;
			float2 uv_Angel_armor_Angel_Armor_MetallicSmoothness = i.uv_texcoord * _Angel_armor_Angel_Armor_MetallicSmoothness_ST.xy + _Angel_armor_Angel_Armor_MetallicSmoothness_ST.zw;
			float4 tex2DNode17 = tex2D( _Angel_armor_Angel_Armor_MetallicSmoothness, uv_Angel_armor_Angel_Armor_MetallicSmoothness );
			o.Metallic = tex2DNode17.r;
			o.Smoothness = tex2DNode17.a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
1091;91;275;615;-443.803;171.858;1.282002;False;False
Node;AmplifyShaderEditor.SamplerNode;21;-1249.628,343.1351;Float;True;Property;_FabricMask;FabricMask;4;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;22;-1224.441,1423.274;Float;True;Property;_FeatherMask;FeatherMask;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;23;-1251.541,970.6229;Float;True;Property;_LeatherMask;LeatherMask;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;31;-967.6556,967.887;Float;False;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.OneMinusNode;32;-938.7773,1423.297;Float;False;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;1;-1050.493,112.5484;Float;True;Property;_ArmorSplatMap;ArmorSplatMap;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;28;-960.1357,351.7857;Float;True;1;0;FLOAT4;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.LerpOp;35;-771.971,1410.939;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;4;-1019.21,-250.4941;Float;False;Property;_BaseMetal;BaseMetal;8;0;0.5882353,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;27;-778.4509,348.7737;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;25;-778.5601,1625.234;Float;False;Property;_Feather;Feather;12;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;3;-1015.843,-81.8844;Float;False;Property;_EdgePlating;EdgePlating;9;0;0,0.9632353,0.3653649,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;29;-795.7588,966.14;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;24;-801.7698,1177.932;Float;False;Property;_Leather;Leather;11;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;-780.8393,561.6458;Float;False;Property;_Fabric;Fabric;10;0;1,0.9724138,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-522.6904,969.2309;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-601.5893,-55.62639;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-496.6245,1410.157;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-602.3527,-265.9565;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-487.384,421.7756;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-135.9292,96.75951;Float;False;5;5;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;15;177.7146,908.1457;Float;True;Property;_Angel_armor_Angel_Armor_AlbedoTransparency;Angel_armor_Angel_Armor_AlbedoTransparency;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;20;39.89214,-150.9979;Float;False;Property;_TintAlpha;TintAlpha;7;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;19;403.4621,-3.798208;Float;False;3;0;COLOR;0.0,0,0,0;False;1;FLOAT4;0.0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.SamplerNode;18;248.3326,508.5153;Float;True;Property;_Angel_armor_Angel_Armor_Normal;Angel_armor_Angel_Armor_Normal;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;17;330.1397,141.3235;Float;True;Property;_Angel_armor_Angel_Armor_MetallicSmoothness;Angel_armor_Angel_Armor_MetallicSmoothness;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;888.0366,82.98837;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;AngelArmor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;23;0
WireConnection;32;0;22;0
WireConnection;28;0;21;0
WireConnection;35;0;1;3
WireConnection;35;2;32;0
WireConnection;27;0;1;3
WireConnection;27;2;28;0
WireConnection;29;0;1;3
WireConnection;29;2;31;0
WireConnection;30;0;29;0
WireConnection;30;1;24;0
WireConnection;11;0;3;0
WireConnection;11;1;1;2
WireConnection;33;0;35;0
WireConnection;33;1;25;0
WireConnection;12;0;4;0
WireConnection;12;1;1;1
WireConnection;13;0;5;0
WireConnection;13;1;27;0
WireConnection;14;0;12;0
WireConnection;14;1;11;0
WireConnection;14;2;13;0
WireConnection;14;3;30;0
WireConnection;14;4;33;0
WireConnection;19;0;14;0
WireConnection;19;1;15;0
WireConnection;19;2;20;0
WireConnection;0;0;19;0
WireConnection;0;1;18;0
WireConnection;0;3;17;1
WireConnection;0;4;17;4
ASEEND*/
//CHKSM=5686201DF686ACFF943C6F3DCD2D5EDEFD32821E