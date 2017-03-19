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

	public class GradationPlotter : GraphPartsBase
	{

		[Header("Plotter"), GraphDataKey("handler")]
		public int dataKey = -1;

		public Gradient colorKey;
		public Color defaultColor;
		public float min = 0;
		public float max = 100;

		[Range(10, 2000), Header("Load reduction")]
		public int drawsLimit = 100;
		[Range(0, 500)]
		public int freshDataProtection = 10;

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			if (min > max)
				max = min = min + (max - min) / 2;
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();
		}


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
			float? prevTime = null;
			var prevColor = defaultColor;
			if (skip > 1)
			{
				for (; i+freshDataProtection<data.Count && i <= last+skip; i+=skip)
				{
					float timeave = 0f;
					float dataave = 0f;
					int datacnt = 0;
					for (int j = i; i-skip < j && 0 <= j; --j)
					{
						if (data[j] != null)
						{
							dataave += data[j].Value;
							timeave += time[j].Value;
							++datacnt;
						}
					}

					timeave /= datacnt;
					var color = GetColor(datacnt > 0 ? dataave / datacnt : (float?)null);

					if (prevColor == color)
					{
						prevTime = timeave;
						continue;
					}
					else
					{
						if (prevTime != null)
						{
							AddGradationVert(vh, prevTime.Value, prevColor);
							prevTime = null;
						}
					}

					AddGradationVert(vh, timeave, color);
					prevColor = color;
				}
			}

			for (; i <= handler.InScopeLastIndex; ++i)
			{
				var color = GetColor(data[i]);

				if (prevColor == color)
				{
					prevTime = time[i].Value;
					continue;
				}
				else if (prevTime != null)
				{ 
					AddGradationVert(vh, prevTime.Value, prevColor);
					prevTime = null;
				}

				AddGradationVert(vh, time[i].Value, color);
				prevColor = color;
			}

			if (prevTime != null)
				AddGradationVert(vh, Mathf.Min(time.LatestValue.Value, handler.ScopeRect.xMax), prevColor);
		}

		Color GetColor(float? data)
		{
			var gradWidth = Mathf.Max(0, max - min);
			if(data != null)
				return gradWidth > 0 ?
					colorKey.Evaluate((Mathf.Clamp(data.Value, min, max) - min) / (gradWidth)) :
					colorKey.Evaluate(data.Value > min ? 1 : 0);
			return defaultColor;
		}

		void AddGradationVert(VertexHelper vh, float time, Color color)
		{
			var x = ScopeToRectX(time);
			vh.AddVert(new Vector3(x, rectTransform.rect.yMin), color, Vector2.zero);
			vh.AddVert(new Vector3(x, rectTransform.rect.yMax), color, Vector2.zero);
			if (vh.currentVertCount > 2)
			{
				var cnt = vh.currentVertCount - 1;
				vh.AddTriangle(cnt - 3, cnt - 2, cnt - 1);
				vh.AddTriangle(cnt, cnt - 1, cnt - 2);
			}
		}
	}

}