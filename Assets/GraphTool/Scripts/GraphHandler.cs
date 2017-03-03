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
		const float DENOMINATOR_MIN = 0.01f;

		public const int COUNT_SYSKEY = 1;
		public const int SYSKEY_TIMESTAMP = 0;

		[SerializeField]List<Data> dataList = new List<Data>();

		float _startTime = 0f;
		public bool recording = true;
		public void RecordToggle(bool toggle)
		{
			recording = toggle;
		}
		public bool autoDetermine = true;
		public bool autoScopeOffset = true;
		public bool acceptUnregisteredKey = false;


		[SerializeField]protected Vector2 _scopeOffset = new Vector2(0, 0);
		[SerializeField]protected Vector2 _scopeScale = new Vector2(5f, 200f);
		[SerializeField]protected Vector2 _scopeMargin = new Vector2(0f, 10f);
		[SerializeField]protected bool _scopeUnsigned = false;

		[SerializeField]protected Vector2 _gridScale = new Vector2(1, 10);
		public Vector2 GridScale
		{
			get { return _gridScale; }
			set { _gridScale = new Vector2(Mathf.Max(value.x, DENOMINATOR_MIN), Mathf.Max(value.y, DENOMINATOR_MIN)); }
		}

		[SerializeField]protected int _gridXSubdivision = 10;
		public int GridSubdivisionX
		{
			get { return _gridXSubdivision; }
			set { _gridXSubdivision = Mathf.Max(value, 1); }
		}

		[SerializeField]protected int _gridYSubdivision = 10;
		public int GridSubdivisionY
		{
			get { return _gridYSubdivision; }
			set { _gridYSubdivision = Mathf.Max(value, 1); }
		}

		[SerializeField]protected int _gridXAutoScalingThreshold = 15;
		public int GridAutoScalingThresholdX {
			get { return _gridXAutoScalingThreshold; }
			set { _gridXAutoScalingThreshold = Mathf.Max(value, 10); }
		}

		[SerializeField]protected int _gridYAutoScalingThreshold = 15;
		public int GridAutoScalingThresholdY
		{
			get { return _gridYAutoScalingThreshold; }
			set { _gridYAutoScalingThreshold = Mathf.Max(value, 10); }
		}

		private Rect _scopeRect;
		public Rect ScopeRect
		{
			get { return _scopeRect; }
			set
			{
				if (_scopeRect != value)
				{
					_scopeRect = value;
					UpdateGraph();
				}
			}
		}

		public Action OnUpdateGraph;



		private void Reset()
		{
			RegisterInternal(SYSKEY_TIMESTAMP, new Data("Timestamp"));
		}

		private void OnValidate()
		{
			_scopeScale = new Vector2(
				_scopeScale.x < DENOMINATOR_MIN ? DENOMINATOR_MIN : _scopeScale.x,
				_scopeScale.y < DENOMINATOR_MIN ? DENOMINATOR_MIN : _scopeScale.y);
			_scopeMargin = new Vector2(
				_scopeMargin.x < 0 ? 0 : _scopeMargin.x,
				_scopeMargin.y < 0 ? 0 : _scopeMargin.y);

			_gridScale = new Vector2(
				_gridScale.x < DENOMINATOR_MIN ? DENOMINATOR_MIN : _gridScale.x,
				_gridScale.y < DENOMINATOR_MIN ? DENOMINATOR_MIN : _gridScale.y);
			_gridXSubdivision = Mathf.Max(_gridXSubdivision, 1);
			_gridYSubdivision = Mathf.Max(_gridYSubdivision, 1);

			_gridXAutoScalingThreshold = Mathf.Max(10, _gridXAutoScalingThreshold);
			_gridYAutoScalingThreshold = Mathf.Max(10, _gridYAutoScalingThreshold);

			UpdateGraph();
		}

		private void OnRectTransformDimensionsChange()
		{
			UpdateGraph();
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
			if (autoDetermine) Determine();

		}



		public bool isKeyValid(int dataKey)
		{
			return 0 <= dataKey && dataKey < dataList.Count && dataList[dataKey] != null;
		}

		public IEnumerator<float?> GetDataEnumerator(int dataKey)
		{
			if (dataKey < 0) throw new ArgumentException("Data does not exist at specified key.", "dataKey");
			if (!isKeyValid(dataKey)) RegisterInternal(dataKey, new Data("data " + dataKey));
			return dataList[dataKey].GetEnumerator();
		}

		public float? GetCurrentValue(int dataKey)
		{
			if (!isKeyValid(dataKey)) return null;
			return dataList[dataKey].GetCurrent();
		}

		public float? GetLatestValue(int dataKey)
		{
			if (!isKeyValid(dataKey)) return null;
			return dataList[dataKey].GetLatest();
		}
		
		public void SetValue(int dataKey, float value)
		{
			if (!recording || dataKey < 0) return;
			if (!isKeyValid(dataKey))
			{
				if(acceptUnregisteredKey) RegisterInternal(dataKey, new Data("data " + dataKey));
				else
				{
					Debug.LogError("Attempted set value using unregistered key.");
					return;
				}
			}
			if (dataKey < COUNT_SYSKEY)
			{
				Debug.LogError("System data can not set values.");
				return;
			}

			dataList[dataKey].SetCurrent(value);
		}



		public void UpdateGraph()
		{
			if (autoScopeOffset)
			{
				_scopeOffset.x = GetLatestValue(SYSKEY_TIMESTAMP) ?? 0f;
			}
			_scopeRect = new Rect(_scopeOffset, _scopeScale);
			_scopeRect.x -= _scopeScale.x;
			if (!_scopeUnsigned) _scopeRect.y -= _scopeScale.y / 2;
			_scopeRect.max += _scopeMargin;
			_scopeRect.min -= _scopeMargin;

			if (OnUpdateGraph != null)
				OnUpdateGraph();
		}

		public void Determine()
		{
			dataList[SYSKEY_TIMESTAMP].SetCurrent(Time.time - _startTime);
			foreach (var data in dataList)
				data.Determine();
			UpdateGraph();
		}

		public void ClearAll()
		{
			foreach (var data in dataList)
				data.Clear();
			_startTime = Time.time;
			_scopeOffset.x = 0;
			UpdateGraph();
		}


		//public int Register(Data data)
		//{
		//	dataList.Add(data);
		//	return dataList.Count - 1;
		//}

		//public void Unregister(int dataKey)
		//{
		//	if (!isKeyValid(dataKey)) throw new ArgumentException("Data does not exist at specified key.", "dataKey");
		//	if (dataKey < COUNT_SYSKEY) throw new ArgumentException("System data is not Unregisteable.", "dataKey");
		//	dataList[dataKey] = null;
		//}

		//public void RegisterAt(int dataKey, Data data)
		//{
		//	if (isKeyValid(dataKey)) throw new ArgumentException("Data already exists at specified key.", "dataKey");
		//	RegisterInternal(dataKey, data);
		//}

		private void RegisterInternal(int dataKey, Data data)
		{
			if (dataKey >= dataList.Count) dataList.Insert(dataKey, data);
			else dataList[dataKey] = data;
		}
		
	}

}