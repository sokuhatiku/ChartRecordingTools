/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTool
{
	[ExecuteInEditMode]
	public class GraphHandler : MonoBehaviour
	{
		const float limit = 0.01f;
		
		[Header("Scope")]
		[SerializeField]protected Vector2 _scopeOffset = new Vector2(0, 0);
		[SerializeField]protected Vector2 _scopeSize = new Vector2(5f, 200f);
		[SerializeField]protected Vector2 _scopeMargin = new Vector2(0f, 10f);
		[SerializeField]protected bool _scopeUnsigned = false;

		public enum UpdateTiming
		{
			EveryFrame,
			ValueUpdate,
			Manual,
		}
		[SerializeField]protected UpdateTiming _updateTiming = UpdateTiming.EveryFrame;



		private Rect ScopeRect;

		public Rect GetScope()
		{
			return ScopeRect;
		}

		public Action OnUpdateGraph;

		public void UpdateGraph()
		{
			ScopeRect = new Rect(_scopeOffset, _scopeSize);
			ScopeRect.x -= _scopeSize.x;
			if (!_scopeUnsigned) ScopeRect.y -= _scopeSize.y / 2;
			ScopeRect.max += _scopeMargin;
			ScopeRect.min -= _scopeMargin;

			if (OnUpdateGraph != null)
				OnUpdateGraph();
		}


		[SerializeField, HideInInspector]List<Data> dataList = new List<Data>();

		public bool HasData(int dataKey)
		{
			return 0 <= dataKey && dataKey < dataList.Count && dataList[dataKey] != null;
		}

		public IEnumerator<float?> GetDataEnumerator(int dataKey)
		{
			if (!HasData(dataKey)) RegisterDataInternal(dataKey, new Data());
			return dataList[dataKey].GetEnumerator();
		}

		public float? GetCurrentData(int dataKey)
		{
			if (!HasData(dataKey)) RegisterDataInternal(dataKey, new Data());
			return dataList[dataKey].GetCurrent();
		}

		public void SetCurrentData(int dataKey, float value)
		{
			if (!HasData(dataKey)) RegisterDataInternal(dataKey, new Data());
			dataList[dataKey].SetCurrent(value);
		}

		public int RegisterData (Data data)
		{
			dataList.Add(data);
			return dataList.Count - 1;
		}

		public void RegisterDataAt(int dataKey, Data data)
		{
			if (HasData(dataKey)) throw new ArgumentException("data exists", "dataKey");
			RegisterDataInternal(dataKey, data);
		}

		private void RegisterDataInternal(int dataKey, Data data)
		{
			if (dataKey >= dataList.Count) dataList.Insert(dataKey, data);
			else dataList[dataKey] = data;
		}


		[SerializeField, HideInInspector]int timestampKey = -1;

		public int getTimestampKey()
		{
			if (timestampKey == -1) throw new Exception("Component has not been initialized");
			return timestampKey;
		}



		private void OnValidate()
		{
			_scopeSize = new Vector2(
				_scopeSize.x < limit ? limit : _scopeSize.x,
				_scopeSize.y < limit ? limit : _scopeSize.y);
			_scopeMargin = new Vector2(
				_scopeMargin.x < 0 ? 0 : _scopeMargin.x,
				_scopeMargin.y < 0 ? 0 : _scopeMargin.y);
			UpdateGraph();
		}

		private void Reset()
		{
			timestampKey = RegisterData(new Data());
#if UNITY_EDITOR
			editorOnlyDataKeyword.Insert(timestampKey, "_Timestamp");
#endif
		}

		private void Start()
		{
			UpdateGraph();
		}

		private void LateUpdate()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) return;
#endif
			dataList[timestampKey].SetCurrent(Time.time);
			for(int key=0; key<dataList.Count; ++key)
				dataList[key].Determine();

			if (_updateTiming == UpdateTiming.EveryFrame)
			{
				_scopeOffset.x = Time.time;
				UpdateGraph();
			}
		}

#if UNITY_EDITOR
		[SerializeField] List<string> editorOnlyDataKeyword = new List<string>();
#endif

	}

}