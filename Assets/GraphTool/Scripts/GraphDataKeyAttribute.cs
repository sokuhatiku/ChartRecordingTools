/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;

namespace GraphTool
{
	public class GraphDataKeyAttribute : PropertyAttribute
	{
		public string handlerProperty;

		public GraphDataKeyAttribute(string handlerPropName)
		{
			handlerProperty = handlerPropName;
		}
	}
}