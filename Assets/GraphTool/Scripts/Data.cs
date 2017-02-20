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
		protected List<float?> data = new List<float?>();
		protected float? currentData;

		public virtual float? GetCurrent()
		{
			return currentData;
		}

		public virtual void SetCurrent(float value)
		{
			currentData = value;
		}

		public virtual void Determine()
		{
			data.Insert(0, currentData);
			currentData = null;
		}

		public virtual IEnumerator<float?> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public virtual void Clear()
		{
			data.Clear();
		}
		
	}

}