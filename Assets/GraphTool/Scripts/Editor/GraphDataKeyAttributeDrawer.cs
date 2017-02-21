using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace GraphTool
{

	[CustomPropertyDrawer(typeof(GraphDataKeyAttribute))]
	public class GraphDataKeyAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, property.displayName, "(This attribute works only on integer property.)");
				return;
			}
			var attr = (GraphDataKeyAttribute)attribute;
			var handler = property.serializedObject.FindProperty(attr.handlerProperty);
			if (handler.objectReferenceValue == null)
			{
				EditorGUI.LabelField(position, property.displayName, "(Handler not found)");
				return;
			}
			var handlerObj = new SerializedObject(handler.objectReferenceValue);
			var list = handlerObj.FindProperty("dataList");
			var namelist = new string[list.arraySize + 2];
			var valuelist = new int[list.arraySize + 2];
			namelist[0] = "none";
			valuelist[0] = -1;
			valuelist[1] = -1;
			for (int i = 0; i < list.arraySize; ++i)
			{
				namelist[i + 2] = list.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
				if (i < GraphHandler.COUNT_SYSKEY) namelist[i + 2] += "(system)";
				valuelist[i + 2] = i;
			}

			property.intValue = EditorGUI.IntPopup(new Rect(position.x, position.y + 2f, position.width - 60f, position.height),
				label.text, property.intValue, namelist, valuelist);

			var buttonRect = new Rect(position.x + position.width - 50f, position.y, 50f, position.height);
			if (GUI.Button(buttonRect, "Edit"))
			{
				DataListEditWindow.Create(handlerObj, property, buttonRect);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 1.2f;
		}
	}



	public class DataListEditWindow : EditorWindow, IHasCustomMenu
	{
		SerializedObject handler;
		SerializedProperty self;
		ReorderableList dataList;

		public static void Create(SerializedObject handler, SerializedProperty self, Rect buttonRect)
		{
			if (handler == null || handler.targetObject.GetType() != typeof(GraphHandler)) return;

			DataListEditWindow window = CreateInstance<DataListEditWindow>();
			window.handler = handler;
			window.self = self;
			window.dataList = GraphHandlerEditor.CreateDataList(handler);
			window.dataList.index = self.intValue;
			window.ShowPopup();
			window.position = new Rect(GUIUtility.GUIToScreenPoint(buttonRect.position), window.position.size);
			window.Focus();


		}

		private void Update()
		{
			if (focusedWindow != this && !locked)
			{
				
				Close();
			}
		}

		private void OnDestroy()
		{
			if (self != null && dataList != null)
			{
				self.intValue = dataList.index;
				self.serializedObject.ApplyModifiedProperties();
			}
		}


		Vector2 scroll = Vector2.zero;
		private void OnGUI()
		{
			if (handler == null || handler.targetObject == null)
				return;
			scroll = EditorGUILayout.BeginScrollView(scroll,GUI.skin.box);
			handler.Update();
			EditorGUILayout.LabelField("DataKey Selector", EditorStyles.boldLabel);
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField(new GUIContent("target"), handler.targetObject, typeof(UnityEngine.Object), true);
			EditorGUILayout.ObjectField(new GUIContent("self"), self.serializedObject.targetObject, typeof(UnityEngine.Object), true);
			EditorGUI.EndDisabledGroup();
			dataList.DoLayoutList();

			var height = Mathf.Min(dataList.count * dataList.elementHeight + 120f, 500f);
			minSize = new Vector2(400f, height);
			maxSize = new Vector2(400f, height);
			handler.ApplyModifiedProperties();

			EditorGUILayout.EndScrollView();
			
		}


		//be helped by https://leahayes.wordpress.com/2013/04/30/adding-the-little-padlock-button-to-your-editorwindow/
		GUIStyle lockButtonStyle;
		bool locked = false;
		void ShowButton(Rect position)
		{
			if (lockButtonStyle == null)
				lockButtonStyle = "IN LockButton";
			locked = GUI.Toggle(position, locked, GUIContent.none, lockButtonStyle);
		}

		public void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Lock"), locked, () => {
				locked = !locked;
			});
		}
	}

}