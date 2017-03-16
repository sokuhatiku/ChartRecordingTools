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

	public class Plotter : GraphPartsBase
	{
		[Header("Plotter"), GraphDataKey("handler")]
		public int dataKey = -1;

		[Header("Plot Option")]
		public bool drawDot = true;
		public float dotRadius = 1f;
		public float dotFloating = 0f;
		public bool drawLine = true;
		public float lineRadius = 0.25f;
		public float skipDrawAngle = 0.25f;


		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
				return;
#endif
			if (handler == null || dataKey == -1) return;
			if (handler.InScopeFirstIndex == -1) return;
			var data = handler.GetDataReader(dataKey);
			var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);
			Vector2? prevPoint = null;
			var rect = rectTransform.rect;
			for (int i = handler.InScopeFirstIndex; i < handler.InScopeLastIndex; ++i)
			{
				if (data[i] == null) continue;
				var point = ScopeToRect(new Vector2(time[i].Value, data[i].Value));

				if (drawDot) AddDot(vh, new Vector3(point.x, point.y, dotFloating), dotRadius);
				if (drawLine && prevPoint != null) AddLine(vh, prevPoint.Value, point, lineRadius);
				prevPoint = point;
			}
		}


	}

}