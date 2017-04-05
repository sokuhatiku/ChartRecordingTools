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
		_Color("Color", COLOR) = (1,1,1,1)
		_PointsCount("Points", int) = 0
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

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma target 5.0
			#pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma geometry geom
	        #pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct PointData
			{
				float2 pos;
				bool drawLine;
			};
			
			StructuredBuffer<PointData> Points;
			int _PointsCount;
			float _Scale;
			fixed4 _Color;
			float4 _ClippingRect;
			float4x4 _S2LMatrix;
			float4x4 _L2WMatrix;

			struct AppData
			{
				uint id : SV_VertexID;
				float4 pos : POSITION;
				
			};

			struct VSOut 
			{
	            float4 pos0 : POSITION0;
				float4 pos1 : POSITION1;
				float drawLine : PSIZE;
				
	        };

			VSOut vert (AppData i)
			{
				VSOut o;

				uint id = floor(i.id / 3);
				uint m =  max(0,_PointsCount-1);
				o.pos0 = mul(_S2LMatrix, float4(Points[min(id , m)].pos, 0, 1));
				o.pos1 = mul(_S2LMatrix, float4(Points[min(id+1, m)].pos, 0, 1));
				o.drawLine = Points[id].drawLine;
				return o;
			}

			struct GSOut
			{
				float4 pos : SV_POSITION;
	            float2 tex : TEXCOORD0;
				float4 wPos : TEXCOORD1;
			};

		   	[maxvertexcount(8)]
		   	void geom (point VSOut input[1], inout TriangleStream<GSOut> outStream)
		   	{
		     	GSOut output;
		     	
				float2 offsH = input[0].pos1 - input[0].pos0;
				offsH = normalize(offsH) * _Scale;
				float2 offsV = float2(offsH.y, -offsH.x);
				bool drawLine = asint(input[0].drawLine);
				
				for(int l =0; l < 2; l++)
				{
					float4 pos = lerp(input[0].pos0, input[0].pos1, l * drawLine);
		      		for(int u = 0; u < 2; u++)
		      		{
						offsH *= -1;
						for(int v = 0; v < 2; v++)
						{
			        		output.tex = float2(l+u, v);
							output.tex.x /= 2;

			      			float4 ppos = pos + float4(offsV + offsH * (1 - l^u), 0, 0);
							output.wPos = ppos;
			          		output.pos = mul(UNITY_MATRIX_VP, mul(_L2WMatrix, ppos));

				      		outStream.Append (output);
							offsV *= -1;
			      		}
		      		}
				}
		      	
		      	outStream.RestartStrip();
		   	}

			fixed4 frag (GSOut i) : COLOR
	        {
				clip(
					min(
						1 - pow(i.tex.x * 2 - 1, 2) - pow(i.tex.y * 2 - 1, 2),
						UnityGet2DClipping(i.wPos.xy, _ClippingRect) - 0.5
					)
				);
				return _Color;
	        }

		ENDCG
		}
	}
}
