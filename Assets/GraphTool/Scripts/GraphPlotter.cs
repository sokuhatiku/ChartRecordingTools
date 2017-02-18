using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace GraphTool
{

	public class GraphPlotter : GraphPartsBase
	{
		[Header("Plot Option")]
		public bool drawDot = true;
		public float dotRadius = 1f;
		public float dotFloating = 0f;
		public bool drawLine = true;
		public float lineRadius = 0.25f;
		[Range(1, 1000)]
		public int capacity = 100;


		protected List<Vector2> points = new List<Vector2>();

		protected override void Awake()
		{
			base.Awake();
			var handler = GetComponentInParent<GraphHandler>();
			if (handler != null) handler.RegisterPlotter(this);
		}

		private void Update()
		{
			UpdateGeometry();
		}

		public void AddPoint(Vector2 point)
		{
			while (points.Count >= capacity)
				points.RemoveAt(capacity - 1);
			points.Insert(0, point);
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			RecalculateScale();
			DrawGraph(vh);
		}

		void DrawGraph(VertexHelper vh)
		{
			if (points == null) return;
			vh.Clear();
			Vector2 prevPoint = Vector2.zero;
			var rect = rectTransform.rect;
			bool skipTrigger = false;
			for (int i=0; i< points.Count; ++i)
			{
				var point = TransformPoint(points[i]);
				if ((point.x < rect.xMin && prevPoint.x < rect.xMin) ||
					(point.x > rect.xMax && prevPoint.x > rect.xMax))
					if (skipTrigger) break; else continue;
				else
				{
					if (drawDot) AddDot(vh, point);
					if (drawLine && i > 0) AddLine(vh, prevPoint, point);
					skipTrigger = true;
				}
				prevPoint = point;
			}
		}

		protected virtual void AddDot(VertexHelper vh, Vector2 pt)
		{
			vh.AddVert(new Vector3(pt.x, pt.y - dotRadius, -dotFloating), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x - dotRadius, pt.y, -dotFloating), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x, pt.y + dotRadius, -dotFloating), color, Vector2.zero);
			vh.AddVert(new Vector3(pt.x + dotRadius, pt.y, -dotFloating), color, Vector2.zero);

			var vertId = vh.currentVertCount-1;
			vh.AddTriangle(vertId-3, vertId-2, vertId-1);
			vh.AddTriangle(vertId-1, vertId  , vertId-3);

		}

		protected virtual void AddLine(VertexHelper vh, Vector2 from, Vector2 to)
		{
			var dir = (to - from).normalized;
			var lSide = new Vector2(-dir.y,dir.x) * lineRadius;
			var rSide = new Vector2(dir.y,-dir.x) * lineRadius;

			vh.AddVert(from + lSide, color, Vector2.zero);
			vh.AddVert(from + rSide, color, Vector2.zero);
			vh.AddVert(to + rSide, color, Vector2.zero);
			vh.AddVert(to + lSide, color, Vector2.zero);
			
			var vertId = vh.currentVertCount - 1;
			vh.AddTriangle(vertId - 3, vertId - 2, vertId-1);
			vh.AddTriangle(vertId - 1, vertId, vertId - 3);
		}
	}

}