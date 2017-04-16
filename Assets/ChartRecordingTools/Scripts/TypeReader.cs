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

			CreateKeyLabelsDelegate singleValueCreateKey =
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
				singleValueCreateKey,
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var num = (float)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preNum = (float)prevValue;
						if (num == preNum) return;
					}
					recorder.SetValue(keys[0].key, num);
					prevValue = value;
				}));

			reader.Add(typeof(double), new TypeReader(
				singleValueCreateKey,
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var num = (double)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preNum = (double)prevValue;
						if (num == preNum) return;
					}
					recorder.SetValue(keys[0].key, (float)num);
					prevValue = value;
				}));

			reader.Add(typeof(byte), new TypeReader(
				singleValueCreateKey,
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var num = (byte)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preNum = (byte)prevValue;
						if (num == preNum) return;
					}
					recorder.SetValue(keys[0].key, num);
					prevValue = value;
				}));

			reader.Add(typeof(int), new TypeReader(
				singleValueCreateKey,
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var num = (int)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preNum = (int)prevValue;
						if (num == preNum) return;
					}
					recorder.SetValue(keys[0].key, num);
					prevValue = value;
				}));

			reader.Add(typeof(long), new TypeReader(
				singleValueCreateKey,
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var num = (long)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preNum = (long)prevValue;
						if (num == preNum) return;
					}
					recorder.SetValue(keys[0].key, num);
					prevValue = value;
				}));

			reader.Add(typeof(bool), new TypeReader(
				singleValueCreateKey,
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var num = (bool)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preNum = (bool)prevValue;
						if (num == preNum) return;
					}
				recorder.SetValue(keys[0].key, num ? 1 : 0);
					prevValue = value;
				}));

			reader.Add(typeof(Vector2), new TypeReader(
				/* CreateKey */
				() =>
				{
					return new string[]
					{
						"x",
						"y",
					};
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
					int i = 0;
					recorder.SetValue(keys[i++].key, vec2.x);
					recorder.SetValue(keys[i++].key, vec2.y);
					prevValue = value;
				}));


			reader.Add(typeof(Vector3), new TypeReader(
				/* CreateKey */
				() =>
				{
					return new string[]
					{
						"x",
						"y",
						"z",
					};
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
					int i = 0;
					recorder.SetValue(keys[i++].key, vec3.x);
					recorder.SetValue(keys[i++].key, vec3.y);
					recorder.SetValue(keys[i++].key, vec3.z);
					prevValue = value;
				}));

			reader.Add(typeof(Vector4), new TypeReader(
				/* CreateKey */
				() =>
				{
					return new string[]
					{
						"w",
						"x",
						"y",
						"z",
					};
				},
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var vec4 = (Vector4)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preVec4 = (Vector4)prevValue;
						if (vec4 == preVec4) return;
					}
					int i = 0;
					recorder.SetValue(keys[i++].key, vec4.w);
					recorder.SetValue(keys[i++].key, vec4.x);
					recorder.SetValue(keys[i++].key, vec4.y);
					recorder.SetValue(keys[i++].key, vec4.z);

					prevValue = value;
				}));

			reader.Add(typeof(Color), new TypeReader(
				/* CreateKey */
				() =>
				{
					return new string[]
					{
						"r",
						"g",
						"b",
						"a",
					};
				},
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var color = (Color)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preColor = (Color)prevValue;
						if (color == preColor) return;
					}
					int i = 0;
					recorder.SetValue(keys[i++].key, color.r);
					recorder.SetValue(keys[i++].key, color.g);
					recorder.SetValue(keys[i++].key, color.b);
					recorder.SetValue(keys[i++].key, color.a);
					prevValue = value;
				}));

			reader.Add(typeof(Matrix4x4), new TypeReader(
				/* CreateKey */
				() =>
				{
					return new string[]
					{
						"m00","m01","m02","m03",
						"m10","m11","m12","m13",
						"m20","m21","m22","m23",
						"m30","m31","m32","m33",
					};
				},
				/*SendDataToRecorder*/
				(Recorder recorder, object value, ref object prevValue,
				UniversalObserver.KeyChain[] keys, bool distinctUntilChanged) =>
				{
					var matrix = (Matrix4x4)value;
					if (distinctUntilChanged && prevValue != null)
					{
						var preMatrix = (Matrix4x4)prevValue;
						if (matrix == preMatrix) return;
					}
					for (int i = 0; i < 16; i++) {
						recorder.SetValue(keys[i].key, matrix[i]);
					}


					prevValue = value;
				}));

			reader.Add(typeof(Rect), new TypeReader(
				/* CreateKey */
				() =>
				{
					return new string[] 
					{
						"xMin", "yMin",
						"xCenter", "yCenter",
						"xMax", "yMax",
						"width", "height",
					};
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
					int i = 0;
					recorder.SetValue(keys[i++].key, rect.min.x);
					recorder.SetValue(keys[i++].key, rect.min.y);
					recorder.SetValue(keys[i++].key, rect.center.x);
					recorder.SetValue(keys[i++].key, rect.center.y);
					recorder.SetValue(keys[i++].key, rect.max.x);
					recorder.SetValue(keys[i++].key, rect.max.y);
					recorder.SetValue(keys[i++].key, rect.size.x);
					recorder.SetValue(keys[i++].key, rect.size.y);

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