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
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				vh.Clear();
				return;
			}
#endif
			if (handler == null || dataKey == -1) return;

			var data = handler.GetDataReader(dataKey);
			var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);

			var colorwidth = Mathf.Max(0, max - min);
			float? prevTime = null;
			var prevColor = defaultColor;
			vh.Clear();
			var rect = rectTransform.rect;
			var scope = handler.ScopeRect;
			if (handler.InScopeFirstIndex == -1)
				return;
			else
			{
				for (int i = handler.InScopeFirstIndex; i < handler.InScopeLastIndex ; ++i)
				{
					var color = defaultColor;
					if (data[i] != null)
					{
						color = colorwidth > 0 ?
							colorKey.Evaluate((Mathf.Clamp(data[i].Value, min, max) - min) / (colorwidth)) :
							colorKey.Evaluate(data[i].Value > min ? 1 : 0);
					}

					if (prevColor == color)
					{
						prevTime = time[i].Value;
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

					AddGradationVert(vh, time[i].Value, color);
					prevColor = color;
				}
				if (prevTime != null)
					AddGradationVert(vh, handler.ScopeRect.xMax, prevColor);
			}
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
				vh.AddTriangle(cnt , cnt - 1, cnt - 2);
			}
		}
	}

}