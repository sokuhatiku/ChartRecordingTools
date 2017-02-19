using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GraphTool
{
	[ExecuteInEditMode]
	public class GraphHandler : MonoBehaviour
	{
		const float limit = 0.01f;
		[SerializeField]protected Vector2 graphOffset = new Vector2(0, 0);
		[SerializeField]protected Vector2 graphScope = new Vector2(5f, 200f);

		private Rect ScopeRect;

		public Rect GetScope()
		{
			return ScopeRect;
		}

		private void Start()
		{
			UpdateGraph();
		}

		private void FixedUpdate()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
				return;
#endif
			graphOffset.x = Time.time;
			UpdateGraph();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			graphScope = new Vector2(
				graphScope.x < limit ? limit : graphScope.x,
				graphScope.y < limit ? limit : graphScope.y);
			UpdateGraph();
		}
#endif

		void UpdateGraph()
		{
			
			ScopeRect = new Rect(graphOffset, graphScope);
			ScopeRect.x -= graphScope.x;
			ScopeRect.y -= (graphScope.y / 2);
			
			if(OnUpdateGraph != null)
				OnUpdateGraph();
		}

		public void AddValue(int key, float value)
		{
			if (OnAddValue != null)
				OnAddValue(key, new Vector2(Time.time, value));
		}

		public Action OnUpdateGraph;
		public Action<int, Vector2> OnAddValue;
	}

}