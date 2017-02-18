using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GraphTool
{
	public abstract class GraphPartsBase : MaskableGraphic,
		ILayoutElement,
		ICanvasRaycastFilter
	{
		protected Rect graphScope = new Rect(0f, 0f, 100f, 100f);
		protected Vector2 graphScale = Vector2.one;
		protected Matrix4x4 scopeMatrix;
		public void UpdateScope(Rect newScope)
		{
			graphScope = newScope;
			RecalculateScale();
		}

		protected virtual void RecalculateScale()
		{
			var rect = rectTransform.rect;
			var pivot = rectTransform.pivot;
			var scale = new Vector3(
				rect.width / graphScope.width,
				rect.height / graphScope.height, 1f);

			scopeMatrix = Matrix4x4.TRS(
				new Vector3(
					-graphScope.x*scale.x + (1 -pivot.x)*rect.width,
					-graphScope.y*scale.y + (0.5f -pivot.y)*rect.height, 0f),
				Quaternion.identity, 
				scale);
		}

		protected Vector2 TransformPoint(Vector2 point)
		{
			return scopeMatrix.MultiplyPoint3x4(point);
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