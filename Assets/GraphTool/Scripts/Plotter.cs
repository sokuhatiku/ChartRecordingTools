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
		public bool CutoffDatalessFrame = true;

		[Range(10, 2000), Header("Load reduction")]
		public int drawsLimit = 100;
		[Range(0, 500)]
		public int freshDataProtection = 10;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (handler == null || dataKey == -1 || !handler.IsKeyValid(dataKey)) return;
			if (handler.InScopeFirstIndex == -1) return;
			var data = handler.GetDataReader(dataKey);
			var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);

			var first = handler.InScopeFirstIndex;
			var last = handler.InScopeLastIndex;
			for (; last < data.Count && data[last] == null; ++last) { }
			if (last == data.Count - 1 && data[last] == null)
				last = handler.InScopeLastIndex;

			var draws = last - first;
			var skip = draws > drawsLimit ? Mathf.CeilToInt(draws / drawsLimit) : 1;
			first -= first % skip;

			int i = first;
			Vector2? prevPoint = null;
			if (skip > 1)
			{
				for (; i + skip + freshDataProtection < data.Count && i <= last + skip; i += skip)
				{
					float timeave = 0f;
					float dataave = 0f;
					int datacnt = 0;
					for (int j = i; 0 <= j && i - skip < j; --j)
					{
						if (data[j] != null)
						{
							dataave += data[j].Value;
							timeave += time[j].Value;
							++datacnt;
						}
					}
					if (datacnt == 0)
					{
						if (CutoffDatalessFrame) prevPoint = null;
						continue;
					}

					Plot(vh, timeave / datacnt, dataave / datacnt, ref prevPoint);
				}
			}

			for (; i < data.Count && i <= last; ++i)
			{
				if (data[i] == null)
				{
					if (CutoffDatalessFrame) prevPoint = null;
					continue;
				}

				Plot(vh, time[i].Value, data[i].Value, ref prevPoint);
			}
			drawCnt = 0;
		}
		int drawCnt;
		void Plot(VertexHelper vh, float time, float data, ref Vector2? prevPoint)
		{
			var point = ScopeToRect(new Vector2(time, data));

			if (drawDot) AddDot(vh, new Vector3(point.x, point.y, dotFloating), dotRadius);
			if (drawLine && prevPoint != null) AddLine(vh, prevPoint.Value, point, lineRadius);
			prevPoint = point;
			drawCnt++;
		}

	}
}