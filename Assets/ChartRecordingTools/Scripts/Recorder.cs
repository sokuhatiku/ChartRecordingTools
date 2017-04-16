/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools
{
	[DisallowMultipleComponent, DefaultExecutionOrder(1000), AddComponentMenu("ChartRecordingTools/Recorder")]
	public class Recorder : MonoBehaviour
	{

		[SerializeField]
		protected bool _acceptData = true;

		[SerializeField]
		protected bool _autoDetermine = true;

		[SerializeField]
		protected bool _acceptUnregisteredKey = false;

		[SerializeField]
		Data timeline = new Data("Timeline");

		[SerializeField]
		List<Data> dataList = new List<Data>();


		public Action OnUpdateData;
		
		float startTime = 0f;


		public void ToggleDataAccepting(bool toggle)
		{
			_acceptData = toggle;
		}


		public bool IsKeyValid(int dataKey)
		{
			return 0 <= dataKey && dataKey < dataList.Count && dataList[dataKey] != null;
		}

		public int GetDataKeyCount()
		{
			return dataList.Count;
		}

		public int GetDataCount()
		{
			return GetTimeline().Count;
		}

		public Data.Reader GetDataReader(int dataKey)
		{
			if (!IsKeyValid(dataKey))
			{
				if (dataKey >= 0 && _acceptUnregisteredKey)
					RegisterInternal(dataKey, new Data("data " + dataKey));
				else throw new ArgumentException("Data does not exist at specified key.", "dataKey");
			}

			return dataList[dataKey].GetReader();
		}

		public Data.Reader GetTimeline()
		{
			return timeline.GetReader();
		}

		public void SetValue(int dataKey, float value)
		{
			if (!_acceptData) return;
			if (!IsKeyValid(dataKey))
			{
				if (dataKey >= 0)
				{
					if (_acceptUnregisteredKey)
						RegisterInternal(dataKey, new Data("data " + dataKey));
					else
					{
						Debug.LogError("Attempted set value using unregistered key.");
						return;
					}
				}
				else return;
			}

			dataList[dataKey].SetCurrent(value);
		}

		public void Determine()
		{
			timeline.SetCurrent(Time.time - startTime);
			timeline.Determine();
			foreach (var data in dataList)
				data.Determine();

			if (OnUpdateData != null)
				OnUpdateData();
		}

		public void ClearAll()
		{
			timeline.Clear();
			foreach (var data in dataList)
				data.Clear();
			startTime = Time.time;

			if (OnUpdateData != null)
				OnUpdateData();
		}

		void RegisterInternal(int dataKey, Data data)
		{
			if (dataKey >= dataList.Count) dataList.Insert(dataKey, data);
			else dataList[dataKey] = data;
		}
		
		
		private void LateUpdate()
		{
			if (_autoDetermine)
			{
				Determine();
			}
		}
		
	}
}