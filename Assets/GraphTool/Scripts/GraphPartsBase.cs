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

	public abstract class GraphPartsBase : MaskableGraphic
	{
		[SerializeField] protected GraphHandler handler;
		protected Vector2 transration;
		protected Vector2 scale;
		protected Vector2 offset;

#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			handler = GetComponentInParent<GraphHandler>();
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();
			handler = GetComponentInParent<GraphHandler>();
			if (handler != null)
			{
				handler.OnUpdateGraph += UpdateGraph;
				RecalculateScale();
				OnUpdateGraph();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (handler != null)
				handler.OnUpdateGraph -= UpdateGraph;
		}
		
		protected void UpdateGraph()
		{
			if (handler == null) return;
			RecalculateScale();
			OnUpdateGraph();
		}

		protected virtual void OnUpdateGraph()
		{
			// for overlide
		}

		protected virtual void RecalculateScale()
		{
			var scopeRect = handler.ScopeRect;
			var tfRect = rectTransform.rect;
			var pivot = rectTransform.pivot;

			transration = -scopeRect.position;
			scale = new Vector2(
				tfRect.width / scopeRect.width,
				tfRect.height / scopeRect.height);
			offset = new Vector2(
				-pivot.x * tfRect.width,
				-pivot.y * tfRect.height);
		}

		#region PointTranslators

		protected Vector2 ScopeToRect(Vector2 point)
		{
			point += transration;
			point.Scale(scale);
			point += offset;
			return point;
		}

		protected float ScopeToRectX(float x)
		{
			return (x + transration.x) * scale.x + offset.x;
		}

		protected float ScopeToRectY(float y)
		{
			return (y + transration.y) * scale.y + offset.y;
		}

		protected Vector2 RectToScope(Vector2 point)
		{
			point -= offset;
			point = new Vector2(point.x / scale.x, point.y / scale.y);
			point -= transration;
			return point;
		}

		protected float RectToScopeX(float x)
		{
			return (x - offset.x) / scale.x - transration.x;
		}

		protected float RectToScopeY(float y)
		{
			return (y - offset.y) / scale.y - transration.y;
		}

		#endregion

	}
}