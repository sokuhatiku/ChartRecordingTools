using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GraphTool
{

	public class GraphAxis : GraphPartsBase
	{

		public Vector2 GridSize;

		protected override void Awake()
		{
			base.Awake();
			var handler = GetComponentInParent<GraphHandler>();
			if (handler != null) handler.RegisterAxis(this);
		}

		private void Update()
		{
			UpdateGeometry();
		}


		protected override void OnPopulateMesh(VertexHelper vh)
		{

			var gridcountX = Mathf.FloorToInt(graphScope.width / GridSize.x);
			var gridcountY = Mathf.FloorToInt(graphScope.height / GridSize.y);

			var offset = new Vector2(graphScope.x % GridSize.x, graphScope.y % GridSize.y);

			vh.Clear();
			{
				var center = new Vector2(rectTransform.rect.center.x, scopeMatrix.m13);
				if (rectTransform.rect.Contains(center))
				{
					vh.AddVert(new Vector3(rectTransform.rect.xMin, center.y - 1, 0f), color, Vector2.zero);
					vh.AddVert(new Vector3(rectTransform.rect.xMin, center.y + 1, 0f), color, Vector2.zero);
					vh.AddVert(new Vector3(rectTransform.rect.xMax, center.y + 1, 0f), color, Vector2.zero);
					vh.AddVert(new Vector3(rectTransform.rect.xMax, center.y - 1, 0f), color, Vector2.zero);

					var vertId = vh.currentVertCount - 1;
					vh.AddTriangle(vertId - 3, vertId - 2, vertId - 1);
					vh.AddTriangle(vertId - 1, vertId, vertId - 3);
				}
			}
		}
	}
}