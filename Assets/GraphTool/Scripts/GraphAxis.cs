using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GraphTool
{

	public class GraphAxis : MaskableGraphic,
		ILayoutElement,
		ICanvasRaycastFilter
	{

		Rect scope;

		public void OnUpdateScope(Rect newScope)
		{
			this.scope = newScope;
			UpdateGeometry();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			
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