Shader "GraphTool/Grid" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_SubColor("SubColor", Color) = (1,1,1,0.5)
		_Offset("Offset", Vector) = (0,0,0,0)
		_Main("Main", Vector) = (0.1,0.1,1,1) // Size, Division
		_Sub("Sub", Vector) = (0.05,0.05,10,10) // Size, Division
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

		Pass // sub grid
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform fixed4 _SubColor;
			uniform fixed4 _Offset;
			uniform fixed4 _Sub;

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed x = frac((i.uv.x + _Offset.x) * _Sub.z - _Sub.x / 2) + _Sub.x;
				fixed y = frac((i.uv.y + _Offset.y) * _Sub.w - _Sub.y / 2) + _Sub.y;

				clip(max(x, y) - 1);
				return _SubColor;
			}

			ENDCG
		}

		Pass // main grid
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform fixed4 _Color;
			uniform fixed4 _Offset;
			uniform fixed4 _Main;

			fixed4 frag(v2f_img i) : COLOR
			{
				fixed x = frac((i.uv.x + _Offset.x) * _Main.z - _Main.x / 2) + _Main.x;
				fixed y = frac((i.uv.y + _Offset.y) * _Main.w - _Main.y / 2) + _Main.y;

				clip(max(x, y) - 1);
				return _Color;
			}

			ENDCG
		}
	}

	SubShader
	{
		Pass{ Color(0,0,0,1) }
	}
}
