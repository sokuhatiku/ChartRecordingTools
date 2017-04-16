/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools.Sample
{
	public class Rotate : MonoBehaviour
	{
		public float speed;
		public Vector3 axis;

		private void Update()
		{
			transform.Rotate(axis, speed * Time.deltaTime);
		}

	}
}