/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Sokuhatiku.ChartRecordingTools
{
	[RequireComponent(typeof(RectTransform)), AddComponentMenu("ChartRecordingTools/Controll/Scaller")]
	public class ScopeScaller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
	{

		public Scope targetScope;
		public bool direction;
		bool isPointed;

		private void Reset()
		{
			targetScope = GetComponentInParent<Scope>();
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
			if (isPointed && targetScope != null)
			{
				var size = targetScope.Size;
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
					if (Mathf.Epsilon < Mathf.Abs(eventData.scrollDelta.y))
						size.y += -Mathf.Sign(eventData.scrollDelta.y) * size.y * scale;
				}

				targetScope.Size = size;
			}
		}
	}
}