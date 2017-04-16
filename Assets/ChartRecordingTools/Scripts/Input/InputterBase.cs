/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools
{
	[RequireComponent(typeof(Recorder)), DefaultExecutionOrder(999)]
	public abstract class InputterBase : MonoBehaviour, ICanNavigateToRecorder
	{
		[SerializeField, HideInInspector]
		private Recorder recorder;
		protected Recorder Recorder
		{
			get
			{
				if (!recorder) recorder = GetComponent<Recorder>();
				return recorder;
			}
		}

		public Recorder GetRecorder()
		{
			return Recorder;
		}
	}
}