/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEngine.UI;


namespace Sokuhatiku.ChartRecordingTools
{
	[AddComponentMenu("ChartRecordingTools/Graphic/Grid")]
	public class Grid : ChartGraphicBase
	{
		const string SHADER_NAME = "UI/ChartRecorder/Grid";

		public float GridRadius = 4f;
		public float subGridRadius = 2f;
		public Color subGridColor = new Color(1, 1, 1, 0.25f);

		int sid_Color = 0,
			sid_SubColor = 0,
			sid_Offset = 0,
			sid_Size = 0,
			sid_Division = 0;

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			OnUpdateScope();
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();

			var shader = Shader.Find(SHADER_NAME);
			if (shader) material = new Material(shader);
			else
			{
				enabled = false;
				return;
			}

			sid_Color = Shader.PropertyToID("_Color");
			sid_SubColor = Shader.PropertyToID("_SubColor");
			sid_Offset = Shader.PropertyToID("_Offset");
			sid_Size = Shader.PropertyToID("_Size");
			sid_Division = Shader.PropertyToID("_Division");

			OnUpdateScope();
		}

		protected override void OnUpdateScope()
		{
			if (!material || !scope) return;

			var rect = scope.ScopeRect;
			var port = rectTransform.rect;
			var offset = new Vector4(rect.x / rect.width, rect.y / rect.height, 0, 0);

			var division = new Vector4(
				rect.size.x / scope.GridCellSize.x,
				rect.size.y / scope.GridCellSize.y);
			division.z = division.x * scope.GridSubdivisionX;
			division.w = division.y * scope.GridSubdivisionY;

			var size = new Vector4(
				GridRadius * division.x / port.width,
				GridRadius * division.y / port.height,
				subGridRadius * division.z / port.width,
				subGridRadius * division.w / port.height);

			material.SetVector(sid_Color, color);
			material.SetVector(sid_SubColor, subGridColor);
			material.SetVector(sid_Offset, offset);
			material.SetVector(sid_Size, size);
			material.SetVector(sid_Division, division);
		}
	}
}