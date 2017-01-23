Shader "Custom/My First Lighting Shader" {

	Properties{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
		_DetailTex("Texture", 2D) = "gray" {}
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		_SpecularTint("Specular", Color) = (0.5, 0.5, 0.5)
		[Gamma]	_Metallic("Metallic", Range(0, 1)) = 0
	}

		SubShader{

				Pass {

					Tags{
						"LightMode" = "ForwardBase"
					}
					
					CGPROGRAM
					#include "UnityPBSLighting.cginc"
					#pragma target 3.0
					#pragma vertex MyVertexProgram
					#pragma fragment MyFragmentProgram
					
					sampler2D _MainTex, _DetailTex, a;
					float4 _MainTex_ST, _DetailTex_ST;
					float4 _Tint;
					float4 _Smoothness;
					float4 _SpecularTint;
					float _Metallic;
					
					struct Interpolators {
						float4 position : SV_POSITION;//used to determine the vertex position
						float2 uv : TEXCOORD0;
						float3 normal : TEXCOORD1;
						float3 worldPos : TEXCOORD2;
						float3 color : COLOR;
					};

					struct VertexData {
						float4 position : POSITION;
						float2 uv : TEXCOORD0;
						float3 normal : NORMAL;
						float3 color : COLOR;
					};
					//vertex program will only get the VertexData's position and determine the position of the vertex
					Interpolators MyVertexProgram(VertexData v){
						Interpolators i;
						i.color = v.color;
						i.position = mul(UNITY_MATRIX_MVP, v.position);
						
						return i;
					}
					//SV_TARGET is indicating float4 return color is for fragment color
					//float4 MyFragmentProgram(Interpolators i) : SV_TARGET{}
						
					ENDCG
			}
	}
}