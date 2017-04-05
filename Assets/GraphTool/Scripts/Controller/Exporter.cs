using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTool
{
	public class Exporter : MonoBehaviour
	{
		[SerializeField]GraphHandler handler;

		public string path = "test.csv";

		public void SetPath(string path)
		{
			this.path = path;
		}

		public void ExportAll()
		{
			if (handler != null)
				handler.ExportAll(path);
			else Debug.LogError("Can't export. handler not set.");
		}

		public void ExportScope()
		{
			if (handler != null)
				handler.ExportScope(path);
			else Debug.LogError("Can't export. handler not set.");
		}
	}
}