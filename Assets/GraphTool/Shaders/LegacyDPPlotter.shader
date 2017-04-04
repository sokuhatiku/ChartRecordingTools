// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

Shader "GraphTool/LegacyDPPlotter"
{
	Properties
	{
		_Scale("Scale", Float) = 1
		_Color("Color", COLOR) = (1,1,1,1)

		// required for UI.Mask
		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 1
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

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
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		// required for UI.Mask
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
		ColorMask [_ColorMask]

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
				float drawLine : PSIZE;
	        };

			struct GSOut
			{
				float4 pos : SV_POSITION;
	            float2 tex : TEXCOORD0;
			};

			StructuredBuffer<PointData> Points;
			uniform float _Scale;
			uniform fixed4 _Color;
			uniform fixed _PtsCount;
			float4x4 _S2LMatrix;
			float4x4 _L2WMatrix;

			VSOut vert (uint id : SV_VertexID)
			{
				VSOut o;

				o.pos = mul(_S2LMatrix, float4(Points[id].pos, 0, 1));
				o.drawLine = Points[id].drawLine;

				return o;
			}

		   	[maxvertexcount(8)]
		   	void geom (line VSOut input[2], inout TriangleStream<GSOut> outStream)
		   	{
		     	GSOut output;
		     	
				float2 offsH = input[1].pos - input[0].pos;
				offsH = normalize(offsH) * _Scale;
				float2 offsV = float2(-offsH.y, offsH.x);
				bool drawLine = asint(input[0].drawLine);
				
				for(int l =0; l < 2; l++)
				{
					float4 pos = input[l * drawLine].pos;
		      		for(int u = 0; u < 2; u++)
		      		{
						offsH *= -1;
						for(int v = 0; v < 2; v++)
						{
			        		output.tex = float2(l+u, v);
							output.tex.x /= 2;
			      		
							output.pos = pos + float4(offsV + offsH * (1 - l^u), 0, 0);

			          		output.pos = mul(UNITY_MATRIX_MVP, mul(_L2WMatrix , output.pos));
			          	
				      		outStream.Append (output);
							offsV *= -1;
			      		}
		      		}
				}
		      	
		      	outStream.RestartStrip();
		   	}

			fixed4 frag (GSOut i) : COLOR
	        {
				clip(1 - pow(i.tex.x * 2 - 1, 2) - pow(i.tex.y * 2 - 1, 2));
	            return _Color;
	        }

			ENDCG
		}
	}
}
