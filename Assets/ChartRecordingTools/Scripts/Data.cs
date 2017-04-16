/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;

namespace Sokuhatiku.ChartRecordingTools
{

	[System.Serializable]
	public class Data
	{
		public string name;
		protected List<float?> values = null;
		protected float? currentValue = null;
		protected int latestIndex = -1;
		protected int firstIndex = -1;

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
			if(values == null) values = new List<float?>();
			values.Add(currentValue);
			if(currentValue != null)
			{
				latestIndex = values.Count - 1;
				if (firstIndex < 0) firstIndex = latestIndex;
			}
			currentValue = null;
		}

		public void Clear()
		{
			values.Clear();
			currentValue = null;
			latestIndex = firstIndex = -1;
		}


		Reader _reader;
		public Reader GetReader()
		{
			if (values == null) values = new List<float?>();
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

			public string Name
			{
				get { return data.name; }
			}

			public float? CurrentValue
			{
				get { return data.currentValue; }
			}

			public float? FirstValue
			{
				get
				{
					var i = data.firstIndex;
					return i < 0 ? null : data.values[i];
				}
			}

			public int FirstIndex
			{
				get
				{
					var i = data.firstIndex;
					return i < 0 ? -1 : i;
				}
			}

			public float? LatestValue
			{
				get
				{
					var i = data.latestIndex;
					return i < 0 ? null 
						: data.values[i];
				}
			}

			public int LatestIndex
			{
				get {
					var i = data.latestIndex;
					return i < 0 ? -1 : i ;
				}
			}
			

			public int Count
			{
				get { return data.values.Count; }
			}

			public float? this[int index]
			{
				get { return data.values[index]; }
			}

			public IEnumerator<float?> GetEnumerator()
			{
				return data.values.GetEnumerator();
			}

		}
	}

}