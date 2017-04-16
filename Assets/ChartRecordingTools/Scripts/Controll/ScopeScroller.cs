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
	[RequireComponent(typeof(RectTransform)), AddComponentMenu("ChartRecordingTools/Controll/Scroller")]
	public class ScopeScroller : MonoBehaviour, IDragHandler
	{
		public Scope handler;
		public float stopThreshold = 0.001f;
		public float dampingCoefficient = 0.25f;
		public float attenuationValue = 0.1f;

		RectTransform rectTransform;
		Vector2 scale;
		Vector2 scopeSize;
		Vector2 memoryPow;

		void Reset()
		{
			handler = GetComponentInParent<Scope>();
		}

		void OnEnable()
		{
			rectTransform = GetComponent<RectTransform>();
			handler.OnUpdateScope += OnUpdateGraph;
		}

		void OnDisable()
		{
			handler.OnUpdateScope -= OnUpdateGraph;
		}

		void OnUpdateGraph()
		{
			if (handler.Size != scopeSize)
			{
				scopeSize = handler.Size;
				scale = new Vector2(
					scopeSize.x / rectTransform.rect.size.x,
					scopeSize.y / rectTransform.rect.size.y);
			}
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if (handler != null)
			{
				memoryPow = -Vector2.Scale(eventData.delta, scale);
				handler.Offset = handler.Offset + memoryPow;
			}
		}

		void Update()
		{
			if (memoryPow != Vector2.zero)
			{
				memoryPow -= memoryPow * dampingCoefficient * Time.deltaTime;
				memoryPow -= new Vector2(
					Mathf.Sign(memoryPow.x) * Mathf.Min(Mathf.Abs(memoryPow.x), attenuationValue * scale.x),
					Mathf.Sign(memoryPow.y) * Mathf.Min(Mathf.Abs(memoryPow.y), attenuationValue * scale.y));
				handler.Offset = handler.Offset + memoryPow;
				if (memoryPow.sqrMagnitude < stopThreshold * stopThreshold)
					memoryPow = Vector2.zero;
			}
		}

	}
}