/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools
{
	[AddComponentMenu("ChartRecordingTools/Input/Universal")]
	public class UniversalObserver : InputterBase
	{
		[System.Serializable]
		public class ObserverTargetData
		{
			public Component target = null;
			public string propertyName = "";
			public KeyChain[] keys;
			public MemberTypes memberType;
			public bool distinctUntilChanged = false;
			public bool enabled = true;

			[System.NonSerialized]
			public TypeReader readerCache = null;
			[System.NonSerialized]
			public MemberInfo memberCache = null;
			[System.NonSerialized]
			public object prevValue = null;

			public ObserverTargetData()
			{
				enabled = true;
			}
		}

		[System.Serializable]
		public struct KeyChain
		{
			public int key;
			public string label;
		}

		[SerializeField]
		ObserverTargetData[] targets;

		public void LateUpdate()
		{
			foreach(var c in targets)
			{
				if (!c.enabled || c.target == null || c.propertyName == "")
					continue;
				if(c.memberCache == null)
				{
					var targettype = c.target.GetType();
					c.memberCache = targettype.GetMember(c.propertyName, c.memberType, MemberBindingFlag).FirstOrDefault();
					if (c.memberCache == null)
					{
						Debug.LogWarning("Can't Get Member at " + c.target + "/" + c.propertyName);
						continue;
					}
				}
				
				object value = null;
				System.Type valueType = null;
				switch (c.memberType)
				{
					case MemberTypes.Property:
						var pInfo = (PropertyInfo)c.memberCache;
						value = pInfo.GetValue(c.target, null);
						valueType = pInfo.PropertyType;
						break;
					case MemberTypes.Field:
						var fInfo = (FieldInfo)c.memberCache;
						value = fInfo.GetValue(c.target);
						valueType = fInfo.FieldType;
						break;
					default:
						break;
				}

				if(c.readerCache == null)
				{
					c.readerCache = TypeReader.Get(valueType);
					if(c.readerCache == null)
					{
						Debug.LogWarning("this type is not supported.");
						continue;
					}
				}
				c.readerCache.SendDataToRecorder(Recorder, value, ref c.prevValue, c.keys, c.distinctUntilChanged);


			}
		}

		public static BindingFlags MemberBindingFlag =
			BindingFlags.Public | BindingFlags.NonPublic |
			BindingFlags.Instance | BindingFlags.Static |
			BindingFlags.GetProperty | BindingFlags.GetField;

	}


}