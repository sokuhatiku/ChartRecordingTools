using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace GraphTool
{
	[RequireComponent(typeof(RectTransform))]
	public class ScopeDragger : MonoBehaviour, IDragHandler
	{
		public GraphHandler handler;
		public float stopThreshold = 0.001f;
		public float dampingCoefficient = 0.25f;
		public float attenuationValue = 0.1f;

		RectTransform rectTransform;
		Vector2 scale;
		Vector2 _scopeSize;
		Vector2 memoryPow;

		void Reset()
		{
			handler = GetComponentInParent<GraphHandler>();
		}

		void OnEnable()
		{
			rectTransform = GetComponent<RectTransform>();
			handler.OnUpdateGraph += OnUpdateGraph;
		}

		void OnDisable()
		{
			handler.OnUpdateGraph -= OnUpdateGraph;
		}

		void OnUpdateGraph()
		{
			if (handler.ScopeSize != _scopeSize)
			{
				_scopeSize = handler.ScopeSize;
				scale = new Vector2(
					_scopeSize.x / rectTransform.rect.size.x,
					_scopeSize.y / rectTransform.rect.size.y);
			}
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			if(handler != null)
			{
				memoryPow = -Vector2.Scale(eventData.delta, scale);
				handler.ScopeOffset = handler.ScopeOffset + memoryPow;
			}
		}

		void Update()
		{
			if(memoryPow != Vector2.zero)
			{
				memoryPow -= memoryPow * dampingCoefficient * Time.deltaTime;
				memoryPow -= new Vector2(
					Mathf.Sign(memoryPow.x) * Mathf.Min(Mathf.Abs(memoryPow.x), attenuationValue * scale.x),
					Mathf.Sign(memoryPow.y) * Mathf.Min(Mathf.Abs(memoryPow.y), attenuationValue * scale.y));
				handler.ScopeOffset = handler.ScopeOffset + memoryPow;
				if (memoryPow.sqrMagnitude < stopThreshold * stopThreshold)
					memoryPow = Vector2.zero;
			}
		}
		
	}
}