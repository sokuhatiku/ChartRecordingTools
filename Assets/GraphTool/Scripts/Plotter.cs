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


		protected override void OnPopulateMesh(VertexHelper vh)
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				vh.Clear();
				return;
			}
#endif
			if (handler == null || dataKey == -1) return;

			var data = handler.GetDataEnumerator(dataKey);
			var time = handler.GetDataEnumerator(GraphHandler.SYSKEY_TIMESTAMP);

			RecalculateScale();

			vh.Clear();
			Vector2? prevPoint = null;
			var rect = rectTransform.rect;
			bool skipTrigger = false;
			for (int i = 0; time.MoveNext() && data.MoveNext(); ++i)
			{
				if (time.Current == null || data.Current == null) continue;
				var point = TransformPoint(new Vector2(time.Current.Value, data.Current.Value));
				if ((point.x < rect.xMin && prevPoint != null && prevPoint.Value.x < rect.xMin) ||
					(point.x > rect.xMax && prevPoint != null && prevPoint.Value.x > rect.xMax))
					if (skipTrigger) break; else continue;
				else
				{
					if (drawDot) AddDot(vh, new Vector3(point.x, point.y, dotFloating), dotRadius);
					if (drawLine && prevPoint != null) AddLine(vh, (Vector2)prevPoint, point, lineRadius);
					skipTrigger = true;
				}
				prevPoint = point;
			}
		}

		
	}

}