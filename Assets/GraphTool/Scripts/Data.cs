/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace GraphTool
{

	[System.Serializable]
	public class Data
	{
		public string name;
		protected List<float?> data = null;
		protected float? currentValue = null;
		protected int latestIndex = -1;

		public Data(string displayName)
		{
			name = displayName;
		}

		public void SetCurrent(float value)
		{
			currentValue = value;
		}

		public void Determine()
		{
			if(data == null) data = new List<float?>();
			data.Add(currentValue);
			if(currentValue != null)
				latestIndex = data.Count -1;
			currentValue = null;
		}

		public void Clear()
		{
			data.Clear();
		}


		Reader _reader;
		public Reader GetReader()
		{
			if (data == null) { data = new List<float?>(); latestIndex = -1; }
			if (_reader == null) _reader = new Reader(this);
			return _reader;
		}


		public class Reader
		{
			private Data data;

			internal Reader(Data data)
			{
				this.data = data;
			}

			public float? CurrentValue
			{
				get { return data.currentValue; }
			}

			public float? LatestValue
			{
				get
				{
					var i = data.latestIndex;
					return i < 0 || i >= data.data.Count ? null 
						: data.data[i];
				}
			}

			public int LatestIndex
			{
				get {
					var i = data.latestIndex;
					return i < 0 || i >= data.data.Count ? -1
						: i ;
				}
			}

			public int Count
			{
				get { return data.data.Count; }
			}

			public float? this[int index]
			{
				get { return data.data[index]; }
			}

			public IEnumerator<float?> GetEnumerator()
			{
				return data.data.GetEnumerator();
			}

		}
	}

}