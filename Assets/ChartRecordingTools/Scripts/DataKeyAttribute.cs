/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools
{
	public class RecorderDataKeyAttribute : PropertyAttribute
	{
		public string targetproperty;
		public bool hideKeyEditor;

		public RecorderDataKeyAttribute(string recorderOrScope = null, bool hideKeyEditor = false)
		{
			targetproperty = recorderOrScope;
		}
	}
}