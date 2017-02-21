/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GraphTool
{

	[CustomEditor(typeof(GraphHandler))]
	public class GraphHandlerEditor : Editor
	{

		ReorderableList dataList;


		private void OnEnable()
		{
			dataList = CreateDataList(serializedObject);
		}

		public override void OnInspectorGUI()
		{
						
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
			EditorGUILayout.ObjectField("Editor", MonoScript.FromScriptableObject(this), typeof(MonoScript), false);
			EditorGUI.EndDisabledGroup();

			serializedObject.Update();

			ScopeEditor();
			DataListEditor();

			serializedObject.ApplyModifiedProperties();

		}


		void ScopeEditor()
		{

			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.LabelField("Scope", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_scopeOffset"),new GUIContent("Offset"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_scopeSize"), new GUIContent("Scale"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_scopeMargin"), new GUIContent("Margin"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_scopeUnsigned"), new GUIContent("Unsigned"));

			EditorGUILayout.EndVertical();
		}

		void DataListEditor()
		{
			dataList.DoLayoutList();
		}

		public static ReorderableList CreateDataList(SerializedObject serializedObject)
		{
			var listProperty = serializedObject.FindProperty("dataList");
			var dataList = new ReorderableList(serializedObject, listProperty);

			dataList.draggable = false;

			dataList.drawHeaderCallback +=
				(position) =>
				{
					EditorGUI.LabelField(position, "DataKey List");
				};

			dataList.drawElementCallback +=
				(position, index, selected, focused) =>
				{
					var margin = (position.height - EditorGUIUtility.singleLineHeight) / 2;
					var prop = listProperty.GetArrayElementAtIndex(index);
					var isSystemKey = index < GraphHandler.COUNT_SYSKEY;
					EditorGUI.BeginDisabledGroup(isSystemKey);
					var name = prop.FindPropertyRelative("name");
					var namePos = new Rect(position.x + 30f, position.y + margin, 200f, EditorGUIUtility.singleLineHeight);

					if (isSystemKey) EditorGUI.TextField(namePos, name.stringValue + "(system)");
					else name.stringValue = EditorGUI.TextField(namePos, name.stringValue);

					EditorGUI.EndDisabledGroup();
				};

			dataList.onCanRemoveCallback +=
				(ReorderableList list) =>
				{
					return !(list.index < GraphHandler.COUNT_SYSKEY);
				};

			//dataList.onSelectCallback +=
			//	(ReorderableList list) =>
			//	{
			//		if (list.index < GraphHandler.COUNT_SYSKEY)
			//			list.index = -1;
			//	};

			dataList.onAddCallback +=
				(ReorderableList list) =>
				{
					var index = listProperty.arraySize;
					listProperty.InsertArrayElementAtIndex(index);
					var prop = listProperty.GetArrayElementAtIndex(index);
					prop.FindPropertyRelative("name").stringValue = "data " + index;
				};

			dataList.onRemoveCallback +=
				(ReorderableList list) =>
				{
					listProperty.DeleteArrayElementAtIndex(list.index);
					if (list.index < GraphHandler.COUNT_SYSKEY)
						list.index = -1;
				};

			return dataList;
		}

	}

}