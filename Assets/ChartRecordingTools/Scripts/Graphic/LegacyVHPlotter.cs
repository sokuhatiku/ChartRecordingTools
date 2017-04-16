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
	[AddComponentMenu("ChartRecordingTools/Graphic/Plotter(Legacy)")]
	public class LegacyVHPlotter : ChartGraphicBase
	{
		[Header("Plotter"), RecorderDataKey]
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

		protected override void OnUpdateScope()
		{
			SetVerticesDirty();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();

			if (scope == null || dataKey < 0 || scope.InScopeFirstIndex < 0) return;

			var recorder = scope.GetRecorder();
			if (recorder == null || !recorder.IsKeyValid(dataKey)) return;

			var data = recorder.GetDataReader(dataKey);
			var time = recorder.GetTimeline();

			var first = scope.InScopeFirstIndex;
			var last = scope.InScopeLastIndex;
			for (; last < data.Count && data[last] == null; ++last) { }
			if (last == data.Count - 1 && data[last] == null)
				last = scope.InScopeLastIndex;

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


		protected virtual void AddDot(VertexHelper vh, Vector3 pt, float radius)
		{
			AddDot(vh, pt, radius, color);
		}

		protected virtual void AddDot(VertexHelper vh, Vector3 pt, float radius, Color32 color)
		{
			if (radius <= 0) return;
			vh.AddVert(new Vector3(pt.x, pt.y - radius, -pt.z), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x - radius, pt.y, -pt.z), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x, pt.y + radius, -pt.z), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x + radius, pt.y, -pt.z), color, Vector2.zero);

			var vertId = vh.currentVertCount - 1;
			vh.AddTriangle(vertId - 3, vertId - 2, vertId - 1);
			vh.AddTriangle(vertId - 1, vertId, vertId - 3);
		}

		protected virtual void AddLine(VertexHelper vh, Vector3 from, Vector3 to, float radius)
		{
			AddLine(vh, from, to, radius, color);
		}

		protected virtual void AddLine(VertexHelper vh, Vector3 from, Vector3 to, float radius, Color32 color)
		{
			AddLine(vh, from, color, to, color, radius);
		}

		protected virtual void AddLine(VertexHelper vh,
			Vector3 from, Color32 fromColor,
			Vector3 to, Color32 toColor, float radius)
		{
			if (radius <= 0) return;

			var dir = (to - from).normalized;
			var lSide = new Vector3(-dir.y, dir.x) * radius;
			var rSide = new Vector3(dir.y, -dir.x) * radius;

			vh.AddVert(from + lSide, fromColor, Vector2.zero);
			vh.AddVert(from + rSide, fromColor, Vector2.zero);
			vh.AddVert(to + rSide, toColor, Vector2.zero);
			vh.AddVert(to + lSide, toColor, Vector2.zero);

			var vertId = vh.currentVertCount - 1;
			vh.AddTriangle(vertId - 1, vertId - 2, vertId - 3);
			vh.AddTriangle(vertId - 3, vertId, vertId - 1);
		}
	}
}