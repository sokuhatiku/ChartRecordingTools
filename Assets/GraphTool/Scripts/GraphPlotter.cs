using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace GraphTool
{

	public class GraphPlotter : MaskableGraphic,
		//ISerializationCallbackReceiver,
		ILayoutElement,
		ICanvasRaycastFilter
	{
		[Header("Plot Option")]
		public bool drawDot = true;
		public float dotRadius = 1f;
		public float dotFloating = 0f;
		public bool drawLine = true;
		public float lineRadius = 0.25f;
		public int capacity = 100;


		protected Rect graphScope = new Rect(0f,50f, 100f, 100f);
		protected Vector2 graphScale = Vector2.one;
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

		public void UpdateScope(Rect newScope)
		{
			graphScope = newScope;
			RecalculateScale();
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

		void RecalculateScale()
		{
			graphScale = new Vector2(rectTransform.rect.width / graphScope.width, rectTransform.rect.height / graphScope.height);
		}

		void DrawGraph(VertexHelper vh)
		{
			if (points == null) return;
			vh.Clear();
			Vector2 prevPoint = Vector2.zero;
			var rect = rectTransform.rect;
			for (int i=0; i< points.Count; ++i)
			{
				var point = new Vector2((points[i].x - graphScope.xMin) * graphScale.x + rect.width/2 ,
					(points[i].y - graphScope.yMin) * graphScale.y);
				if (rect.Contains(point) || rect.Contains(prevPoint))
				{
					if (drawDot) AddDot(vh, point);
					if (drawLine && i > 0 ) AddLine(vh, prevPoint, point);
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

		public void CalculateLayoutInputHorizontal() { }
		public void CalculateLayoutInputVertical() { }

		public float minWidth { get { return 0; } }
		public float preferredWidth { get { return 0; } }
		public float flexibleWidth { get { return -1; } }

		public float minHeight { get { return 0; } }
		public float preferredHeight { get { return 0; } }
		public float flexibleHeight { get { return -1; } }

		public int layoutPriority { get { return 0; } }
		

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return false;
		}
	}

}