/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sokuhatiku.ChartRecordingTools
{

	public class TypeReader
	{
		static Dictionary<Type, TypeReader> Readers;

		private TypeReader(
			CreateKeyLabelsDelegate onCreateKeyLabels,
			 SendDataToRecorderDelegate onSendData)
		{
			OnCreateKeyLabels = onCreateKeyLabels;
			OnSendDataToRecorder = onSendData;
		}
		

		static Dictionary<Type, TypeReader> CreateReaders()
		{
			var reader = new Dictionary<Type, TypeReader>();

			CreateKeyLabelsDelegate floatCompatibleCreateKey =
				/* CreateKey */
				() =>
				{
					var labels = new string[] {"value"};
					return labels;
				};

			SendDataToRecorderDelegate floatCompatibleSendData =
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					if (distinctUntilChanged && value == prevValue) return;
					recorder.SetValue(keys[0].key, (float)value);
					prevValue = value;
				};



			reader.Add(typeof(float), new TypeReader(
				floatCompatibleCreateKey,
				floatCompatibleSendData));


			reader.Add(typeof(int), new TypeReader(
				floatCompatibleCreateKey,
				floatCompatibleSendData));


			reader.Add(typeof(double), new TypeReader(
				floatCompatibleCreateKey,
				floatCompatibleSendData));


			reader.Add(typeof(Vector2), new TypeReader(
				/* CreateKey */
				() =>
				{
					var labels = new string[]
					{
						"x",
						"y",
					};
					return labels;
				},
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var vec2 = (Vector2)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preVec2 = (Vector2)prevValue;
						if (vec2 == preVec2) return;
					}
					recorder.SetValue(keys[0].key, vec2.x);
					recorder.SetValue(keys[1].key, vec2.y);
					prevValue = value;
				}));


			reader.Add(typeof(Vector3), new TypeReader(
				/* CreateKey */
				() =>
				{
					var labels = new string[]
					{
						"x",
						"y",
						"z",
					};
					return labels;
				},
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var vec3 = (Vector3)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preVec3 = (Vector3)prevValue;
						if (vec3 == preVec3) return;
					}
					recorder.SetValue(keys[0].key, vec3.x);
					recorder.SetValue(keys[1].key, vec3.y);
					recorder.SetValue(keys[2].key, vec3.z);
					prevValue = value;
				}));


			reader.Add(typeof(Rect), new TypeReader(
				/* CreateKey */
				() =>
				{
					var labels = new string[] 
					{
						"xMin",
						"yMin",
						"xCenter",
						"yCenter",
						"xMax",
						"yMax",
						"width",
						"height",
					};
					return labels;
				},
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var rect = (Rect)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preRect = (Rect)prevValue;
						if (rect == preRect) return;
					}
					recorder.SetValue(keys[0].key, rect.min.x);
					recorder.SetValue(keys[1].key, rect.min.y);
					recorder.SetValue(keys[2].key, rect.center.x);
					recorder.SetValue(keys[3].key, rect.center.y);
					recorder.SetValue(keys[4].key, rect.max.x);
					recorder.SetValue(keys[5].key, rect.max.y);
					recorder.SetValue(keys[6].key, rect.size.x);
					recorder.SetValue(keys[7].key, rect.size.y);

					prevValue = value;
				}));


			return reader;
		}


		public static TypeReader Get(Type type)
		{
			if (Readers == null) Readers = CreateReaders();
			return Readers.ContainsKey(type) ? Readers[type] : null;
		}

		public static bool Contains(Type type)
		{
			if (Readers == null) Readers = CreateReaders();
			return Readers.ContainsKey(type);
		}

		delegate string[] CreateKeyLabelsDelegate();
		CreateKeyLabelsDelegate OnCreateKeyLabels;
		public string[] CreateKeyLabels()
		{
			return OnCreateKeyLabels();
		}
		delegate void SendDataToRecorderDelegate(Recorder recorder, object value, ref object prevValue, UniversalObserver.KeyChain[] keys, bool distinctUntilChanged);
		SendDataToRecorderDelegate OnSendDataToRecorder;
		public void SendDataToRecorder(Recorder recorder, object value, ref object prevValue, UniversalObserver.KeyChain[] keys, bool distinctUntilChanged)
		{
			OnSendDataToRecorder(recorder, value, ref prevValue, keys, distinctUntilChanged);
		}
	}
}