/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEngine.UI;


namespace GraphTool
{

	[ExecuteInEditMode]
	public class Grid : GraphPartsBase
	{
		public float GridRadius = 1f;
		public float subGridRadius = 0.5f;
		public Color subGridColor = new Color(1,1,1,0.5f);

		int sid_Color=0, sid_SubColor=0, sid_Offset=0, sid_Size=0, sid_Division=0;

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			UpdateGraph();
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();
			
			var shader = Shader.Find("GraphTool/Grid");
			if (shader) material = new Material(shader);

			sid_Color = Shader.PropertyToID("_Color");
			sid_SubColor = Shader.PropertyToID("_SubColor");
			sid_Offset = Shader.PropertyToID("_Offset");
			sid_Size = Shader.PropertyToID("_Size");
			sid_Division = Shader.PropertyToID("_Division");
		}

		protected override void OnUpdateGraph()
		{
			if (!material) return;

			var scope = handler.ScopeRect;
			var port = rectTransform.rect;
			var offset = new Vector4(scope.x / scope.width, scope.y / scope.height, 0, 0);

			var division = new Vector4(
				scope.size.x / handler.GridCellSize.x,
				scope.size.y / handler.GridCellSize.y);
			division.z = division.x * handler.GridSubdivisionX;
			division.w = division.y * handler.GridSubdivisionY;

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