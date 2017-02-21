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


		[SerializeField]List<Data> dataList = new List<Data>();

		public bool HasData(int dataKey)
		{
			return 0 <= dataKey && dataKey < dataList.Count && dataList[dataKey] != null;
		}

		public IEnumerator<float?> GetDataEnumerator(int dataKey)
		{
			if (!HasData(dataKey)) RegisterInternal(dataKey, new Data("data " + dataKey));
			return dataList[dataKey].GetEnumerator();
		}

		public float? GetCurrentData(int dataKey)
		{
			if (!HasData(dataKey)) RegisterInternal(dataKey, new Data("data " + dataKey));
			return dataList[dataKey].GetCurrent();
		}

		public void SetCurrentData(int dataKey, float value)
		{
			if (!HasData(dataKey)) RegisterInternal(dataKey, new Data("data " + dataKey));
			if (dataKey < COUNT_SYSKEY) throw new ArgumentException("System data can not set data.", "dataKey");
			dataList[dataKey].SetCurrent(value);
		}

		public int Register (Data data)
		{
			dataList.Add(data);
			return dataList.Count - 1;
		}

		public void Unregister(int dataKey)
		{
			if (!HasData(dataKey)) throw new ArgumentException("Data does not exist at specified key.", "dataKey");
			if (dataKey < COUNT_SYSKEY) throw new ArgumentException("System data is not Unregisteable.", "dataKey");
			dataList[dataKey] = null;
		}

		public void RegisterAt(int dataKey, Data data)
		{
			if (HasData(dataKey)) throw new ArgumentException("Data already exists at specified key.", "dataKey");
			RegisterInternal(dataKey, data);
		}

		private void RegisterInternal(int dataKey, Data data)
		{
			if (dataKey >= dataList.Count) dataList.Insert(dataKey, data);
			else dataList[dataKey] = data;
		}


		public const int COUNT_SYSKEY = 1;
		public const int SYSKEY_TIMESTAMP = 0;



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
			RegisterInternal(SYSKEY_TIMESTAMP, new Data("Timestamp"));
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
			dataList[SYSKEY_TIMESTAMP].SetCurrent(Time.time);
			for(int key=0; key<dataList.Count; ++key)
				dataList[key].Determine();

			if (_updateTiming == UpdateTiming.EveryFrame)
			{
				_scopeOffset.x = Time.time;
				UpdateGraph();
			}
		}

	}

}