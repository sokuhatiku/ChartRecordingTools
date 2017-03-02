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

		public virtual float? GetCurrent()
		{
			return current;
		}

		public virtual float? GetLatest()
		{
			return latest;
		}

		public virtual void SetCurrent(float value)
		{
			current = value;
			latest = value;
		}

		public virtual void Determine()
		{
			if(data == null) data = new List<float?>();
			data.Insert(0, current);
			current = null;
		}

		public virtual IEnumerator<float?> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public virtual int GetCount()
		{
			return data.Count;
		}

		public virtual void Clear()
		{
			data.Clear();
		}
		
	}

}