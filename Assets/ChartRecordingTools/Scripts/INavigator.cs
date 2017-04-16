/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

namespace Sokuhatiku.ChartRecordingTools
{
	public interface ICanNavigateToScope
	{
		Scope GetScope();
	}

	public interface ICanNavigateToRecorder
	{
		Recorder GetRecorder();
	}
}