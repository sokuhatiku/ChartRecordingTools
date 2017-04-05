using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace GraphTool
{
	public static class GraphExporter
	{

		public static void Export(
			GraphHandler handler,
			int firstIndex, int lastIndex,
			string path)
		{
			var dir = Path.GetDirectoryName(path);

			if (dir != "" && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			if (Path.GetExtension(path) != ".csv")
				path += ".csv";
			if (File.Exists(path))
				path = GetNonOverlappingPath(path);

			var keys = handler.GetDataKeyCount();
			var datas = new List<Data.Reader>(keys);

			for(int key=0; key < keys; key++)
			{
				datas.Add(handler.GetDataReader(key));
			}
			
			using (var sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8))
			{
				for (int key = 0; key < keys; key++)
				{
					sw.Write(SetDoubleQuote(datas[key].Name));
					if (key < keys - 1) sw.Write(",");
					else sw.Write("\n");
				}

				for (int i = firstIndex; i <= lastIndex; i++)
				{
					for (int key = 0; key < keys; key++)
					{
						sw.Write(datas[key][i] != null ? datas[key][i].ToString() : "\"\"");
						if (key < keys-1) sw.Write(",");
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

		public static string GetNonOverlappingPath(string path)
		{
			var name = Path.GetFileNameWithoutExtension(path);
			var dir = Path.GetDirectoryName(path);
			var ex = Path.GetExtension(path);

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