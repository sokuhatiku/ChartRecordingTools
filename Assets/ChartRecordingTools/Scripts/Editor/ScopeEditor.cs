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

	[CustomEditor(typeof(Scope))]
	public class ScopeEditor : Editor
	{
		
		public override void OnInspectorGUI()
		{
			this.DrawEditorMaintenanceField();

			serializedObject.Update();

			DrawRecorderRefelenceEditor();
			DrawScopeEditor();
			DrawGridEditor();

			serializedObject.ApplyModifiedProperties();
		}

		void DrawRecorderRefelenceEditor()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("recorder"), new GUIContent("Recorder object"));

			EditorGUILayout.EndVertical();
		}

		void DrawScopeEditor()
		{

			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("FollowLatest"), new GUIContent("Follow latest data"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_offset"), new GUIContent("Offset"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_size"), new GUIContent("Size"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Unsigned"), new GUIContent("Unsigned"));

			EditorGUILayout.EndVertical();
		}

		void DrawGridEditor()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_gridCellSize"), new GUIContent("CellSize"));
			IntVectorField("Subdivision", serializedObject.FindProperty("_gridSubdivisionX"), serializedObject.FindProperty("_gridSubdivisionY"));

			EditorGUILayout.EndVertical();
		}

		void IntVectorField(string label, SerializedProperty x, SerializedProperty y)
		{
			var newDiv = EditorGUILayout.Vector2Field(label, new Vector2(x.intValue, y.intValue));
			newDiv -= new Vector2(x.intValue, y.intValue);
			if (newDiv.x > 0) x.intValue += Mathf.CeilToInt(newDiv.x);
			else if (newDiv.x < 0) x.intValue += Mathf.FloorToInt(newDiv.x);
			if (newDiv.y > 0) y.intValue += Mathf.CeilToInt(newDiv.y);
			else if (newDiv.y < 0) y.intValue += Mathf.FloorToInt(newDiv.y);
		}
	}
}