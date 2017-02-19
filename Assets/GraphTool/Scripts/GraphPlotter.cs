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
		[Header("Plotter")]
		public string keyword;
		[Header("Plot Option")]
		public bool drawDot = true;
		public float dotRadius = 1f;
		public float dotFloating = 0f;
		public bool drawLine = true;
		public float lineRadius = 0.25f;
		[Range(1, 1000)]
		public int capacity = 100;

		protected int key;
		protected List<Vector2> points = new List<Vector2>();

		protected override void OnEnable()
		{
			base.OnEnable();
			if (handler != null)
			{
				key = handler.GetKey(keyword);
				handler.OnAddValue += AddValue;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (handler != null)
				handler.OnAddValue -= AddValue;
		}

		void AddValue(int key, Vector2 value)
		{
			if (key != this.key) return;
			while (points.Count >= capacity)
				points.RemoveAt(capacity - 1);
			points.Insert(0, value);
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null || points == null) return;

			RecalculateScale();

			vh.Clear();
			Vector2 prevPoint = Vector2.zero;
			var rect = rectTransform.rect;
			bool skipTrigger = false;
			for (int i = 0; i < points.Count; ++i)
			{
				var point = TransformPoint(points[i]);
				if ((point.x < rect.xMin && prevPoint.x < rect.xMin) ||
					(point.x > rect.xMax && prevPoint.x > rect.xMax))
					if (skipTrigger) break; else continue;
				else
				{
					if (drawDot) AddDot(vh, new Vector3(point.x, point.y, dotFloating), dotRadius);
					if (drawLine && i > 0) AddLine(vh, prevPoint, point, lineRadius);
					skipTrigger = true;
				}
				prevPoint = point;
			}
		}
	}

}