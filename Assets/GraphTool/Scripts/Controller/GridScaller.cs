using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace GraphTool
{
	[RequireComponent(typeof(Graphic))]
	public class GridScaller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler {

		public GraphHandler handler;
		public bool direction;
		bool isPointed;

		private void Reset()
		{
			handler = GetComponentInParent<GraphHandler>();
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			isPointed = true;
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			isPointed = false;
		}

		void IScrollHandler.OnScroll(PointerEventData eventData)
		{
			if(isPointed && handler != null)
			{
				var size = handler.ScopeSize;
				var scale =
					Input.GetKey(KeyCode.LeftShift) ||
					Input.GetKey(KeyCode.RightShift) ? 0.5f : 0.1f;
				if (direction)
				{
					if (Mathf.Epsilon < Mathf.Abs(eventData.scrollDelta.y))
						size.x += -Mathf.Sign(eventData.scrollDelta.y) * size.x * scale;
				}
				else
				{
					if(Mathf.Epsilon < Mathf.Abs(eventData.scrollDelta.y))
						size.y += -Mathf.Sign(eventData.scrollDelta.y) * size.y * scale;
				}

				handler.ScopeSize = size;
			}
		}
	}
}