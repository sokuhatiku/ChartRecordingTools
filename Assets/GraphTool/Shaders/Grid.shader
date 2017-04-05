/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

Shader "GraphTool/Grid"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_SubColor("SubColor", Color) = (1,1,1,0.5)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Size("Size", Vector) = (0.1,0.1,0.05,0.05)
		_Division("Division", Vector) = (1,1,10,10)
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
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform fixed4 _Offset;
			uniform fixed4 _Color;
			uniform fixed4 _SubColor;
			uniform fixed4 _Size;
			uniform fixed4 _Division;

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed main = max(
					frac((i.uv.x + _Offset.x) * _Division.x - _Size.x / 2) + _Size.x,
					frac((i.uv.y + _Offset.y) * _Division.y - _Size.y / 2) + _Size.y);

				fixed sub = max(
					frac((i.uv.x + _Offset.x) * _Division.z - _Size.z / 2) + _Size.z,
					frac((i.uv.y + _Offset.y) * _Division.w - _Size.w / 2) + _Size.w);

				clip(max(main, sub) -1);
				return lerp(_Color, _SubColor, step(main, 1));
			}

			ENDCG
		}
	}

	SubShader
	{
		Pass{ Color(0,0,0,0) }
	}
}
