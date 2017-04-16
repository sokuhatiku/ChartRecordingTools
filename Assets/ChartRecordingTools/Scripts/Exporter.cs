/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace Sokuhatiku.ChartRecordingTools
{
	[AddComponentMenu("ChartRecordingTools/Exporter")]
	public class Exporter : MonoBehaviour
	{
		public Recorder targetRecorder;
		public Scope targetScope;

		public string outputPath = "Test.csv";

		public void ExportScope()
		{
			int start = -1, end = -1;

			if (targetScope == null)
			{
				Debug.LogError("Recorder not specified.");
				return;
			}
			start = targetScope.InScopeFirstIndex;
			end = targetScope.InScopeLastIndex;

			if(start < 0 || end < 0)
			{
				Debug.LogWarning("Export failured. The scope not presents any datas.");
			}

			Export(start, end);
		}

		public void ExportAll()
		{
			int start = -1, end = -1;
			if (targetRecorder == null)
			{
				Debug.LogError("Recorder not specified.");
				return;
			}
			start = targetRecorder.GetTimeline().FirstIndex;
			end = targetRecorder.GetTimeline().LatestIndex;

			if (start < 0 || end < 0)
			{
				Debug.LogWarning("Export failured. The recorder doesn't have any datas.");
			}

			Export(start, end);
		}

		public void Export(
			int firstIndex, int lastIndex)
		{
			if(targetRecorder == null)
			{
				Debug.LogError("Recorder not specified.");
				return;
			}
			var path = outputPath;

			var dir = Path.GetDirectoryName(path);

			if (dir != "" && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			if (Path.GetExtension(path) != ".csv")
				path += ".csv";
			if (File.Exists(path))
				path = GetUnconflictedPath(path);

			var keyCount = targetRecorder.GetDataKeyCount();
			var datas = new List<Data.Reader>(keyCount);

			datas.Add(targetRecorder.GetTimeline());
			for (int key = 0; key < keyCount; key++)
				datas.Add(targetRecorder.GetDataReader(key));
			keyCount++;

			using (var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
			{
				for (int key = 0; key < keyCount; key++)
				{
					sw.Write(SetDoubleQuote(datas[key].Name));
					if (key < keyCount - 1) sw.Write(",");
					else sw.Write("\n");
				}

				for (int i = firstIndex; i <= lastIndex; i++)
				{
					for (int key = 0; key < keyCount; key++)
					{
						sw.Write(datas[key][i] != null ? datas[key][i].ToString() : "\"\"");
						if (key < keyCount - 1) sw.Write(",");
						else sw.Write("\n");
					}
				}

				sw.Close();
			}

			Debug.Log("Exported:" + path);
		}

		public static string SetDoubleQuote(string value)
		{
			if (value.IndexOf('"') > -1)
				value = value.Replace("\"", "\"\"");
			return "\"" + value + "\"";
		}

		public static string GetUnconflictedPath(string path)
		{
			var name = Path.GetFileNameWithoutExtension(path);
			var dir = Path.GetDirectoryName(path);
			var ex = Path.GetExtension(path);

			if (dir == "") dir = Directory.GetCurrentDirectory();
			Debug.Log(name + "\n" + dir + "\n" + ex);

			var reg = name + @"_(\d*)\" + ex + @"$";
			var max = Directory.GetFiles(dir, name + "*" + ex, SearchOption.TopDirectoryOnly)
				.Select(s => Regex.Match(s, reg))
				.Where(s => s.Success)
				.Select(m => int.Parse(m.Groups[1].Value))
				.DefaultIfEmpty(-1)
				.Max();

			return String.Format(dir + @"\{0}_{1:d3}{2}", name, max + 1, ex);
		}

	}
}