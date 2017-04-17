/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools.EditorScript
{
	[CustomEditor(typeof(UniversalObserver))]
	public class UniversalObserverEditor : Editor
	{
		ReorderableList list;

		Texture2D removeButton;

		public override void OnInspectorGUI()
		{
			this.DrawEditorMaintenanceField();

			serializedObject.Update();

			if (removeButton == null)
				removeButton = EditorGUIUtility.FindTexture("Toolbar Minus");

			if (list == null)
			{
				list = CreateReorderableList(serializedObject.FindProperty("targets"));
			}
			list.DoLayoutList();
			
			DragAndDropField(list.serializedProperty);
	
			serializedObject.ApplyModifiedProperties();
		}

		void DragAndDropField(SerializedProperty list)
		{
			var objs = GetObservableObjects(DragAndDrop.objectReferences);

			if (objs.Count() == 0) return;
			var curRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 3);
			EditorGUI.LabelField(curRect, "Drop and create New Observation target here.", GUI.skin.box);

			var e = Event.current;
			var type = e.type;

			if (!curRect.Contains(e.mousePosition)) return;
			switch (type)
			{
				case EventType.DragUpdated:
					if (objs.Count() > 0) DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					else DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

					e.Use();
					break;

				case EventType.DragPerform:
					foreach(var obj in objs)
					{
						Component comp;
						if (obj is GameObject)
							comp = ((GameObject)obj).transform;
						else comp = (Component)obj;

						var i = list.arraySize;
						list.arraySize = i + 1;
						var element = list.GetArrayElementAtIndex(i);
						element.FindPropertyRelative("target")
							.objectReferenceValue = comp;
						element.FindPropertyRelative("enabled")
							.boolValue = true;
					}

					e.Use();

					break;
			}
		}

		System.Collections.Generic.IEnumerable<Object> GetObservableObjects(Object[] objs)
		{
			return objs.Where(r => (r is Component || r is GameObject));
		}

		ReorderableList CreateReorderableList(SerializedProperty listProp)
		{
			var list = new ReorderableList(listProp.serializedObject, listProp);
			var recorder = ((UniversalObserver)listProp.serializedObject.targetObject).GetRecorder();

			list.drawHeaderCallback +=
				(Rect position) =>
				{
					EditorGUI.LabelField(position, "Observation Targets");
				};

			list.elementHeightCallback =
				(int index) =>
				{
					if(listProp.arraySize == 0)
						return EditorGUIUtility.singleLineHeight + 10;

					var container = listProp.GetArrayElementAtIndex(index);
					var target = container.FindPropertyRelative("target");
					var keyChain = container.FindPropertyRelative("keys");
					var height = 0f;
					if (target.objectReferenceValue == null || keyChain.arraySize == 0)
					{
						height = EditorGUIUtility.singleLineHeight + 10;
					}
					else
					{
						height = EditorGUIUtility.singleLineHeight * (2 + keyChain.arraySize) + 10;
					}
					return height;
				};

			list.drawElementBackgroundCallback =
				(Rect position, int index, bool selected, bool focused) =>
				{
					position.height = list.elementHeightCallback(index);
					if (focused)
						EditorGUI.DrawRect(position, new Color(0.35f, 0.54f, 0.81f));
					else if (selected)
						EditorGUI.DrawRect(position, new Color(0.6f, 0.6f, 0.6f));
					else if ((index & 1) == 0)
						EditorGUI.DrawRect(position, new Color(0f, 0f, 0f, 0.1f));

				};

			list.drawElementCallback =
				(Rect position, int index, bool selected, bool focused) => 
				{
					var container = listProp.GetArrayElementAtIndex(index);

					var enabled = container.FindPropertyRelative("enabled");
					var target = container.FindPropertyRelative("target");
					var propName = container.FindPropertyRelative("propertyName");
					var keyChain = container.FindPropertyRelative("keys");
					var memberType = container.FindPropertyRelative("memberType");
					var ducTrigger = container.FindPropertyRelative("distinctUntilChanged");
					
					var mPos = new Rect(position);
					mPos.height = EditorGUIUtility.singleLineHeight;
					mPos.y += 5;

					if (target.objectReferenceValue != null)
					{
						mPos.xMin = mPos.xMax - 20;
						enabled.boolValue = EditorGUI.Toggle(mPos, enabled.boolValue);
						mPos.xMin = position.xMin;
					}
					else enabled.boolValue = true;
					EditorGUI.BeginDisabledGroup(!enabled.boolValue);

					var prevName = propName.stringValue;
					var type = TargetSelectField(mPos, target, propName, memberType);
					if (type == null)
					{
						keyChain.arraySize = 0;
					}
					else
					{
						if (prevName != propName.stringValue)
						{
							var reader = TypeReader.Get(type);
							if (reader != null)
							{
								var labels = reader.CreateKeyLabels();
								var prevLength = keyChain.arraySize;
								keyChain.arraySize = labels.Length;
								for (int i = 0; i < keyChain.arraySize; i++)
								{
									var elem = keyChain.GetArrayElementAtIndex(i);
									if (prevLength <= i)
										elem.FindPropertyRelative("key").intValue = -1;
									elem.FindPropertyRelative("label").stringValue = labels[i];
								}
							}
						}

						var recorderObj = new SerializedObject(recorder);
						for (int i = 0; i < keyChain.arraySize; i++)
						{
							var elem = keyChain.GetArrayElementAtIndex(i);
							var label = elem.FindPropertyRelative("label");
							var key = elem.FindPropertyRelative("key");

							mPos.y += mPos.height;
							key.intValue = GraphDataKeyAttributeDrawer.RecorderKeyField(
									mPos, recorderObj, label.stringValue, key.intValue, false);
						}

						mPos.y += mPos.height + 5;
						EditorGUI.PropertyField(mPos, ducTrigger);
					}
					EditorGUI.EndDisabledGroup();
				};
			return list;
		}


		static System.Type TargetSelectField(
			Rect position, SerializedProperty component,
			SerializedProperty propName, SerializedProperty memberType = null)
		{
			var compObj = component.objectReferenceValue as Component;
			var attachedObj = compObj != null ? compObj.gameObject : null;

			var mPos = new Rect(position);
			mPos.width *= 0.3f;
			var newAttachedObj = EditorGUI.ObjectField(mPos, attachedObj, typeof(GameObject), true) as GameObject;
			if(newAttachedObj != attachedObj)
			{
				component.objectReferenceValue = newAttachedObj != null ? newAttachedObj.transform : null;
				propName.stringValue = "";
			}

			mPos.x += mPos.width + 5;
			if (newAttachedObj != null)
			{
				component.objectReferenceValue = ComponentSelectField(mPos, newAttachedObj, compObj);
			}

			mPos.x += mPos.width + 5;
			if(component.objectReferenceValue != null)
			{
				var info = MemberSelectField(mPos, component.objectReferenceValue as Component, propName.stringValue);
				if (info != null)
				{
					propName.stringValue = info.name;
					if (memberType != null)
						memberType.intValue = (int)info.memberType;
					return info.type;
				}
			}
			return null;
		}

		static Component ComponentSelectField(Rect position, GameObject baseObj, Component selected)
		{
			var componentList = baseObj.GetComponents<Component>();
			var nameList = componentList.Select(c => c != null ? c.GetType().Name : "").ToArray();

			int selection = 0;
			if (selected != null)
			{
				for (int i=0; i < componentList.Length; i++)
				{
					if (componentList[i] == selected)
					{
						selection = i;
						break;
					}
				}
			}

			var newselection = EditorGUI.Popup(position, selection, nameList);
			if (0 <= newselection && newselection < componentList.Length)
				return componentList[newselection];
			else return selected;
		}

		static MemberInfo MemberSelectField(Rect position, Component target, string selected)
		{
			var propertyList = target.GetType().GetProperties(UniversalObserver.MemberBindingFlag)
				.Where(p => TypeReader.Contains(p.PropertyType))
				.ToArray();
			var fieldList = target.GetType().GetFields(UniversalObserver.MemberBindingFlag)
				.Where(f => TypeReader.Contains(f.FieldType))
				.ToArray();
			if(propertyList.Length == 0 && fieldList.Length == 0)
			{
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Popup(position, 0, new string[] { "{no observable properties}" });
				EditorGUI.EndDisabledGroup();
				return null;
			}

			var namelist = propertyList
				.Select(p => p.Name + " (" + p.PropertyType.Name+ ")" )
				.Concat(fieldList.Select(f => f.Name + " (" + f.FieldType.Name + ")") ).ToArray();

			int selection = 0;
			if(selected != null)
			{
				for(int i=0; i<propertyList.Length; i++)
				{
					if(propertyList[i].Name == selected)
					{
						selection = i;
						break;
					}
				}
				for (int i = 0; i < fieldList.Length; i++)
				{
					if (fieldList[i].Name == selected)
					{
						selection = i + propertyList.Length;
						break;
					}
				}
			}

			var newselection = EditorGUI.Popup(position, selection, namelist);
			if (newselection < propertyList.Length)
			{
				var tgt = propertyList[newselection];
				return new MemberInfo(tgt.Name, tgt.PropertyType, MemberTypes.Property);
			}
			else if (newselection < propertyList.Length + fieldList.Length)
			{
				var tgt = fieldList[newselection - propertyList.Length];
				return new MemberInfo(tgt.Name, tgt.FieldType, MemberTypes.Field);
			}
			else return null;
		}

		class MemberInfo
		{
			public readonly string name;
			public readonly System.Type type;
			public readonly MemberTypes memberType;

			public MemberInfo(string name, System.Type type, MemberTypes memberType)
			{
				this.name = name;
				this.type = type;
				this.memberType = memberType;
			}
		}
		
		
	}
}