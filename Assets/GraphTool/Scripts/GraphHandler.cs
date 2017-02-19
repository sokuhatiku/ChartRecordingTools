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

		public enum UpdateType
		{
			EveryFrame,
			ValueUpdate,
			None,
		}
		public UpdateType updateType = UpdateType.EveryFrame;

		private Rect ScopeRect;
		Dictionary<string, int> keyList = new Dictionary<string, int>();

		public Rect GetScope()
		{
			return ScopeRect;
		}

		public int GetKey(string keyword)
		{
			if (keyList.ContainsKey(keyword))
				return keyList[keyword];
			else
			{
				var key = keyList.Count + 1;
				keyList.Add(keyword, key);
				return key;
			}
		}

#if UNITY_EDITOR
		private void Start()
		{
			UpdateGraph();
		}
#endif

		private void Update()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
			if (updateType == UpdateType.EveryFrame)
			{
				graphOffset.x = Time.time;
				UpdateGraph();
			}
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
			if (updateType == UpdateType.ValueUpdate)
			{
				graphOffset.x = Time.time;
				UpdateGraph();
			}

		}

		public Action OnUpdateGraph;
		public Action<int, Vector2> OnAddValue;
	}

}