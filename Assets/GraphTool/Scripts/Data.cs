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
		protected List<float?> data = new List<float?>();
		protected float? current = null;
		protected float? latest = null;

		public Data(string displayName)
		{
			name = displayName;
		}

		public void SetCurrent(float value)
		{
			current = value;
		}

		public void Determine()
		{
			if(data == null) data = new List<float?>();
			data.Add(current);
			if(current != null) latest = current;
			current = null;
		}

		public void Clear()
		{
			data.Clear();
		}


		Reader _reader;
		public Reader GetReader()
		{
			if (data == null) data = new List<float?>();
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
				get { return data.current; }
			}

			public float? LatestValue
			{
				get { return data.latest; }
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