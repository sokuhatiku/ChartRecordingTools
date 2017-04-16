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

	[CustomPropertyDrawer(typeof(RecorderDataKeyAttribute))]
	public class GraphDataKeyAttributeDrawer : PropertyDrawer
	{


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, property.displayName, "(This attribute works only on integer property.)");
				return;
			}
			if (property.hasMultipleDifferentValues)
			{
				EditorGUI.LabelField(position, property.displayName, "(Cannot edit multiple values)");
				return;
			}
			
			var attr = (RecorderDataKeyAttribute)attribute;

			var recorderObj = FindRecorder(property, attr.targetproperty, position);
			if (recorderObj == null)
				return;

			property.intValue = RecorderKeyField(position, recorderObj, label.text, property.intValue);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 1.2f;
		}


		public static int RecorderKeyField(Rect position, SerializedObject recorderObj, string label, int selectedValue, bool showEditButton = true)
		{
			var list = recorderObj.FindProperty("dataList");
			var namelist = new string[list.arraySize + 2];
			var valuelist = new int[list.arraySize + 2];
			namelist[0] = "none";
			valuelist[0] = -1;
			valuelist[1] = -1;
			for (int i = 0; i < list.arraySize; ++i)
			{
				namelist[i + 2] = list.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
				valuelist[i + 2] = i;
			}

			var buttonSpace = showEditButton ? 60f : 0f;
			var returnint = EditorGUI.IntPopup(new Rect(position.x, position.y + 2f, position.width - buttonSpace, position.height),
				label, selectedValue, namelist, valuelist);

			if (showEditButton)
			{
				var buttonRect = new Rect(position.x + position.width - 50f, position.y, 50f, position.height);
				if (GUI.Button(buttonRect, "Edit"))
				{
					DataListEditWindow.Create(recorderObj, buttonRect);
				}
			}

			return returnint;

		}

		public static SerializedObject FindRecorder(SerializedProperty property, string name, Rect position)
		{
			if (name == null)
			{
				var obj = property.serializedObject.targetObject;
				if (obj is ICanNavigateToScope)
				{
					var scope = ((ICanNavigateToScope)obj).GetScope();

					if (scope == null)
					{
						EditorGUI.LabelField(position, property.displayName, "(scope not set)");
						return null;
					}
					var recorder = scope.GetRecorder();

					if (recorder == null)
					{
						// recorder not set in scope
						EditorGUI.LabelField(position, property.displayName, "(recorder not set in scope)");
						return null;
					}

					return new SerializedObject(recorder);
				}
				else if (obj is ICanNavigateToRecorder)
				{
					var recorder = ((ICanNavigateToRecorder)obj).GetRecorder();
					if (recorder == null)
					{
						// recorder not set
						EditorGUI.LabelField(position, property.displayName, "(recorder not set)");
						return null;
					}

					return new SerializedObject(recorder);
				}
				else
				{
					// Object has not implement required interface for skip property name
					EditorGUI.LabelField(position, property.displayName, "(Object has not implement required interface for skip property name)");
					return null;
				}
			}
			else
			{
				var scopeOrRecorderProp = property.serializedObject.FindProperty(name);
				if (scopeOrRecorderProp == null)
				{
					// invalid property name
					EditorGUI.LabelField(position, property.displayName, "(invalid property name)");
					return null;
				}

				var propValue = scopeOrRecorderProp.objectReferenceValue;
				if (propValue == null)
				{
					// object not set
					EditorGUI.LabelField(position, property.displayName, "(object not set)");
					return null;
				}

				Recorder recorder = null;
				if (propValue.GetType() == typeof(Scope))
				{
					recorder = ((Scope)propValue).GetRecorder();
				}
				else if (propValue.GetType() == typeof(Recorder))
				{
					recorder = ((Recorder)propValue);
				}
				else
				{
					// invalid type
					EditorGUI.LabelField(position, property.displayName, "(invalid type)");
					return null;
				}

				return new SerializedObject(recorder);
			}
		}
	}


	public class DataListEditWindow : EditorWindow
	{
		const float WINDOW_WIDTH = 400f;
		const float WINDOW_MAX_HEIGHT = 500f;

		SerializedObject handler;
		ReorderableList dataList;
		bool closeRequest = false;

		public static void Create(SerializedObject handler, Rect buttonRect)
		{
			if (handler == null || handler.targetObject.GetType() != typeof(Recorder)) return;

			DataListEditWindow window = CreateInstance<DataListEditWindow>();
			window.handler = handler;
			window.dataList = RecorderEditor.CreateDataList(handler);
			window.dataList.index = -1;
			window.ShowPopup();
			window.position = new Rect(GUIUtility.GUIToScreenPoint(new Vector2(buttonRect.xMax - WINDOW_WIDTH, buttonRect.yMax)), window.position.size);
			window.Focus();

		}

		private void Update()
		{
			if (focusedWindow != this || closeRequest)
			{
				Close();
			}
		}

		//private void OnDestroy()
		//{
		//	if (self != null && dataList != null)
		//	{
		//		self.intValue = dataList.index;
		//		self.serializedObject.ApplyModifiedProperties();
		//	}
		//}


		Vector2 scroll = Vector2.zero;
		private void OnGUI()
		{
			if (handler == null || handler.targetObject == null)
				return;
			handler.Update();

			scroll = EditorGUILayout.BeginScrollView(scroll, GUI.skin.box);
			try
			{
				EditorGUILayout.LabelField("DataKey Editor", EditorStyles.boldLabel);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.ObjectField(new GUIContent("Recorder"), handler.targetObject, typeof(UnityEngine.Object), true);
				EditorGUI.EndDisabledGroup();

				dataList.DoLayoutList();

				var height = Mathf.Min(dataList.count * dataList.elementHeight + 120f, WINDOW_MAX_HEIGHT);
				minSize = new Vector2(WINDOW_WIDTH, height);
				maxSize = new Vector2(WINDOW_WIDTH, height);
			}
			catch (System.NullReferenceException e)
			{
				if (e.Message.Contains("SerializedObject has been Disposed"))
					closeRequest = true;
				else throw e;
			}
			finally
			{
				EditorGUILayout.EndScrollView();
			}

			handler.ApplyModifiedProperties();

		}

	}

}