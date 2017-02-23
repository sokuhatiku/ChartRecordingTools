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
		const float limit = 1f / 60f;

		public Vector2 GridSpacing = new Vector2(1f,10f);
		public float GridRadius = 1f;

		[Space]
		public int xSubdivision = 1;
		public int ySubdivision = 10;
		public float subGridRadius = 0.5f;
		public Color subGridColor = new Color(1,1,1,0.5f);


#if UNITY_EDITOR
		protected override void OnValidate()
		{
			GridSpacing = new Vector2(
				GridSpacing.x < limit ? limit : GridSpacing.x,
				GridSpacing.y < limit ? limit : GridSpacing.y);
			xSubdivision = Mathf.Max(xSubdivision, 1);
			ySubdivision = Mathf.Max(ySubdivision, 1);
			UpdateGraph();
		}
#endif

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null) return;

			vh.Clear();
			if (GridRadius <= 0) return;

			var xSpacing = GridSpacing.x / xSubdivision;
			var ySpacing = GridSpacing.y / ySubdivision;
			var scope = handler.GetScope();
			scope.xMin -= xSpacing;
			scope.xMax += xSpacing;
			scope.yMin -= ySpacing;
			scope.yMax += ySpacing;


			bool SkipTrigger = false;
			float xOffset = scope.xMin - (scope.xMin % GridSpacing.x);
			int xCount = Mathf.CeilToInt(scope.width / GridSpacing.x) * xSubdivision;
			if (scope.x >= 0) xCount += xSubdivision;
			for (int i= scope.x < 0 ? -xSubdivision : 0 ; i < xCount ; ++i)
			{
				var x = xOffset + xSpacing * i;
				if (!scope.Contains(new Vector2(x, scope.center.y)))
					if (!SkipTrigger) continue; else break;
				else SkipTrigger = true;

				var from = ScopeToRect(new Vector3(x, scope.yMin));
				var to = ScopeToRect(new Vector3(x, scope.yMax));

				var isMain = i % xSubdivision == 0;
				AddLine(vh, from, to, isMain ? GridRadius : subGridRadius, isMain ? color : subGridColor);
			}

			SkipTrigger = false;
			float yOffset = scope.yMin - (scope.yMin % GridSpacing.y);
			int yCount = Mathf.CeilToInt(scope.height / GridSpacing.y) * ySubdivision;
			if (scope.y >= 0) yCount += ySubdivision;
			for (int i = scope.y < 0 ? -ySubdivision : 0 ; i < yCount ; ++i)
			{
				var y = yOffset + ySpacing * i;
				if (!scope.Contains(new Vector2(scope.center.x, y)))
					if (!SkipTrigger) continue; else break;
				else SkipTrigger = true;

				var from = ScopeToRect(new Vector3(scope.xMin, y));
				var to = ScopeToRect(new Vector3(scope.xMax, y));

				var isMain = i % ySubdivision == 0;
				AddLine(vh, from, to, isMain ? GridRadius : subGridRadius, isMain ? color : subGridColor);
			}

		}
	}
}