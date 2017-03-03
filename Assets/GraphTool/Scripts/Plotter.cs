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

			vh.Clear();
			Vector2? prevPoint = null;
			var rect = rectTransform.rect;
			for (int i = 0; time.MoveNext() && data.MoveNext(); ++i)
			{
				if (time.Current == null || data.Current == null) continue;
				var point = ScopeToRect(new Vector2(time.Current.Value, data.Current.Value));
				if (prevPoint != null)
				{
					if (point.x > rect.xMax && prevPoint.Value.x > rect.xMax) continue;
					else if (point.x < rect.xMin && prevPoint.Value.x < rect.xMin) break;
					else if (Mathf.Abs(prevPoint.Value.x - point.x) < 1f) continue;
				}else
				{
					if (point.x > rect.xMax) continue;
					else if (point.x < rect.xMin) break;
				}

				if (drawDot) AddDot(vh, new Vector3(point.x, point.y, dotFloating), dotRadius);
				if (drawLine && prevPoint != null)	AddLine(vh, prevPoint.Value, point, lineRadius);
				prevPoint = point;
			}
		}

		
	}

}