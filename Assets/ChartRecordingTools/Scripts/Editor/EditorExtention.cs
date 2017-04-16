/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEditor;
using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools.EditorScript
{
	public static class EditorExtention
	{
		public static void DrawEditorMaintenanceField(this Editor editor)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)editor.target), typeof(MonoScript), false);
			EditorGUILayout.ObjectField("Editor", MonoScript.FromScriptableObject(editor), typeof(MonoScript), false);
			EditorGUI.EndDisabledGroup();
		}
	}
}
