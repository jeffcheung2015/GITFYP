Shader "Custom/ExampleVertexColorShader" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex wfiVertCol
			#pragma fragment passThrough
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			//sampler2D _AlphaTex;
			struct VertOut
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			struct VertIn
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			VertOut wfiVertCol(VertIn input)
			{
				VertOut output;
				output.position = mul(UNITY_MATRIX_MVP,input.vertex);
				output.color = input.color;
				output.uv = input.uv;
				return output;
			}
			
			float4 passThrough(VertOut v) : SV_TARGET
			{						

				return tex2D(_MainTex, v.uv) * v.color; //black out the hole
				
				//return normalize(color);
				//return float4(c.xyz, tex2D(_MainTex, v.uv).w);
				//return float4(v.uv, 0,1);
			}
			

			ENDCG
		}
	}
	FallBack "Diffuse"


}
