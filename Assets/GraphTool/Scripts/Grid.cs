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

		[Space]
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

		protected override void UpdateGraph()
		{
			if (!material) return;
			base.UpdateGraph();

			var scope = handler.ScopeRect;
			var port = rectTransform.rect;
			var offset = new Vector4(scope.x / scope.width, scope.y / scope.height, 0, 0);

			var division = new Vector4(
				scope.size.x /  handler.GridScale.x,
				scope.size.y / handler.GridScale.y);
			while (division.x > handler.GridAutoScalingThresholdX)
				division.x /= handler.GridSubdivisionX;
			while (division.y > handler.GridAutoScalingThresholdY)
				division.y /= handler.GridSubdivisionY;
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

		//protected override void OnPopulateMesh(VertexHelper vh)
		//{
		//	if (handler == null) return;

		//	//vh.Clear();
		//	//DrawGrid(vh, true);
		//	//DrawGrid(vh, false);

		//}

		//void DrawGrid(VertexHelper vh, bool direction)
		//{
		//	var gridsize = direction ?
		//		handler.GridScale.x :
		//		handler.GridScale.y;
		//	if (gridsize < 0.01f) return;

		//	var subdivision = direction ?
		//		handler.GridSubdivisionX :
		//		handler.GridSubdivisionY;
		//	if (subdivision < 1) return;

		//	var width = direction ?
		//		handler.ScopeRect.width :
		//		handler.ScopeRect.height;

		//	var start = direction ?
		//		handler.ScopeRect.xMin :
		//		handler.ScopeRect.yMin;


		//	var draws = Mathf.CeilToInt(width / gridsize) + 1;
		//	while (draws > handler.GridAutoScalingThresholdX)
		//	{ gridsize *= subdivision; draws = Mathf.CeilToInt(draws / subdivision); }
		//	draws += 2;


		//	// split to subgrid
		//	draws *= subdivision;
		//	gridsize /= subdivision;

		//	//if(direction)Debug.Log(start / gridsize);
		//	var startNum = Mathf.FloorToInt(start / gridsize);
		//	var offset = -(start % gridsize);
		//	if (offset > 0) offset -= gridsize; // Graph shape to "/|/|/|/|/|"


		//	// convert to rectTransform position
		//	var tf_set = direction ?
		//		ScopeToRectX(start + offset) :
		//		ScopeToRectY(start + offset);
		//	var tf_gain = direction ?
		//		gridsize * scale.x :
		//		gridsize * scale.y;

		//	// draw
		//	for (int i = 0; i < draws; ++i)
		//	{
		//		var isMain = (startNum + i) % subdivision == 0;
		//		var translated = tf_set + tf_gain * i;
		//		if (direction ?
		//			translated < rectTransform.rect.xMin - 1 || rectTransform.rect.xMax + 1 < translated :
		//			translated < rectTransform.rect.yMin - 1 || rectTransform.rect.yMax + 1 < translated) continue;

		//		var from = direction ?
		//			new Vector3(translated, rectTransform.rect.yMin) :
		//			new Vector3(rectTransform.rect.xMin, translated);
		//		var to = direction ?
		//			new Vector3(translated, rectTransform.rect.yMax) :
		//			new Vector3(rectTransform.rect.xMax, translated);

		//		AddLine(vh, from, to, isMain ? GridRadius : subGridRadius, isMain ? color : subGridColor);
		//	}
		//}
	}
}