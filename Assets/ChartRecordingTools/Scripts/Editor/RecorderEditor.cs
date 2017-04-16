/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Sokuhatiku.ChartRecordingTools.EditorScript
{

	[CustomEditor(typeof(Recorder))]
	public class RecorderEditor : Editor
	{

		ReorderableList dataList;


		private void OnEnable()
		{
			dataList = CreateDataList(serializedObject);
		}

		public override void OnInspectorGUI()
		{
			this.DrawEditorMaintenanceField();
			
			serializedObject.Update();

			GeneralEditor();
			DataListEditor();

			serializedObject.ApplyModifiedProperties();
		}

		void GeneralEditor()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_acceptData"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_autoDetermine"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_acceptUnregisteredKey"));

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
				(Rect position) =>
				{
					EditorGUI.LabelField(position, "Data List");
				};

			dataList.drawElementCallback +=
				(Rect position, int index, bool selected, bool focused) =>
				{
					var margin = (position.height - EditorGUIUtility.singleLineHeight) / 2;
					var prop = listProperty.GetArrayElementAtIndex(index);

					
					var partsPos = new Rect(position.x, position.y + margin, 40f, EditorGUIUtility.singleLineHeight);
					if(focused)EditorGUI.LabelField(partsPos, "name");

					partsPos.x += partsPos.width;
					partsPos.width = 200f;
					var name = prop.FindPropertyRelative("name");
					name.stringValue = EditorGUI.TextField(partsPos, name.stringValue);

					EditorGUI.EndDisabledGroup();
				};

			//dataList.onCanRemoveCallback +=
			//	(ReorderableList list) =>
			//	{
			//		return !(list.index < GraphHandler.COUNT_SYSKEY);
			//	};

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

			//dataList.onRemoveCallback +=
			//	(ReorderableList list) =>
			//	{
			//		listProperty.DeleteArrayElementAtIndex(list.index);
			//		if (list.index < GraphHandler.COUNT_SYSKEY)
			//			list.index = -1;
			//	};

			return dataList;
		}


	}
}