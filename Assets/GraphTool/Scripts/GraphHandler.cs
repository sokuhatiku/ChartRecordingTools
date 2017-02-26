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
		[SerializeField]protected Vector2 _scopeScale = new Vector2(5f, 200f);
		[SerializeField]protected Vector2 _scopeMargin = new Vector2(0f, 10f);
		[SerializeField]protected bool _scopeUnsigned = false;

		public enum UpdateTiming
		{
			EveryFrame,
			ValueUpdate,
			Manual,
		}
		[SerializeField]protected UpdateTiming _updateTiming = UpdateTiming.EveryFrame;

		private Rect _scopeRect;
		public Rect ScopeRect { get { return _scopeRect; } }
		[SerializeField]protected Vector2 _gridScale = new Vector2(1, 10);
		public Vector2 GridScale{ get { return _gridScale; } }
		[SerializeField]protected int _gridXSubdivision = 10;
		public int GridSubdivisionX { get { return _gridXSubdivision; } }
		[SerializeField]protected int _gridYSubdivision = 10;
		public int GridSubdivisionY { get { return _gridYSubdivision; } }
		[SerializeField]protected int _gridAutoScalingThreshold = 15;
		public int GridAutoScalingThreshold { get { return _gridAutoScalingThreshold; } }

		private void OnValidate()
		{
			_scopeScale = new Vector2(
				_scopeScale.x < limit ? limit : _scopeScale.x,
				_scopeScale.y < limit ? limit : _scopeScale.y);
			_scopeMargin = new Vector2(
				_scopeMargin.x < 0 ? 0 : _scopeMargin.x,
				_scopeMargin.y < 0 ? 0 : _scopeMargin.y);

			_gridScale = new Vector2(
				_gridScale.x < limit ? limit : _gridScale.x,
				_gridScale.y < limit ? limit : _gridScale.y);
			_gridXSubdivision = Mathf.Max(_gridXSubdivision, 1);
			_gridYSubdivision = Mathf.Max(_gridYSubdivision, 1);

			_gridAutoScalingThreshold = Mathf.Max(10, _gridAutoScalingThreshold);

			UpdateGraph();
		}

		private void OnRectTransformDimensionsChange()
		{
			UpdateGraph();
		}

		public Action OnUpdateGraph;

		public void UpdateGraph()
		{
			_scopeRect = new Rect(_scopeOffset, _scopeScale);
			_scopeRect.x -= _scopeScale.x;
			if (!_scopeUnsigned) _scopeRect.y -= _scopeScale.y / 2;
			_scopeRect.max += _scopeMargin;
			_scopeRect.min -= _scopeMargin;

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
			if (dataKey < 0) throw new ArgumentException("Data does not exist at specified key.", "dataKey");
			if (!HasData(dataKey)) RegisterInternal(dataKey, new Data("data " + dataKey));
			return dataList[dataKey].GetEnumerator();
		}

		public float? GetData(int dataKey)
		{
			if (dataKey < 0) return null;
			if (!HasData(dataKey)) RegisterInternal(dataKey, new Data("data " + dataKey));
			return dataList[dataKey].GetCurrent();
		}

		public void SetData(int dataKey, float value)
		{
			if (dataKey < 0) return;
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