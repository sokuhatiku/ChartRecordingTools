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

			var colorwidth = max - min;
			float? prevTime = null;
			var prevColor = defaultColor;
			vh.Clear();
			var rect = rectTransform.rect;
			var scope = handler.ScopeRect;
			for (int i = 0; time.MoveNext() && data.MoveNext(); ++i)
			{
				if (time.Current == null) continue;
				else if(prevTime != null)
				{
					if (time.Current.Value > scope.xMax && prevTime.Value > scope.xMax)
					{
						prevTime = time.Current.Value;
						continue;
					}
					else if (time.Current.Value < scope.xMin && prevTime.Value < scope.xMin)
					{
						AddGradationVert(vh, prevTime.Value, prevColor);
						break;
					}
				}else
				{
					if (time.Current.Value > scope.xMax)
					{
						prevTime = time.Current.Value;
						continue;
					}
					else if (time.Current.Value < scope.xMin )
					{
						break;
					}
				}
				var color = data != null ?
					(colorwidth > 0 ? 
					colorKey.Evaluate((data.Current.Value + min) / (colorwidth)) :
					colorKey.Evaluate(data.Current.Value > min ? 1 : 0)) : 
					defaultColor;

				if(prevColor == color)
				{
					prevTime = time.Current.Value;
					continue;
				}
				else
				{
					if(prevTime != null)
					{
						AddGradationVert(vh, prevTime.Value, prevColor);
						prevTime = null;
					}
				}

				AddGradationVert(vh, time.Current.Value, color);
				prevColor = color;
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
				vh.AddTriangle(cnt - 1, cnt - 2, cnt - 3);
				vh.AddTriangle(cnt - 2, cnt - 1, cnt);
			}
		}
	}

}