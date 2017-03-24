// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

Shader "GraphTool/Plotter"
{
	Properties
	{
		_Scale("Scale", Float) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite On
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma target 5.0
			#pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma geometry geom
	        #pragma fragment frag

			#include "UnityCG.cginc"

			struct PointData
			{
				float2 pos;
				bool drawLine;
			};

			struct VSOut 
			{
	            float4 pos : SV_POSITION;
	        };

			struct GSOut
			{
				float4 pos : SV_POSITION;
	            float2 tex : TEXCOORD0;
			};

			StructuredBuffer<PointData> Points;
			uniform float _Scale;
			float4x4 _L2WMatrix;

			VSOut vert (uint id : SV_VertexID)
			{
				VSOut o;

				o.pos = mul(_L2WMatrix, float4(Points[id].pos, 0, 1));

				return o;
			}

		   	[maxvertexcount(4)]
		   	void geom (point VSOut input[1], inout TriangleStream<GSOut> outStream)
		   	{
		     	GSOut output;
		      	float4 pos = input[0].pos;
		     	
				for(int y = 0; y < 2; y++)
			    {
		      		for(int x = 0; x < 2; x++)
		      		{
			      		float2 tex = float2(x, y);
			        	output.tex = tex;
			      		
				      	output.pos = pos + mul(_L2WMatrix, float4(tex*2 - float2(1,1) , 0, 0) * _Scale);
			          	output.pos = mul (UNITY_MATRIX_MVP, output.pos);
			          	
				      	outStream.Append (output);
			      	}
		      	}
		      	
		      	// トライアングルストリップを終了
		      	outStream.RestartStrip();
		   	}

			// フラグメントシェーダ
			fixed4 frag (GSOut i) : COLOR
	        {
	            return fixed4(1,1,1,1);
	        }

			ENDCG
		}
	}
}
